using System.Text.Json.Serialization;

namespace Fab.Web.Exceptions;

public class ExceptionDetailDto
{
    public string Label { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Field { get; }

    public string Key { get; }

    public ExceptionDetailDto(string key, string label)
    {
        Key = key;
        Label = label;
    }

    public ExceptionDetailDto(string key, string field, string label) : this(key, label)
    {
        Field = field;
    }

    public ExceptionDetailDto(Exception e) : this(e.GetType().Name, e.Message)
    {
    }
}