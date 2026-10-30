using KuraVet.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Infrastructure.Persistence.Repositories;

internal static class QueryablePaginacaoExtensions
{
    public static async Task<PagedList<T>> PaginarAsync<T>(
        this IQueryable<T> consultaFiltrada,
        Func<IQueryable<T>, IQueryable<T>> ordenar,
        PaginacaoQuery paginacao,
        CancellationToken cancellationToken)
    {
        var total = await consultaFiltrada.CountAsync(cancellationToken);
        var itens = await ordenar(consultaFiltrada)
            .Skip(paginacao.CalcularSkip())
            .Take(paginacao.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<T>(itens, total);
    }
}
