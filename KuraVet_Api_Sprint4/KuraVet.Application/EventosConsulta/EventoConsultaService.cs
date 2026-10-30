using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace KuraVet.Application.EventosConsulta;

public interface IEventoConsultaService
{
    Task<PagedResult<EventoConsultaResponse>> ListarAsync(EventoConsultaFiltro filtro, CancellationToken cancellationToken);

    Task<PagedResult<EventoConsultaResponse>> ListarPorPetAsync(int petId, EventoConsultaFiltro filtro, CancellationToken cancellationToken);

    Task<EventoConsultaResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<EventoConsultaResponse> CriarAsync(EventoConsultaRequest request, CancellationToken cancellationToken);

    Task<EventoConsultaResponse> AtualizarAsync(int id, EventoConsultaRequest request, CancellationToken cancellationToken);

    Task RemoverAsync(int id, CancellationToken cancellationToken);
}

public sealed class EventoConsultaService : IEventoConsultaService
{
    private const string Recurso = "Evento clínico";

    private readonly IEventoConsultaRepository _eventoRepository;
    private readonly IPetRepository _petRepository;
    private readonly ILogger<EventoConsultaService> _logger;

    public EventoConsultaService(
        IEventoConsultaRepository eventoRepository,
        IPetRepository petRepository,
        ILogger<EventoConsultaService> logger)
    {
        _eventoRepository = eventoRepository;
        _petRepository = petRepository;
        _logger = logger;
    }

    public async Task<PagedResult<EventoConsultaResponse>> ListarAsync(EventoConsultaFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.ValidarOrdenacao();
        var pagina = await _eventoRepository.ListarAsync(filtro, cancellationToken);
        return PagedResult<EventoConsultaResponse>.Criar(
            pagina.Items.Select(e => e.ParaResponse()), filtro.Page, filtro.PageSize, pagina.TotalItems);
    }

    public async Task<PagedResult<EventoConsultaResponse>> ListarPorPetAsync(
        int petId, EventoConsultaFiltro filtro, CancellationToken cancellationToken)
    {
        if (!await _petRepository.ExisteAsync(petId, cancellationToken))
        {
            throw new NotFoundException("Pet", petId);
        }

        filtro.PetId = petId;
        return await ListarAsync(filtro, cancellationToken);
    }

    public async Task<EventoConsultaResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var evento = await ObterEntidadeAsync(id, cancellationToken);
        return evento.ParaResponse();
    }

    public async Task<EventoConsultaResponse> CriarAsync(EventoConsultaRequest request, CancellationToken cancellationToken)
    {
        await GarantirPetExisteAsync(request.PetId, cancellationToken);

        var evento = new EventoConsulta(
            request.PetId,
            request.TipoEvento,
            request.DataEvento.GetValueOrDefault(),
            request.Descricao,
            request.VeterinarioResponsavel);

        await _eventoRepository.AdicionarAsync(evento, cancellationToken);
        _logger.LogInformation(
            "Evento clínico {EventoId} ({TipoEvento}) registrado para o pet {PetId}.",
            evento.Id, evento.TipoEvento, evento.PetId);
        return evento.ParaResponse();
    }

    public async Task<EventoConsultaResponse> AtualizarAsync(
        int id, EventoConsultaRequest request, CancellationToken cancellationToken)
    {
        var evento = await ObterEntidadeAsync(id, cancellationToken);
        await GarantirPetExisteAsync(request.PetId, cancellationToken);

        evento.Atualizar(
            request.PetId,
            request.TipoEvento,
            request.DataEvento.GetValueOrDefault(),
            request.Descricao,
            request.VeterinarioResponsavel);

        await _eventoRepository.AtualizarAsync(evento, cancellationToken);
        _logger.LogInformation("Evento clínico {EventoId} atualizado com sucesso.", id);
        return evento.ParaResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken)
    {
        var evento = await ObterEntidadeAsync(id, cancellationToken);
        await _eventoRepository.RemoverAsync(evento, cancellationToken);
        _logger.LogInformation("Evento clínico {EventoId} removido com sucesso.", id);
    }

    private async Task<EventoConsulta> ObterEntidadeAsync(int id, CancellationToken cancellationToken)
    {
        return await _eventoRepository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new NotFoundException(Recurso, id);
    }

    private async Task GarantirPetExisteAsync(int petId, CancellationToken cancellationToken)
    {
        if (!await _petRepository.ExisteAsync(petId, cancellationToken))
        {
            throw new ValidacaoException(
                nameof(EventoConsultaRequest.PetId), $"PetId {petId} não corresponde a um pet cadastrado.");
        }
    }
}

internal static class EventoConsultaMapeamento
{
    public static EventoConsultaResponse ParaResponse(this EventoConsulta evento) => new()
    {
        Id = evento.Id,
        TipoEvento = evento.TipoEvento,
        DataEvento = evento.DataEvento,
        Descricao = evento.Descricao,
        VeterinarioResponsavel = evento.VeterinarioResponsavel,
        PetId = evento.PetId
    };
}
