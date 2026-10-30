using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Pets;
using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Infrastructure.Persistence.Repositories;

public sealed class PetRepository : RepositoryBase<Pet>, IPetRepository
{
    public PetRepository(KuraVetDbContext context) : base(context)
    {
    }

    public Task<PagedList<Pet>> ListarAsync(PetFiltro filtro, CancellationToken cancellationToken)
    {
        var consulta = Set.AsNoTracking();

        if (filtro.TutorId.HasValue)
        {
            var tutorId = filtro.TutorId.Value;
            consulta = consulta.Where(p => p.TutorId == tutorId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToLower();
            consulta = consulta.Where(p => p.Nome.ToLower().Contains(nome));
        }

        var desc = filtro.EhDescendente();
        return consulta.PaginarAsync(
            q => filtro.CampoOrdenacaoNormalizado() switch
            {
                "nome" => desc ? q.OrderByDescending(p => p.Nome) : q.OrderBy(p => p.Nome),
                "tutorid" => desc ? q.OrderByDescending(p => p.TutorId) : q.OrderBy(p => p.TutorId),
                _ => desc ? q.OrderByDescending(p => p.Id) : q.OrderBy(p => p.Id)
            },
            filtro,
            cancellationToken);
    }
}
