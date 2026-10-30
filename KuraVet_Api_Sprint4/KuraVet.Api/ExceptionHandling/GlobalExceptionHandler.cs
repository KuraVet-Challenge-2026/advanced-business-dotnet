using KuraVet.Application.Common.Exceptions;
using KuraVet.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace KuraVet.Api.ExceptionHandling;

/// Tratamento global de exceções: converte qualquer erro com status adequado.
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, titulo) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            ValidacaoException => (StatusCodes.Status400BadRequest, "Requisição inválida"),
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            ConflitoException => (StatusCodes.Status409Conflict, "Conflito"),
            CredenciaisInvalidasException => (StatusCodes.Status401Unauthorized, "Não autorizado"),
            TimeoutException => (StatusCodes.Status503ServiceUnavailable, "Serviço temporariamente indisponível"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno no servidor")
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro não tratado em {Method} {Path}.", httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning("{Titulo}: {Mensagem}", titulo, exception.Message);
        }

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = DetalheSeguro(exception, status),
            Instance = httpContext.Request.Path,
            Type = $"https://httpstatuses.io/{status}"
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (exception is ValidacaoException validacao)
        {
            problemDetails.Extensions["errors"] = validacao.Erros;
        }

        httpContext.Response.StatusCode = status;

        var escrito = await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        if (!escrito)
        {
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        }

        return true;
    }

    private static string DetalheSeguro(Exception exception, int status)
    {
        return status switch
        {
            StatusCodes.Status503ServiceUnavailable =>
                "Um serviço externo (ex.: banco de dados) não respondeu a tempo. Tente novamente em instantes.",
            >= StatusCodes.Status500InternalServerError =>
                "Ocorreu um erro inesperado. Informe o traceId ao suporte.",
            _ => exception.Message
        };
    }
}
