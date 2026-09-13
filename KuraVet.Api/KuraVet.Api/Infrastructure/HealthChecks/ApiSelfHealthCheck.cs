using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KuraVet.Api.Infrastructure.HealthChecks
{
    public class ApiSelfHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                HealthCheckResult.Healthy("A API KuraVet está em execução e respondendo normalmente."));
        }
    }
}
