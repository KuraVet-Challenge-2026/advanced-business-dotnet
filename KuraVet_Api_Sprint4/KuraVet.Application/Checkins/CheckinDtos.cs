using System.ComponentModel.DataAnnotations;
using KuraVet.Application.Common;

namespace KuraVet.Application.Checkins;

/// Sinais vitais informados pelo tutor no check-in
public sealed class CheckinRequest
{
    /// Data/hora do check-in. Se omitida, usa o momento atual.
    public DateTime? DataCheckin { get; set; }

    /// Movimentos respiratórios por minuto
    [Range(1, 300, ErrorMessage = "FrequenciaRespiratoria deve estar entre 1 e 300.")]
    public int FrequenciaRespiratoria { get; set; }

    /// Tempo de preenchimento capilar, em segundos.
    [Range(0, 30, ErrorMessage = "TempoPreenchimentoCapilar deve estar entre 0 e 30.")]
    public int TempoPreenchimentoCapilar { get; set; }

    [Required(ErrorMessage = "A cor da mucosa é obrigatória.")]
    [MaxLength(50)]
    public string CorMucosa { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nível de hidratação é obrigatório.")]
    [MaxLength(50)]
    public string NivelHidratacao { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "PetId deve ser maior que zero.")]
    public int PetId { get; set; }
}

public sealed class CheckinResponse : RecursoComLinks
{
    public int Id { get; set; }

    public DateTime DataCheckin { get; set; }

    public int FrequenciaRespiratoria { get; set; }

    public int TempoPreenchimentoCapilar { get; set; }

    public string CorMucosa { get; set; } = string.Empty;

    public string NivelHidratacao { get; set; } = string.Empty;

    public string? NivelRiscoIA { get; set; }

    public int PetId { get; set; }
}


/// Filtros de check-ins. sortBy aceito: id, dataCheckin, frequenciaRespiratoria, tempoPreenchimentoCapilar, nivelRiscoIA.
/// Padrão: dataCheckin desc.

public sealed class CheckinFiltro : PaginacaoQuery
{
    private static readonly string[] Campos =
    {
        "id", "dataCheckin", "frequenciaRespiratoria", "tempoPreenchimentoCapilar", "nivelRiscoIA"
    };

    public CheckinFiltro()
    {
        SortBy = "dataCheckin";
        SortDir = "desc";
    }

    /// Filtra pelo pet.
    public int? PetId { get; set; }

    /// Filtra pelo nível de risco (Baixo, Moderado, Grave).
    public string? NivelRisco { get; set; }

    /// Check-ins a partir desta data (inclusive).
    public DateTime? DataInicio { get; set; }

    /// Check-ins até esta data (inclusive).
    public DateTime? DataFim { get; set; }

    protected override IReadOnlyCollection<string> CamposOrdenacao => Campos;
}
