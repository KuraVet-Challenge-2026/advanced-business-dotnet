using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace KuraVet.Application.Tutores;

public interface ITutorService
{
    Task<PagedResult<TutorResponse>> ListarAsync(TutorFiltro filtro, CancellationToken cancellationToken);

    Task<TutorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<TutorResponse> CriarAsync(TutorRequest request, CancellationToken cancellationToken);

    Task<TutorResponse> AtualizarAsync(int id, TutorRequest request, CancellationToken cancellationToken);

    Task RemoverAsync(int id, CancellationToken cancellationToken);
}

public sealed class TutorService : ITutorService
{
    private const string Recurso = "Tutor";

    private readonly ITutorRepository _repository;
    private readonly ILogger<TutorService> _logger;

    public TutorService(ITutorRepository repository, ILogger<TutorService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResult<TutorResponse>> ListarAsync(TutorFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.ValidarOrdenacao();
        var pagina = await _repository.ListarAsync(filtro, cancellationToken);
        return PagedResult<TutorResponse>.Criar(
            pagina.Items.Select(t => t.ParaResponse()), filtro.Page, filtro.PageSize, pagina.TotalItems);
    }

    public async Task<TutorResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var tutor = await ObterEntidadeAsync(id, cancellationToken);
        return tutor.ParaResponse();
    }

    public async Task<TutorResponse> CriarAsync(TutorRequest request, CancellationToken cancellationToken)
    {
        var tutor = new Tutor(request.Nome);
        await _repository.AdicionarAsync(tutor, cancellationToken);
        _logger.LogInformation("Tutor {TutorId} cadastrado com sucesso.", tutor.Id);
        return tutor.ParaResponse();
    }

    public async Task<TutorResponse> AtualizarAsync(int id, TutorRequest request, CancellationToken cancellationToken)
    {
        var tutor = await ObterEntidadeAsync(id, cancellationToken);
        tutor.AlterarNome(request.Nome);
        await _repository.AtualizarAsync(tutor, cancellationToken);
        _logger.LogInformation("Tutor {TutorId} atualizado com sucesso.", id);
        return tutor.ParaResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken)
    {
        var tutor = await ObterEntidadeAsync(id, cancellationToken);
        await _repository.RemoverAsync(tutor, cancellationToken);
        _logger.LogInformation("Tutor {TutorId} removido com sucesso.", id);
    }

    private async Task<Tutor> ObterEntidadeAsync(int id, CancellationToken cancellationToken)
    {
        return await _repository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new NotFoundException(Recurso, id);
    }
}

internal static class TutorMapeamento
{
    public static TutorResponse ParaResponse(this Tutor tutor) => new()
    {
        Id = tutor.Id,
        Nome = tutor.Nome
    };
}
