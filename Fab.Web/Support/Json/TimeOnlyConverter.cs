using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fab.Web.Support.Json;

public class TimeOnlyConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        try
        {
            return TimeOnly.Parse(reader.GetString()!, CultureInfo.InvariantCulture);
        }
        catch (Exception e)
        {
            throw new JsonException(e.Message, e);
        }
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("T"));
}