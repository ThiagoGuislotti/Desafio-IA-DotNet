using CustomerPlatform.Application.Abstractions.Validation;
using CustomerPlatform.Application.Cqrs.Queries;

namespace CustomerPlatform.Application.Validators
{
    /// <summary>
    /// Validacoes simples para consulta por identificador.
    /// </summary>
    public sealed class GetCustomerByIdQueryValidator : IValidator<GetCustomerByIdQuery>
    {
        #region Public Methods/Operators
        /// <summary>
        /// Executa as validacoes simples da consulta.
        /// </summary>
        /// <param name="instance">Consulta.</param>
        /// <returns>Resultado da validacao.</returns>
        public ValidationResult Validate(GetCustomerByIdQuery instance)
        {
            if (instance is null)
                return ValidationResult.Failure(new[] { new ValidationError("Request", "Requisicao obrigatoria.") });

            if (instance.Id == Guid.Empty)
                return ValidationResult.Failure(new[] { new ValidationError("Id", "Id obrigatorio.") });

            return ValidationResult.Success();
        }
        #endregion
    }
}
