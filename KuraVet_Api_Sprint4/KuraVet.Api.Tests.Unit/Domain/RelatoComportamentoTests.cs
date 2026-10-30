using KuraVet.Domain.Entities;
using KuraVet.Domain.Exceptions;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain;

public class RelatoComportamentoTests
{
    [Fact]
    public void Construtor_DadosValidos_DeveNormalizarCategoriaETags()
    {
        // Act
        var relato = new RelatoComportamento(
            1, "alimentacao", "Comeu pouco", 3, new[] { " Apetite ", "apetite", "", "Agua" }, null, "tutor@kuravet.com");

        // Assert
        Assert.Equal(CategoriasRelato.Alimentacao, relato.Categoria);
        Assert.Equal(new[] { "apetite", "agua" }, relato.Tags);
        Assert.Equal("tutor@kuravet.com", relato.RegistradoPor);
        Assert.Null(relato.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Construtor_IntensidadeForaDaFaixa_DeveLancarDomainException(int intensidade)
    {
        // Act & Assert
        Assert.Throws<DomainException>(
            () => new RelatoComportamento(1, "Sono", "Dormiu mal", intensidade, null, null, null));
    }

    [Fact]
    public void Construtor_CategoriaInvalida_DeveLancarDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => new RelatoComportamento(1, "Viagem", "Ficou ansioso", 2, null, null, null));
        Assert.Contains("Categoria", ex.Message);
    }

    [Fact]
    public void Construtor_MaisTagsQueOPermitido_DeveLancarDomainException()
    {
        // Arrange
        var tags = Enumerable.Range(1, RelatoComportamento.QuantidadeMaximaTags + 1).Select(i => $"tag{i}");

        // Act & Assert
        Assert.Throws<DomainException>(
            () => new RelatoComportamento(1, "Outro", "Muitas tags", 1, tags, null, null));
    }

    [Fact]
    public void Reconstituir_DadosPersistidos_DeveManterIdEDatas()
    {
        // Arrange
        var data = new DateTime(2026, 10, 2, 9, 0, 0, DateTimeKind.Utc);
        var criadoEm = data.AddMinutes(5);

        // Act
        var relato = RelatoComportamento.Reconstituir(
            "652f1c2e9b1e8a0012345678", 1, "Sono", "Acordou várias vezes", 2, null, data, null, criadoEm);

        // Assert
        Assert.Equal("652f1c2e9b1e8a0012345678", relato.Id);
        Assert.Equal(data, relato.DataRelato);
        Assert.Equal(criadoEm, relato.CriadoEm);
    }
}
