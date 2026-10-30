using KuraVet.Domain.Entities;
using KuraVet.Domain.Exceptions;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain;

public class TutorTests
{
    [Fact]
    public void Construtor_NomeValido_DeveCriarTutorComNomeSemEspacosExtras()
    {
        // Act
        var tutor = new Tutor("  Juliana Ferreira  ");

        // Assert
        Assert.Equal("Juliana Ferreira", tutor.Nome);
        Assert.Empty(tutor.Pets);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_NomeVazio_DeveLancarDomainException(string nome)
    {
        // Act
        var acao = () => new Tutor(nome);

        // Assert
        var ex = Assert.Throws<DomainException>(acao);
        Assert.Contains("Nome", ex.Message);
    }

    [Fact]
    public void Construtor_NomeAcimaDoLimite_DeveLancarDomainException()
    {
        // Arrange
        var nome = new string('A', Tutor.NomeTamanhoMaximo + 1);

        // Act & Assert
        Assert.Throws<DomainException>(() => new Tutor(nome));
    }

    [Fact]
    public void AlterarNome_NomeValido_DeveAtualizarNome()
    {
        // Arrange
        var tutor = new Tutor("Nome Original");

        // Act
        tutor.AlterarNome("Nome Atualizado");

        // Assert
        Assert.Equal("Nome Atualizado", tutor.Nome);
    }
}
