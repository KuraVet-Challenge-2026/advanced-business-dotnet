using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Tests.Integration.Fixtures;
using KuraVet.Application.Common;
using KuraVet.Application.Pets;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints;

[Collection(KuraVetApiCollection.Name)]
public class PetEndpointsTests
{
    private readonly HttpClient _client;

    public PetEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CriarClienteAutenticado();
    }

    [Fact]
    public async Task Post_PetComTutorValido_DeveRetornar201ComLinksRelacionados()
    {
        // Arrange
        var tutor = await _client.CriarTutorAsync();

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Pet", new PetRequest { Nome = "Thor", TutorId = tutor.Id });
        var pet = await resposta.Content.ReadFromJsonAsync<PetResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.Equal(tutor.Id, pet!.TutorId);
        Assert.Contains(pet.Links, l => l.Rel == "tutor" && l.Href.EndsWith($"/api/Tutor/{tutor.Id}"));
        Assert.Contains(pet.Links, l => l.Rel == "checkins" && l.Href.EndsWith($"/api/Checkin/pet/{pet.Id}"));
        Assert.Contains(pet.Links, l => l.Rel == "timeline" && l.Href.EndsWith($"/api/EventoConsulta/pet/{pet.Id}"));
        Assert.Contains(pet.Links, l => l.Rel == "relatos" && l.Href.Contains($"petId={pet.Id}"));
    }

    [Fact]
    public async Task Post_PetComTutorInexistente_DeveRetornar400()
    {
        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Pet", new PetRequest { Nome = "Órfão", TutorId = 999999 });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetPorTutor_TutorComPets_DeveRetornarSomenteOsPetsDoTutor()
    {
        // Arrange
        var tutor = await _client.CriarTutorAsync("Tutor com dois pets");
        await _client.PostAsJsonAsync("/api/Pet", new PetRequest { Nome = "Mel", TutorId = tutor.Id });
        await _client.PostAsJsonAsync("/api/Pet", new PetRequest { Nome = "Bolt", TutorId = tutor.Id });

        // Act
        var resposta = await _client.GetAsync($"/api/Pet/tutor/{tutor.Id}?sortBy=nome");
        var pagina = await resposta.Content.ReadFromJsonAsync<PagedResult<PetResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal(2, pagina!.TotalItems);
        Assert.Equal(new[] { "Bolt", "Mel" }, pagina.Items.Select(p => p.Nome));
        Assert.All(pagina.Items, p => Assert.Equal(tutor.Id, p.TutorId));
    }

    [Fact]
    public async Task GetPorTutor_TutorInexistente_DeveRetornar404()
    {
        // Act
        var resposta = await _client.GetAsync("/api/Pet/tutor/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task Put_PetExistente_DeveAtualizarNome()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Nome Antigo");

        // Act
        var resposta = await _client.PutAsJsonAsync(
            $"/api/Pet/{pet.Id}", new PetRequest { Nome = "Nome Novo", TutorId = pet.TutorId });
        var atualizado = await resposta.Content.ReadFromJsonAsync<PetResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("Nome Novo", atualizado!.Nome);
    }
}
