using KuraVet.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KuraVet.Infrastructure.HealthChecks;

public static class InfrastructureHealthChecks
{
    public const string Oracle = "oracle-database";
    public const string MongoDb = "mongodb";

  
    /// Oracle indisponível deixa a API Unhealthy; MongoDB indisponível deixa a API Degraded
  
    public static IHealthChecksBuilder AddInfrastructureHealthChecks(this IHealthChecksBuilder builder)
    {
        builder.AddDbContextCheck<KuraVetDbContext>(
            name: Oracle,
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready", "db" });

        builder.AddCheck<MongoDbHealthCheck>(
            MongoDb,
            failureStatus: HealthStatus.Degraded,
            tags: new[] { "ready", "db", "nosql" });

        return builder;
    }
}
