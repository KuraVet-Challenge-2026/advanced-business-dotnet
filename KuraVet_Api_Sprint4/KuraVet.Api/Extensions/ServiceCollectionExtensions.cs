using System.Text;
using KuraVet.Api.ExceptionHandling;
using KuraVet.Api.Hateoas;
using KuraVet.Api.Observability;
using KuraVet.Application.Abstractions.Security;
using KuraVet.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace KuraVet.Api.Extensions;

public static class ServiceCollectionExtensions
{
    /// Controllers, ProblemDetails, tratamento global de exceções e HATEOAS.
    public static IServiceCollection AddApiPresentation(this IServiceCollection services)
    {
        services.AddControllers();

        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.TryAdd("traceId", context.HttpContext.TraceIdentifier);
            };
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddHttpContextAccessor();
        services.AddScoped<IHateoasLinkBuilder, HateoasLinkBuilder>();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
    {
        services.AddOptions<JwtSettings>().ValidateOnStart();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services
            .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
            {
                var jwt = jwtOptions.Value;

                // Mantém os nomes curtos das claims ("sub", "role"...)
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1),
                    NameClaimType = ClaimsKuraVet.Nome,
                    RoleClaimType = ClaimsKuraVet.Perfil
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "KuraVet API",
                Version = "v1",
                Description = "API RESTful do ecossistema de saúde veterinária contínua KuraVet " +
                              "tutores, pets, check-ins de sinais vitais, " +
                              "linha do tempo clínica e relatos de comportamento."
            });

            c.EnableAnnotations();

            foreach (var xml in new[] { "KuraVet.Api.xml", "KuraVet.Application.xml" })
            {
                var caminho = Path.Combine(AppContext.BaseDirectory, xml);
                if (File.Exists(caminho))
                {
                    c.IncludeXmlComments(caminho);
                }
            }

            c.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Faça POST /api/auth/login e cole aqui apenas o accessToken."
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }

    /// Tracing e métricas com OpenTelemetry
    public static IServiceCollection AddObservability(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(
                serviceName: "KuraVet.Api",
                serviceVersion: "4.0.0"))
            .WithTracing(tracing => tracing
                .AddSource(ApiTelemetry.ActivitySourceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddConsoleExporter())
            .WithMetrics(metrics => metrics
                .AddMeter(ApiTelemetry.MeterName)
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());

        return services;
    }
}
