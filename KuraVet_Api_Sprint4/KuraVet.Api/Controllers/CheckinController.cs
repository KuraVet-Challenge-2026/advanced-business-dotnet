using KuraVet.Api.Hateoas;
using KuraVet.Application.Checkins;
using KuraVet.Application.Common;
using KuraVet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KuraVet.Api.Controllers;

/// Check-ins de sinais vitais com classificação automática de risco.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class CheckinController : ControllerBase
{
    private readonly ICheckinService _service;
    private readonly IHateoasLinkBuilder _links;

    public CheckinController(ICheckinService service, IHateoasLinkBuilder links)
    {
        _service = service;
        _links = links;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista check-ins (paginado)",
        Description = "Filtros: petId, nivelRisco, dataInicio, dataFim. Ordenação (sortBy): id, dataCheckin, " +
                      "frequenciaRespiratoria, tempoPreenchimentoCapilar, nivelRiscoIA. Padrão: dataCheckin desc.")]
    [ProducesResponseType(typeof(PagedResult<CheckinResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<CheckinResponse>>> Listar(
        [FromQuery] CheckinFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarAsync(filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, c => _links.ComLinks(c)));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Obtém um check-in pelo ID")]
    [ProducesResponseType(typeof(CheckinResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CheckinResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var checkin = await _service.ObterPorIdAsync(id, cancellationToken);
        return Ok(_links.ComLinks(checkin));
    }

    [HttpGet("pet/{petId:int}")]
    [SwaggerOperation(Summary = "Histórico de check-ins de um pet (paginado, mais recentes primeiro)")]
    [ProducesResponseType(typeof(PagedResult<CheckinResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<CheckinResponse>>> ListarPorPet(
        int petId, [FromQuery] CheckinFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarPorPetAsync(petId, filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, c => _links.ComLinks(c)));
    }

    [HttpGet("risco/{nivelRisco}")]
    [SwaggerOperation(Summary = "Check-ins por nível de risco (Baixo, Moderado, Grave)")]
    [ProducesResponseType(typeof(PagedResult<CheckinResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<CheckinResponse>>> ListarPorRisco(
        string nivelRisco, [FromQuery] CheckinFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.NivelRisco = nivelRisco;
        var resultado = await _service.ListarAsync(filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, c => _links.ComLinks(c)));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Registra um check-in",
        Description = "O nível de risco é calculado automaticamente a partir do tempo de preenchimento capilar.")]
    [ProducesResponseType(typeof(CheckinResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckinResponse>> Criar(
        [FromBody] CheckinRequest request, CancellationToken cancellationToken)
    {
        var checkin = await _service.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = checkin.Id }, _links.ComLinks(checkin));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza um check-in e recalcula o risco")]
    [ProducesResponseType(typeof(CheckinResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CheckinResponse>> Atualizar(
        int id, [FromBody] CheckinRequest request, CancellationToken cancellationToken)
    {
        var checkin = await _service.AtualizarAsync(id, request, cancellationToken);
        return Ok(_links.ComLinks(checkin));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Admin)]
    [SwaggerOperation(Summary = "Remove um check-in (somente Admin)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _service.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
