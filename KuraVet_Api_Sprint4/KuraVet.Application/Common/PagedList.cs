namespace KuraVet.Application.Common;

/// Resultado bruto de uma consulta paginada no repositório (itens da página + total geral).
public sealed record PagedList<T>(IReadOnlyList<T> Items, int TotalItems);
