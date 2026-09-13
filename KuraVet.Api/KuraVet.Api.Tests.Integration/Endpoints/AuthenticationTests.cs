using System.Net;
using KuraVet.Api.Infrastructure.Authentication;
using KuraVet.Api.Tests.Integration.Fixtures;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints
{

    [Collection(KuraVetApiCollection.Name)]
    public class AuthenticationTests
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthenticationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_SemApiKey_DeveRetornar401()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var resposta = await client.GetAsync("/api/Tutor");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }

        [Fact]
        public async Task Get_ComApiKeyInvalida_DeveRetornar401()
        {
            // Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiKeyDefaults.HeaderName, "chave-incorreta");

            // Act
            var resposta = await client.GetAsync("/api/Tutor");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        }

        [Fact]
        public async Task Get_ComApiKeyValida_DeveRetornarSucesso()
        {
            // Arrange
            var client = _factory.CreateAuthenticatedClient();

            // Act
            var resposta = await client.GetAsync("/api/Tutor");

            // Assert
            Assert.True(
                resposta.StatusCode is HttpStatusCode.OK or HttpStatusCode.NoContent,
                $"Esperado 200 ou 204, mas foi {resposta.StatusCode}");
        }

        [Fact]
        public async Task Get_HealthCheck_NaoExigeApiKey()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var resposta = await client.GetAsync("/health/live");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }
    }
}
