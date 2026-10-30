namespace KuraVet.Application.Common.Exceptions;

/// Requisição inválida detectada pela camada de aplicação (ex.: referência inexistente, ordenação inválida).
/// A API converte em HTTP 400 
public sealed class ValidacaoException : Exception
{
    public ValidacaoException(string campo, string mensagem)
        : base(mensagem)
    {
        Erros = new Dictionary<string, string[]> { [campo] = new[] { mensagem } };
    }

    public IReadOnlyDictionary<string, string[]> Erros { get; }
}
