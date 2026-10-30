using System.ComponentModel.DataAnnotations;
using KuraVet.Application.Common;

namespace KuraVet.Application.Pets;

/// Dados para criar ou atualizar um pet.
public sealed class PetRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "TutorId deve ser maior que zero.")]
    public int TutorId { get; set; }
}

public sealed class PetResponse : RecursoComLinks
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public int TutorId { get; set; }
}

/// Filtros de listagem de pets. 
public sealed class PetFiltro : PaginacaoQuery
{
    private static readonly string[] Campos = { "id", "nome", "tutorId" };

    /// Filtra pets cujo nome contém o texto.
    public string? Nome { get; set; }

    /// Filtra pets de um tutor.
    public int? TutorId { get; set; }

    protected override IReadOnlyCollection<string> CamposOrdenacao => Campos;
}
