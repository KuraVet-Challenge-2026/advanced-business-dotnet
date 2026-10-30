using KuraVet.Domain.Common;

namespace KuraVet.Application.Abstractions.Repositories;

public interface IRepository<T> where T : Entidade
{
    Task<T?> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken);

    Task AdicionarAsync(T entidade, CancellationToken cancellationToken);

    Task AtualizarAsync(T entidade, CancellationToken cancellationToken);

    Task RemoverAsync(T entidade, CancellationToken cancellationToken);
}
