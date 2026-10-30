using System.Text;
using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Abstractions.Security;
using KuraVet.Infrastructure.Mongo;
using KuraVet.Infrastructure.Persistence;
using KuraVet.Infrastructure.Persistence.Repositories;
using KuraVet.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace KuraVet.Infrastructure;

public static class DependencyInjection
{
    public const string OracleConnectionName = "OracleConnection";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddOracle(services, configuration);
        AddMongo(services, configuration);
        AddSecurity(services, configuration);
        return services;
    }

    private static void AddOracle(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<KuraVetDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString(OracleConnectionName);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    $"ConnectionStrings:{OracleConnectionName} não configurada. Use dotnet user-secrets (veja o README).");
            }

            options.UseOracle(connectionString);
        });

        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<ICheckinRepository, CheckinRepository>();
        services.AddScoped<IEventoConsultaRepository, EventoConsultaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
    }

    private static void AddMongo(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<MongoSettings>()
            .Bind(configuration.GetSection(MongoSettings.SectionName));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(settings.ServerSelectionTimeoutSeconds);
            return new MongoClient(clientSettings);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
            return sp.GetRequiredService<IMongoClient>().GetDatabase(settings.DatabaseName);
        });

        services.AddScoped<IRelatoComportamentoRepository, RelatoComportamentoRepository>();
    }

    private static void AddSecurity(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.SectionName))
            .Validate(
                s => !string.IsNullOrWhiteSpace(s.Key) && Encoding.UTF8.GetByteCount(s.Key) >= JwtSettings.TamanhoMinimoChave,
                $"Jwt:Key não configurada ou com menos de {JwtSettings.TamanhoMinimoChave} caracteres. Use dotnet user-secrets (veja o README).")
            .Validate(s => s.ExpirationMinutes > 0, "Jwt:ExpirationMinutes deve ser maior que zero.");

        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
    }
}
