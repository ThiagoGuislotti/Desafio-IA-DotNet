using CustomerPlatform.Api.Middlewares;

namespace CustomerPlatform.Api.Extensions
{
    /// <summary>
    /// Extensoes para HttpContext.
    /// </summary>
    internal static class HttpContextExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Retorna o CorrelationId atual.
        /// </summary>
        /// <param name="context">Contexto HTTP.</param>
        /// <returns>CorrelationId atual.</returns>
        public static string? GetCorrelationId(this HttpContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (context.Items.TryGetValue(CorrelationIdMiddleware.CorrelationIdHeaderName, out var value))
                return value?.ToString();

            if (context.Request.Headers.TryGetValue(CorrelationIdMiddleware.CorrelationIdHeaderName, out var headerValue))
                return headerValue.ToString();

            return null;
        }
        #endregion
    }
}