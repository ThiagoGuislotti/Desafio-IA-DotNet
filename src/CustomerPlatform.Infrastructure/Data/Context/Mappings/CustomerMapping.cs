using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Domain.ValueObjects;
using CustomerPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerPlatform.Infrastructure.Data.Context.Mappings
{
    /// <summary>
    /// Mapeamento da entidade base de clientes.
    /// </summary>
    public sealed class CustomerMapping : IEntityTypeConfiguration<Customer>
    {
        #region Public Methods/Operators
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            builder.ToTable("Customers");

            builder.HasKey(customer => customer.Id);
            builder.Property(customer => customer.Id)
                .HasColumnName("customerId")
                .ValueGeneratedNever();

            builder.Property(customer => customer.DataCriacao)
                .HasColumnName("createdAt")
                .IsRequired();

            builder.Property(customer => customer.DataAtualizacao)
                .HasColumnName("updatedAt");

            builder.Property(customer => customer.Email)
                .HasColumnName("email")
                .HasMaxLength(254)
                .HasConversion(ValueObjectConverters.EmailConverter)
                .IsRequired();

            builder.Property(customer => customer.Telefone)
                .HasColumnName("phone")
                .HasMaxLength(15)
                .HasConversion(ValueObjectConverters.TelefoneConverter)
                .IsRequired();

            builder.OwnsOne(customer => customer.Endereco, endereco =>
            {
                endereco.Property(value => value.Logradouro)
                    .HasColumnName("addressStreet")
                    .HasMaxLength(200)
                    .IsRequired();

                endereco.Property(value => value.Numero)
                    .HasColumnName("addressNumber")
                    .HasMaxLength(200)
                    .IsRequired();

                endereco.Property(value => value.Complemento)
                    .HasColumnName("addressComplement")
                    .HasMaxLength(200);

                endereco.Property(value => value.Cep)
                    .HasColumnName("addressPostalCode")
                    .HasMaxLength(10)
                    .IsRequired();

                endereco.Property(value => value.Cidade)
                    .HasColumnName("addressCity")
                    .HasMaxLength(100)
                    .IsRequired();

                endereco.Property(value => value.Estado)
                    .HasColumnName("addressState")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.Navigation(customer => customer.Endereco).IsRequired();

            builder.Property<string>("customerType")
                .HasMaxLength(2)
                .IsRequired();

            builder.HasDiscriminator<string>("customerType")
                .HasValue<ClientePessoaFisica>("PF")
                .HasValue<ClientePessoaJuridica>("PJ");
        }
        #endregion
    }
}
