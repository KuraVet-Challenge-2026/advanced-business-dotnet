using KuraVet.Domain.Entities;
using KuraVet.Domain.Exceptions;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain;

public class UsuarioTests
{
    [Fact]
    public void Construtor_DadosValidos_DeveNormalizarEmailEPerfil()
    {
        // Act
        var usuario = new Usuario("Ana", "  Ana@KuraVet.COM ", "hash-bcrypt", "veterinario");

        // Assert
        Assert.Equal("ana@kuravet.com", usuario.Email);
        Assert.Equal(Perfis.Veterinario, usuario.Perfil);
        Assert.True(usuario.DataCriacao <= DateTime.UtcNow);
    }

    [Theory]
    [InlineData("sem-arroba")]
    [InlineData("ana@")]
    [InlineData("@kuravet.com")]
    public void Construtor_EmailInvalido_DeveLancarDomainException(string email)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Usuario("Ana", email, "hash", Perfis.Tutor));
    }

    [Fact]
    public void Construtor_PerfilInexistente_DeveLancarDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Usuario("Ana", "ana@kuravet.com", "hash", "Gerente"));
        Assert.Contains("Perfil", ex.Message);
    }

    [Fact]
    public void Construtor_SenhaHashVazia_DeveLancarDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Usuario("Ana", "ana@kuravet.com", "", Perfis.Tutor));
    }
}
