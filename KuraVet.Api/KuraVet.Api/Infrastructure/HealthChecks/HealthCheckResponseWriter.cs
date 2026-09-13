using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace KuraVet.Api.Infrastructure.HealthChecks
{
    public static class HealthCheckResponseWriter
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true
        };

        public static async Task WriteResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            var payload = new
            {
                status = report.Status.ToString(),
                totalDurationMs = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    durationMs = entry.Value.Duration.TotalMilliseconds,
                    tags = entry.Value.Tags,
                    data = entry.Value.Data,
                    error = entry.Value.Exception?.Message
                })
            };

            var json = JsonSerializer.Serialize(payload, JsonOptions);

            await context.Response.WriteAsync(json);
        }
    }
}
