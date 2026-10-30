using KuraVet.Domain.Entities;
using KuraVet.Domain.Exceptions;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Domain;

public class CheckinHistoricoTests
{
    private static CheckinHistorico CriarValido(DateTime? data = null) =>
        new(petId: 1, frequenciaRespiratoria: 20, tempoPreenchimentoCapilar: 1, corMucosa: "Rosada",
            nivelHidratacao: "Normal", dataCheckin: data);

    [Fact]
    public void Construtor_DadosValidosSemData_DeveUsarDataAtualUtc()
    {
        // Arrange
        var antes = DateTime.UtcNow.AddSeconds(-1);

        // Act
        var checkin = CriarValido();

        // Assert
        Assert.True(checkin.DataCheckin >= antes);
        Assert.Null(checkin.NivelRiscoIA);
    }

    [Fact]
    public void Construtor_DataInformada_DeveManterData()
    {
        // Arrange
        var data = new DateTime(2026, 10, 1, 8, 0, 0, DateTimeKind.Utc);

        // Act
        var checkin = CriarValido(data);

        // Assert
        Assert.Equal(data, checkin.DataCheckin);
    }

    [Fact]
    public void Construtor_CorMucosaVazia_DeveLancarDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new CheckinHistorico(1, 20, 1, "", "Normal"));
        Assert.Contains("CorMucosa", ex.Message);
    }

    [Fact]
    public void Construtor_NivelHidratacaoVazio_DeveLancarDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new CheckinHistorico(1, 20, 1, "Rosada", " "));
        Assert.Contains("NivelHidratacao", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(301)]
    public void Construtor_FrequenciaRespiratoriaForaDaFaixa_DeveLancarDomainException(int frequencia)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new CheckinHistorico(1, frequencia, 1, "Rosada", "Normal"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(31)]
    public void Construtor_TpcForaDaFaixa_DeveLancarDomainException(int tpc)
    {
        // Act & Assert
        Assert.Throws<DomainException>(() => new CheckinHistorico(1, 20, tpc, "Rosada", "Normal"));
    }

    [Fact]
    public void DefinirNivelRisco_ValorEmMinusculas_DeveNormalizar()
    {
        // Arrange
        var checkin = CriarValido();

        // Act
        checkin.DefinirNivelRisco("moderado");

        // Assert
        Assert.Equal(NiveisRisco.Moderado, checkin.NivelRiscoIA);
    }

    [Fact]
    public void DefinirNivelRisco_ValorInvalido_DeveLancarDomainException()
    {
        // Arrange
        var checkin = CriarValido();

        // Act & Assert
        Assert.Throws<DomainException>(() => checkin.DefinirNivelRisco("Altissimo"));
    }

    [Fact]
    public void AtualizarSinais_SemNovaData_DeveManterDataOriginal()
    {
        // Arrange
        var data = new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc);
        var checkin = CriarValido(data);

        // Act
        checkin.AtualizarSinais(1, 30, 3, "Pálida", "Reduzida");

        // Assert
        Assert.Equal(data, checkin.DataCheckin);
        Assert.Equal(30, checkin.FrequenciaRespiratoria);
        Assert.Equal(3, checkin.TempoPreenchimentoCapilar);
    }
}
