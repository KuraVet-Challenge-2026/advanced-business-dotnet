using KuraVet.Api.Data;
using KuraVet.Api.Domain;
using KuraVet.Api.Infrastructure.Authentication;
using KuraVet.Api.Infrastructure.HealthChecks;
using KuraVet.Api.Infrastructure.Observability;
using KuraVet.Api.Middlewares;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Reconfigura o Serilog 
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // servios de Controllers
    builder.Services.AddControllers();

    // Configura Swagger com Annotations
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "KuraVet API",
            Version = "v1",
            Description = "API RESTful para o ecossistema de sade veterinria contnua KuraVet."
        });

        // Habilita o uso do SwaggerOperation
        c.EnableAnnotations();

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }

        // permite testar os endpoints autenticados direto pelo Swagger 
        c.AddSecurityDefinition(ApiKeyDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Name = ApiKeyDefaults.HeaderName,
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = "Informe a API Key configurada em 'Authentication:ApiKey' (appsettings.json)."
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = ApiKeyDefaults.AuthenticationScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // Configurao do Banco de Dados Oracle
    builder.Services.AddDbContext<KuraVetDbContext>(options =>
        options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

    // classificação de risco 
    builder.Services.AddScoped<ICheckinRiscoService, CheckinRiscoService>();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<KuraVetDbContext>(
            name: "oracle-database",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready", "db" })
        .AddCheck<ApiSelfHealthCheck>("api-self", tags: new[] { "live" })
        .AddCheck<SystemResourcesHealthCheck>("system-resources", tags: new[] { "ready" });

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource.AddService(
            serviceName: "KuraVet.Api",
            serviceVersion: "1.0.0"))
        .WithTracing(tracing => tracing
            .AddSource(ApiTelemetry.ActivitySourceName)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter())
        .WithMetrics(metrics => metrics
            .AddMeter(ApiTelemetry.MeterName)
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter());

    builder.Services
        .AddAuthentication(ApiKeyDefaults.AuthenticationScheme)
        .AddScheme<ApiKeyAuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
            ApiKeyDefaults.AuthenticationScheme, options => { });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    app.UseMiddleware<RequestMetricsMiddleware>();

    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "KuraVet API v1");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Endpoints de Health Check
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("live"),
        ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = HealthCheckResponseWriter.WriteResponse
    });

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Console.Error.WriteLine($"A aplicação KuraVet API falhou ao iniciar: {ex}");
    throw;
}

public partial class Program { }
