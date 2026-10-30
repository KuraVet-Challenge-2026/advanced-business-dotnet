using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Tutores;
using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Infrastructure.Persistence.Repositories;

public sealed class TutorRepository : RepositoryBase<Tutor>, ITutorRepository
{
    public TutorRepository(KuraVetDbContext context) : base(context)
    {
    }

    public Task<PagedList<Tutor>> ListarAsync(TutorFiltro filtro, CancellationToken cancellationToken)
    {
        var consulta = Set.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Nome))
        {
            var nome = filtro.Nome.Trim().ToLower();
            consulta = consulta.Where(t => t.Nome.ToLower().Contains(nome));
        }

        var desc = filtro.EhDescendente();
        return consulta.PaginarAsync(
            q => filtro.CampoOrdenacaoNormalizado() switch
            {
                "nome" => desc ? q.OrderByDescending(t => t.Nome) : q.OrderBy(t => t.Nome),
                _ => desc ? q.OrderByDescending(t => t.Id) : q.OrderBy(t => t.Id)
            },
            filtro,
            cancellationToken);
    }
}
