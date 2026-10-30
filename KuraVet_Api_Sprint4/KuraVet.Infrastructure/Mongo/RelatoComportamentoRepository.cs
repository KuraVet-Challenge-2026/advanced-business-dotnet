using KuraVet.Application.Abstractions.Repositories;
using KuraVet.Application.Common;
using KuraVet.Application.RelatosComportamento;
using KuraVet.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace KuraVet.Infrastructure.Mongo;

public sealed class RelatoComportamentoRepository : IRelatoComportamentoRepository
{
    public const string CollectionName = "relatos_comportamento";

    private readonly IMongoCollection<RelatoComportamentoDocument> _collection;

    public RelatoComportamentoRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<RelatoComportamentoDocument>(CollectionName);
    }

    public async Task<PagedList<RelatoComportamento>> ListarAsync(
        RelatoComportamentoFiltro filtro, CancellationToken cancellationToken)
    {
        var builder = Builders<RelatoComportamentoDocument>.Filter;
        var filter = builder.Empty;

        if (filtro.PetId.HasValue)
        {
            filter &= builder.Eq(d => d.PetId, filtro.PetId.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
        {
            filter &= builder.Eq(d => d.Categoria, filtro.Categoria);
        }

        if (filtro.IntensidadeMinima.HasValue)
        {
            filter &= builder.Gte(d => d.Intensidade, filtro.IntensidadeMinima.Value);
        }

        if (filtro.DataInicio.HasValue)
        {
            filter &= builder.Gte(d => d.DataRelato, filtro.DataInicio.Value);
        }

        if (filtro.DataFim.HasValue)
        {
            filter &= builder.Lte(d => d.DataRelato, filtro.DataFim.Value);
        }

        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var documentos = await _collection
            .Find(filter)
            .Sort(CriarOrdenacao(filtro))
            .Skip(filtro.CalcularSkip())
            .Limit(filtro.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<RelatoComportamento>(
            documentos.Select(d => d.ParaEntidade()).ToList(),
            (int)total);
    }

    public async Task<RelatoComportamento?> ObterPorIdAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        var documento = await _collection
            .Find(d => d.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return documento?.ParaEntidade();
    }

    public async Task<RelatoComportamento> AdicionarAsync(RelatoComportamento relato, CancellationToken cancellationToken)
    {
        var documento = RelatoComportamentoDocument.DeEntidade(relato);
        documento.Id = null; // o driver gera o ObjectId na inserção
        await _collection.InsertOneAsync(documento, cancellationToken: cancellationToken);
        return documento.ParaEntidade();
    }

    public async Task<bool> AtualizarAsync(RelatoComportamento relato, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(relato.Id) || !ObjectId.TryParse(relato.Id, out _))
        {
            return false;
        }

        var id = relato.Id;
        var documento = RelatoComportamentoDocument.DeEntidade(relato);
        var resultado = await _collection.ReplaceOneAsync(
            d => d.Id == id, documento, cancellationToken: cancellationToken);

        return resultado.MatchedCount > 0;
    }

    public async Task<bool> RemoverAsync(string id, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return false;
        }

        var resultado = await _collection.DeleteOneAsync(d => d.Id == id, cancellationToken);
        return resultado.DeletedCount > 0;
    }

    private static SortDefinition<RelatoComportamentoDocument> CriarOrdenacao(RelatoComportamentoFiltro filtro)
    {
        var sort = Builders<RelatoComportamentoDocument>.Sort;
        var desc = filtro.EhDescendente();

        return filtro.CampoOrdenacaoNormalizado() switch
        {
            "intensidade" => desc ? sort.Descending(d => d.Intensidade) : sort.Ascending(d => d.Intensidade),
            "categoria" => desc ? sort.Descending(d => d.Categoria) : sort.Ascending(d => d.Categoria),
            "criadoem" => desc ? sort.Descending(d => d.CriadoEm) : sort.Ascending(d => d.CriadoEm),
            _ => desc ? sort.Descending(d => d.DataRelato) : sort.Ascending(d => d.DataRelato)
        };
    }
}
