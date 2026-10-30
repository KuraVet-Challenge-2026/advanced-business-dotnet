using KuraVet.Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KuraVet.Infrastructure.Mongo;

public sealed class RelatoComportamentoDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("petId")]
    public int PetId { get; set; }

    [BsonElement("categoria")]
    public string Categoria { get; set; } = string.Empty;

    [BsonElement("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [BsonElement("intensidade")]
    public int Intensidade { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();

    [BsonElement("dataRelato")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime DataRelato { get; set; }

    [BsonElement("registradoPor")]
    [BsonIgnoreIfNull]
    public string? RegistradoPor { get; set; }

    [BsonElement("criadoEm")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CriadoEm { get; set; }

    public static RelatoComportamentoDocument DeEntidade(RelatoComportamento relato) => new()
    {
        Id = relato.Id,
        PetId = relato.PetId,
        Categoria = relato.Categoria,
        Descricao = relato.Descricao,
        Intensidade = relato.Intensidade,
        Tags = relato.Tags.ToList(),
        DataRelato = relato.DataRelato,
        RegistradoPor = relato.RegistradoPor,
        CriadoEm = relato.CriadoEm
    };

    public RelatoComportamento ParaEntidade() => RelatoComportamento.Reconstituir(
        Id ?? string.Empty,
        PetId,
        Categoria,
        Descricao,
        Intensidade,
        Tags,
        DataRelato,
        RegistradoPor,
        CriadoEm);
}
