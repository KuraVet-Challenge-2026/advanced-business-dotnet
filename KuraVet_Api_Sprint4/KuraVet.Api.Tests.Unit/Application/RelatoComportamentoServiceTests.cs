using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Application.RelatosComportamento;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application;

public class RelatoComportamentoServiceTests
{
    private const string IdValido = "652f1c2e9b1e8a0012345678";

    private readonly Mock<IRelatoComportamentoRepository> _relatos = new();
    private readonly Mock<IPetRepository> _pets = new();
    private readonly RelatoComportamentoService _sut;

    public RelatoComportamentoServiceTests()
    {
        _sut = new RelatoComportamentoService(_relatos.Object, _pets.Object, NullLogger<RelatoComportamentoService>.Instance);
    }

    private static RelatoComportamentoRequest CriarRequest(int petId = 1) => new()
    {
        PetId = petId,
        Categoria = "Alimentacao",
        Descricao = "Comeu metade da ração.",
        Intensidade = 3,
        Tags = new List<string> { "apetite" }
    };

    [Fact]
    public async Task CriarAsync_PetExistente_DevePersistirComAutor()
    {
        // Arrange
        _pets.Setup(r => r.ExisteAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _relatos
            .Setup(r => r.AdicionarAsync(It.IsAny<RelatoComportamento>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RelatoComportamento r, CancellationToken _) => RelatoComportamento.Reconstituir(
                IdValido, r.PetId, r.Categoria, r.Descricao, r.Intensidade, r.Tags, r.DataRelato, r.RegistradoPor, r.CriadoEm));

        // Act
        var resposta = await _sut.CriarAsync(CriarRequest(), "tutor@kuravet.com", CancellationToken.None);

        // Assert
        Assert.Equal(IdValido, resposta.Id);
        Assert.Equal("tutor@kuravet.com", resposta.RegistradoPor);
        Assert.Equal(CategoriasRelato.Alimentacao, resposta.Categoria);
    }

    [Fact]
    public async Task CriarAsync_PetInexistente_DeveLancarValidacaoException()
    {
        // Arrange
        _pets.Setup(r => r.ExisteAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<ValidacaoException>(() => _sut.CriarAsync(CriarRequest(9), null, CancellationToken.None));
        _relatos.Verify(
            r => r.AdicionarAsync(It.IsAny<RelatoComportamento>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_RelatoInexistente_DeveLancarNotFoundException()
    {
        // Arrange
        _relatos
            .Setup(r => r.ObterPorIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((RelatoComportamento?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ObterPorIdAsync("inexistente", CancellationToken.None));
    }

    [Fact]
    public async Task RemoverAsync_RepositorioNaoEncontra_DeveLancarNotFoundException()
    {
        // Arrange
        _relatos.Setup(r => r.RemoverAsync(IdValido, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.RemoverAsync(IdValido, CancellationToken.None));
    }

    [Fact]
    public async Task ListarAsync_CategoriaEmMinusculas_DeveNormalizar()
    {
        // Arrange
        var filtro = new RelatoComportamentoFiltro { Categoria = "sono" };
        _relatos
            .Setup(r => r.ListarAsync(It.IsAny<RelatoComportamentoFiltro>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedList<RelatoComportamento>(new List<RelatoComportamento>(), 0));

        // Act
        await _sut.ListarAsync(filtro, CancellationToken.None);

        // Assert
        Assert.Equal(CategoriasRelato.Sono, filtro.Categoria);
    }
}
