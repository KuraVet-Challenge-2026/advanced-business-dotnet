using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace KuraVet.Application.Pets;

public interface IPetService
{
    Task<PagedResult<PetResponse>> ListarAsync(PetFiltro filtro, CancellationToken cancellationToken);

    Task<PagedResult<PetResponse>> ListarPorTutorAsync(int tutorId, PetFiltro filtro, CancellationToken cancellationToken);

    Task<PetResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<PetResponse> CriarAsync(PetRequest request, CancellationToken cancellationToken);

    Task<PetResponse> AtualizarAsync(int id, PetRequest request, CancellationToken cancellationToken);

    Task RemoverAsync(int id, CancellationToken cancellationToken);
}

public sealed class PetService : IPetService
{
    private const string Recurso = "Pet";

    private readonly IPetRepository _petRepository;
    private readonly ITutorRepository _tutorRepository;
    private readonly ILogger<PetService> _logger;

    public PetService(IPetRepository petRepository, ITutorRepository tutorRepository, ILogger<PetService> logger)
    {
        _petRepository = petRepository;
        _tutorRepository = tutorRepository;
        _logger = logger;
    }

    public async Task<PagedResult<PetResponse>> ListarAsync(PetFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.ValidarOrdenacao();
        var pagina = await _petRepository.ListarAsync(filtro, cancellationToken);
        return PagedResult<PetResponse>.Criar(
            pagina.Items.Select(p => p.ParaResponse()), filtro.Page, filtro.PageSize, pagina.TotalItems);
    }

    public async Task<PagedResult<PetResponse>> ListarPorTutorAsync(int tutorId, PetFiltro filtro, CancellationToken cancellationToken)
    {
        if (!await _tutorRepository.ExisteAsync(tutorId, cancellationToken))
        {
            throw new NotFoundException("Tutor", tutorId);
        }

        filtro.TutorId = tutorId;
        return await ListarAsync(filtro, cancellationToken);
    }

    public async Task<PetResponse> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        var pet = await ObterEntidadeAsync(id, cancellationToken);
        return pet.ParaResponse();
    }

    public async Task<PetResponse> CriarAsync(PetRequest request, CancellationToken cancellationToken)
    {
        await GarantirTutorExisteAsync(request.TutorId, cancellationToken);

        var pet = new Pet(request.Nome, request.TutorId);
        await _petRepository.AdicionarAsync(pet, cancellationToken);
        _logger.LogInformation("Pet {PetId} cadastrado para o tutor {TutorId}.", pet.Id, pet.TutorId);
        return pet.ParaResponse();
    }

    public async Task<PetResponse> AtualizarAsync(int id, PetRequest request, CancellationToken cancellationToken)
    {
        var pet = await ObterEntidadeAsync(id, cancellationToken);
        await GarantirTutorExisteAsync(request.TutorId, cancellationToken);

        pet.Atualizar(request.Nome, request.TutorId);
        await _petRepository.AtualizarAsync(pet, cancellationToken);
        _logger.LogInformation("Pet {PetId} atualizado com sucesso.", id);
        return pet.ParaResponse();
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken)
    {
        var pet = await ObterEntidadeAsync(id, cancellationToken);
        await _petRepository.RemoverAsync(pet, cancellationToken);
        _logger.LogInformation("Pet {PetId} removido com sucesso.", id);
    }

    private async Task<Pet> ObterEntidadeAsync(int id, CancellationToken cancellationToken)
    {
        return await _petRepository.ObterPorIdAsync(id, cancellationToken)
               ?? throw new NotFoundException(Recurso, id);
    }

    private async Task GarantirTutorExisteAsync(int tutorId, CancellationToken cancellationToken)
    {
        if (!await _tutorRepository.ExisteAsync(tutorId, cancellationToken))
        {
            throw new ValidacaoException(nameof(PetRequest.TutorId), $"TutorId {tutorId} não corresponde a um tutor cadastrado.");
        }
    }
}

internal static class PetMapeamento
{
    public static PetResponse ParaResponse(this Pet pet) => new()
    {
        Id = pet.Id,
        Nome = pet.Nome,
        TutorId = pet.TutorId
    };
}
