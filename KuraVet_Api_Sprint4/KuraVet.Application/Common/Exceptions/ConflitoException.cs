namespace KuraVet.Application.Common.Exceptions;

/// Conflito com o estado atual (ex.: e-mail já cadastrado). A API converte em HTTP 409.
public sealed class ConflitoException : Exception
{
    public ConflitoException(string message) : base(message)
    {
    }

    public ConflitoException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
