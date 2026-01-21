using CustomerPlatform.Application.Abstractions.Validation;
using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.Cqrs.Queries;
using CustomerPlatform.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace CustomerPlatform.Application.DependencyInjections
{
    /// <summary>
    /// Extensoes de DI para a camada Application.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Registra servicos da camada Application.
        /// </summary>
        /// <param name="services">Collection de servicos.</param>
        /// <returns>Collection de servicos atualizada.</returns>
        public static IServiceCollection AddCustomerPlatformApplication(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

            services.AddTransient<IValidator<CreateIndividualCustomerCommand>, CreateIndividualCustomerValidator>();
            services.AddTransient<IValidator<CreateCompanyCustomerCommand>, CreateCompanyCustomerValidator>();
            services.AddTransient<IValidator<UpdateIndividualCustomerCommand>, UpdateIndividualCustomerValidator>();
            services.AddTransient<IValidator<UpdateCompanyCustomerCommand>, UpdateCompanyCustomerValidator>();
            services.AddTransient<IValidator<GetCustomerByIdQuery>, GetCustomerByIdQueryValidator>();
            services.AddTransient<IValidator<SearchCustomersQuery>, SearchCustomersQueryValidator>();

            return services;
        }
        #endregion
    }
}