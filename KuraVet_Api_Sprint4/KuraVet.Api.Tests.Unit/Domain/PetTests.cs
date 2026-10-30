using KuraVet.Domain.Entities;
using KuraVet.Domain.Exceptions;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain;

public class PetTests
{
    [Fact]
    public void Construtor_DadosValidos_DeveCriarPet()
    {
        // Act
        var pet = new Pet("Rex", 1);

        // Assert
        Assert.Equal("Rex", pet.Nome);
        Assert.Equal(1, pet.TutorId);
    }

    [Fact]
    public void Construtor_NomeVazio_DeveLancarDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new Pet(string.Empty, 1));
    }

    [Fact]
    public void Construtor_NomeAcimaDoLimite_DeveLancarDomainException()
    {
        // Arrange
        var nome = new string('B', Pet.NomeTamanhoMaximo + 1);

        // Act & Assert
        Assert.Throws<DomainException>(() => new Pet(nome, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Construtor_TutorIdInvalido_DeveLancarDomainException(int tutorId)
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new Pet("Rex", tutorId));
        Assert.Contains("TutorId", ex.Message);
    }

    [Fact]
    public void Atualizar_DadosValidos_DeveAlterarNomeETutor()
    {
        // Arrange
        var pet = new Pet("Rex", 1);

        // Act
        pet.Atualizar("Thor", 2);

        // Assert
        Assert.Equal("Thor", pet.Nome);
        Assert.Equal(2, pet.TutorId);
    }
}
