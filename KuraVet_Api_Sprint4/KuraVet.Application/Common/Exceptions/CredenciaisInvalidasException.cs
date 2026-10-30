namespace KuraVet.Application.Common.Exceptions;

/// Falha de login. A API converte em HTTP 401 sem revelar se o e-mail existe.
public sealed class CredenciaisInvalidasException : Exception
{
    public CredenciaisInvalidasException() : base("E-mail ou senha inválidos.")
    {
    }
}
