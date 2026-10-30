using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Checkins;
using KuraVet.Application.Common;
using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Infrastructure.Persistence.Repositories;

public sealed class CheckinRepository : RepositoryBase<CheckinHistorico>, ICheckinRepository
{
    public CheckinRepository(KuraVetDbContext context) : base(context)
    {
    }

    public Task<PagedList<CheckinHistorico>> ListarAsync(CheckinFiltro filtro, CancellationToken cancellationToken)
    {
        var consulta = Set.AsNoTracking();

        if (filtro.PetId.HasValue)
        {
            var petId = filtro.PetId.Value;
            consulta = consulta.Where(c => c.PetId == petId);
        }

        if (!string.IsNullOrWhiteSpace(filtro.NivelRisco))
        {
            var nivel = filtro.NivelRisco;
            consulta = consulta.Where(c => c.NivelRiscoIA == nivel);
        }

        if (filtro.DataInicio.HasValue)
        {
            var inicio = filtro.DataInicio.Value;
            consulta = consulta.Where(c => c.DataCheckin >= inicio);
        }

        if (filtro.DataFim.HasValue)
        {
            var fim = filtro.DataFim.Value;
            consulta = consulta.Where(c => c.DataCheckin <= fim);
        }

        var desc = filtro.EhDescendente();
        return consulta.PaginarAsync(
            q => filtro.CampoOrdenacaoNormalizado() switch
            {
                "datacheckin" => desc ? q.OrderByDescending(c => c.DataCheckin) : q.OrderBy(c => c.DataCheckin),
                "frequenciarespiratoria" => desc
                    ? q.OrderByDescending(c => c.FrequenciaRespiratoria)
                    : q.OrderBy(c => c.FrequenciaRespiratoria),
                "tempopreenchimentocapilar" => desc
                    ? q.OrderByDescending(c => c.TempoPreenchimentoCapilar)
                    : q.OrderBy(c => c.TempoPreenchimentoCapilar),
                "nivelriscoia" => desc ? q.OrderByDescending(c => c.NivelRiscoIA) : q.OrderBy(c => c.NivelRiscoIA),
                _ => desc ? q.OrderByDescending(c => c.Id) : q.OrderBy(c => c.Id)
            },
            filtro,
            cancellationToken);
    }
}
