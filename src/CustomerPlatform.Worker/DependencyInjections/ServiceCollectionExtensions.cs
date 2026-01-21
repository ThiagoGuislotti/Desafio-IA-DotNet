using CustomerPlatform.Application.Abstractions.Messaging;
using CustomerPlatform.Application.Abstractions.Search;
using CustomerPlatform.Infrastructure.Messaging;
using CustomerPlatform.Infrastructure.Search;
using CustomerPlatform.Worker.HostedServices;
using CustomerPlatform.Worker.Resilience;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerPlatform.Worker.DependencyInjections
{
    /// <summary>
    /// Extensoes de DI para o Worker.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Registra servicos do Worker.
        /// </summary>
        /// <param name="services">Collection de servicos.</param>
        /// <param name="configuration">Configuracao da aplicacao.</param>
        /// <returns>Collection atualizada.</returns>
        public static IServiceCollection AddCustomerPlatformWorker(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            if (configuration is null)
                throw new ArgumentNullException(nameof(configuration));

            services.AddHostedService<OutboxPublisherHostedService>();
            services.AddHostedService<CustomerEventsConsumerHostedService>();

            services.AddScoped<RabbitMqEventPublisher>();
            services.AddScoped<IEventPublisher>(sp =>
                new ResilientEventPublisher(sp.GetRequiredService<RabbitMqEventPublisher>()));
            services.AddScoped<ElasticCustomerSearchService>();
            services.AddScoped<ICustomerSearchService>(sp =>
                new ResilientCustomerSearchService(sp.GetRequiredService<ElasticCustomerSearchService>()));

            return services;
        }
        #endregion
    }
}
