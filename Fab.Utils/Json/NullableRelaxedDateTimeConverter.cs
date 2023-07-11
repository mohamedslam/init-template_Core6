using Fab.Utils.Extensions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fab.Utils.Json;

public class RelaxedDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()
              .Let(x =>
              {
                  if (string.IsNullOrWhiteSpace(x))
                  {
                      throw new JsonException();
                  }

                  return long.TryParse(x, out var l)
                      ? DateTime.UnixEpoch.AddSeconds(l)
                      : DateTime.Parse(x);
              });

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToString("O"));
}