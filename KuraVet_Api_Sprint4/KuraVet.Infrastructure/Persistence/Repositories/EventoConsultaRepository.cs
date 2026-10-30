using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.EventosConsulta;
using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Infrastructure.Persistence.Repositories;

public sealed class EventoConsultaRepository : RepositoryBase<EventoConsulta>, IEventoConsultaRepository
{
    public EventoConsultaRepository(KuraVetDbContext context) : base(context)
    {
    }

    public Task<PagedList<EventoConsulta>> ListarAsync(EventoConsultaFiltro filtro, CancellationToken cancellationToken)
    {
        var consulta = Set.AsNoTracking();

        if (filtro.PetId.HasValue)
        {
            var petId = filtro.PetId.Value;
            consulta = consulta.Where(e => e.PetId == petId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.TipoEvento))
        {
            var tipo = filtro.TipoEvento.Trim().ToLower();
            consulta = consulta.Where(e => e.TipoEvento.ToLower() == tipo);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Veterinario))
        {
            var veterinario = filtro.Veterinario.Trim().ToLower();
            consulta = consulta.Where(e => e.VeterinarioResponsavel.ToLower().Contains(veterinario));
        }

        if (filtro.DataInicio.HasValue)
        {
            var inicio = filtro.DataInicio.Value;
            consulta = consulta.Where(e => e.DataEvento >= inicio);
        }

        if (filtro.DataFim.HasValue)
        {
            var fim = filtro.DataFim.Value;
            consulta = consulta.Where(e => e.DataEvento <= fim);
        }

        var desc = filtro.EhDescendente();
        return consulta.PaginarAsync(
            q => filtro.CampoOrdenacaoNormalizado() switch
            {
                "dataevento" => desc ? q.OrderByDescending(e => e.DataEvento) : q.OrderBy(e => e.DataEvento),
                "tipoevento" => desc ? q.OrderByDescending(e => e.TipoEvento) : q.OrderBy(e => e.TipoEvento),
                _ => desc ? q.OrderByDescending(e => e.Id) : q.OrderBy(e => e.Id)
            },
            filtro,
            cancellationToken);
    }
}
