using System.Net;
using System.Net.Http.Json;
using KuraVet.Api.Models;
using KuraVet.Api.Tests.Integration.Fixtures;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints
{
    [Collection(KuraVetApiCollection.Name)]
    public class TutorEndpointsTests
    {
        private readonly HttpClient _client;

        public TutorEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateAuthenticatedClient();
        }

        [Fact]
        public async Task Post_TutorValido_DeveRetornar201EPermitirConsultaPosterior()
        {
            // Arrange
            var novoTutor = new Tutor { Nome = "Juliana Ferreira" };

            // Act
            var respostaPost = await _client.PostAsJsonAsync("/api/Tutor", novoTutor);

            // Assert
            Assert.Equal(HttpStatusCode.Created, respostaPost.StatusCode);

            var tutorCriado = await respostaPost.Content.ReadFromJsonAsync<Tutor>();
            Assert.NotNull(tutorCriado);
            Assert.True(tutorCriado!.Id > 0);

            var respostaGet = await _client.GetAsync($"/api/Tutor/{tutorCriado.Id}");
            Assert.Equal(HttpStatusCode.OK, respostaGet.StatusCode);
        }

        [Fact]
        public async Task Post_TutorSemNome_DeveRetornar400()
        {
            // Arrange
            var tutorInvalido = new { Nome = "" };

            // Act
            var resposta = await _client.PostAsJsonAsync("/api/Tutor", tutorInvalido);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        }

        [Fact]
        public async Task Get_TutorInexistente_DeveRetornar404()
        {
            // Act
            var resposta = await _client.GetAsync("/api/Tutor/999999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task Put_TutorExistente_DeveRetornar200ComNomeAtualizado()
        {
            // Arrange
            var tutorOriginal = new Tutor { Nome = "Nome Original" };
            var respostaPost = await _client.PostAsJsonAsync("/api/Tutor", tutorOriginal);
            var tutorCriado = await respostaPost.Content.ReadFromJsonAsync<Tutor>();

            var tutorAtualizado = new Tutor { Nome = "Nome Atualizado" };

            // Act
            var respostaPut = await _client.PutAsJsonAsync($"/api/Tutor/{tutorCriado!.Id}", tutorAtualizado);

            // Assert
            Assert.Equal(HttpStatusCode.OK, respostaPut.StatusCode);
            var retornado = await respostaPut.Content.ReadFromJsonAsync<Tutor>();
            Assert.Equal("Nome Atualizado", retornado!.Nome);
        }

        [Fact]
        public async Task Put_TutorInexistente_DeveRetornar404()
        {
            // Arrange
            var tutorAtualizado = new Tutor { Nome = "Não Importa" };

            // Act
            var resposta = await _client.PutAsJsonAsync("/api/Tutor/999999", tutorAtualizado);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        }

        [Fact]
        public async Task Delete_TutorExistente_DeveRetornar200EDepoisNaoDeveMaisExistir()
        {
            // Arrange
            var tutor = new Tutor { Nome = "Tutor Para Remover" };
            var respostaPost = await _client.PostAsJsonAsync("/api/Tutor", tutor);
            var tutorCriado = await respostaPost.Content.ReadFromJsonAsync<Tutor>();

            // Act
            var respostaDelete = await _client.DeleteAsync($"/api/Tutor/{tutorCriado!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, respostaDelete.StatusCode);

            var respostaGet = await _client.GetAsync($"/api/Tutor/{tutorCriado.Id}");
            Assert.Equal(HttpStatusCode.NotFound, respostaGet.StatusCode);
        }
    }
}
