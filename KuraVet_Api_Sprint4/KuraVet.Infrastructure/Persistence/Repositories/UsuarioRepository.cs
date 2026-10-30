using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace KuraVet.Infrastructure.Persistence.Repositories;

public sealed class UsuarioRepository : RepositoryBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(KuraVetDbContext context) : base(context)
    {
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken)
    {
        var normalizado = email.Trim().ToLowerInvariant();
        return Set.AsNoTracking().FirstOrDefaultAsync(u => u.Email == normalizado, cancellationToken);
    }

    public Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken)
    {
        var normalizado = email.Trim().ToLowerInvariant();
        return Set.AnyAsync(u => u.Email == normalizado, cancellationToken);
    }
}
