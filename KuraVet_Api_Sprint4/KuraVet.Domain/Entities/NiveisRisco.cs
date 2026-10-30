using KuraVet.Domain.Exceptions;

namespace KuraVet.Domain.Entities;

/// Níveis de risco atribuídos a um check-in de sinais vitais.
public static class NiveisRisco
{
    public const string Baixo = "Baixo";
    public const string Moderado = "Moderado";
    public const string Grave = "Grave";

    public static readonly IReadOnlyList<string> Todos = new[] { Baixo, Moderado, Grave };

    public static string Normalizar(string nivel)
    {
        var encontrado = Todos.FirstOrDefault(n => string.Equals(n, nivel?.Trim(), StringComparison.OrdinalIgnoreCase));
        return encontrado ?? throw new DomainException(
            $"Nível de risco '{nivel}' inválido. Valores aceitos: {string.Join(", ", Todos)}.");
    }
}
