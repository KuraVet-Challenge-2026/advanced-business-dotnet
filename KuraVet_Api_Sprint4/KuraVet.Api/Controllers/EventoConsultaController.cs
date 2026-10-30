using KuraVet.Api.Hateoas;
using KuraVet.Application.Common;
using KuraVet.Application.EventosConsulta;
using KuraVet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KuraVet.Api.Controllers;

/// Linha do tempo clínica: vacinas, exames, consultas, retornos e cirurgias.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class EventoConsultaController : ControllerBase
{
    private const string PerfisClinicos = Perfis.Admin + "," + Perfis.Veterinario;

    private readonly IEventoConsultaService _service;
    private readonly IHateoasLinkBuilder _links;

    public EventoConsultaController(IEventoConsultaService service, IHateoasLinkBuilder links)
    {
        _service = service;
        _links = links;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista eventos clínicos (paginado)",
        Description = "Filtros: petId, tipoEvento, veterinario, dataInicio, dataFim. Ordenação (sortBy): id, dataEvento, tipoEvento.")]
    [ProducesResponseType(typeof(PagedResult<EventoConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<EventoConsultaResponse>>> Listar(
        [FromQuery] EventoConsultaFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarAsync(filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, e => _links.ComLinks(e)));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Obtém um evento clínico pelo ID")]
    [ProducesResponseType(typeof(EventoConsultaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoConsultaResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var evento = await _service.ObterPorIdAsync(id, cancellationToken);
        return Ok(_links.ComLinks(evento));
    }

    [HttpGet("pet/{petId:int}")]
    [SwaggerOperation(Summary = "Linha do tempo clínica de um pet (paginada, mais recentes primeiro)")]
    [ProducesResponseType(typeof(PagedResult<EventoConsultaResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<EventoConsultaResponse>>> ListarPorPet(
        int petId, [FromQuery] EventoConsultaFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarPorPetAsync(petId, filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, e => _links.ComLinks(e)));
    }

    [HttpPost]
    [Authorize(Roles = PerfisClinicos)]
    [SwaggerOperation(Summary = "Registra um evento clínico (Admin ou Veterinario)")]
    [ProducesResponseType(typeof(EventoConsultaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<EventoConsultaResponse>> Criar(
        [FromBody] EventoConsultaRequest request, CancellationToken cancellationToken)
    {
        var evento = await _service.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = evento.Id }, _links.ComLinks(evento));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = PerfisClinicos)]
    [SwaggerOperation(Summary = "Atualiza um evento clínico (Admin ou Veterinario)")]
    [ProducesResponseType(typeof(EventoConsultaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoConsultaResponse>> Atualizar(
        int id, [FromBody] EventoConsultaRequest request, CancellationToken cancellationToken)
    {
        var evento = await _service.AtualizarAsync(id, request, cancellationToken);
        return Ok(_links.ComLinks(evento));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Admin)]
    [SwaggerOperation(Summary = "Remove um evento clínico (somente Admin)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _service.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
