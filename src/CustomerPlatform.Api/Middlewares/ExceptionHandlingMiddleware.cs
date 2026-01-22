using CustomerPlatform.Api.Extensions;
using CustomerPlatform.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CustomerPlatform.Api.Middlewares
{
    /// <summary>
    /// Middleware para tratamento centralizado de excecoes.
    /// </summary>
    public sealed class ExceptionHandlingMiddleware
    {
        #region Static Variables
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        #endregion

        #region Variables
        private readonly RequestDelegate _next;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="next">Proximo middleware.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Executa o middleware.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        /// <param name="logger">Logger da aplicacao.</param>
        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlingMiddleware> logger)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (logger is null)
                throw new ArgumentNullException(nameof(logger));

            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (DomainException ex)
            {
                logger.LogWarning(ex, "Erro de dominio (CorrelationId: {CorrelationId})", context.GetCorrelationId());
                await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Erro de dominio.", ex.Message)
                    .ConfigureAwait(false);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Erro de argumento (CorrelationId: {CorrelationId})", context.GetCorrelationId());
                await WriteProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Erro de validacao.", ex.Message)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro nao tratado (CorrelationId: {CorrelationId})", context.GetCorrelationId());
                await WriteProblemDetailsAsync(
                        context,
                        StatusCodes.Status500InternalServerError,
                        "Erro inesperado.",
                        "Erro interno do servidor.")
                    .ConfigureAwait(false);
            }
        }
        #endregion

        #region Private Methods/Operators
        private static async Task WriteProblemDetailsAsync(
            HttpContext context,
            int statusCode,
            string title,
            string detail)
        {
            if (context.Response.HasStarted)
                return;

            var correlationId = context.GetCorrelationId();
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path,
            };

            if (!string.IsNullOrWhiteSpace(correlationId))
                problemDetails.Extensions["correlationId"] = correlationId;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var payload = JsonSerializer.Serialize(problemDetails, SerializerOptions);
            await context.Response.WriteAsync(payload).ConfigureAwait(false);
        }
        #endregion
    }
}