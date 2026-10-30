using KuraVet.Domain.Exceptions;

namespace KuraVet.Domain.Entities;

/// Perfis (roles) de acesso usados na autorização JWT.
public static class Perfis
{
    public const string Admin = "Admin";
    public const string Veterinario = "Veterinario";
    public const string Tutor = "Tutor";

    public static readonly IReadOnlyList<string> Todos = new[] { Admin, Veterinario, Tutor };

    public static string Normalizar(string perfil)
    {
        var encontrado = Todos.FirstOrDefault(p => string.Equals(p, perfil?.Trim(), StringComparison.OrdinalIgnoreCase));
        return encontrado ?? throw new DomainException(
            $"Perfil '{perfil}' inválido. Valores aceitos: {string.Join(", ", Todos)}.");
    }
}
