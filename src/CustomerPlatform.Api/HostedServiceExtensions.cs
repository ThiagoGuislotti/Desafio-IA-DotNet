namespace CustomerPlatform.Api
{
    /// <summary>
    /// Extensoes para registrar hosted services da API.
    /// </summary>
    public static class HostedServiceExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Adiciona hosted services da API.
        /// </summary>
        /// <param name="services">Collection de servicos.</param>
        /// <returns>Collection atualizada.</returns>
        public static IServiceCollection AddCustomHostedService(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

#if DEBUG
            services.AddHostedService<DevelopmentDockerComposeService>();
#endif
            services.AddHostedService<DevelopmentServiceInitializer>();

            return services;
        }
        #endregion
    }
}