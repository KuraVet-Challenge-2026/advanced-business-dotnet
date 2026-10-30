using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KuraVet.Application.Abstractions.Security;
using KuraVet.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KuraVet.Infrastructure.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly IOptions<JwtSettings> _settings;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        _settings = settings;
    }

    public TokenGerado GerarToken(Usuario usuario)
    {
        var settings = _settings.Value;
        var agora = DateTime.UtcNow;
        var expiraEm = agora.AddMinutes(settings.ExpirationMinutes);

        var claims = new List<Claim>
        {
            new(ClaimsKuraVet.Sub, usuario.Id.ToString()),
            new(ClaimsKuraVet.Nome, usuario.Nome),
            new(ClaimsKuraVet.Email, usuario.Email),
            new(ClaimsKuraVet.Perfil, usuario.Perfil),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            notBefore: agora,
            expires: expiraEm,
            signingCredentials: credenciais);

        return new TokenGerado(new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }
}
