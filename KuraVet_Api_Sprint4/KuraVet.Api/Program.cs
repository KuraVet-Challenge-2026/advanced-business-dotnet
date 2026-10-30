using KuraVet.Api.Extensions;
using KuraVet.Api.HealthChecks;
using KuraVet.Api.Middlewares;
using KuraVet.Application;
using KuraVet.Infrastructure;
using KuraVet.Infrastructure.HealthChecks;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddApiPresentation()
        .AddJwtAuthentication()
        .AddSwaggerDocumentation()
        .AddObservability();

    builder.Services.AddHealthChecks()
        .AddInfrastructureHealthChecks()
        .AddCheck<ApiSelfHealthCheck>("api-self", tags: new[] { "live" })
        .AddCheck<SystemResourcesHealthCheck>("system-resources", tags: new[] { "ready" });

    var app = builder.Build();

    app.UseMiddleware<RequestMetricsMiddleware>();
    app.UseExceptionHandler();
    app.UseStatusCodePages();
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "KuraVet API v1");
            c.DocumentTitle = "KuraVet API";
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapHealthEndpoints();

    await app.SeedAdministradorAsync();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Console.Error.WriteLine($"A aplicação KuraVet API falhou ao iniciar: {ex}");
    throw;
}

public partial class Program
{
}
