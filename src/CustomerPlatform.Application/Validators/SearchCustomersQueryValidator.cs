using CustomerPlatform.Application.Abstractions.Validation;
using CustomerPlatform.Application.Cqrs.Queries;

namespace CustomerPlatform.Application.Validators
{
    /// <summary>
    /// Validacoes simples para busca de clientes.
    /// </summary>
    public sealed class SearchCustomersQueryValidator : IValidator<SearchCustomersQuery>
    {
        #region Constants
        private const int NameMaxLength = 200;
        private const int DocumentMaxLength = 18;
        private const int EmailMaxLength = 254;
        private const int PhoneMaxLength = 15;
        private const int MaxPageSize = 100;
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Executa as validacoes simples da consulta.
        /// </summary>
        /// <param name="instance">Consulta.</param>
        /// <returns>Resultado da validacao.</returns>
        public ValidationResult Validate(SearchCustomersQuery instance)
        {
            if (instance is null)
                return ValidationResult.Failure(new[] { new ValidationError("Request", "Requisicao obrigatoria.") });

            var errors = new List<ValidationError>();

            if (instance.PageNumber < 1)
                errors.Add(new ValidationError("PageNumber", "Numero da pagina deve ser maior que zero."));

            if (instance.PageSize < 1 || instance.PageSize > MaxPageSize)
                errors.Add(new ValidationError("PageSize", $"Tamanho da pagina deve estar entre 1 e {MaxPageSize}."));

            ValidateOptional(instance.Name, "Name", NameMaxLength, errors);
            ValidateOptional(instance.Document, "Document", DocumentMaxLength, errors);
            ValidateOptional(instance.Email, "Email", EmailMaxLength, errors);
            ValidateOptional(instance.Phone, "Phone", PhoneMaxLength, errors);

            return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
        }
        #endregion

        #region Private Methods/Operators
        private static void ValidateOptional(
            string? value,
            string fieldName,
            int maxLength,
            ICollection<ValidationError> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            var length = value.Trim().Length;
            if (length > maxLength)
                errors.Add(new ValidationError(fieldName, $"Tamanho maximo {maxLength}."));
        }
        #endregion
    }
}
