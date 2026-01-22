using CustomerPlatform.Api.Extensions;
using System.Diagnostics;

namespace CustomerPlatform.Api.Middlewares
{
    /// <summary>
    /// Middleware para logar requisicoes e respostas.
    /// </summary>
    public sealed class RequestLoggingMiddleware
    {
        #region Variables
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="next">Proximo middleware.</param>
        /// <param name="logger">Logger da requisicao.</param>
        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Executa o middleware.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            var correlationId = context.GetCorrelationId();
            var path = $"{context.Request.Path}{context.Request.QueryString}";
            var method = context.Request.Method;

            var stopwatch = Stopwatch.StartNew();
            await _next(context).ConfigureAwait(false);
            stopwatch.Stop();

            _logger.LogInformation(
                "HTTP {Method} {Path} respondeu {StatusCode} em {ElapsedMs}ms (CorrelationId: {CorrelationId})",
                method,
                path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);
        }
        #endregion
    }
}