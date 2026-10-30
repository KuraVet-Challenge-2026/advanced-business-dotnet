using KuraVet.Api.Extensions;
using KuraVet.Api.Hateoas;
using KuraVet.Application.Common;
using KuraVet.Application.RelatosComportamento;
using KuraVet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KuraVet.Api.Controllers;

/// Relatos de comportamento do pet armazenados no MongoDB.
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public sealed class RelatoComportamentoController : ControllerBase
{
    private readonly IRelatoComportamentoService _service;
    private readonly IHateoasLinkBuilder _links;

    public RelatoComportamentoController(IRelatoComportamentoService service, IHateoasLinkBuilder links)
    {
        _service = service;
        _links = links;
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Lista relatos de comportamento (paginado)",
        Description = "Filtros: petId, categoria, intensidadeMinima, dataInicio, dataFim. " +
                      "Ordenação (sortBy): dataRelato, intensidade, categoria, criadoEm. Padrão: dataRelato desc.")]
    [ProducesResponseType(typeof(PagedResult<RelatoComportamentoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<RelatoComportamentoResponse>>> Listar(
        [FromQuery] RelatoComportamentoFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _service.ListarAsync(filtro, cancellationToken);
        return Ok(_links.ComLinks(resultado, r => _links.ComLinks(r)));
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Obtém um relato pelo ID (ObjectId do MongoDB)")]
    [ProducesResponseType(typeof(RelatoComportamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelatoComportamentoResponse>> ObterPorId(string id, CancellationToken cancellationToken)
    {
        var relato = await _service.ObterPorIdAsync(id, cancellationToken);
        return Ok(_links.ComLinks(relato));
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Registra um relato de comportamento",
        Description = "Categorias: Alimentacao, Sono, Atividade, Comportamento, Higiene, Outro. Intensidade de 1 a 5.")]
    [ProducesResponseType(typeof(RelatoComportamentoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RelatoComportamentoResponse>> Criar(
        [FromBody] RelatoComportamentoRequest request, CancellationToken cancellationToken)
    {
        var relato = await _service.CriarAsync(request, User.ObterEmail(), cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = relato.Id }, _links.ComLinks(relato));
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Atualiza um relato de comportamento")]
    [ProducesResponseType(typeof(RelatoComportamentoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RelatoComportamentoResponse>> Atualizar(
        string id, [FromBody] RelatoComportamentoRequest request, CancellationToken cancellationToken)
    {
        var relato = await _service.AtualizarAsync(id, request, cancellationToken);
        return Ok(_links.ComLinks(relato));
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Perfis.Admin)]
    [SwaggerOperation(Summary = "Remove um relato (somente Admin)")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id, CancellationToken cancellationToken)
    {
        await _service.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
