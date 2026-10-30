using KuraVet.Api.Tests.Unit.Fixtures;
using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Application.EventosConsulta;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application;

public class EventoConsultaServiceTests
{
    private readonly Mock<IEventoConsultaRepository> _eventoRepositorio = new();
    private readonly Mock<IPetRepository> _petRepositorio = new();
    private readonly EventoConsultaService _sut;

    public EventoConsultaServiceTests()
    {
        _sut = new EventoConsultaService(
            _eventoRepositorio.Object, _petRepositorio.Object, NullLogger<EventoConsultaService>.Instance);
    }

    private static EventoConsultaRequest CriarRequest(int petId = 1) => new()
    {
        TipoEvento = "Vacina",
        DataEvento = new DateTime(2026, 10, 1, 14, 30, 0, DateTimeKind.Utc),
        Descricao = "V10 - reforço anual",
        VeterinarioResponsavel = "Dra. Marina",
        PetId = petId
    };

    [Fact]
    public async Task CriarAsync_PetExistente_DeveRegistrarEvento()
    {
        // Arrange
        _petRepositorio.Setup(r => r.ExisteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _eventoRepositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<EventoConsulta>(), It.IsAny<CancellationToken>()))
            .Callback<EventoConsulta, CancellationToken>((evento, _) => evento.ComId(12))
            .Returns(Task.CompletedTask);

        // Act
        var resposta = await _sut.CriarAsync(CriarRequest(), CancellationToken.None);

        // Assert
        Assert.Equal(12, resposta.Id);
        Assert.Equal("Vacina", resposta.TipoEvento);
        Assert.Equal("Dra. Marina", resposta.VeterinarioResponsavel);
    }

    [Fact]
    public async Task CriarAsync_PetInexistente_DeveLancarValidacaoException()
    {
        // Arrange
        _petRepositorio.Setup(r => r.ExisteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ValidacaoException>(() => _sut.CriarAsync(CriarRequest(99), CancellationToken.None));
    }

    [Fact]
    public async Task ObterPorIdAsync_EventoInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _eventoRepositorio
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EventoConsulta?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterPorIdAsync(1, CancellationToken.None));
    }

    [Fact]
    public async Task RemoverAsync_EventoExistente_DeveRemover()
    {
        // Arrange
        var evento = new EventoConsulta(1, "Exame", DateTime.UtcNow, "Hemograma", "Dr. Paulo").ComId(8);
        _eventoRepositorio.Setup(r => r.ObterPorIdAsync(8, It.IsAny<CancellationToken>())).ReturnsAsync(evento);

        // Act
        await _sut.RemoverAsync(8, CancellationToken.None);

        // Assert
        _eventoRepositorio.Verify(r => r.RemoverAsync(evento, It.IsAny<CancellationToken>()), Times.Once);
    }
}
