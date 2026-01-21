using CustomerPlatform.Application.Abstractions.Validation;
using CustomerPlatform.Application.Cqrs.Commands;
using CustomerPlatform.Application.DTOs;

namespace CustomerPlatform.Application.Validators
{
    /// <summary>
    /// Validacoes simples para atualizacao de cliente pessoa juridica.
    /// </summary>
    public sealed class UpdateCompanyCustomerValidator : IValidator<UpdateCompanyCustomerCommand>
    {
        #region Constants
        private const int NameMinLength = 2;
        private const int NameMaxLength = 200;
        private const int EmailMaxLength = 254;
        private const int PhoneMinLength = 10;
        private const int PhoneMaxLength = 15;
        private const int AddressMaxLength = 200;
        private const int PostalCodeMaxLength = 10;
        private const int CityMaxLength = 100;
        private const int StateMaxLength = 100;
        #endregion

        #region Public Methods/Operators
        /// <summary>
        /// Executa as validacoes simples do comando.
        /// </summary>
        /// <param name="instance">Comando de atualizacao.</param>
        /// <returns>Resultado da validacao.</returns>
        public ValidationResult Validate(UpdateCompanyCustomerCommand instance)
        {
            if (instance is null)
                return ValidationResult.Failure(new[] { new ValidationError("Request", "Requisicao obrigatoria.") });

            var errors = new List<ValidationError>();

            if (instance.Id == Guid.Empty)
                errors.Add(new ValidationError("Id", "Id obrigatorio."));

            ValidateRequired(instance.CorporateName, "CorporateName", NameMinLength, NameMaxLength, errors);
            ValidateRequired(instance.TradeName, "TradeName", NameMinLength, NameMaxLength, errors);
            ValidateRequired(instance.Email, "Email", 5, EmailMaxLength, errors);
            ValidateRequired(instance.Phone, "Phone", PhoneMinLength, PhoneMaxLength, errors);

            ValidateAddress(instance.Address, errors);

            return errors.Count == 0 ? ValidationResult.Success() : ValidationResult.Failure(errors);
        }
        #endregion

        #region Private Methods/Operators
        private static void ValidateAddress(AddressDto? address, ICollection<ValidationError> errors)
        {
            if (address is null)
            {
                errors.Add(new ValidationError("Address", "Endereco obrigatorio."));
                return;
            }

            ValidateRequired(address.Street, "Address.Street", 2, AddressMaxLength, errors);
            ValidateRequired(address.Number, "Address.Number", 1, AddressMaxLength, errors);
            ValidateRequired(address.PostalCode, "Address.PostalCode", 5, PostalCodeMaxLength, errors);
            ValidateRequired(address.City, "Address.City", 2, CityMaxLength, errors);
            ValidateRequired(address.State, "Address.State", 2, StateMaxLength, errors);
        }

        private static void ValidateRequired(
            string? value,
            string fieldName,
            int minLength,
            int maxLength,
            ICollection<ValidationError> errors)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                errors.Add(new ValidationError(fieldName, "Campo obrigatorio."));
                return;
            }

            var length = value.Trim().Length;

            if (length < minLength)
                errors.Add(new ValidationError(fieldName, $"Tamanho minimo {minLength}."));

            if (length > maxLength)
                errors.Add(new ValidationError(fieldName, $"Tamanho maximo {maxLength}."));
        }
        #endregion
    }
}