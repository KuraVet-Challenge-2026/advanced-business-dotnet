using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace KuraVet.Api.Infrastructure.Observability
{
    public static class ApiTelemetry
    {
        public const string ActivitySourceName = "KuraVet.Api";
        public const string MeterName = "KuraVet.Api.Metrics";

        public static readonly ActivitySource ActivitySource = new(ActivitySourceName);

        private static readonly Meter Meter = new(MeterName);

        public static readonly Counter<long> RequestsTotal = Meter.CreateCounter<long>(
            "kuravet_api_requests_total",
            description: "Total de requisições HTTP recebidas pela API.");

        public static readonly Counter<long> RequestErrorsTotal = Meter.CreateCounter<long>(
            "kuravet_api_requests_errors_total",
            description: "Total de requisições HTTP finalizadas com status de erro (>= 400).");

        public static readonly Histogram<double> RequestDuration = Meter.CreateHistogram<double>(
            "kuravet_api_request_duration_ms",
            unit: "ms",
            description: "Tempo de resposta das requisições HTTP, em milissegundos.");
    }
}
