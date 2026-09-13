using System.Diagnostics;
using KuraVet.Api.Infrastructure.Observability;

namespace KuraVet.Api.Middlewares
{
    public class RequestMetricsMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestMetricsMiddleware> _logger;

        public RequestMetricsMiddleware(RequestDelegate next, ILogger<RequestMetricsMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            context.Response.OnStarting(() =>
            {
                context.Response.Headers["X-Correlation-Id"] = context.TraceIdentifier;
                return Task.CompletedTask;
            });

            using (Serilog.Context.LogContext.PushProperty("CorrelationId", context.TraceIdentifier))
            {
                try
                {
                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();

                    var route = context.GetEndpoint()?.DisplayName ?? context.Request.Path.ToString();
                    var statusCode = context.Response.StatusCode;

                    var tags = new KeyValuePair<string, object?>[]
                    {
                        new("http.method", context.Request.Method),
                        new("http.route", route),
                        new("http.status_code", statusCode)
                    };

                    ApiTelemetry.RequestsTotal.Add(1, tags);
                    ApiTelemetry.RequestDuration.Record(stopwatch.Elapsed.TotalMilliseconds, tags);

                    if (statusCode >= 400)
                    {
                        ApiTelemetry.RequestErrorsTotal.Add(1, tags);

                        _logger.LogWarning(
                            "Requisição {Method} {Path} finalizada com status {StatusCode} em {ElapsedMs}ms",
                            context.Request.Method, context.Request.Path, statusCode, stopwatch.Elapsed.TotalMilliseconds);
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Requisição {Method} {Path} finalizada com status {StatusCode} em {ElapsedMs}ms",
                            context.Request.Method, context.Request.Path, statusCode, stopwatch.Elapsed.TotalMilliseconds);
                    }
                }
            }
        }
    }
}
