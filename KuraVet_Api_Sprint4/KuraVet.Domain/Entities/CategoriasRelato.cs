using KuraVet.Domain.Exceptions;

namespace KuraVet.Domain.Entities;

/// Categorias aceitas para os relatos de comportamento registrados pelo tutor.
public static class CategoriasRelato
{
    public const string Alimentacao = "Alimentacao";
    public const string Sono = "Sono";
    public const string Atividade = "Atividade";
    public const string Comportamento = "Comportamento";
    public const string Higiene = "Higiene";
    public const string Outro = "Outro";

    public static readonly IReadOnlyList<string> Todas = new[]
    {
        Alimentacao, Sono, Atividade, Comportamento, Higiene, Outro
    };

    public static string Normalizar(string categoria)
    {
        var encontrada = Todas.FirstOrDefault(c => string.Equals(c, categoria?.Trim(), StringComparison.OrdinalIgnoreCase));
        return encontrada ?? throw new DomainException(
            $"Categoria '{categoria}' inválida. Valores aceitos: {string.Join(", ", Todas)}.");
    }
}
