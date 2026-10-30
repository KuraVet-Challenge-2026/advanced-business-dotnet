using KuraVet.Domain.Entities;

namespace KuraVet.Application.Abstractions.Security;

public interface ITokenService
{
    TokenGerado GerarToken(Usuario usuario);
}

public sealed record TokenGerado(string AccessToken, DateTime ExpiraEm);
