using KuraVet.Api.Tests.Unit.Fixtures;
using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Application.Pets;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application;

public class PetServiceTests
{
    private readonly Mock<IPetRepository> _petRepositorio = new();
    private readonly Mock<ITutorRepository> _tutorRepositorio = new();
    private readonly PetService _sut;

    public PetServiceTests()
    {
        _sut = new PetService(_petRepositorio.Object, _tutorRepositorio.Object, NullLogger<PetService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_TutorExistente_DeveCriarPet()
    {
        // Arrange
        _tutorRepositorio
            .Setup(r => r.ExisteAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _petRepositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()))
            .Callback<Pet, CancellationToken>((pet, _) => pet.ComId(7))
            .Returns(Task.CompletedTask);

        // Act
        var resposta = await _sut.CriarAsync(new PetRequest { Nome = "Thor", TutorId = 1 }, CancellationToken.None);

        // Assert
        Assert.Equal(7, resposta.Id);
        Assert.Equal("Thor", resposta.Nome);
        Assert.Equal(1, resposta.TutorId);
    }

    [Fact]
    public async Task CriarAsync_TutorInexistente_DeveLancarValidacaoExceptionSemPersistir()
    {
        // Arrange
        _tutorRepositorio
            .Setup(r => r.ExisteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ValidacaoException>(
            () => _sut.CriarAsync(new PetRequest { Nome = "Thor", TutorId = 99 }, CancellationToken.None));

        Assert.True(ex.Erros.ContainsKey(nameof(PetRequest.TutorId)));
        _petRepositorio.Verify(r => r.AdicionarAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ListarPorTutorAsync_TutorInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _tutorRepositorio
            .Setup(r => r.ExisteAsync(50, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.ListarPorTutorAsync(50, new PetFiltro(), CancellationToken.None));
    }

    [Fact]
    public async Task ListarPorTutorAsync_TutorExistente_DeveAplicarFiltroDoTutor()
    {
        // Arrange
        var filtro = new PetFiltro();
        _tutorRepositorio
            .Setup(r => r.ExisteAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _petRepositorio
            .Setup(r => r.ListarAsync(It.IsAny<PetFiltro>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedList<Pet>(new List<Pet> { new Pet("Mel", 2).ComId(1) }, 1));

        // Act
        var resultado = await _sut.ListarPorTutorAsync(2, filtro, CancellationToken.None);

        // Assert
        Assert.Equal(2, filtro.TutorId);
        Assert.Single(resultado.Items);
        _petRepositorio.Verify(
            r => r.ListarAsync(It.Is<PetFiltro>(f => f.TutorId == 2), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_PetInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _petRepositorio
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Pet?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.AtualizarAsync(1, new PetRequest { Nome = "Rex", TutorId = 1 }, CancellationToken.None));
    }
}
