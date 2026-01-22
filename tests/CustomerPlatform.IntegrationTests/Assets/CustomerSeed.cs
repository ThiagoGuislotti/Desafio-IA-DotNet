using CustomerPlatform.Application.DTOs;
using CustomerPlatform.Domain.Enums;

namespace CustomerPlatform.IntegrationTests.Assets
{
    /// <summary>
    /// Seed de dados para testes de integracao.
    /// </summary>
    public static class CustomerSeed
    {
        #region Public Methods/Operators
        /// <summary>
        /// Cria um cliente PF para testes.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <param name="name">Nome completo.</param>
        /// <param name="document">Documento.</param>
        /// <param name="email">Email.</param>
        /// <param name="phone">Telefone.</param>
        /// <returns>DTO do cliente.</returns>
        public static CustomerDto CreateIndividual(
            Guid id,
            string name,
            string document,
            string email,
            string phone)
        {
            return new CustomerDto
            {
                Id = id,
                CustomerType = TipoCliente.PF,
                Document = document,
                Name = name,
                Email = email,
                Phone = phone,
                Address = CreateAddress(),
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Cria um cliente PJ para testes.
        /// </summary>
        /// <param name="id">Identificador do cliente.</param>
        /// <param name="corporateName">Razao social.</param>
        /// <param name="tradeName">Nome fantasia.</param>
        /// <param name="document">Documento.</param>
        /// <param name="email">Email.</param>
        /// <param name="phone">Telefone.</param>
        /// <returns>DTO do cliente.</returns>
        public static CustomerDto CreateCompany(
            Guid id,
            string corporateName,
            string tradeName,
            string document,
            string email,
            string phone)
        {
            return new CustomerDto
            {
                Id = id,
                CustomerType = TipoCliente.PJ,
                Document = document,
                Name = corporateName,
                TradeName = tradeName,
                Email = email,
                Phone = phone,
                Address = CreateAddress(),
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Cria uma lista fixa de clientes PF para testes.
        /// </summary>
        /// <param name="count">Quantidade de clientes.</param>
        /// <param name="namePrefix">Prefixo do nome.</param>
        /// <returns>Lista de clientes.</returns>
        public static IReadOnlyList<CustomerDto> CreateIndividualsBatch(
            int count,
            string namePrefix = "Cliente Seed")
        {
            if (count <= 0)
                return Array.Empty<CustomerDto>();

            var customers = new List<CustomerDto>(count);
            for (var i = 1; i <= count; i++)
            {
                var id = CreateDeterministicGuid(i);
                var name = $"{namePrefix} {i:000}";
                var document = (10000000000 + i).ToString("D11");
                var email = $"seed{i:000}@teste.com";
                var phone = $"119{(80000000 + i):D8}";

                customers.Add(CreateIndividual(id, name, document, email, phone));
            }

            return customers;
        }
        #endregion

        #region Private Methods/Operators
        private static Guid CreateDeterministicGuid(int value)
        {
            var suffix = value.ToString("D12");
            return Guid.Parse($"00000000-0000-0000-0000-{suffix}");
        }

        private static AddressDto CreateAddress()
        {
            return new AddressDto
            {
                Street = "Rua Teste",
                Number = "100",
                Complement = "Apto 10",
                PostalCode = "12345000",
                City = "Sao Paulo",
                State = "SP"
            };
        }
        #endregion
    }
}