using KuraVet.Domain.Exceptions;

namespace KuraVet.Domain.Common;

/// Validações reutilizáveis das invariantes de domínio.
internal static class Guard
{
    public static string TextoObrigatorio(string? valor, string campo, int tamanhoMaximo)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            throw new DomainException($"O campo '{campo}' é obrigatório.");
        }

        var texto = valor.Trim();
        if (texto.Length > tamanhoMaximo)
        {
            throw new DomainException($"O campo '{campo}' deve ter no máximo {tamanhoMaximo} caracteres.");
        }

        return texto;
    }

    public static int IdentificadorValido(int valor, string campo)
    {
        if (valor <= 0)
        {
            throw new DomainException($"O campo '{campo}' deve ser maior que zero.");
        }

        return valor;
    }

    public static int Intervalo(int valor, int minimo, int maximo, string campo)
    {
        if (valor < minimo || valor > maximo)
        {
            throw new DomainException($"O campo '{campo}' deve estar entre {minimo} e {maximo}.");
        }

        return valor;
    }
}
