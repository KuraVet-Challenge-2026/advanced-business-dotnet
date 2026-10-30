using System.Collections.Concurrent;
using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.RelatosComportamento;
using KuraVet.Domain.Entities;
using MongoDB.Bson;

namespace KuraVet.Api.Tests.Integration.Fixtures;

/// Substitui o repositório MongoDB nos testes de integração
public sealed class InMemoryRelatoComportamentoRepository : IRelatoComportamentoRepository
{
    private readonly ConcurrentDictionary<string, RelatoComportamento> _relatos = new();

    public Task<PagedList<RelatoComportamento>> ListarAsync(RelatoComportamentoFiltro filtro, CancellationToken cancellationToken)
    {
        var consulta = _relatos.Values.AsEnumerable();

        if (filtro.PetId.HasValue)
        {
            consulta = consulta.Where(r => r.PetId == filtro.PetId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
        {
            consulta = consulta.Where(r => r.Categoria == filtro.Categoria);
        }

        if (filtro.IntensidadeMinima.HasValue)
        {
            consulta = consulta.Where(r => r.Intensidade >= filtro.IntensidadeMinima.Value);
        }

        var lista = consulta.ToList();
        var ordenada = filtro.EhDescendente()
            ? lista.OrderByDescending(r => r.DataRelato)
            : lista.OrderBy(r => r.DataRelato);

        var pagina = ordenada.Skip(filtro.CalcularSkip()).Take(filtro.PageSize).ToList();
        return Task.FromResult(new PagedList<RelatoComportamento>(pagina, lista.Count));
    }

    public Task<RelatoComportamento?> ObterPorIdAsync(string id, CancellationToken cancellationToken)
    {
        _relatos.TryGetValue(id, out var relato);
        return Task.FromResult(relato);
    }

    public Task<RelatoComportamento> AdicionarAsync(RelatoComportamento relato, CancellationToken cancellationToken)
    {
        var id = ObjectId.GenerateNewId().ToString();
        var salvo = Copiar(id, relato);
        _relatos[id] = salvo;
        return Task.FromResult(salvo);
    }

    public Task<bool> AtualizarAsync(RelatoComportamento relato, CancellationToken cancellationToken)
    {
        if (relato.Id is null || !_relatos.ContainsKey(relato.Id))
        {
            return Task.FromResult(false);
        }

        _relatos[relato.Id] = Copiar(relato.Id, relato);
        return Task.FromResult(true);
    }

    public Task<bool> RemoverAsync(string id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_relatos.TryRemove(id, out _));
    }

    private static RelatoComportamento Copiar(string id, RelatoComportamento relato) => RelatoComportamento.Reconstituir(
        id,
        relato.PetId,
        relato.Categoria,
        relato.Descricao,
        relato.Intensidade,
        relato.Tags,
        relato.DataRelato,
        relato.RegistradoPor,
        relato.CriadoEm);
}
