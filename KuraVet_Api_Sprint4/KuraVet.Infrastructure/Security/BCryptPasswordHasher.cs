using KuraVet.Application.Abstractions.Security;

namespace KuraVet.Infrastructure.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 11;

    public string Hash(string senha)
    {
        return BCrypt.Net.BCrypt.HashPassword(senha, WorkFactor);
    }

    public bool Verificar(string senha, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch (Exception ex) when (ex is BCrypt.Net.SaltParseException or ArgumentException)
        {
            // Hash malformado: trata como senha inválida em vez de 500
            return false;
        }
    }
}
