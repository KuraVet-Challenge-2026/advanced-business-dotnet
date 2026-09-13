using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Models;
using KuraVet.Api.Tests.Integration.Fixtures;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints
{
    [Collection(KuraVetApiCollection.Name)]
    public class CheckinEndpointsTests
    {
        private readonly HttpClient _client;

        public CheckinEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateAuthenticatedClient();
        }

        private async Task<Pet> CriarPetAsync()
        {
            var respostaTutor = await _client.PostAsJsonAsync("/api/Tutor", new Tutor { Nome = "Tutor de Checkin" });
            var tutor = await respostaTutor.Content.ReadFromJsonAsync<Tutor>();

            var respostaPet = await _client.PostAsJsonAsync("/api/Pet", new Pet { Nome = "Pet de Checkin", TutorId = tutor!.Id });
            var pet = await respostaPet.Content.ReadFromJsonAsync<Pet>();
            return pet!;
        }

        [Fact]
        public async Task Post_CheckinComTpcAcimaDeDois_DeveRetornarRiscoModerado()
        {
            // Arrange
            var pet = await CriarPetAsync();
            var checkin = new CheckinHistorico
            {
                DataCheckin = DateTime.Now,
                FrequenciaRespiratoria = 30,
                TempoPreenchimentoCapilar = 4,
                CorMucosa = "Pálida",
                NivelHidratacao = "Reduzida",
                PetId = pet.Id
            };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/Checkin", checkin);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
            var checkinCriado = await resposta.Content.ReadFromJsonAsync<CheckinHistorico>();
            Assert.Equal("Moderado", checkinCriado!.NivelRiscoIA);
        }

        [Fact]
        public async Task Post_CheckinComTpcAteDois_DeveRetornarRiscoBaixo()
        {
            // Arrange
            var pet = await CriarPetAsync();
            var checkin = new CheckinHistorico
            {
                DataCheckin = DateTime.Now,
                FrequenciaRespiratoria = 20,
                TempoPreenchimentoCapilar = 1,
                CorMucosa = "Rosada",
                NivelHidratacao = "Normal",
                PetId = pet.Id
            };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/Checkin", checkin);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
            var checkinCriado = await resposta.Content.ReadFromJsonAsync<CheckinHistorico>();
            Assert.Equal("Baixo", checkinCriado!.NivelRiscoIA);
        }

        [Fact]
        public async Task GetPorRisco_FiltroModerado_DeveRetornarApenasCheckinsModerados()
        {
            // Arrange
            var pet = await CriarPetAsync();
            await _client.PostAsJsonAsync("/api/Checkin", new CheckinHistorico
            {
                DataCheckin = DateTime.Now,
                FrequenciaRespiratoria = 30,
                TempoPreenchimentoCapilar = 5,
                CorMucosa = "Pálida",
                NivelHidratacao = "Reduzida",
                PetId = pet.Id
            });

            // Act
            var resposta = await _client.GetAsync("/api/Checkin/risco/Moderado");
            var checkins = await resposta.Content.ReadFromJsonAsync<List<CheckinHistorico>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.NotEmpty(checkins!);
            Assert.All(checkins!, c => Assert.Equal("Moderado", c.NivelRiscoIA));
        }

        [Fact]
        public async Task Get_CheckinInexistente_DeveRetornar404()
        {
            // Act
            var resposta = await _client.GetAsync("/api/Checkin/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }
    }
}
