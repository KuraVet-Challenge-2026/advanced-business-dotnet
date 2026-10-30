using System.IdentityModel.Tokens.Jwt;
using KuraVet.Api.Tests.Unit.Fixtures;
using KuraVet.Domain.Entities;
using KuraVet.Infrastructure.Security;
using Microsoft.Extensions.Options;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Infrastructure;

public class JwtTokenServiceTests
{
    private static JwtTokenService CriarSut() => new(Options.Create(new JwtSettings
    {
        Issuer = "KuraVet.Api",
        Audience = "KuraVet.Clients",
        Key = "chave-de-teste-unitario-com-mais-de-32-caracteres",
        ExpirationMinutes = 30
    }));

    [Fact]
    public void GerarToken_Usuario_DeveConterClaimsDePerfilEmailEIdentificador()
    {
        // Arrange
        var usuario = new Usuario("Ana", "ana@kuravet.com", "hash", Perfis.Veterinario).ComId(42);

        // Act
        var token = CriarSut().GerarToken(usuario);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token.AccessToken);

        // Assert
        Assert.Equal("KuraVet.Api", jwt.Issuer);
        Assert.Contains("KuraVet.Clients", jwt.Audiences);
        Assert.Equal("42", jwt.Claims.First(c => c.Type == "sub").Value);
        Assert.Equal("ana@kuravet.com", jwt.Claims.First(c => c.Type == "email").Value);
        Assert.Equal(Perfis.Veterinario, jwt.Claims.First(c => c.Type == "role").Value);
    }

    [Fact]
    public void GerarToken_Usuario_DeveExpirarConformeConfiguracao()
    {
        // Arrange
        var usuario = new Usuario("Ana", "ana@kuravet.com", "hash", Perfis.Tutor);
        var esperado = DateTime.UtcNow.AddMinutes(30);

        // Act
        var token = CriarSut().GerarToken(usuario);

        // Assert
        Assert.InRange(token.ExpiraEm, esperado.AddSeconds(-5), esperado.AddSeconds(5));
    }
}
