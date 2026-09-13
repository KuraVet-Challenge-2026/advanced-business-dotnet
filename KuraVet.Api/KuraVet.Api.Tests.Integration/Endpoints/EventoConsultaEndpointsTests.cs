using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Models;
using KuraVet.Api.Tests.Integration.Fixtures;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints
{
    [Collection(KuraVetApiCollection.Name)]
    public class EventoConsultaEndpointsTests
    {
        private readonly HttpClient _client;

        public EventoConsultaEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateAuthenticatedClient();
        }

        private async Task<Pet> CriarPetAsync()
        {
            var respostaTutor = await _client.PostAsJsonAsync("/api/Tutor", new Tutor { Nome = "Tutor de Evento" });
            var tutor = await respostaTutor.Content.ReadFromJsonAsync<Tutor>();

            var respostaPet = await _client.PostAsJsonAsync("/api/Pet", new Pet { Nome = "Pet de Evento", TutorId = tutor!.Id });
            var pet = await respostaPet.Content.ReadFromJsonAsync<Pet>();
            return pet!;
        }

        [Fact]
        public async Task Post_EventoValido_DeveRetornar201()
        {
            // Arrange
            var pet = await CriarPetAsync();
            var evento = new EventoConsulta
            {
                TipoEvento = "Vacina",
                DataEvento = DateTime.Now,
                Descricao = "Vacina antirrábica",
                VeterinarioResponsavel = "Dr. Marcos",
                PetId = pet.Id
            };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/EventoConsulta", evento);

            // Assert
            Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        }

        [Fact]
        public async Task GetPorPet_LinhaDoTempoComEventos_DeveRetornarOkOrdenadaPorData()
        {
            // Arrange
            var pet = await CriarPetAsync();
            await _client.PostAsJsonAsync("/api/EventoConsulta", new EventoConsulta
            {
                TipoEvento = "Consulta",
                DataEvento = DateTime.Now.AddDays(-5),
                VeterinarioResponsavel = "Dra. Renata",
                PetId = pet.Id
            });
            await _client.PostAsJsonAsync("/api/EventoConsulta", new EventoConsulta
            {
                TipoEvento = "Exame",
                DataEvento = DateTime.Now,
                VeterinarioResponsavel = "Dra. Renata",
                PetId = pet.Id
            });

            // Act
            var resposta = await _client.GetAsync($"/api/EventoConsulta/pet/{pet.Id}");
            var eventos = await resposta.Content.ReadFromJsonAsync<List<EventoConsulta>>();

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Equal(2, eventos!.Count);
            Assert.Equal("Exame", eventos.First().TipoEvento);
        }

        [Fact]
        public async Task Delete_EventoInexistente_DeveRetornar404()
        {
            // Act
            var resposta = await _client.DeleteAsync("/api/EventoConsulta/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }
    }
}
