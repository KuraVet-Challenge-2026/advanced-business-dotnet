using KuraVet.Application.Common;
using KuraVet.Application.RelatosComportamento;
using KuraVet.Domain.Entities;

namespace KuraVet.Application.Abstractions.Repositories;

/// Repositório NoSQL dos relatos de comportamento.
public interface IRelatoComportamentoRepository
{
    Task<PagedList<RelatoComportamento>> ListarAsync(RelatoComportamentoFiltro filtro, CancellationToken cancellationToken);

    Task<RelatoComportamento?> ObterPorIdAsync(string id, CancellationToken cancellationToken);

    /// Insere o relato e devolve a entidade com o Id gerado pelo banco.
    Task<RelatoComportamento> AdicionarAsync(RelatoComportamento relato, CancellationToken cancellationToken);

    /// Retorna false quando o documento não existe.
    Task<bool> AtualizarAsync(RelatoComportamento relato, CancellationToken cancellationToken);

    /// Retorna false quando o documento não existe. 
    Task<bool> RemoverAsync(string id, CancellationToken cancellationToken);
}
