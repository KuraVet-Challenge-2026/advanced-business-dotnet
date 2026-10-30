using System.Globalization;
using System.Security.Claims;
using KuraVet.Application.Abstractions.Security;

namespace KuraVet.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int? ObterUsuarioId(this ClaimsPrincipal principal)
    {
        var valor = principal.FindFirst(ClaimsKuraVet.Sub)?.Value;
        return int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) && id > 0 ? id : null;
    }

    public static string? ObterEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimsKuraVet.Email)?.Value;
    }
}
