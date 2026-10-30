using KuraVet.Api.Hateoas;
using KuraVet.Application.Common;
using KuraVet.Application.Tutores;
using KuraVet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KuraVet.Api.Controllers;

/// Tutores responsáveis pelos pets.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class TutorController : ControllerBase
{
    private readonly ITutorService _service;
    private readonly IHateoasLinkBuilder _links;

    public TutorController(ITutorService service, IHateoasLinkBuilder links)
    {
        _service = service;
        _links = links;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista tutores (paginado)",
        Description = "Filtros: nome. Ordenação (sortBy): id, nome. Ex.: ?page=1&pageSize=10&sortBy=nome&sortDir=asc&nome=ana")]
    [ProducesResponseType(typeof(PagedResult<TutorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<TutorResponse>>> Listar(
        [FromQuery] TutorFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarAsync(filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, t => _links.ComLinks(t)));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Obtém um tutor pelo ID")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TutorResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var tutor = await _service.ObterPorIdAsync(id, cancellationToken);
        return Ok(_links.ComLinks(tutor));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra um tutor")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TutorResponse>> Criar(
        [FromBody] TutorRequest request, CancellationToken cancellationToken)
    {
        var tutor = await _service.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = tutor.Id }, _links.ComLinks(tutor));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza um tutor")]
    [ProducesResponseType(typeof(TutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TutorResponse>> Atualizar(
        int id, [FromBody] TutorRequest request, CancellationToken cancellationToken)
    {
        var tutor = await _service.AtualizarAsync(id, request, cancellationToken);
        return Ok(_links.ComLinks(tutor));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Admin)]
    [SwaggerOperation(Summary = "Remove um tutor (somente Admin)", Description = "Remove também os pets vinculados (cascade).")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _service.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
