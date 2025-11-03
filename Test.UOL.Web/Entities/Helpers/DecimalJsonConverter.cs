using System.Text.Json;
using System.Text.Json.Serialization;

namespace Test.UOL.Web.Entities.Helpers;

public class DecimalJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDecimal();

        if (reader.TokenType == JsonTokenType.String &&
            decimal.TryParse(reader.GetString(), out var result))
            return result;

        throw new JsonException($"Valor '{reader.GetString()}' não é um decimal válido.");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
