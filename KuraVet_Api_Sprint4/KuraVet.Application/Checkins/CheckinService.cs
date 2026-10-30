using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Entities;
using KuraVet.Domain.Services;
using Microsoft.Extensions.Logging;

namespace KuraVet.Application.Checkins;

public interface ICheckinService
{
    Task<PagedResult<CheckinResponse>> ListarAsync(CheckinFiltro filtro, CancellationToken cancellationToken);

    Task<PagedResult<CheckinResponse>> ListarPorPetAsync(int petId, CheckinFiltro filtro, CancellationToken cancellationToken);

    Task<CheckinResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<CheckinResponse> CriarAsync(CheckinRequest request, CancellationToken cancellationToken);

    Task<CheckinResponse> AtualizarAsync(int id, CheckinRequest request, CancellationToken cancellationToken);

    Task RemoverAsync(int id, CancellationToken cancellationToken);
}

public sealed class CheckinService : ICheckinService
{
    private const string Recurso = "Check-in";

    private readonly ICheckinRepository _checkinRepository;
    private readonly IPetRepository _petRepository;
    private readonly ICheckinRiscoService _riscoService;
    private readonly ILogger<CheckinService> _logger;

    public CheckinService(
        ICheckinRepository checkinRepository,
        IPetRepository petRepository,
        ICheckinRiscoService riscoService,
        ILogger<CheckinService> logger)
    {
        _checkinRepository = checkinRepository;
        _petRepository = petRepository;
        _riscoService = riscoService;
        _logger = logger;
    }

    public async Task<PagedResult<CheckinResponse>> ListarAsync(CheckinFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.ValidarOrdenacao();
        if (!string.IsNullOrWhiteSpace(filtro.NivelRisco))
        {
            filtro.NivelRisco = NiveisRisco.Normalizar(filtro.NivelRisco);
        }

        var pagina = await _checkinRepository.ListarAsync(filtro, cancellationToken);
        return PagedResult<CheckinResponse>.Criar(
            pagina.Items.Select(c => c.ParaResponse()), filtro.Page, filtro.PageSize, pagina.TotalItems);
    }

    public async Task<PagedResult<CheckinResponse>> ListarPorPetAsync(int petId, CheckinFiltro filtro, CancellationToken cancellationToken)
    {
        if (!await _petRepository.ExisteAsync(petId, cancellationToken))
        {
            throw new NotFoundException("Pet", petId);
        }

        filtro.PetId = petId;
        return await ListarAsync(filtro, cancellationToken);
    }

    public async Task<CheckinResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var checkin = await ObterEntidadeAsync(id, cancellationToken);
        return checkin.ParaResponse();
    }

    public async Task<CheckinResponse> CriarAsync(CheckinRequest request, CancellationToken cancellationToken)
    {
        await GarantirPetExisteAsync(request.PetId, cancellationToken);

        var checkin = new CheckinHistorico(
            request.PetId,
            request.FrequenciaRespiratoria,
            request.TempoPreenchimentoCapilar,
            request.CorMucosa,
            request.NivelHidratacao,
            request.DataCheckin);

        checkin.DefinirNivelRisco(_riscoService.ClassificarRisco(checkin.TempoPreenchimentoCapilar));

        await _checkinRepository.AdicionarAsync(checkin, cancellationToken);
        _logger.LogInformation(
            "Check-in {CheckinId} registrado para o pet {PetId} com risco '{NivelRisco}'.",
            checkin.Id, checkin.PetId, checkin.NivelRiscoIA);
        return checkin.ParaResponse();
    }

    public async Task<CheckinResponse> AtualizarAsync(int id, CheckinRequest request, CancellationToken cancellationToken)
    {
        var checkin = await ObterEntidadeAsync(id, cancellationToken);
        await GarantirPetExisteAsync(request.PetId, cancellationToken);

        checkin.AtualizarSinais(
            request.PetId,
            request.FrequenciaRespiratoria,
            request.TempoPreenchimentoCapilar,
            request.CorMucosa,
            request.NivelHidratacao,
            request.DataCheckin);

        // Recalcula o risco sempre que os sinais vitais mudam.
        checkin.DefinirNivelRisco(_riscoService.ClassificarRisco(checkin.TempoPreenchimentoCapilar));

        await _checkinRepository.AtualizarAsync(checkin, cancellationToken);
        _logger.LogInformation("Check-in {CheckinId} atualizado com risco '{NivelRisco}'.", id, checkin.NivelRiscoIA);
        return checkin.ParaResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken)
    {
        var checkin = await ObterEntidadeAsync(id, cancellationToken);
        await _checkinRepository.RemoverAsync(checkin, cancellationToken);
        _logger.LogInformation("Check-in {CheckinId} removido com sucesso.", id);
    }

    private async Task<CheckinHistorico> ObterEntidadeAsync(int id, CancellationToken cancellationToken)
    {
        return await _checkinRepository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new NotFoundException(Recurso, id);
    }

    private async Task GarantirPetExisteAsync(int petId, CancellationToken cancellationToken)
    {
        if (!await _petRepository.ExisteAsync(petId, cancellationToken))
        {
            throw new ValidacaoException(nameof(CheckinRequest.PetId), $"PetId {petId} não corresponde a um pet cadastrado.");
        }
    }
}

internal static class CheckinMapeamento
{
    public static CheckinResponse ParaResponse(this CheckinHistorico checkin) => new()
    {
        Id = checkin.Id,
        DataCheckin = checkin.DataCheckin,
        FrequenciaRespiratoria = checkin.FrequenciaRespiratoria,
        TempoPreenchimentoCapilar = checkin.TempoPreenchimentoCapilar,
        CorMucosa = checkin.CorMucosa,
        NivelHidratacao = checkin.NivelHidratacao,
        NivelRiscoIA = checkin.NivelRiscoIA,
        PetId = checkin.PetId
    };
}
