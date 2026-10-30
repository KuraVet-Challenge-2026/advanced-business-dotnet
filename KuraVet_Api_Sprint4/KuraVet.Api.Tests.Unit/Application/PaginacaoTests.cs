using KuraVet.Application.Common;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Application.Tutores;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Application;

public class PaginacaoTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-3, 1)]
    [InlineData(4, 4)]
    public void Page_ValorInformado_DeveSerNormalizado(int informado, int esperado)
    {
        // Act
        var filtro = new TutorFiltro { Page = informado };

        // Assert
        Assert.Equal(esperado, filtro.Page);
    }

    [Theory]
    [InlineData(0, PaginacaoQuery.PageSizePadrao)]
    [InlineData(500, PaginacaoQuery.PageSizeMaximo)]
    [InlineData(25, 25)]
    public void PageSize_ValorInformado_DeveRespeitarLimites(int informado, int esperado)
    {
        // Act
        var filtro = new TutorFiltro { PageSize = informado };

        // Assert
        Assert.Equal(esperado, filtro.PageSize);
    }

    [Fact]
    public void CalcularSkip_TerceiraPaginaComDezItens_DeveRetornarVinte()
    {
        // Arrange
        var filtro = new TutorFiltro { Page = 3, PageSize = 10 };

        // Act
        var skip = filtro.CalcularSkip();

        // Assert
        Assert.Equal(20, skip);
    }

    [Fact]
    public void ValidarOrdenacao_CampoInvalido_DeveLancarValidacaoException()
    {
        // Arrange
        var filtro = new TutorFiltro { SortBy = "senha" };

        // Act & Assert
        var ex = Assert.Throws<ValidacaoException>(() => filtro.ValidarOrdenacao());
        Assert.True(ex.Erros.ContainsKey("SortBy"));
    }

    [Fact]
    public void ValidarOrdenacao_DirecaoInvalida_DeveLancarValidacaoException()
    {
        // Arrange
        var filtro = new TutorFiltro { SortBy = "nome", SortDir = "para-cima" };

        // Act & Assert
        Assert.Throws<ValidacaoException>(() => filtro.ValidarOrdenacao());
    }

    [Fact]
    public void ValidarOrdenacao_CampoValidoComOutraCaixa_NaoDeveLancar()
    {
        // Arrange
        var filtro = new TutorFiltro { SortBy = "NOME", SortDir = "DESC" };

        // Act
        filtro.ValidarOrdenacao();

        // Assert
        Assert.True(filtro.EhDescendente());
        Assert.Equal("nome", filtro.CampoOrdenacaoNormalizado());
    }

    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(95, 20, 5)]
    public void Criar_TotalDeItens_DeveCalcularTotalDePaginas(int total, int pageSize, int paginasEsperadas)
    {
        // Act
        var resultado = PagedResult<int>.Criar(Array.Empty<int>(), 1, pageSize, total);

        // Assert
        Assert.Equal(paginasEsperadas, resultado.TotalPages);
        Assert.Equal(total, resultado.TotalItems);
    }
}
