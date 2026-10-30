using KuraVet.Domain.Common;
using KuraVet.Domain.Exceptions;

namespace KuraVet.Domain.Entities;

/// Relato livre do tutor sobre hábitos/comportamento do pet (dado IoB).
/// Persistido no MongoDB (coleção relatos_comportamento) e consumido pela triagem por IA.
public sealed class RelatoComportamento
{
    public const int DescricaoTamanhoMaximo = 2000;
    public const int TagTamanhoMaximo = 30;
    public const int QuantidadeMaximaTags = 10;
    public const int IntensidadeMinima = 1;
    public const int IntensidadeMaxima = 5;

    private RelatoComportamento()
    {
    }

    public RelatoComportamento(
        int petId,
        string categoria,
        string descricao,
        int intensidade,
        IEnumerable<string>? tags,
        DateTime? dataRelato,
        string? registradoPor)
    {
        Atualizar(petId, categoria, descricao, intensidade, tags, dataRelato);
        RegistradoPor = string.IsNullOrWhiteSpace(registradoPor) ? null : registradoPor.Trim();
        CriadoEm = DateTime.UtcNow;
    }

    public string? Id { get; private set; }

    public int PetId { get; private set; }

    public string Categoria { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;

    public int Intensidade { get; private set; }

    public IReadOnlyList<string> Tags { get; private set; } = Array.Empty<string>();

    public DateTime DataRelato { get; private set; }

    public string? RegistradoPor { get; private set; }

    public DateTime CriadoEm { get; private set; }

    public void Atualizar(
        int petId,
        string categoria,
        string descricao,
        int intensidade,
        IEnumerable<string>? tags,
        DateTime? dataRelato)
    {
        PetId = Guard.IdentificadorValido(petId, nameof(PetId));
        Categoria = CategoriasRelato.Normalizar(categoria);
        Descricao = Guard.TextoObrigatorio(descricao, nameof(Descricao), DescricaoTamanhoMaximo);
        Intensidade = Guard.Intervalo(intensidade, IntensidadeMinima, IntensidadeMaxima, nameof(Intensidade));
        Tags = NormalizarTags(tags);
        DataRelato = dataRelato ?? (DataRelato == default ? DateTime.UtcNow : DataRelato);
    }


    /// Recria a entidade a partir de dados já persistidos (usado pelo repositório MongoDB).
    public static RelatoComportamento Reconstituir(
        string id,
        int petId,
        string categoria,
        string descricao,
        int intensidade,
        IEnumerable<string>? tags,
        DateTime dataRelato,
        string? registradoPor,
        DateTime criadoEm)
    {
        var relato = new RelatoComportamento
        {
            Id = id,
            RegistradoPor = registradoPor,
            CriadoEm = criadoEm
        };
        relato.Atualizar(petId, categoria, descricao, intensidade, tags, dataRelato);
        return relato;
    }

    private static IReadOnlyList<string> NormalizarTags(IEnumerable<string>? tags)
    {
        if (tags is null)
        {
            return Array.Empty<string>();
        }

        var normalizadas = tags
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim().ToLowerInvariant())
            .Distinct()
            .ToList();

        if (normalizadas.Count > QuantidadeMaximaTags)
        {
            throw new DomainException($"Informe no máximo {QuantidadeMaximaTags} tags.");
        }

        if (normalizadas.Any(t => t.Length > TagTamanhoMaximo))
        {
            throw new DomainException($"Cada tag deve ter no máximo {TagTamanhoMaximo} caracteres.");
        }

        return normalizadas;
    }
}
