namespace KuraVet.Domain.Common;

/// Classe base das entidades persistidas no banco relacional (Oracle).

public abstract class Entidade
{
    public int Id { get; protected set; }
}
