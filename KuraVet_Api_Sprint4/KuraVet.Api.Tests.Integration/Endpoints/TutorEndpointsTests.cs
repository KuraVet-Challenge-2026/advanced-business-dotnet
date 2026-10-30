using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Tests.Integration.Fixtures;
using KuraVet.Application.Common;
using KuraVet.Application.Tutores;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints;

[Collection(KuraVetApiCollection.Name)]
public class TutorEndpointsTests
{
    private readonly HttpClient _client;

    public TutorEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CriarClienteAutenticado();
    }

    [Fact]
    public async Task Post_TutorValido_DeveRetornar201ComLocationELinks()
    {
        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Tutor", new TutorRequest { Nome = "Juliana Ferreira" });
        var tutor = await resposta.Content.ReadFromJsonAsync<TutorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);
        Assert.True(tutor!.Id > 0);
        Assert.Contains(tutor.Links, l => l.Rel == "self" && l.Method == "GET" && l.Href.EndsWith($"/api/Tutor/{tutor.Id}"));
        Assert.Contains(tutor.Links, l => l.Rel == "update" && l.Method == "PUT");
        Assert.Contains(tutor.Links, l => l.Rel == "delete" && l.Method == "DELETE");
        Assert.Contains(tutor.Links, l => l.Rel == "pets" && l.Href.EndsWith($"/api/Pet/tutor/{tutor.Id}"));
    }

    [Fact]
    public async Task Post_TutorSemNome_DeveRetornar400()
    {
        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Tutor", new { Nome = "" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Get_TutorInexistente_DeveRetornar404ComProblemDetails()
    {
        // Act
        var resposta = await _client.GetAsync("/api/Tutor/999999");
        var corpo = await resposta.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        Assert.Contains("\"status\":404", corpo);
        Assert.Contains("traceId", corpo);
    }

    [Fact]
    public async Task Put_TutorExistente_DeveRetornar200ComNomeAtualizado()
    {
        // Arrange
        var tutor = await _client.CriarTutorAsync("Nome Original");

        // Act
        var resposta = await _client.PutAsJsonAsync($"/api/Tutor/{tutor.Id}", new TutorRequest { Nome = "Nome Atualizado" });
        var atualizado = await resposta.Content.ReadFromJsonAsync<TutorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("Nome Atualizado", atualizado!.Nome);
        Assert.Equal(tutor.Id, atualizado.Id);
    }

    [Fact]
    public async Task Put_TutorInexistente_DeveRetornar404()
    {
        // Act
        var resposta = await _client.PutAsJsonAsync("/api/Tutor/999999", new TutorRequest { Nome = "Não Importa" });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Delete_TutorExistenteComoAdmin_DeveRetornar204EDepois404()
    {
        // Arrange
        var tutor = await _client.CriarTutorAsync("Tutor Para Remover");

        // Act
        var respostaDelete = await _client.DeleteAsync($"/api/Tutor/{tutor.Id}");
        var respostaGet = await _client.GetAsync($"/api/Tutor/{tutor.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, respostaDelete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, respostaGet.StatusCode);
    }

    [Fact]
    public async Task Get_ListaComPaginacaoOrdenacaoEFiltro_DeveRetornarMetadadosELinksDeNavegacao()
    {
        // Arrange
        var prefixo = $"Paginado{Guid.NewGuid():N}"[..20];
        foreach (var sufixo in new[] { "C", "A", "B" })
        {
            await _client.CriarTutorAsync($"{prefixo} {sufixo}");
        }

        // Act
        var resposta = await _client.GetAsync($"/api/Tutor?nome={prefixo}&page=1&pageSize=2&sortBy=nome&sortDir=asc");
        var pagina = await resposta.Content.ReadFromJsonAsync<PagedResult<TutorResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal(1, pagina!.Page);
        Assert.Equal(2, pagina.PageSize);
        Assert.Equal(3, pagina.TotalItems);
        Assert.Equal(2, pagina.TotalPages);
        Assert.Equal(new[] { $"{prefixo} A", $"{prefixo} B" }, pagina.Items.Select(t => t.Nome));
        Assert.Contains(pagina.Links, l => l.Rel == "next" && l.Href.Contains("page=2"));
        Assert.DoesNotContain(pagina.Links, l => l.Rel == "prev");
        Assert.All(pagina.Items, t => Assert.NotEmpty(t.Links));
    }

    [Fact]
    public async Task Get_ListaComSortByInvalido_DeveRetornar400()
    {
        // Act
        var resposta = await _client.GetAsync("/api/Tutor?sortBy=senha");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Get_ListaVazia_DeveRetornar200ComItensVazios()
    {
        // Act
        var resposta = await _client.GetAsync($"/api/Tutor?nome=inexistente-{Guid.NewGuid():N}");
        var pagina = await resposta.Content.ReadFromJsonAsync<PagedResult<TutorResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Empty(pagina!.Items);
        Assert.Equal(0, pagina.TotalPages);
    }
}
