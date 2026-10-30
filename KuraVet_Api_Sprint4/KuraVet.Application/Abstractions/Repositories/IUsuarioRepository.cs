using KuraVet.Domain.Entities;

namespace KuraVet.Application.Abstractions.Repositories;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> EmailExisteAsync(string email, CancellationToken cancellationToken);
}
