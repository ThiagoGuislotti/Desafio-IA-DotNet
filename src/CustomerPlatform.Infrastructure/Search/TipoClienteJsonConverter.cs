using CustomerPlatform.Domain.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CustomerPlatform.Infrastructure.Search
{
    /// <summary>
    /// Conversor JSON para TipoCliente.
    /// </summary>
    public sealed class TipoClienteJsonConverter : JsonConverter<TipoCliente>
    {
        #region Public Methods/Operators
        /// <inheritdoc />
        public override TipoCliente Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var numeric))
                return (TipoCliente)numeric;

            if (reader.TokenType == JsonTokenType.String)
            {
                var raw = reader.GetString();
                if (string.IsNullOrWhiteSpace(raw))
                    throw new JsonException("TipoCliente vazio.");

                if (int.TryParse(raw, out var parsedNumeric))
                    return (TipoCliente)parsedNumeric;

                if (Enum.TryParse(raw, true, out TipoCliente parsedEnum))
                    return parsedEnum;
            }

            throw new JsonException($"Valor invalido para {nameof(TipoCliente)}.");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TipoCliente value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((int)value);
        }
        #endregion
    }
}
