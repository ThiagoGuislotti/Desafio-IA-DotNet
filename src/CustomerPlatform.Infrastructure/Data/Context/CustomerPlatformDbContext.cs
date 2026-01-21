using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Infrastructure.Data.Context.Entities;
using CustomerPlatform.Infrastructure.Data.Context.Mappings;
using Microsoft.EntityFrameworkCore;

namespace CustomerPlatform.Infrastructure.Data.Context
{
    /// <summary>
    /// DbContext da plataforma de clientes.
    /// </summary>
    public sealed class CustomerPlatformDbContext : DbContext
    {
        #region Public Properties
        /// <summary>
        /// Clientes persistidos.
        /// </summary>
        public DbSet<Customer> Customers { get; set; } = null!;

        /// <summary>
        /// Eventos pendentes para publicacao (outbox).
        /// </summary>
        public DbSet<OutboxEvent> OutboxEvents { get; set; } = null!;

        /// <summary>
        /// Suspeitas de duplicidade.
        /// </summary>
        public DbSet<DuplicateSuspicion> DuplicateSuspicions { get; set; } = null!;
        #endregion

        #region Constructors
        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="options">Opcoes do contexto.</param>
        public CustomerPlatformDbContext(DbContextOptions<CustomerPlatformDbContext> options)
            : base(options)
        {
        }
        #endregion

        #region Protected Methods/Operators
        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
                throw new ArgumentNullException(nameof(modelBuilder));

            modelBuilder.ApplyConfiguration(new CustomerMapping());
            modelBuilder.ApplyConfiguration(new ClientePessoaFisicaMapping());
            modelBuilder.ApplyConfiguration(new ClientePessoaJuridicaMapping());
            modelBuilder.ApplyConfiguration(new OutboxEventMapping());
            modelBuilder.ApplyConfiguration(new DuplicateSuspicionMapping());

            base.OnModelCreating(modelBuilder);
        }
        #endregion
    }
}
