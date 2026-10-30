using KuraVet.Application.Checkins;
using KuraVet.Application.Common;
using KuraVet.Domain.Entities;

namespace KuraVet.Application.Abstractions.Repositories;

public interface ICheckinRepository : IRepository<CheckinHistorico>
{
    Task<PagedList<CheckinHistorico>> ListarAsync(CheckinFiltro filtro, CancellationToken cancellationToken);
}
