using System.Net;
using System.Text.Json;

namespace tcc.Middleware;

/// <summary>
/// Captura qualquer exceção não tratada no pipeline HTTP, registra no log
/// (Serilog) e devolve uma resposta 500 padronizada, sem vazar stack trace
/// para o cliente. É o "log de erros" central da API.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro não tratado em {Method} {Path} (traceId {TraceId})",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier);

            if (context.Response.HasStarted)
            {
                // Resposta já começou a ser enviada — não dá para reescrever.
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problem = new
            {
                type = "about:blank",
                title = "Erro interno no servidor",
                status = 500,
                traceId = context.TraceIdentifier,
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
