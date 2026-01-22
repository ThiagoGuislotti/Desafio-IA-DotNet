using CustomerPlatform.Application.Abstractions.Results;
using CustomerPlatform.Application.Abstractions.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CustomerPlatform.Api.Extensions
{
    /// <summary>
    /// Extensoes para conversao de Result em ActionResult.
    /// </summary>
    internal static class ResultExtensions
    {
        #region Public Methods/Operators
        /// <summary>
        /// Converte Result em ActionResult com ProblemDetails.
        /// </summary>
        /// <typeparam name="T">Tipo do retorno.</typeparam>
        /// <param name="controller">Controller base.</param>
        /// <param name="result">Resultado da operacao.</param>
        /// <param name="successStatusCode">Status code de sucesso.</param>
        /// <returns>ActionResult padronizado.</returns>
        public static IActionResult ToActionResult<T>(
            this ControllerBase controller,
            Result<T> result,
            int successStatusCode = StatusCodes.Status200OK)
        {
            if (controller is null)
                throw new ArgumentNullException(nameof(controller));

            if (result is null)
                throw new ArgumentNullException(nameof(result));

            if (result.IsSuccess)
            {
                if (successStatusCode == StatusCodes.Status201Created)
                    return new ObjectResult(result.Data) { StatusCode = successStatusCode };

                return controller.Ok(result.Data);
            }

            if (IsNotFound(result))
                return CreateProblemResult(controller, StatusCodes.Status404NotFound, "Recurso nao encontrado.", result.Message);

            if (result.Errors.Count > 0)
                return CreateValidationProblemResult(controller, result.Errors, result.Message);

            return CreateProblemResult(controller, StatusCodes.Status400BadRequest, "Requisicao invalida.", result.Message);
        }
        #endregion

        #region Private Methods/Operators
        private static IActionResult CreateProblemResult(
            ControllerBase controller,
            int statusCode,
            string title,
            string? detail)
        {
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = string.IsNullOrWhiteSpace(detail) ? title : detail,
                Instance = controller.HttpContext.Request.Path
            };

            AddCorrelationId(controller, problem);
            return new ObjectResult(problem) { StatusCode = statusCode };
        }

        private static IActionResult CreateValidationProblemResult(
            ControllerBase controller,
            IReadOnlyCollection<ValidationError> errors,
            string? detail)
        {
            var problem = new ValidationProblemDetails(ToErrorDictionary(errors))
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Erro de validacao.",
                Detail = detail,
                Instance = controller.HttpContext.Request.Path
            };

            AddCorrelationId(controller, problem);
            return new BadRequestObjectResult(problem);
        }

        private static IDictionary<string, string[]> ToErrorDictionary(IEnumerable<ValidationError> errors)
        {
            return errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(item => item.ErrorMessage).ToArray());
        }

        private static void AddCorrelationId(ControllerBase controller, ProblemDetails problemDetails)
        {
            var correlationId = controller.HttpContext.GetCorrelationId();
            if (!string.IsNullOrWhiteSpace(correlationId))
                problemDetails.Extensions["correlationId"] = correlationId;
        }

        private static bool IsNotFound<T>(Result<T> result)
        {
            return string.Equals(result.Message, "Cliente nao encontrado.", StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}