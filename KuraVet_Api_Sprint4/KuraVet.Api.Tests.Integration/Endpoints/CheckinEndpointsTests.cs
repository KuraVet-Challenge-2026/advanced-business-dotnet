using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Tests.Integration.Fixtures;
using KuraVet.Application.Checkins;
using KuraVet.Application.Common;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints;

[Collection(KuraVetApiCollection.Name)]
public class CheckinEndpointsTests
{
    private readonly HttpClient _client;

    public CheckinEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CriarClienteAutenticado();
    }

    private static CheckinRequest CriarRequest(int petId, int tpc, DateTime? data = null) => new()
    {
        DataCheckin = data,
        FrequenciaRespiratoria = 30,
        TempoPreenchimentoCapilar = tpc,
        CorMucosa = "Pálida",
        NivelHidratacao = "Reduzida",
        PetId = petId
    };

    [Fact]
    public async Task Post_CheckinComTpcAcimaDeDois_DeveRetornarRiscoModerado()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Pet Moderado");

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Checkin", CriarRequest(pet.Id, tpc: 4));
        var checkin = await resposta.Content.ReadFromJsonAsync<CheckinResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.Equal("Moderado", checkin!.NivelRiscoIA);
        Assert.Contains(checkin.Links, l => l.Rel == "pet" && l.Href.EndsWith($"/api/Pet/{pet.Id}"));
    }

    [Fact]
    public async Task Post_CheckinComTpcNormal_DeveRetornarRiscoBaixo()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Pet Baixo");

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Checkin", CriarRequest(pet.Id, tpc: 1));
        var checkin = await resposta.Content.ReadFromJsonAsync<CheckinResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.Equal("Baixo", checkin!.NivelRiscoIA);
    }

    [Fact]
    public async Task Post_CheckinComFrequenciaInvalida_DeveRetornar400()
    {
        // Arrange
        var pet = await _client.CriarPetAsync();
        var request = CriarRequest(pet.Id, tpc: 1);
        request.FrequenciaRespiratoria = 0;

        // Act
        var resposta = await _client.PostAsJsonAsync("/api/Checkin", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetPorPet_VariosCheckins_DeveRetornarMaisRecentesPrimeiro()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Pet Histórico");
        var antigo = new DateTime(2026, 9, 1, 8, 0, 0, DateTimeKind.Utc);
        var recente = new DateTime(2026, 10, 1, 8, 0, 0, DateTimeKind.Utc);
        await _client.PostAsJsonAsync("/api/Checkin", CriarRequest(pet.Id, 1, antigo));
        await _client.PostAsJsonAsync("/api/Checkin", CriarRequest(pet.Id, 3, recente));

        // Act
        var resposta = await _client.GetAsync($"/api/Checkin/pet/{pet.Id}");
        var pagina = await resposta.Content.ReadFromJsonAsync<PagedResult<CheckinResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal(2, pagina!.TotalItems);
        Assert.Equal(recente, pagina.Items[0].DataCheckin.ToUniversalTime());
    }

    [Fact]
    public async Task GetPorRisco_Moderado_DeveRetornarSomenteModerados()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Pet Filtro Risco");
        await _client.PostAsJsonAsync("/api/Checkin", CriarRequest(pet.Id, tpc: 5));
        await _client.PostAsJsonAsync("/api/Checkin", CriarRequest(pet.Id, tpc: 1));

        // Act
        var resposta = await _client.GetAsync($"/api/Checkin/risco/moderado?petId={pet.Id}");
        var pagina = await resposta.Content.ReadFromJsonAsync<PagedResult<CheckinResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Single(pagina!.Items);
        Assert.All(pagina.Items, c => Assert.Equal("Moderado", c.NivelRiscoIA));
    }

    [Fact]
    public async Task GetPorRisco_NivelInvalido_DeveRetornar400()
    {
        // Act
        var resposta = await _client.GetAsync("/api/Checkin/risco/altissimo");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task Put_CheckinExistente_DeveRecalcularRisco()
    {
        // Arrange
        var pet = await _client.CriarPetAsync("Pet Recalculo");
        var criado = await (await _client.PostAsJsonAsync("/api/Checkin", CriarRequest(pet.Id, tpc: 1)))
            .Content.ReadFromJsonAsync<CheckinResponse>();

        // Act
        var resposta = await _client.PutAsJsonAsync($"/api/Checkin/{criado!.Id}", CriarRequest(pet.Id, tpc: 4));
        var atualizado = await resposta.Content.ReadFromJsonAsync<CheckinResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("Moderado", atualizado!.NivelRiscoIA);
    }
}
