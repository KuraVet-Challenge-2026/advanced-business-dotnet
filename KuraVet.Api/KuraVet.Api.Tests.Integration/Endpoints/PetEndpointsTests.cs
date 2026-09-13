using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Models;
using KuraVet.Api.Tests.Integration.Fixtures;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints
{
    [Collection(KuraVetApiCollection.Name)]
    public class PetEndpointsTests
    {
        private readonly HttpClient _client;

        public PetEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateAuthenticatedClient();
        }

        private async Task<Tutor> CriarTutorAsync(string nome)
        {
            var resposta = await _client.PostAsJsonAsync("/api/Tutor", new Tutor { Nome = nome });
            var tutor = await resposta.Content.ReadFromJsonAsync<Tutor>();
            return tutor!;
        }

        [Fact]
        public async Task Post_PetValido_DeveRetornar201()
        {
            // Arrange
            var tutor = await CriarTutorAsync("Tutor do Pet Válido");
            var novoPet = new Pet { Nome = "Thor", TutorId = tutor.Id };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/Pet", novoPet);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task GetPorTutor_TutorComPetsCadastrados_DeveRetornarOkComLista()
        {
            // Arrange
            var tutor = await CriarTutorAsync("Tutor com Pets");
            await _client.PostAsJsonAsync("/api/Pet", new Pet { Nome = "Mel", TutorId = tutor.Id });
            await _client.PostAsJsonAsync("/api/Pet", new Pet { Nome = "Luna", TutorId = tutor.Id });

            // Act
            var resposta = await _client.GetAsync($"/api/Pet/tutor/{tutor.Id}");
            var pets = await resposta.Content.ReadFromJsonAsync<List<Pet>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(2, pets!.Count);
        }

        [Fact]
        public async Task GetPorTutor_TutorSemPets_DeveRetornar204()
        {
            // Act
            var resposta = await _client.GetAsync("/api/Pet/tutor/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, resposta.StatusCode);
        }

        [Fact]
        public async Task Get_PetInexistente_DeveRetornar404()
        {
            // Act
            var resposta = await _client.GetAsync("/api/Pet/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }
    }
}
