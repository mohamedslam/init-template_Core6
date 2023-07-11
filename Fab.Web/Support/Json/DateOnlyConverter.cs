using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fab.Web.Support.Json;

public class DateOnlyConverter : JsonConverter<DateOnly>
{
    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return DateOnly.Parse(reader.GetString()!, CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            throw new JsonException(e.Message, e);
        }
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("O"));
}