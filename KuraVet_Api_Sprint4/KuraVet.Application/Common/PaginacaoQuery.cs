using KuraVet.Application.Common.Exceptions;

namespace KuraVet.Application.Common;

/// Parâmetros de paginação e ordenação recebidos via query string:
public abstract class PaginacaoQuery
{
    public const int PageSizePadrao = 10;
    public const int PageSizeMaximo = 100;

    private int _page = 1;
    private int _pageSize = PageSizePadrao;

    /// Página desejada (começa em 1).
    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    /// Itens por página (1 a 100, padrão 10).
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 ? PageSizePadrao : Math.Min(value, PageSizeMaximo);
    }

    /// Campo de ordenação (veja os valores aceitos em cada endpoint).
    public string? SortBy { get; set; }

    /// Direção da ordenação: asc ou desc.
    public string SortDir { get; set; } = "asc";

    /// Campos aceitos em <see cref="SortBy"/> para o recurso.
    protected abstract IReadOnlyCollection<string> CamposOrdenacao { get; }

    public int CalcularSkip() => (Page - 1) * PageSize;

    public bool EhDescendente() => string.Equals(SortDir, "desc", StringComparison.OrdinalIgnoreCase);

    /// Campo de ordenação em minúsculas (ou null quando não informado).
    public string? CampoOrdenacaoNormalizado() => string.IsNullOrWhiteSpace(SortBy) ? null : SortBy.Trim().ToLowerInvariant();

    public void ValidarOrdenacao()
    {
        if (!string.IsNullOrWhiteSpace(SortBy) &&
            !CamposOrdenacao.Any(c => string.Equals(c, SortBy.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidacaoException(
                nameof(SortBy),
                $"sortBy '{SortBy}' inválido. Valores aceitos: {string.Join(", ", CamposOrdenacao)}.");
        }

        if (!string.Equals(SortDir, "asc", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(SortDir, "desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidacaoException(nameof(SortDir), $"sortDir '{SortDir}' inválido. Use 'asc' ou 'desc'.");
        }
    }
}
