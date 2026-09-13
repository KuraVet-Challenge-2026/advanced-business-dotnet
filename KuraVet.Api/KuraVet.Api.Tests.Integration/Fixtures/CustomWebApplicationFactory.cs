using KuraVet.Api.Data;
using KuraVet.Api.Infrastructure.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KuraVet.Api.Tests.Integration.Fixtures
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public string ApiKey => "kuravet-test-key";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((_, config) =>
            {
                var overrides = new Dictionary<string, string?>
                {
                    ["Authentication:ApiKey"] = ApiKey
                };

                config.AddInMemoryCollection(overrides);
            });

            var inMemoryDatabaseName = $"KuraVetTestDb_{Guid.NewGuid()}";

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<KuraVetDbContext>));

                if (descriptor is not null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<KuraVetDbContext>(options =>
                {
                    options.UseInMemoryDatabase(inMemoryDatabaseName);
                });
            });
        }

        public HttpClient CreateAuthenticatedClient()
        {
            var client = CreateClient();
            client.DefaultRequestHeaders.Add(ApiKeyDefaults.HeaderName, ApiKey);
            return client;
        }
    }
}
