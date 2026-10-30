using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Tests.Integration.Fixtures;
using KuraVet.Application.Common;
using KuraVet.Application.EventosConsulta;
using KuraVet.Domain.Entities;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints;

[Collection(KuraVetApiCollection.Name)]
public class EventoConsultaEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _admin;

    public EventoConsultaEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _admin = factory.CriarClienteAutenticado(Perfis.Admin);
    }

    private static EventoConsultaRequest CriarRequest(int petId, string tipo, DateTime data) => new()
    {
        TipoEvento = tipo,
        DataEvento = data,
        Descricao = $"{tipo} registrada no teste",
        VeterinarioResponsavel = "Dra. Marina Lopes",
        PetId = petId
    };

    [Fact]
    public async Task Post_PerfilVeterinario_DeveRetornar201()
    {
        // Arrange
        var pet = await _admin.CriarPetAsync("Pet Vacina");
        var veterinario = _factory.CriarClienteAutenticado(Perfis.Veterinario);

        // Act
        var resposta = await veterinario.PostAsJsonAsync(
            "/api/EventoConsulta", CriarRequest(pet.Id, "Vacina", DateTime.UtcNow));
        var evento = await resposta.Content.ReadFromJsonAsync<EventoConsultaResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.Equal("Vacina", evento!.TipoEvento);
        Assert.Contains(evento.Links, l => l.Rel == "self");
    }

    [Fact]
    public async Task Post_PerfilTutor_DeveRetornar403()
    {
        // Arrange
        var pet = await _admin.CriarPetAsync("Pet Sem Permissão");
        var tutor = _factory.CriarClienteAutenticado(Perfis.Tutor);

        // Act
        var resposta = await tutor.PostAsJsonAsync("/api/EventoConsulta", CriarRequest(pet.Id, "Exame", DateTime.UtcNow));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task Post_SemDescricao_DeveRetornar400()
    {
        // Arrange
        var pet = await _admin.CriarPetAsync();
        var request = CriarRequest(pet.Id, "Exame", DateTime.UtcNow);
        request.Descricao = "";

        // Act
        var resposta = await _admin.PostAsJsonAsync("/api/EventoConsulta", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task GetTimeline_DoisEventos_DeveRetornarOrdenadoPorDataDecrescente()
    {
        // Arrange
        var pet = await _admin.CriarPetAsync("Pet Timeline");
        await _admin.PostAsJsonAsync("/api/EventoConsulta", CriarRequest(pet.Id, "Consulta", new DateTime(2026, 8, 1, 10, 0, 0, DateTimeKind.Utc)));
        await _admin.PostAsJsonAsync("/api/EventoConsulta", CriarRequest(pet.Id, "Retorno", new DateTime(2026, 9, 1, 10, 0, 0, DateTimeKind.Utc)));

        // Act
        var resposta = await _admin.GetAsync($"/api/EventoConsulta/pet/{pet.Id}");
        var pagina = await resposta.Content.ReadFromJsonAsync<PagedResult<EventoConsultaResponse>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal(new[] { "Retorno", "Consulta" }, pagina!.Items.Select(e => e.TipoEvento));
    }

    [Fact]
    public async Task GetTimeline_FiltroPorTipo_DeveRetornarSomenteOTipo()
    {
        // Arrange
        var pet = await _admin.CriarPetAsync("Pet Filtro Tipo");
        await _admin.PostAsJsonAsync("/api/EventoConsulta", CriarRequest(pet.Id, "Vacina", DateTime.UtcNow));
        await _admin.PostAsJsonAsync("/api/EventoConsulta", CriarRequest(pet.Id, "Exame", DateTime.UtcNow));

        // Act
        var resposta = await _admin.GetAsync($"/api/EventoConsulta?petId={pet.Id}&tipoEvento=vacina");
        var pagina = await resposta.Content.ReadFromJsonAsync<PagedResult<EventoConsultaResponse>>();

        // Assert
        Assert.Single(pagina!.Items);
        Assert.Equal("Vacina", pagina.Items[0].TipoEvento);
    }
}
