using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Infrastructure.Persistence.Repositories;

public abstract class RepositoryBase<T> : IRepository<T> where T : Entidade
{
    protected RepositoryBase(KuraVetDbContext context)
    {
        Context = context;
    }

    protected KuraVetDbContext Context { get; }

    protected DbSet<T> Set => Context.Set<T>();

    public virtual Task<T?> ObterPorIdAsync(int id, CancellationToken cancellationToken)
    {
        return Set.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public Task<bool> ExisteAsync(int id, CancellationToken cancellationToken)
    {
        return Set.AnyAsync(e => e.Id == id, cancellationToken);
    }

    public async Task AdicionarAsync(T entidade, CancellationToken cancellationToken)
    {
        await Set.AddAsync(entidade, cancellationToken);
        await SalvarAsync(cancellationToken);
    }

    public async Task AtualizarAsync(T entidade, CancellationToken cancellationToken)
    {
        Set.Update(entidade);
        await SalvarAsync(cancellationToken);
    }

    public async Task RemoverAsync(T entidade, CancellationToken cancellationToken)
    {
        Set.Remove(entidade);
        await SalvarAsync(cancellationToken);
    }

    /// Converte violações de FK/índice único em ConflitoException (409).
    private async Task SalvarAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new ConflitoException(
                "A operação viola uma restrição do banco de dados (registro relacionado ou duplicado).", ex);
        }
    }
}
