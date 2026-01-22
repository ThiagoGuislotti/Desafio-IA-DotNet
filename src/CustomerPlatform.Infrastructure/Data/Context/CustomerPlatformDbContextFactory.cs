using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CustomerPlatform.Infrastructure.Data.Context
{
    /// <summary>
    /// Fabrica de DbContext para tempo de design.
    /// </summary>
    public sealed class CustomerPlatformDbContextFactory : IDesignTimeDbContextFactory<CustomerPlatformDbContext>
    {
        #region Public Methods/Operators
        /// <summary>
        /// Cria uma instancia de <see cref="CustomerPlatformDbContext"/> para tempo de design.
        /// </summary>
        /// <param name="args">Argumentos de execucao.</param>
        /// <returns>Instancia configurada do contexto.</returns>
        public CustomerPlatformDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CustomerPlatformDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=CustomerPlatformDbDevelopment;Username=postgres;Password=NetToolsKit.Pass!;Timeout=5;SSL Mode=Disable;Trust Server Certificate=true;");

            return new CustomerPlatformDbContext(optionsBuilder.Options);
        }
        #endregion
    }
}