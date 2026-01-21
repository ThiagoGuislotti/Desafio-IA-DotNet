using CustomerPlatform.Domain.Entities;
using CustomerPlatform.Domain.ValueObjects;

namespace CustomerPlatform.Application.DTOs
{
    /// <summary>
    /// Mapeia entidades de dominio para DTOs.
    /// </summary>
    public static class CustomerDtoMapper
    {
        #region Public Methods/Operators
        /// <summary>
        /// Converte um cliente para <see cref="CustomerDto"/>.
        /// </summary>
        /// <param name="customer">Entidade do cliente.</param>
        /// <returns>DTO do cliente.</returns>
        public static CustomerDto Map(Customer customer)
        {
            if (customer is null)
                throw new ArgumentNullException(nameof(customer));

            var tradeName = customer is ClientePessoaJuridica legalEntity ? legalEntity.NomeFantasia : null;
            var birthDate = customer is ClientePessoaFisica individual ? individual.DataNascimento : (DateOnly?)null;

            return new CustomerDto
            {
                Id = customer.Id,
                CustomerType = customer.TipoCliente,
                Document = customer.GetDocumento(),
                Name = customer.GetNome(),
                TradeName = tradeName,
                BirthDate = birthDate,
                Email = customer.Email.Endereco,
                Phone = customer.Telefone.Numero,
                Address = MapAddress(customer.Endereco),
                CreatedAt = customer.DataCriacao,
                UpdatedAt = customer.DataAtualizacao,
            };
        }
        #endregion

        #region Private Methods/Operators
        private static AddressDto MapAddress(Endereco endereco)
        {
            if (endereco is null)
                throw new ArgumentNullException(nameof(endereco));

            return new AddressDto
            {
                Street = endereco.Logradouro,
                Number = endereco.Numero,
                Complement = endereco.Complemento,
                PostalCode = endereco.Cep,
                City = endereco.Cidade,
                State = endereco.Estado,
            };
        }
        #endregion
    }
}
