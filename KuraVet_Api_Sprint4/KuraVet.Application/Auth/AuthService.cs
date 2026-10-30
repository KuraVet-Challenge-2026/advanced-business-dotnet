using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Abstractions.Security;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace KuraVet.Application.Auth;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<UsuarioResponse> RegistrarAsync(RegistroRequest request, CancellationToken cancellationToken);

    Task<UsuarioResponse> CriarUsuarioAsync(CriarUsuarioRequest request, CancellationToken cancellationToken);

    Task<UsuarioResponse> ObterUsuarioAsync(int id, CancellationToken cancellationToken);

    /// Cria o administrador inicial se o e-mail ainda não existir. Retorna true quando criou.
    Task<bool> GarantirAdministradorAsync(string nome, string email, string senha, CancellationToken cancellationToken);
}

public sealed class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var usuario = await _usuarioRepository.ObterPorEmailAsync(email, cancellationToken);

        if (usuario is null || !_passwordHasher.Verificar(request.Senha, usuario.SenhaHash))
        {
            _logger.LogWarning("Tentativa de login inválida para o e-mail informado.");
            throw new CredenciaisInvalidasException();
        }

        var token = _tokenService.GerarToken(usuario);
        _logger.LogInformation("Usuário {UsuarioId} autenticado com perfil {Perfil}.", usuario.Id, usuario.Perfil);

        return new TokenResponse
        {
            AccessToken = token.AccessToken,
            TokenType = "Bearer",
            ExpiraEm = token.ExpiraEm,
            Usuario = usuario.ParaResponse()
        };
    }

    public Task<UsuarioResponse> RegistrarAsync(RegistroRequest request, CancellationToken cancellationToken)
    {
        return CriarAsync(request.Nome, request.Email, request.Senha, Perfis.Tutor, cancellationToken);
    }

    public Task<UsuarioResponse> CriarUsuarioAsync(CriarUsuarioRequest request, CancellationToken cancellationToken)
    {
        return CriarAsync(request.Nome, request.Email, request.Senha, request.Perfil, cancellationToken);
    }

    public async Task<UsuarioResponse> ObterUsuarioAsync(int id, CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObterPorIdAsync(id, cancellationToken)
                      ?? throw new NotFoundException("Usuário", id);
        return usuario.ParaResponse();
    }

    public async Task<bool> GarantirAdministradorAsync(string nome, string email, string senha, CancellationToken cancellationToken)
    {
        var emailNormalizado = Usuario.NormalizarEmail(email);
        if (await _usuarioRepository.EmailExisteAsync(emailNormalizado, cancellationToken))
        {
            return false;
        }

        await CriarAsync(nome, emailNormalizado, senha, Perfis.Admin, cancellationToken);
        return true;
    }

    private async Task<UsuarioResponse> CriarAsync(
        string nome, string email, string senha, string perfil, CancellationToken cancellationToken)
    {
        var emailNormalizado = Usuario.NormalizarEmail(email);
        if (await _usuarioRepository.EmailExisteAsync(emailNormalizado, cancellationToken))
        {
            throw new ConflitoException("Já existe um usuário cadastrado com este e-mail.");
        }

        var usuario = new Usuario(nome, emailNormalizado, _passwordHasher.Hash(senha), perfil);
        await _usuarioRepository.AdicionarAsync(usuario, cancellationToken);
        _logger.LogInformation("Usuário {UsuarioId} criado com perfil {Perfil}.", usuario.Id, usuario.Perfil);
        return usuario.ParaResponse();
    }
}

internal static class UsuarioMapeamento
{
    public static UsuarioResponse ParaResponse(this Usuario usuario) => new()
    {
        Id = usuario.Id,
        Nome = usuario.Nome,
        Email = usuario.Email,
        Perfil = usuario.Perfil
    };
}
