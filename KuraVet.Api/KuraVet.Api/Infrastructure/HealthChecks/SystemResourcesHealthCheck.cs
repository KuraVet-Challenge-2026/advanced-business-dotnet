using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KuraVet.Api.Infrastructure.HealthChecks
{
    public class SystemResourcesHealthCheck : IHealthCheck
    {
        private const long MemoriaLimiteBytes = 1024L * 1024L * 1024L; 

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var memoriaAlocadaBytes = GC.GetTotalMemory(forceFullCollection: false);

            var data = new Dictionary<string, object>
            {
                ["memoriaAlocadaBytes"] = memoriaAlocadaBytes,
                ["memoriaAlocadaMB"] = Math.Round(memoriaAlocadaBytes / 1024.0 / 1024.0, 2)
            };

            if (memoriaAlocadaBytes < MemoriaLimiteBytes)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("Uso de memória dentro do esperado.", data));
            }

            return Task.FromResult(
                HealthCheckResult.Degraded("Uso de memória acima do limite configurado.", data: data));
        }
    }
}
