using KuraVet.Api.Tests.Unit.Fixtures;
using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Checkins;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Entities;
using KuraVet.Domain.Exceptions;
using KuraVet.Domain.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application;

public class CheckinServiceTests
{
    private readonly Mock<ICheckinRepository> _checkinRepositorio = new();
    private readonly Mock<IPetRepository> _petRepositorio = new();
    private readonly Mock<ICheckinRiscoService> _riscoService = new();
    private readonly CheckinService _sut;

    public CheckinServiceTests()
    {
        _sut = new CheckinService(
            _checkinRepositorio.Object,
            _petRepositorio.Object,
            _riscoService.Object,
            NullLogger<CheckinService>.Instance);

        _petRepositorio
            .Setup(r => r.ExisteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    private static CheckinRequest CriarRequest(int tpc = 3, int petId = 1) => new()
    {
        FrequenciaRespiratoria = 22,
        TempoPreenchimentoCapilar = tpc,
        CorMucosa = "Pálida",
        NivelHidratacao = "Reduzida",
        PetId = petId
    };

    [Fact]
    public async Task CriarAsync_CheckinValido_DeveDelegarClassificacaoAoServicoDeRisco()
    {
        // Arrange
        _riscoService.Setup(s => s.ClassificarRisco(It.IsAny<int>())).Returns(NiveisRisco.Moderado);

        // Act
        var resposta = await _sut.CriarAsync(CriarRequest(tpc: 3), CancellationToken.None);

        // Assert
        Assert.Equal(NiveisRisco.Moderado, resposta.NivelRiscoIA);
        _riscoService.Verify(s => s.ClassificarRisco(3), Times.Once);
        _checkinRepositorio.Verify(
            r => r.AdicionarAsync(It.Is<CheckinHistorico>(c => c.NivelRiscoIA == NiveisRisco.Moderado), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ServicoDeRiscoRetornaBaixo_DeveGravarNivelBaixo()
    {
        // Arrange
        _riscoService.Setup(s => s.ClassificarRisco(It.IsAny<int>())).Returns(NiveisRisco.Baixo);

        // Act
        var resposta = await _sut.CriarAsync(CriarRequest(tpc: 1), CancellationToken.None);

        // Assert
        Assert.Equal(NiveisRisco.Baixo, resposta.NivelRiscoIA);
    }

    [Fact]
    public async Task CriarAsync_PetInexistente_DeveLancarValidacaoException()
    {
        // Arrange
        _petRepositorio
            .Setup(r => r.ExisteAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ValidacaoException>(
            () => _sut.CriarAsync(CriarRequest(petId: 99), CancellationToken.None));
        _riscoService.Verify(s => s.ClassificarRisco(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_NovosSinais_DeveRecalcularRisco()
    {
        // Arrange
        var existente = new CheckinHistorico(1, 20, 1, "Rosada", "Normal").ComId(4);
        existente.DefinirNivelRisco(NiveisRisco.Baixo);
        _checkinRepositorio
            .Setup(r => r.ObterPorIdAsync(4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existente);
        _riscoService.Setup(s => s.ClassificarRisco(5)).Returns(NiveisRisco.Moderado);

        // Act
        var resposta = await _sut.AtualizarAsync(4, CriarRequest(tpc: 5), CancellationToken.None);

        // Assert
        Assert.Equal(NiveisRisco.Moderado, resposta.NivelRiscoIA);
        Assert.Equal(5, resposta.TempoPreenchimentoCapilar);
        _checkinRepositorio.Verify(r => r.AtualizarAsync(existente, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarPorPetAsync_PetInexistente_DeveLancarNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.ListarPorPetAsync(42, new CheckinFiltro(), CancellationToken.None));
    }

    [Fact]
    public async Task ListarAsync_NivelRiscoEmMinusculas_DeveNormalizarAntesDeConsultar()
    {
        // Arrange
        var filtro = new CheckinFiltro { NivelRisco = "moderado" };
        _checkinRepositorio
            .Setup(r => r.ListarAsync(It.IsAny<CheckinFiltro>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedList<CheckinHistorico>(new List<CheckinHistorico>(), 0));

        // Act
        var resultado = await _sut.ListarAsync(filtro, CancellationToken.None);

        // Assert
        Assert.Equal(NiveisRisco.Moderado, filtro.NivelRisco);
        Assert.Empty(resultado.Items);
        Assert.Equal(0, resultado.TotalPages);
    }

    [Fact]
    public async Task ListarAsync_NivelRiscoInvalido_DeveLancarDomainException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(
            () => _sut.ListarAsync(new CheckinFiltro { NivelRisco = "altissimo" }, CancellationToken.None));
    }

    [Fact]
    public void CheckinFiltro_SemParametros_DeveOrdenarPorDataDecrescente()
    {
        // Act
        var filtro = new CheckinFiltro();

        // Assert
        Assert.Equal("datacheckin", filtro.CampoOrdenacaoNormalizado());
        Assert.True(filtro.EhDescendente());
    }
}
