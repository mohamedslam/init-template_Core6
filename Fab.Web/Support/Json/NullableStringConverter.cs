using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fab.Web.Support.Json;

public class NullableStringConverter : JsonConverter<string>
{
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();

        return !string.IsNullOrWhiteSpace(value)
            ? value
            : null;
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value);
}