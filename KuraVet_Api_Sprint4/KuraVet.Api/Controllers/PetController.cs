using KuraVet.Api.Hateoas;
using KuraVet.Application.Common;
using KuraVet.Application.Pets;
using KuraVet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KuraVet.Api.Controllers;

/// Pets acompanhados pela plataforma.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class PetController : ControllerBase
{
    private readonly IPetService _service;
    private readonly IHateoasLinkBuilder _links;

    public PetController(IPetService service, IHateoasLinkBuilder links)
    {
        _service = service;
        _links = links;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista pets (paginado)",
        Description = "Filtros: nome, tutorId. Ordenação (sortBy): id, nome, tutorId.")]
    [ProducesResponseType(typeof(PagedResult<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<PetResponse>>> Listar(
        [FromQuery] PetFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarAsync(filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, p => _links.ComLinks(p)));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Obtém um pet pelo ID")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PetResponse>> ObterPorId(int id, CancellationToken cancellationToken)
    {
        var pet = await _service.ObterPorIdAsync(id, cancellationToken);
        return Ok(_links.ComLinks(pet));
    }

    [HttpGet("tutor/{tutorId:int}")]
    [SwaggerOperation(Summary = "Lista os pets de um tutor (paginado)")]
    [ProducesResponseType(typeof(PagedResult<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<PetResponse>>> ListarPorTutor(
        int tutorId, [FromQuery] PetFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarPorTutorAsync(tutorId, filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, p => _links.ComLinks(p)));
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Cadastra um pet vinculado a um tutor")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PetResponse>> Criar([FromBody] PetRequest request, CancellationToken cancellationToken)
    {
        var pet = await _service.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = pet.Id }, _links.ComLinks(pet));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation(Summary = "Atualiza um pet")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PetResponse>> Atualizar(
        int id, [FromBody] PetRequest request, CancellationToken cancellationToken)
    {
        var pet = await _service.AtualizarAsync(id, request, cancellationToken);
        return Ok(_links.ComLinks(pet));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Perfis.Admin)]
    [SwaggerOperation(Summary = "Remove um pet (somente Admin)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _service.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
