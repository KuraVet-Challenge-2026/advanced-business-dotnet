using System.Net.Http.Json;
using KuraVet.Application.Pets;
using KuraVet.Application.Tutores;

namespace KuraVet.Api.Tests.Integration.Fixtures;

/// criar dados pela própria API nos testes de integração.
public static class ApiTestData
{
    public static async Task<TutorResponse> CriarTutorAsync(this HttpClient client, string nome = "Tutor de Teste")
    {
        var resposta = await client.PostAsJsonAsync("/api/Tutor", new TutorRequest { Nome = nome });
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<TutorResponse>())!;
    }

    public static async Task<PetResponse> CriarPetAsync(this HttpClient client, string nome = "Pet de Teste")
    {
        var tutor = await client.CriarTutorAsync($"Tutor de {nome}");
        var resposta = await client.PostAsJsonAsync("/api/Pet", new PetRequest { Nome = nome, TutorId = tutor.Id });
        resposta.EnsureSuccessStatusCode();
        return (await resposta.Content.ReadFromJsonAsync<PetResponse>())!;
    }
}
