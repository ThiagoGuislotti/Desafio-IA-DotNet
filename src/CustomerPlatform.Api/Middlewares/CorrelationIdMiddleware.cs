namespace CustomerPlatform.Api.Middlewares
{
    /// <summary>
    /// Middleware para gerenciar o CorrelationId.
    /// </summary>
    public sealed class CorrelationIdMiddleware
    {
        #region Constants
        /// <summary>
        /// Nome do header de correlation id.
        /// </summary>
        public const string CorrelationIdHeaderName = "X-Correlation-Id";
        #endregion

        #region Variables
        private readonly RequestDelegate _next;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="next">Proximo middleware.</param>
        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
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

            var correlationId = EnsureCorrelationId(context);
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[CorrelationIdHeaderName] = correlationId;
                return Task.CompletedTask;
            });

            await _next(context).ConfigureAwait(false);
        }
        #endregion

        #region Private Methods/Operators
        private static string EnsureCorrelationId(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var headerValue) &&
                !string.IsNullOrWhiteSpace(headerValue))
            {
                var existing = headerValue.ToString();
                context.Items[CorrelationIdHeaderName] = existing;
                return existing;
            }

            var generated = Guid.NewGuid().ToString("N");
            context.Items[CorrelationIdHeaderName] = generated;
            return generated;
        }
        #endregion
    }
}