namespace KuraVet.Application.Common.Exceptions;

/// Recurso inexistente. A API converte em HTTP 404
public sealed class NotFoundException : Exception
{
    public NotFoundException(string recurso, object chave)
        : base($"{recurso} '{chave}' não encontrado(a).")
    {
        Recurso = recurso;
        Chave = chave;
    }

    public string Recurso { get; }

    public object Chave { get; }
}
