using CustomerPlatform.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CustomerPlatform.Infrastructure.Data
{
    /// <summary>
    /// Conversores de value objects para o EF Core.
    /// </summary>
    internal static class ValueObjectConverters
    {
        #region Static Properties
        /// <summary>
        /// Converte <see cref="Documento"/> para string.
        /// </summary>
        public static ValueConverter<Documento, string> DocumentoConverter { get; } =
            new ValueConverter<Documento, string>(
                documento => documento.Numero,
                numero => new Documento(numero));

        /// <summary>
        /// Converte <see cref="Email"/> para string.
        /// </summary>
        public static ValueConverter<Email, string> EmailConverter { get; } =
            new ValueConverter<Email, string>(
                email => email.Endereco,
                endereco => new Email(endereco));

        /// <summary>
        /// Converte <see cref="Telefone"/> para string.
        /// </summary>
        public static ValueConverter<Telefone, string> TelefoneConverter { get; } =
            new ValueConverter<Telefone, string>(
                telefone => telefone.Numero,
                numero => new Telefone(numero));
        #endregion
    }
}
