namespace KuraVet.Infrastructure.Mongo;

public sealed class MongoSettings
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = "mongodb://localhost:27017";

    public string DatabaseName { get; set; } = "kuravet";

    public int ServerSelectionTimeoutSeconds { get; set; } = 5;
}
