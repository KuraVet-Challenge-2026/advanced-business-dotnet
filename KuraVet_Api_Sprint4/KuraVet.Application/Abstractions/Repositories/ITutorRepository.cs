using KuraVet.Application.Common;
using KuraVet.Application.Tutores;
using KuraVet.Domain.Entities;

namespace KuraVet.Application.Abstractions.Repositories;

public interface ITutorRepository : IRepository<Tutor>
{
    Task<PagedList<Tutor>> ListarAsync(TutorFiltro filtro, CancellationToken cancellationToken);
}
