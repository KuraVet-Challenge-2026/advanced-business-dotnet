using System.Text.RegularExpressions;
using KuraVet.Domain.Common;
using KuraVet.Domain.Exceptions;

namespace KuraVet.Domain.Entities;

/// Usuário autenticável da API. 
public sealed class Usuario : Entidade
{
    public const int NomeTamanhoMaximo = 100;
    public const int EmailTamanhoMaximo = 150;
    public const int SenhaHashTamanhoMaximo = 100;
    public const int PerfilTamanhoMaximo = 20;

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private Usuario()
    {
    }

    public Usuario(string nome, string email, string senhaHash, string perfil)
    {
        Nome = Guard.TextoObrigatorio(nome, nameof(Nome), NomeTamanhoMaximo);
        Email = NormalizarEmail(email);
        SenhaHash = Guard.TextoObrigatorio(senhaHash, nameof(SenhaHash), SenhaHashTamanhoMaximo);
        Perfil = Perfis.Normalizar(perfil);
        DataCriacao = DateTime.UtcNow;
    }

    public string Nome { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string SenhaHash { get; private set; } = string.Empty;

    public string Perfil { get; private set; } = Perfis.Tutor;

    public DateTime DataCriacao { get; private set; }

    public static string NormalizarEmail(string email)
    {
        var normalizado = Guard.TextoObrigatorio(email, nameof(Email), EmailTamanhoMaximo).ToLowerInvariant();
        if (!EmailRegex.IsMatch(normalizado))
        {
            throw new DomainException($"O e-mail '{normalizado}' é inválido.");
        }

        return normalizado;
    }
}
