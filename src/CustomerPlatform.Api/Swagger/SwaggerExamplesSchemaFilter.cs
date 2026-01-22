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
                ["fullName"] = new OpenApiString("Maria da Silva"),
                ["cpf"] = new OpenApiString("17871018434"),
                ["email"] = new OpenApiString("maria@teste.com"),
                ["phone"] = new OpenApiString("11999999999"),
                ["birthDate"] = new OpenApiString("1990-01-10"),
                ["address"] = CreateAddressExample()
            };
        }

        private static IOpenApiAny CreateCompanyExample()
        {
            return new OpenApiObject
            {
                ["corporateName"] = new OpenApiString("Empresa Exemplo LTDA"),
                ["tradeName"] = new OpenApiString("Empresa Exemplo"),
                ["cnpj"] = new OpenApiString("29267190000190"),
                ["email"] = new OpenApiString("contato@empresa.com"),
                ["phone"] = new OpenApiString("1133334444"),
                ["address"] = CreateAddressExample()
            };
        }

        private static IOpenApiAny CreateUpdateExample()
        {
            return new OpenApiObject
            {
                ["customerType"] = new OpenApiString("PF"),
                ["fullName"] = new OpenApiString("Maria da Silva Atualizada"),
                ["email"] = new OpenApiString("maria@teste.com"),
                ["phone"] = new OpenApiString("11988887777"),
                ["birthDate"] = new OpenApiString("1990-01-10"),
                ["address"] = CreateAddressExample()
            };
        }

        private static IOpenApiAny CreateCustomerDtoExample()
        {
            return new OpenApiObject
            {
                ["id"] = new OpenApiString("9f2d39a0f0d344c7883b9f6f2eddd061"),
                ["customerType"] = new OpenApiString("PF"),
                ["document"] = new OpenApiString("17871018434"),
                ["name"] = new OpenApiString("Maria da Silva"),
                ["tradeName"] = new OpenApiString(""),
                ["email"] = new OpenApiString("maria@teste.com"),
                ["phone"] = new OpenApiString("11999999999"),
                ["address"] = CreateAddressExample(),
                ["createdAt"] = new OpenApiString("2024-01-10T10:00:00Z"),
                ["updatedAt"] = new OpenApiString("2024-01-10T10:00:00Z")
            };
        }

        private static IOpenApiAny CreateAddressExample()
        {
            return new OpenApiObject
            {
                ["street"] = new OpenApiString("Rua Central"),
                ["number"] = new OpenApiString("100"),
                ["complement"] = new OpenApiString("Apto 12"),
                ["postalCode"] = new OpenApiString("12345000"),
                ["city"] = new OpenApiString("Sao Paulo"),
                ["state"] = new OpenApiString("SP")
            };
        }
        #endregion
    }
}