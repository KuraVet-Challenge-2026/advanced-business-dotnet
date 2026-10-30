using KuraVet.Application.Auth;
using KuraVet.Application.Checkins;
using KuraVet.Application.EventosConsulta;
using KuraVet.Application.Pets;
using KuraVet.Application.RelatosComportamento;
using KuraVet.Application.Tutores;
using KuraVet.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace KuraVet.Application;

public static class DependencyInjection
{
    /// Registra os casos de uso (services) e as regras de domínio.
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddSingleton<ICheckinRiscoService, CheckinRiscoService>();

        services.AddScoped<ITutorService, TutorService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<ICheckinService, CheckinService>();
        services.AddScoped<IEventoConsultaService, EventoConsultaService>();
        services.AddScoped<IRelatoComportamentoService, RelatoComportamentoService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
