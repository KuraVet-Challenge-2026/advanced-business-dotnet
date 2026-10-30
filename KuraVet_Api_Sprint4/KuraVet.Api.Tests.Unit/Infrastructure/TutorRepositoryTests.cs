using KuraVet.Api.Tests.Unit.Fixtures;
using KuraVet.Application.Tutores;
using KuraVet.Domain.Entities;
using KuraVet.Infrastructure.Persistence.Repositories;
using Xunit;

namespace KuraVet.Api.Tests.Unit.Infrastructure;

public class TutorRepositoryTests
{
    private static async Task<TutorRepository> CriarRepositorioComDadosAsync()
    {
        var context = InMemoryDbContextFactory.CreateContext();
        var repositorio = new TutorRepository(context);

        foreach (var nome in new[] { "Carla", "Ana", "Bruno", "Daniel", "Anabela" })
        {
            await repositorio.AdicionarAsync(new Tutor(nome), CancellationToken.None);
        }

        return repositorio;
    }

    [Fact]
    public async Task ListarAsync_OrdenadoPorNomeAsc_DeveRetornarPrimeiraPagina()
    {
        // Arrange
        var repositorio = await CriarRepositorioComDadosAsync();
        var filtro = new TutorFiltro { Page = 1, PageSize = 2, SortBy = "nome", SortDir = "asc" };

        // Act
        var pagina = await repositorio.ListarAsync(filtro, CancellationToken.None);

        // Assert
        Assert.Equal(5, pagina.TotalItems);
        Assert.Equal(new[] { "Ana", "Anabela" }, pagina.Items.Select(t => t.Nome));
    }

    [Fact]
    public async Task ListarAsync_OrdenadoPorNomeDescSegundaPagina_DeveRetornarItensCorretos()
    {
        // Arrange
        var repositorio = await CriarRepositorioComDadosAsync();
        var filtro = new TutorFiltro { Page = 2, PageSize = 2, SortBy = "nome", SortDir = "desc" };

        // Act
        var pagina = await repositorio.ListarAsync(filtro, CancellationToken.None);

        // Assert
        Assert.Equal(new[] { "Bruno", "Anabela" }, pagina.Items.Select(t => t.Nome));
    }

    [Fact]
    public async Task ListarAsync_FiltroPorNome_DeveIgnorarMaiusculas()
    {
        // Arrange
        var repositorio = await CriarRepositorioComDadosAsync();
        var filtro = new TutorFiltro { Nome = "ANA", SortBy = "nome" };

        // Act
        var pagina = await repositorio.ListarAsync(filtro, CancellationToken.None);

        // Assert
        Assert.Equal(2, pagina.TotalItems);
        Assert.All(pagina.Items, t => Assert.Contains("ana", t.Nome, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ExisteAsync_IdInexistente_DeveRetornarFalse()
    {
        // Arrange
        var repositorio = await CriarRepositorioComDadosAsync();

        // Act
        var existe = await repositorio.ExisteAsync(999, CancellationToken.None);

        // Assert
        Assert.False(existe);
    }
}
