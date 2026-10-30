using KuraVet.Application.Checkins;
using KuraVet.Application.Common;
using KuraVet.Application.EventosConsulta;
using KuraVet.Application.Pets;
using KuraVet.Application.RelatosComportamento;
using KuraVet.Application.Tutores;

namespace KuraVet.Api.Hateoas;

/// Preenche os links HATEOAS
public interface IHateoasLinkBuilder
{
    TutorResponse ComLinks(TutorResponse tutor);

    PetResponse ComLinks(PetResponse pet);

    CheckinResponse ComLinks(CheckinResponse checkin);

    EventoConsultaResponse ComLinks(EventoConsultaResponse evento);

    RelatoComportamentoResponse ComLinks(RelatoComportamentoResponse relato);

    PagedResult<T> ComLinks<T>(PagedResult<T> pagina, Func<T, T> linksDoItem);
}
