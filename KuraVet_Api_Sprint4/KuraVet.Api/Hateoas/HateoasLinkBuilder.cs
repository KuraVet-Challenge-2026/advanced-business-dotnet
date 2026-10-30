using System.Globalization;
using KuraVet.Api.Controllers;
using KuraVet.Application.Checkins;
using KuraVet.Application.Common;
using KuraVet.Application.EventosConsulta;
using KuraVet.Application.Pets;
using KuraVet.Application.RelatosComportamento;
using KuraVet.Application.Tutores;
using Microsoft.AspNetCore.Http.Extensions;

namespace KuraVet.Api.Hateoas;

public sealed class HateoasLinkBuilder : IHateoasLinkBuilder
{
    private const string Tutor = "Tutor";
    private const string Pet = "Pet";
    private const string Checkin = "Checkin";
    private const string EventoConsulta = "EventoConsulta";
    private const string Relato = "RelatoComportamento";

    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HateoasLinkBuilder(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
    {
        _linkGenerator = linkGenerator;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext Contexto => _httpContextAccessor.HttpContext
        ?? throw new InvalidOperationException("A geração de links HATEOAS requer uma requisição HTTP ativa.");

    public TutorResponse ComLinks(TutorResponse tutor)
    {
        tutor.Links.Clear();
        var id = new { id = tutor.Id };
        Adicionar(tutor, "self", "GET", nameof(TutorController.ObterPorId), Tutor, id);
        Adicionar(tutor, "update", "PUT", nameof(TutorController.Atualizar), Tutor, id);
        Adicionar(tutor, "delete", "DELETE", nameof(TutorController.Remover), Tutor, id);
        Adicionar(tutor, "pets", "GET", nameof(PetController.ListarPorTutor), Pet, new { tutorId = tutor.Id });
        return tutor;
    }

    public PetResponse ComLinks(PetResponse pet)
    {
        pet.Links.Clear();
        var id = new { id = pet.Id };
        Adicionar(pet, "self", "GET", nameof(PetController.ObterPorId), Pet, id);
        Adicionar(pet, "update", "PUT", nameof(PetController.Atualizar), Pet, id);
        Adicionar(pet, "delete", "DELETE", nameof(PetController.Remover), Pet, id);
        Adicionar(pet, "tutor", "GET", nameof(TutorController.ObterPorId), Tutor, new { id = pet.TutorId });
        Adicionar(pet, "checkins", "GET", nameof(CheckinController.ListarPorPet), Checkin, new { petId = pet.Id });
        Adicionar(pet, "timeline", "GET", nameof(EventoConsultaController.ListarPorPet), EventoConsulta, new { petId = pet.Id });
        Adicionar(pet, "relatos", "GET", nameof(RelatoComportamentoController.Listar), Relato, new { petId = pet.Id });
        return pet;
    }

    public CheckinResponse ComLinks(CheckinResponse checkin)
    {
        checkin.Links.Clear();
        var id = new { id = checkin.Id };
        Adicionar(checkin, "self", "GET", nameof(CheckinController.ObterPorId), Checkin, id);
        Adicionar(checkin, "update", "PUT", nameof(CheckinController.Atualizar), Checkin, id);
        Adicionar(checkin, "delete", "DELETE", nameof(CheckinController.Remover), Checkin, id);
        Adicionar(checkin, "pet", "GET", nameof(PetController.ObterPorId), Pet, new { id = checkin.PetId });
        return checkin;
    }

    public EventoConsultaResponse ComLinks(EventoConsultaResponse evento)
    {
        evento.Links.Clear();
        var id = new { id = evento.Id };
        Adicionar(evento, "self", "GET", nameof(EventoConsultaController.ObterPorId), EventoConsulta, id);
        Adicionar(evento, "update", "PUT", nameof(EventoConsultaController.Atualizar), EventoConsulta, id);
        Adicionar(evento, "delete", "DELETE", nameof(EventoConsultaController.Remover), EventoConsulta, id);
        Adicionar(evento, "pet", "GET", nameof(PetController.ObterPorId), Pet, new { id = evento.PetId });
        return evento;
    }

    public RelatoComportamentoResponse ComLinks(RelatoComportamentoResponse relato)
    {
        relato.Links.Clear();
        var id = new { id = relato.Id };
        Adicionar(relato, "self", "GET", nameof(RelatoComportamentoController.ObterPorId), Relato, id);
        Adicionar(relato, "update", "PUT", nameof(RelatoComportamentoController.Atualizar), Relato, id);
        Adicionar(relato, "delete", "DELETE", nameof(RelatoComportamentoController.Remover), Relato, id);
        Adicionar(relato, "pet", "GET", nameof(PetController.ObterPorId), Pet, new { id = relato.PetId });
        return relato;
    }

    public PagedResult<T> ComLinks<T>(PagedResult<T> pagina, Func<T, T> linksDoItem)
    {
        foreach (var item in pagina.Items)
        {
            linksDoItem(item);
        }

        pagina.Links.Clear();
        pagina.Links.Add(new LinkDto("self", UrlDaPagina(pagina.Page, pagina.PageSize), "GET"));

        if (pagina.TotalPages > 0)
        {
            pagina.Links.Add(new LinkDto("first", UrlDaPagina(1, pagina.PageSize), "GET"));
            pagina.Links.Add(new LinkDto("last", UrlDaPagina(pagina.TotalPages, pagina.PageSize), "GET"));
        }

        if (pagina.Page > 1 && pagina.TotalPages > 0)
        {
            var anterior = Math.Min(pagina.Page - 1, pagina.TotalPages);
            pagina.Links.Add(new LinkDto("prev", UrlDaPagina(anterior, pagina.PageSize), "GET"));
        }

        if (pagina.Page < pagina.TotalPages)
        {
            pagina.Links.Add(new LinkDto("next", UrlDaPagina(pagina.Page + 1, pagina.PageSize), "GET"));
        }

        return pagina;
    }

    private void Adicionar(RecursoComLinks recurso, string rel, string metodo, string action, string controller, object valores)
    {
        var href = _linkGenerator.GetUriByAction(Contexto, action, controller, valores);
        if (href is not null)
        {
            recurso.Links.Add(new LinkDto(rel, href, metodo));
        }
    }

    /// Reaproveita a URL atual 
    private string UrlDaPagina(int page, int pageSize)
    {
        var request = Contexto.Request;
        var query = request.Query.ToDictionary(
            q => q.Key,
            q => (string?)q.Value.ToString(),
            StringComparer.OrdinalIgnoreCase);

        query["page"] = page.ToString(CultureInfo.InvariantCulture);
        query["pageSize"] = pageSize.ToString(CultureInfo.InvariantCulture);

        return UriHelper.BuildAbsolute(
            request.Scheme, request.Host, request.PathBase, request.Path, QueryString.Create(query));
    }
}
