using KuraVet.Api.Extensions;
using KuraVet.Application.Auth;
using KuraVet.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace KuraVet.Api.Controllers;

/// Autenticação JWT e gestão de usuários.
[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _service;

    public AuthController(IAuthService service)
    {
        _service = service;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Autentica e emite um token JWT",
        Description = "Use o accessToken no botão Authorize do Swagger (apenas o token, sem o prefixo Bearer).")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _service.LoginAsync(request, cancellationToken));
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Autocadastro de tutor (perfil Tutor)")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Registrar(
        [FromBody] RegistroRequest request, CancellationToken cancellationToken)
    {
        var usuario = await _service.RegistrarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterUsuario), new { id = usuario.Id }, usuario);
    }

    [HttpPost("usuarios")]
    [Authorize(Roles = Perfis.Admin)]
    [SwaggerOperation(Summary = "Cria usuário com perfil escolhido (somente Admin)")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> CriarUsuario(
        [FromBody] CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        var usuario = await _service.CriarUsuarioAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterUsuario), new { id = usuario.Id }, usuario);
    }

    [HttpGet("usuarios/{id:int}")]
    [Authorize(Roles = Perfis.Admin)]
    [SwaggerOperation(Summary = "Obtém um usuário pelo ID (somente Admin)")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> ObterUsuario(int id, CancellationToken cancellationToken)
    {
        return Ok(await _service.ObterUsuarioAsync(id, cancellationToken));
    }

    [HttpGet("me")]
    [Authorize]
    [SwaggerOperation(Summary = "Dados do usuário autenticado")]
    [ProducesResponseType(typeof(UsuarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioResponse>> Me(CancellationToken cancellationToken)
    {
        var usuarioId = User.ObterUsuarioId();
        if (usuarioId is null)
        {
            return Unauthorized();
        }

        return Ok(await _service.ObterUsuarioAsync(usuarioId.Value, cancellationToken));
    }
}
