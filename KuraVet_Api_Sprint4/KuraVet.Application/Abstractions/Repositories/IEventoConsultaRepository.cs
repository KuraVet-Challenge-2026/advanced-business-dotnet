using KuraVet.Application.Common;
using KuraVet.Application.EventosConsulta;
using KuraVet.Domain.Entities;

namespace KuraVet.Application.Abstractions.Repositories;

public interface IEventoConsultaRepository : IRepository<EventoConsulta>
{
    Task<PagedList<EventoConsulta>> ListarAsync(EventoConsultaFiltro filtro, CancellationToken cancellationToken);
}
