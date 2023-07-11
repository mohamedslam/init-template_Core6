using Fab.Utils.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fab.Utils.Json;

public class NullableRelaxedBoolConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.String => reader.GetString()
                                          .Let(x => !string.IsNullOrWhiteSpace(x)
                                              ? bool.TryParse(x, out var b)
                                                  ? (bool?)b
                                                  : throw new JsonException()
                                              : null),
            JsonTokenType.Number => Convert.ToBoolean(reader.GetDecimal()),
            _ => reader.GetBoolean()
        };

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteBooleanValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}