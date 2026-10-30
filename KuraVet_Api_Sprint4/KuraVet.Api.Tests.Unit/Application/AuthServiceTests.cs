using KuraVet.Api.Tests.Unit.Fixtures;
using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Abstractions.Security;
using KuraVet.Application.Auth;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _sut = new AuthService(_usuarios.Object, _hasher.Object, _tokens.Object, NullLogger<AuthService>.Instance);
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hash-gerado");
    }

    [Fact]
    public async Task LoginAsync_CredenciaisValidas_DeveRetornarToken()
    {
        // Arrange
        var usuario = new Usuario("Ana", "ana@kuravet.com", "hash", Perfis.Veterinario).ComId(1);
        var expira = DateTime.UtcNow.AddHours(1);
        _usuarios.Setup(r => r.ObterPorEmailAsync("ana@kuravet.com", It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _hasher.Setup(h => h.Verificar("Senha@123", "hash")).Returns(true);
        _tokens.Setup(t => t.GerarToken(usuario)).Returns(new TokenGerado("jwt-token", expira));

        // Act
        var resposta = await _sut.LoginAsync(
            new LoginRequest { Email = " ANA@kuravet.com ", Senha = "Senha@123" }, CancellationToken.None);

        // Assert
        Assert.Equal("jwt-token", resposta.AccessToken);
        Assert.Equal("Bearer", resposta.TokenType);
        Assert.Equal(expira, resposta.ExpiraEm);
        Assert.Equal(Perfis.Veterinario, resposta.Usuario.Perfil);
    }

    [Fact]
    public async Task LoginAsync_EmailInexistente_DeveLancarCredenciaisInvalidas()
    {
        // Arrange
        _usuarios
            .Setup(r => r.ObterPorEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        // Act & Assert
        await Assert.ThrowsAsync<CredenciaisInvalidasException>(
            () => _sut.LoginAsync(new LoginRequest { Email = "x@kuravet.com", Senha = "123" }, CancellationToken.None));
        _tokens.Verify(t => t.GerarToken(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_SenhaIncorreta_DeveLancarCredenciaisInvalidas()
    {
        // Arrange
        var usuario = new Usuario("Ana", "ana@kuravet.com", "hash", Perfis.Tutor);
        _usuarios.Setup(r => r.ObterPorEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(usuario);
        _hasher.Setup(h => h.Verificar(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<CredenciaisInvalidasException>(
            () => _sut.LoginAsync(new LoginRequest { Email = "ana@kuravet.com", Senha = "errada" }, CancellationToken.None));
    }

    [Fact]
    public async Task RegistrarAsync_EmailNovo_DeveCriarUsuarioComPerfilTutorEHashDaSenha()
    {
        // Arrange
        _usuarios.Setup(r => r.EmailExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var resposta = await _sut.RegistrarAsync(
            new RegistroRequest { Nome = "Bia", Email = "Bia@KuraVet.com", Senha = "SenhaForte@1" }, CancellationToken.None);

        // Assert
        Assert.Equal(Perfis.Tutor, resposta.Perfil);
        Assert.Equal("bia@kuravet.com", resposta.Email);
        _hasher.Verify(h => h.Hash("SenhaForte@1"), Times.Once);
        _usuarios.Verify(
            r => r.AdicionarAsync(It.Is<Usuario>(u => u.SenhaHash == "hash-gerado" && u.Perfil == Perfis.Tutor), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_EmailJaCadastrado_DeveLancarConflitoException()
    {
        // Arrange
        _usuarios.Setup(r => r.EmailExisteAsync("bia@kuravet.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<ConflitoException>(
            () => _sut.RegistrarAsync(
                new RegistroRequest { Nome = "Bia", Email = "bia@kuravet.com", Senha = "SenhaForte@1" }, CancellationToken.None));
        _usuarios.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CriarUsuarioAsync_PerfilVeterinario_DeveCriarComPerfilInformado()
    {
        // Arrange
        _usuarios.Setup(r => r.EmailExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var resposta = await _sut.CriarUsuarioAsync(
            new CriarUsuarioRequest { Nome = "Dr. Paulo", Email = "paulo@kuravet.com", Senha = "SenhaForte@1", Perfil = "veterinario" },
            CancellationToken.None);

        // Assert
        Assert.Equal(Perfis.Veterinario, resposta.Perfil);
    }

    [Fact]
    public async Task GarantirAdministradorAsync_AdminJaExiste_NaoDeveCriarNovamente()
    {
        // Arrange
        _usuarios.Setup(r => r.EmailExisteAsync("admin@kuravet.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var criado = await _sut.GarantirAdministradorAsync("Admin", "admin@kuravet.com", "Admin@12345", CancellationToken.None);

        // Assert
        Assert.False(criado);
        _usuarios.Verify(r => r.AdicionarAsync(It.IsAny<Usuario>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GarantirAdministradorAsync_AdminInexistente_DeveCriarComPerfilAdmin()
    {
        // Arrange
        _usuarios.Setup(r => r.EmailExisteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var criado = await _sut.GarantirAdministradorAsync("Admin", "admin@kuravet.com", "Admin@12345", CancellationToken.None);

        // Assert
        Assert.True(criado);
        _usuarios.Verify(
            r => r.AdicionarAsync(It.Is<Usuario>(u => u.Perfil == Perfis.Admin), It.IsAny<CancellationToken>()), Times.Once);
    }
}
