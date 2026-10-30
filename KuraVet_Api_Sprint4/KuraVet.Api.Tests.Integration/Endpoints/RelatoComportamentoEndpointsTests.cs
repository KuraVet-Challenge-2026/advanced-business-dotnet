using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Tests.Integration.Fixtures;
using KuraVet.Application.Common;
using KuraVet.Application.RelatosComportamento;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints;

[Collection(KuraVetApiCollection.Name)]
public class RelatoComportamentoEndpointsTests
{
    private readonly HttpClient _client;

    public RelatoComportamentoEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CriarClienteAutenticado();
    }

    private static RelatoComportamentoRequest CriarRequest(int petId, string categoria = "Alimentacao", int intensidade = 3) => new()
    {
        PetId = petId,
        Categoria = categoria,
        Descricao = "Comeu metade da ração e bebeu menos água.",
        Intensidade = intensidade,
        Tags = new List<string> { "apetite", "agua" }
    };

    [Fact]
    public async Task Post_RelatoValido_DeveRetornar201ComIdERegistradoPor()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Pet Relato");

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/RelatoComportamento", CriarRequest(pet.Id));
        var relato = await resposta.Content.ReadFromJsonAsync<RelatoComportamentoResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.False(string.IsNullOrWhiteSpace(relato!.Id));
        Assert.Equal("admin@teste.kuravet.com", relato.RegistradoPor);
        Assert.Contains(relato.Links, l => l.Rel == "pet" && l.Href.EndsWith($"/api/Pet/{pet.Id}"));
    }

    [Fact]
    public async Task Post_CategoriaInvalida_DeveRetornar400()
    {
        // Arrange
        var pet = await _client.CriarPetAsync();

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/RelatoComportamento", CriarRequest(pet.Id, categoria: "Viagem"));

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task CrudCompleto_RelatoExistente_DeveAtualizarERemover()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Pet CRUD Relato");
        var criado = await (await _client.PostAsJsonAsync("/api/RelatoComportamento", CriarRequest(pet.Id)))
            .Content.ReadFromJsonAsync<RelatoComportamentoResponse>();

        // Act
        var respostaPut = await _client.PutAsJsonAsync(
            $"/api/RelatoComportamento/{criado!.Id}", CriarRequest(pet.Id, "Sono", 5));
        var atualizado = await respostaPut.Content.ReadFromJsonAsync<RelatoComportamentoResponse>();
        var respostaDelete = await _client.DeleteAsync($"/api/RelatoComportamento/{criado.Id}");
        var respostaGet = await _client.GetAsync($"/api/RelatoComportamento/{criado.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, respostaPut.StatusCode);
        Assert.Equal("Sono", atualizado!.Categoria);
        Assert.Equal(5, atualizado.Intensidade);
        Assert.Equal(HttpStatusCode.NoContent, respostaDelete.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, respostaGet.StatusCode);
    }

    [Fact]
    public async Task Get_FiltroPorPetEIntensidade_DeveRetornarSomenteRelatosCorrespondentes()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Pet Filtro Relato");
        await _client.PostAsJsonAsync("/api/RelatoComportamento", CriarRequest(pet.Id, intensidade: 1));
        await _client.PostAsJsonAsync("/api/RelatoComportamento", CriarRequest(pet.Id, intensidade: 4));

        // Act
        var resposta = await _client.GetAsync($"/api/RelatoComportamento?petId={pet.Id}&intensidadeMinima=3");
        var pagina = await resposta.Content.ReadFromJsonAsync<PagedResult<RelatoComportamentoResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Single(pagina!.Items);
        Assert.Equal(4, pagina.Items[0].Intensidade);
    }
}
