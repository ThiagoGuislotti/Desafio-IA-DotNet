using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CustomerPlatform.Api.Swagger
{
    /// <summary>
    /// Adiciona exemplos para parametros de operacoes no Swagger.
    /// </summary>
    public sealed class SwaggerExamplesOperationFilter : IOperationFilter
    {
        #region Public Methods/Operators
        /// <inheritdoc />
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation is null)
                throw new ArgumentNullException(nameof(operation));

            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (!IsSearchOperation(context))
                return;

            if (operation.Parameters is null || operation.Parameters.Count == 0)
                return;

            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Example is not null)
                    continue;

                parameter.Example = GetExample(parameter.Name);
            }
        }
        #endregion

        #region Private Methods/Operators
        private static bool IsSearchOperation(OperationFilterContext context)
        {
            var relativePath = context.ApiDescription.RelativePath;
            if (string.IsNullOrWhiteSpace(relativePath))
                return false;

            return relativePath.Contains("customers/search", StringComparison.OrdinalIgnoreCase);
        }

        private static IOpenApiAny? GetExample(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return name.ToLowerInvariant() switch
            {
                "name" => new OpenApiString("Maria Silva"),
                "document" => new OpenApiString("17871018434"),
                "email" => new OpenApiString("maria@teste.com"),
                "phone" => new OpenApiString("11999999999"),
                "customertype" => new OpenApiString("PF"),
                "pagenumber" => new OpenApiInteger(1),
                "pagesize" => new OpenApiInteger(20),
                _ => null
            };
        }
        #endregion
    }
}