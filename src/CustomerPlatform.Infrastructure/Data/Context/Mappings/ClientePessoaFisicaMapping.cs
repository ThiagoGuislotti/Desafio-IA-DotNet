using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerPlatform.Infrastructure.Data.Context.Mappings
{
    /// <summary>
    /// Mapeamento de cliente pessoa fisica.
    /// </summary>
    public sealed class ClientePessoaFisicaMapping : IEntityTypeConfiguration<ClientePessoaFisica>
    {
        #region Public Methods/Operators
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ClientePessoaFisica> builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.Property(customer => customer.Nome)
                .HasColumnName("fullName")
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(customer => customer.Documento)
                .HasColumnName("document")
                .HasMaxLength(14)
                .HasConversion(ValueObjectConverters.DocumentoConverter)
                .IsRequired();

            builder.Property(customer => customer.DataNascimento)
                .HasColumnName("birthDate")
                .IsRequired();

            builder.HasIndex(customer => customer.Documento)
                .IsUnique();
        }
        #endregion
    }
}
