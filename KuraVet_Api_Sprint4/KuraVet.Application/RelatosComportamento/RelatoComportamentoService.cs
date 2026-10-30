using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace KuraVet.Application.RelatosComportamento;

public interface IRelatoComportamentoService
{
    Task<PagedResult<RelatoComportamentoResponse>> ListarAsync(RelatoComportamentoFiltro filtro, CancellationToken cancellationToken);

    Task<RelatoComportamentoResponse> ObterPorIdAsync(string id, CancellationToken cancellationToken);

    Task<RelatoComportamentoResponse> CriarAsync(
        RelatoComportamentoRequest request, string? registradoPor, CancellationToken cancellationToken);

    Task<RelatoComportamentoResponse> AtualizarAsync(
        string id, RelatoComportamentoRequest request, CancellationToken cancellationToken);

    Task RemoverAsync(string id, CancellationToken cancellationToken);
}

public sealed class RelatoComportamentoService : IRelatoComportamentoService
{
    private const string Recurso = "Relato de comportamento";

    private readonly IRelatoComportamentoRepository _relatoRepository;
    private readonly IPetRepository _petRepository;
    private readonly ILogger<RelatoComportamentoService> _logger;

    public RelatoComportamentoService(
        IRelatoComportamentoRepository relatoRepository,
        IPetRepository petRepository,
        ILogger<RelatoComportamentoService> logger)
    {
        _relatoRepository = relatoRepository;
        _petRepository = petRepository;
        _logger = logger;
    }

    public async Task<PagedResult<RelatoComportamentoResponse>> ListarAsync(
        RelatoComportamentoFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.ValidarOrdenacao();
        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
        {
            filtro.Categoria = CategoriasRelato.Normalizar(filtro.Categoria);
        }

        var pagina = await _relatoRepository.ListarAsync(filtro, cancellationToken);
        return PagedResult<RelatoComportamentoResponse>.Criar(
            pagina.Items.Select(r => r.ParaResponse()), filtro.Page, filtro.PageSize, pagina.TotalItems);
    }

    public async Task<RelatoComportamentoResponse> ObterPorIdAsync(string id, CancellationToken cancellationToken)
    {
        var relato = await ObterEntidadeAsync(id, cancellationToken);
        return relato.ParaResponse();
    }

    public async Task<RelatoComportamentoResponse> CriarAsync(
        RelatoComportamentoRequest request, string? registradoPor, CancellationToken cancellationToken)
    {
        await GarantirPetExisteAsync(request.PetId, cancellationToken);

        var relato = new RelatoComportamento(
            request.PetId,
            request.Categoria,
            request.Descricao,
            request.Intensidade,
            request.Tags,
            request.DataRelato,
            registradoPor);

        var criado = await _relatoRepository.AdicionarAsync(relato, cancellationToken);
        _logger.LogInformation(
            "Relato {RelatoId} ({Categoria}) registrado para o pet {PetId}.", criado.Id, criado.Categoria, criado.PetId);
        return criado.ParaResponse();
    }

    public async Task<RelatoComportamentoResponse> AtualizarAsync(
        string id, RelatoComportamentoRequest request, CancellationToken cancellationToken)
    {
        var relato = await ObterEntidadeAsync(id, cancellationToken);
        await GarantirPetExisteAsync(request.PetId, cancellationToken);

        relato.Atualizar(
            request.PetId, request.Categoria, request.Descricao, request.Intensidade, request.Tags, request.DataRelato);

        if (!await _relatoRepository.AtualizarAsync(relato, cancellationToken))
        {
            throw new NotFoundException(Recurso, id);
        }

        _logger.LogInformation("Relato {RelatoId} atualizado com sucesso.", id);
        return relato.ParaResponse();
    }

    public async Task RemoverAsync(string id, CancellationToken cancellationToken)
    {
        if (!await _relatoRepository.RemoverAsync(id, cancellationToken))
        {
            throw new NotFoundException(Recurso, id);
        }

        _logger.LogInformation("Relato {RelatoId} removido com sucesso.", id);
    }

    private async Task<RelatoComportamento> ObterEntidadeAsync(string id, CancellationToken cancellationToken)
    {
        return await _relatoRepository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new NotFoundException(Recurso, id);
    }

    private async Task GarantirPetExisteAsync(int petId, CancellationToken cancellationToken)
    {
        if (!await _petRepository.ExisteAsync(petId, cancellationToken))
        {
            throw new ValidacaoException(
                nameof(RelatoComportamentoRequest.PetId), $"PetId {petId} não corresponde a um pet cadastrado.");
        }
    }
}

internal static class RelatoComportamentoMapeamento
{
    public static RelatoComportamentoResponse ParaResponse(this RelatoComportamento relato) => new()
    {
        Id = relato.Id ?? string.Empty,
        PetId = relato.PetId,
        Categoria = relato.Categoria,
        Descricao = relato.Descricao,
        Intensidade = relato.Intensidade,
        Tags = relato.Tags.ToList(),
        DataRelato = relato.DataRelato,
        RegistradoPor = relato.RegistradoPor,
        CriadoEm = relato.CriadoEm
    };
}
