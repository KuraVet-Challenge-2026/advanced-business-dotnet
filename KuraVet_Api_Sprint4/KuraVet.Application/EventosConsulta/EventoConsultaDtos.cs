using System.ComponentModel.DataAnnotations;
using KuraVet.Application.Common;

namespace KuraVet.Application.EventosConsulta;

/// Evento da linha do tempo clínica (vacina, exame, consulta, retorno, cirurgia...).
public sealed class EventoConsultaRequest
{
    [Required(ErrorMessage = "O tipo do evento é obrigatório.")]
    [MaxLength(50)]
    public string TipoEvento { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data do evento é obrigatória.")]
    public DateTime? DataEvento { get; set; }

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [MaxLength(500)]
    public string Descricao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O veterinário responsável é obrigatório.")]
    [MaxLength(100)]
    public string VeterinarioResponsavel { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "PetId deve ser maior que zero.")]
    public int PetId { get; set; }
}

public sealed class EventoConsultaResponse : RecursoComLinks
{
    public int Id { get; set; }

    public string TipoEvento { get; set; } = string.Empty;

    public DateTime DataEvento { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public string VeterinarioResponsavel { get; set; } = string.Empty;

    public int PetId { get; set; }
}

/// Filtros da linha do tempo clínica.
public sealed class EventoConsultaFiltro : PaginacaoQuery
{
    private static readonly string[] Campos = { "id", "dataEvento", "tipoEvento" };

    public EventoConsultaFiltro()
    {
        SortBy = "dataEvento";
        SortDir = "desc";
    }

    /// Filtra pelo pet.
    public int? PetId { get; set; }

    /// Filtra pelo tipo do evento (igualdade, sem diferenciar maiúsculas).
    public string? TipoEvento { get; set; }

    /// Filtra pelo nome do veterinário (contém).
    public string? Veterinario { get; set; }

    /// Eventos a partir desta data (inclusive).
    public DateTime? DataInicio { get; set; }

    /// Eventos até esta data (inclusive).
    public DateTime? DataFim { get; set; }

    protected override IReadOnlyCollection<string> CamposOrdenacao => Campos;
}
