namespace KuraVet.Application.Common;

/// Base dos DTOs de resposta que expõem links HATEOAS (preenchidos pela camada Api).
public abstract class RecursoComLinks
{
    public List<LinkDto> Links { get; set; } = new();
}
