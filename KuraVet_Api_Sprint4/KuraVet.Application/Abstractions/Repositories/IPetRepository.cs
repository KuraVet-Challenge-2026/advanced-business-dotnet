using KuraVet.Application.Common;
using KuraVet.Application.Pets;
using KuraVet.Domain.Entities;

namespace KuraVet.Application.Abstractions.Repositories;

public interface IPetRepository : IRepository<Pet>
{
    Task<PagedList<Pet>> ListarAsync(PetFiltro filtro, CancellationToken cancellationToken);
}
