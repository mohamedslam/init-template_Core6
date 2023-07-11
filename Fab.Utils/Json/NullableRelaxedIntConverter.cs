using Fab.Utils.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fab.Utils.Json;

public class NullableRelaxedIntConverter : JsonConverter<int?>
{
    public override int? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType switch
        {
            JsonTokenType.Null => null,
            JsonTokenType.String => reader.GetString()
                                          .Let(x => !string.IsNullOrWhiteSpace(x)
                                              ? int.TryParse(x, out var i)
                                                  ? (int?)i
                                                  : throw new JsonException()
                                              : null),
            JsonTokenType.Number => reader.GetInt32(),
            _ => throw new JsonException()
        };

    public override void Write(Utf8JsonWriter writer, int? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteNumberValue(value.Value);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}