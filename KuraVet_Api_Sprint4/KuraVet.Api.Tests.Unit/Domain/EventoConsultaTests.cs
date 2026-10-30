using KuraVet.Domain.Entities;
using KuraVet.Domain.Exceptions;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain;

public class EventoConsultaTests
{
    private static readonly DateTime Data = new(2026, 10, 1, 14, 30, 0, DateTimeKind.Utc);

    [Fact]
    public void Construtor_DadosValidos_DeveCriarEvento()
    {
        // Act
        var evento = new EventoConsulta(1, "Vacina", Data, "V10 - reforço anual", "Dra. Marina");

        // Assert
        Assert.Equal("Vacina", evento.TipoEvento);
        Assert.Equal(Data, evento.DataEvento);
        Assert.Equal(1, evento.PetId);
    }

    [Fact]
    public void Construtor_TipoVazio_DeveLancarDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new EventoConsulta(1, "", Data, "Descrição", "Dra. Marina"));
    }

    [Fact]
    public void Construtor_VeterinarioVazio_DeveLancarDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new EventoConsulta(1, "Vacina", Data, "Descrição", " "));
    }

    [Fact]
    public void Construtor_DescricaoAcimaDoLimite_DeveLancarDomainException()
    {
        // Arrange
        var descricao = new string('x', EventoConsulta.DescricaoTamanhoMaximo + 1);

        // Act & Assert
        Assert.Throws<DomainException>(() => new EventoConsulta(1, "Exame", Data, descricao, "Dr. Paulo"));
    }

    [Fact]
    public void Construtor_DataPadrao_DeveLancarDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new EventoConsulta(1, "Exame", default, "Hemograma", "Dr. Paulo"));
        Assert.Contains("DataEvento", ex.Message);
    }
}
