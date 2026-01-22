using CustomerPlatform.Api.Models;
using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CustomerPlatform.Api.Swagger
{
    /// <summary>
    /// Adiciona exemplos de payload aos schemas do Swagger.
    /// </summary>
    public sealed class SwaggerExamplesSchemaFilter : ISchemaFilter
    {
        #region Public Methods/Operators
        /// <inheritdoc />
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema is null)
                throw new ArgumentNullException(nameof(schema));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (context.Type == typeof(CreateIndividualCustomerCommand))
                schema.Example = CreateIndividualExample();

            if (context.Type == typeof(CreateCompanyCustomerCommand))
                schema.Example = CreateCompanyExample();

            if (context.Type == typeof(UpdateCustomerRequest))
                schema.Example = CreateUpdateExample();

            if (context.Type == typeof(CustomerDto))
                schema.Example = CreateCustomerDtoExample();
        }
        #endregion

        #region Private Methods/Operators
        private static IOpenApiAny CreateIndividualExample()
        {
            return new OpenApiObject
            {
                ["nome"] = new OpenApiString("Maria da Silva"),
                ["cpf"] = new OpenApiString("17871018434"),
                ["email"] = new OpenApiString("maria@teste.com"),
                ["telefone"] = new OpenApiString("11999999999"),
                ["dataNascimento"] = new OpenApiString("1990-01-10"),
                ["endereco"] = CreateAddressExample()
            };
        }

        private static IOpenApiAny CreateCompanyExample()
        {
            return new OpenApiObject
            {
                ["razaoSocial"] = new OpenApiString("Empresa Exemplo LTDA"),
                ["nomeFantasia"] = new OpenApiString("Empresa Exemplo"),
                ["cnpj"] = new OpenApiString("29267190000190"),
                ["email"] = new OpenApiString("contato@empresa.com"),
                ["telefone"] = new OpenApiString("1133334444"),
                ["endereco"] = CreateAddressExample()
            };
        }

        private static IOpenApiAny CreateUpdateExample()
        {
            return new OpenApiObject
            {
                ["tipo"] = new OpenApiString("PF"),
                ["nome"] = new OpenApiString("Maria da Silva Atualizada"),
                ["email"] = new OpenApiString("maria@teste.com"),
                ["telefone"] = new OpenApiString("11988887777"),
                ["dataNascimento"] = new OpenApiString("1990-01-10"),
                ["endereco"] = CreateAddressExample()
            };
        }

        private static IOpenApiAny CreateCustomerDtoExample()
        {
            return new OpenApiObject
            {
                ["id"] = new OpenApiString("9f2d39a0f0d344c7883b9f6f2eddd061"),
                ["tipo"] = new OpenApiString("PF"),
                ["documento"] = new OpenApiString("17871018434"),
                ["nome"] = new OpenApiString("Maria da Silva"),
                ["nomeFantasia"] = new OpenApiString(""),
                ["email"] = new OpenApiString("maria@teste.com"),
                ["telefone"] = new OpenApiString("11999999999"),
                ["endereco"] = CreateAddressExample(),
                ["criadoEm"] = new OpenApiString("2024-01-10T10:00:00Z"),
                ["atualizadoEm"] = new OpenApiString("2024-01-10T10:00:00Z")
            };
        }

        private static IOpenApiAny CreateAddressExample()
        {
            return new OpenApiObject
            {
                ["logradouro"] = new OpenApiString("Rua Central"),
                ["numero"] = new OpenApiString("100"),
                ["complemento"] = new OpenApiString("Apto 12"),
                ["cep"] = new OpenApiString("12345000"),
                ["cidade"] = new OpenApiString("Sao Paulo"),
                ["estado"] = new OpenApiString("SP")
            };
        }
        #endregion
    }
}