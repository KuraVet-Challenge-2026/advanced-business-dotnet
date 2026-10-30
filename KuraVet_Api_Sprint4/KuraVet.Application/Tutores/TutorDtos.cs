using System.ComponentModel.DataAnnotations;
using KuraVet.Application.Common;

namespace KuraVet.Application.Tutores;

/// Dados para criar ou atualizar um tutor.
public sealed class TutorRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;
}

public sealed class TutorResponse : RecursoComLinks
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
}

/// Filtros de listagem de tutores. sortBy aceito: id, nome.
public sealed class TutorFiltro : PaginacaoQuery
{
    private static readonly string[] Campos = { "id", "nome" };

    /// Filtra tutores cujo nome contém o texto (sem diferenciar maiúsculas)
    public string? Nome { get; set; }

    protected override IReadOnlyCollection<string> CamposOrdenacao => Campos;
}
