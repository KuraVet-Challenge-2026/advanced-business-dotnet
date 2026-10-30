using KuraVet.Infrastructure.Security;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Infrastructure;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _sut = new();

    [Fact]
    public void Hash_Senha_NaoDeveArmazenarTextoPuroEDeveVerificar()
    {
        // Act
        var hash = _sut.Hash("SenhaForte@123");

        // Assert
        Assert.NotEqual("SenhaForte@123", hash);
        Assert.StartsWith("$2", hash);
        Assert.True(_sut.Verificar("SenhaForte@123", hash));
    }

    [Fact]
    public void Verificar_SenhaIncorreta_DeveRetornarFalse()
    {
        // Arrange
        var hash = _sut.Hash("SenhaForte@123");

        // Act & Assert
        Assert.False(_sut.Verificar("outra-senha", hash));
    }

    [Theory]
    [InlineData("hash-invalido")]
    [InlineData("")]
    public void Verificar_HashMalformado_DeveRetornarFalseSemLancar(string hash)
    {
        // Act & Assert
        Assert.False(_sut.Verificar("qualquer", hash));
    }
}
