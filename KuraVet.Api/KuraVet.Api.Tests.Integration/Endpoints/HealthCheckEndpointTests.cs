using System.Net;
using System.Text.Json;
using KuraVet.Api.Tests.Integration.Fixtures;
using Xunit;

namespace KuraVet.Api.Tests.Integration.Endpoints
{
    [Collection(KuraVetApiCollection.Name)]
    public class HealthCheckEndpointTests
    {
        private readonly CustomWebApplicationFactory _factory;

        public HealthCheckEndpointTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Get_Health_DeveRetornarOkComStatusHealthy()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var resposta = await client.GetAsync("/health");
            var corpo = await resposta.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

            using var json = JsonDocument.Parse(corpo);
            var status = json.RootElement.GetProperty("status").GetString();
            Assert.Equal("Healthy", status);
        }

        [Fact]
        public async Task Get_HealthLive_DeveRetornarOk()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var resposta = await client.GetAsync("/health/live");

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        }

        [Fact]
        public async Task Get_HealthReady_DeveRetornarOkComChecagemDeBancoDeDados()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var resposta = await client.GetAsync("/health/ready");
            var corpo = await resposta.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
            Assert.Contains("oracle-database", corpo);
        }
    }
}
