using System.Net.Http.Headers;
using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Abstractions.Security;
using KuraVet.Domain.Entities;
using KuraVet.Infrastructure.HealthChecks;
using KuraVet.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KuraVet.Api.Tests.Integration.Fixtures;

/// Sobe a API completa em memória
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string AdminEmail = "admin@teste.kuravet.com";
    public const string AdminSenha = "AdminTeste@123";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:OracleConnection"] = "User Id=teste;Password=teste;Data Source=localhost:1521/XE;",
                ["Jwt:Issuer"] = "KuraVet.Api",
                ["Jwt:Audience"] = "KuraVet.Clients",
                ["Jwt:Key"] = "chave-de-teste-de-integracao-kuravet-com-mais-de-32-caracteres",
                ["Jwt:ExpirationMinutes"] = "60",
                ["Seed:Admin:Nome"] = "Admin de Teste",
                ["Seed:Admin:Email"] = AdminEmail,
                ["Seed:Admin:Senha"] = AdminSenha
            });
        });

        var inMemoryDatabaseName = $"KuraVetTestDb_{Guid.NewGuid()}";

        builder.ConfigureServices(services =>
        {
            // Oracle 
            services.RemoveAll<DbContextOptions<KuraVetDbContext>>();
            services.AddDbContext<KuraVetDbContext>(options => options.UseInMemoryDatabase(inMemoryDatabaseName));

            // MongoDB -> repositório em memória
            services.RemoveAll<IRelatoComportamentoRepository>();
            services.AddSingleton<IRelatoComportamentoRepository, InMemoryRelatoComportamentoRepository>();

            // Sem MongoDB real nos testes: remove o health check correspondente
            services.Configure<HealthCheckServiceOptions>(options =>
            {
                var mongo = options.Registrations.Where(r => r.Name == InfrastructureHealthChecks.MongoDb).ToList();
                foreach (var registro in mongo)
                {
                    options.Registrations.Remove(registro);
                }
            });
        });
    }

    /// Cliente com um JWT válido 
    public HttpClient CriarClienteAutenticado(string perfil = Perfis.Admin)
    {
        var client = CreateClient();

        using var scope = Services.CreateScope();
        var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        var usuario = new Usuario(
            $"Usuário {perfil}", $"{perfil.ToLowerInvariant()}@teste.kuravet.com", "hash-nao-utilizado", perfil);

        var token = tokenService.GerarToken(usuario);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        return client;
    }
}
