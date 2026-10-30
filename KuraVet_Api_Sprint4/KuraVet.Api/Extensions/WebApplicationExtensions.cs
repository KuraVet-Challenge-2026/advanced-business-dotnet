using KuraVet.Api.HealthChecks;
using KuraVet.Application.Auth;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace KuraVet.Api.Extensions;

public static class WebApplicationExtensions
{
    /// Health checks públicos
    public static WebApplication MapHealthEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        }).AllowAnonymous();

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        }).AllowAnonymous();

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = HealthCheckResponseWriter.WriteResponse
        }).AllowAnonymous();

        return app;
    }

    /// Cria o administrador inicial a partir de Seed:Admin (user-secrets), se ainda não existir.
    public static async Task SeedAdministradorAsync(this WebApplication app)
    {
        var secao = app.Configuration.GetSection("Seed:Admin");
        var email = secao["Email"];
        var senha = secao["Senha"];
        var nome = string.IsNullOrWhiteSpace(secao["Nome"]) ? "Administrador KuraVet" : secao["Nome"]!;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            app.Logger.LogInformation("Seed do administrador ignorado: Seed:Admin:Email/Senha não configurados.");
            return;
        }

        try
        {
            using var scope = app.Services.CreateScope();
            var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();
            var criado = await authService.GarantirAdministradorAsync(nome, email, senha, CancellationToken.None);

            if (criado)
            {
                app.Logger.LogInformation("Administrador inicial criado.");
            }
            else
            {
                app.Logger.LogInformation("Administrador inicial já existente.");
            }
        }
        catch (Exception ex)
        {
            app.Logger.LogWarning(ex, "Não foi possível executar o seed do administrador. As migrations foram aplicadas?");
        }
    }
}
