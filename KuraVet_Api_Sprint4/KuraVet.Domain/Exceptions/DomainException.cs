namespace KuraVet.Domain.Exceptions;

/// Violação de uma regra/invariante de negócio. A API converte em HTTP 400 
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
