namespace KuraVet.Application.Abstractions.Security;

public interface IPasswordHasher
{
    string Hash(string senha);

    bool Verificar(string senha, string hash);
}
