using System.ComponentModel.DataAnnotations;
using KuraVet.Application.Common;

namespace KuraVet.Application.RelatosComportamento;

/// Relato livre do tutor sobre hábitos e comportamento do pet (armazenado no MongoDB).
public sealed class RelatoComportamentoRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "PetId deve ser maior que zero.")]
    public int PetId { get; set; }

    /// Alimentacao, Sono, Atividade, Comportamento, Higiene ou Outro
    [Required(ErrorMessage = "A categoria é obrigatória.")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(2000)]
    public string Descricao { get; set; } = string.Empty;

    /// Intensidade percebida pelo tutor, de 1 (leve) a 5 (muito intensa).
    [Range(1, 5, ErrorMessage = "Intensidade deve estar entre 1 e 5.")]
    public int Intensidade { get; set; }

    public List<string>? Tags { get; set; }

    /// Quando o comportamento foi observado (UTC). Se omitido, usa o momento atual.
    public DateTime? DataRelato { get; set; }
}

public sealed class RelatoComportamentoResponse : RecursoComLinks
{
    public string Id { get; set; } = string.Empty;

    public int PetId { get; set; }

    public string Categoria { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public int Intensidade { get; set; }

    public List<string> Tags { get; set; } = new();

    public DateTime DataRelato { get; set; }

    public string? RegistradoPor { get; set; }

    public DateTime CriadoEm { get; set; }
}

/// Filtros dos relatos. sortBy aceito: dataRelato, intensidade, categoria, criadoEm. Padrão: dataRelato desc.
public sealed class RelatoComportamentoFiltro : PaginacaoQuery
{
    private static readonly string[] Campos = { "dataRelato", "intensidade", "categoria", "criadoEm" };

    public RelatoComportamentoFiltro()
    {
        SortBy = "dataRelato";
        SortDir = "desc";
    }

    /// Filtra pelo pet.
    public int? PetId { get; set; }

    /// Filtra pela categoria.
    public string? Categoria { get; set; }

    /// Intensidade mínima (1 a 5).
    public int? IntensidadeMinima { get; set; }

    /// Relatos a partir desta data (inclusive)
    public DateTime? DataInicio { get; set; }

    /// Relatos até esta data (inclusive).
    public DateTime? DataFim { get; set; }

    protected override IReadOnlyCollection<string> CamposOrdenacao => Campos;
}
