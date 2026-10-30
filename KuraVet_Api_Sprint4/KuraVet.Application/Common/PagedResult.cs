namespace KuraVet.Application.Common;

/// Resposta paginada da API com metadados e links de navegação 
public sealed class PagedResult<T>
{
    public List<T> Items { get; set; } = new();

    public int Page { get; set; }

    public int PageSize { get; set; }

    public int TotalItems { get; set; }

    public int TotalPages { get; set; }

    public List<LinkDto> Links { get; set; } = new();

    public static PagedResult<T> Criar(IEnumerable<T> items, int page, int pageSize, int totalItems)
    {
        return new PagedResult<T>
        {
            Items = items.ToList(),
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)pageSize)
        };
    }
}
