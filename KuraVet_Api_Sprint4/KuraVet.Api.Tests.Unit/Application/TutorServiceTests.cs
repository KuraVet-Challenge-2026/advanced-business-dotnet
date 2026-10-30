using KuraVet.Api.Tests.Unit.Fixtures;
using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Application.Tutores;
using KuraVet.Domain.Entities;
using KuraVet.Domain.Exceptions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application;

public class TutorServiceTests
{
    private readonly Mock<ITutorRepository> _repositorio = new();
    private readonly TutorService _sut;

    public TutorServiceTests()
    {
        _sut = new TutorService(_repositorio.Object, NullLogger<TutorService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_RequestValido_DeveAdicionarERetornarTutorComId()
    {
        // Arrange
        _repositorio
            .Setup(r => r.AdicionarAsync(It.IsAny<Tutor>(), It.IsAny<CancellationToken>()))
            .Callback<Tutor, CancellationToken>((tutor, _) => tutor.ComId(10))
            .Returns(Task.CompletedTask);

        // Act
        var resposta = await _sut.CriarAsync(new TutorRequest { Nome = "Juliana" }, CancellationToken.None);

        // Assert
        Assert.Equal(10, resposta.Id);
        Assert.Equal("Juliana", resposta.Nome);
        _repositorio.Verify(r => r.AdicionarAsync(It.Is<Tutor>(t => t.Nome == "Juliana"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_NomeEmBranco_DeveLancarDomainExceptionSemPersistir()
    {
        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(
            () => _sut.CriarAsync(new TutorRequest { Nome = "  " }, CancellationToken.None));

        _repositorio.Verify(r => r.AdicionarAsync(It.IsAny<Tutor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_TutorInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _repositorio
            .Setup(r => r.ObterPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tutor?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterPorIdAsync(99, CancellationToken.None));
    }

    [Fact]
    public async Task AtualizarAsync_TutorExistente_DeveAlterarNomeEPersistir()
    {
        // Arrange
        var tutor = new Tutor("Nome Original").ComId(5);
        _repositorio
            .Setup(r => r.ObterPorIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tutor);

        // Act
        var resposta = await _sut.AtualizarAsync(5, new TutorRequest { Nome = "Nome Novo" }, CancellationToken.None);

        // Assert
        Assert.Equal("Nome Novo", resposta.Nome);
        Assert.Equal(5, resposta.Id);
        _repositorio.Verify(r => r.AtualizarAsync(tutor, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_TutorInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _repositorio
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tutor?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.AtualizarAsync(1, new TutorRequest { Nome = "X" }, CancellationToken.None));
        _repositorio.Verify(r => r.AtualizarAsync(It.IsAny<Tutor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_TutorExistente_DeveRemover()
    {
        // Arrange
        var tutor = new Tutor("Para Remover").ComId(3);
        _repositorio
            .Setup(r => r.ObterPorIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tutor);

        // Act
        await _sut.RemoverAsync(3, CancellationToken.None);

        // Assert
        _repositorio.Verify(r => r.RemoverAsync(tutor, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_RepositorioComItens_DeveRetornarMetadadosDePaginacao()
    {
        // Arrange
        var filtro = new TutorFiltro { Page = 2, PageSize = 2 };
        var itens = new List<Tutor> { new Tutor("C").ComId(3), new Tutor("D").ComId(4) };
        _repositorio
            .Setup(r => r.ListarAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedList<Tutor>(itens, 5));

        // Act
        var resultado = await _sut.ListarAsync(filtro, CancellationToken.None);

        // Assert
        Assert.Equal(2, resultado.Page);
        Assert.Equal(2, resultado.PageSize);
        Assert.Equal(5, resultado.TotalItems);
        Assert.Equal(3, resultado.TotalPages);
        Assert.Equal(new[] { 3, 4 }, resultado.Items.Select(t => t.Id));
    }

    [Fact]
    public async Task ListarAsync_SortByInvalido_DeveLancarValidacaoExceptionSemConsultar()
    {
        // Arrange
        var filtro = new TutorFiltro { SortBy = "inexistente" };

        // Act & Assert
        await Assert.ThrowsAsync<ValidacaoException>(() => _sut.ListarAsync(filtro, CancellationToken.None));
        _repositorio.Verify(
            r => r.ListarAsync(It.IsAny<TutorFiltro>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
