using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Fab.Web.Support.Json;

public class PolymorphicJsonConverter : JsonConverterFactory
{
    private static readonly string NamespacePrefix = typeof(PolymorphicJsonConverter).Namespace!.Split('.', 2)[0];

    private static readonly HashSet<Type> Types = AppDomain.CurrentDomain
                                                           .GetAssemblies()
                                                           .Where(IsOwnAssembly)
                                                           .SelectMany(x => x.GetTypes())
                                                           .Where(x => x.BaseType != null &&
                                                                       !x.IsGenericType &&
                                                                       !x.BaseType.IsGenericType &&
                                                                       IsOwnAssemblyType(x) &&
                                                                       IsOwnAssemblyType(x.BaseType))
                                                           .Select(x => GetBaseType(x.BaseType!))
                                                           .ToHashSet();

    private static bool IsOwnAssembly(Assembly assembly) =>
        assembly.GetName().Name?.StartsWith(NamespacePrefix) ?? false;

    private static bool IsOwnAssemblyType(Type type) =>
        type.Namespace?.StartsWith(NamespacePrefix) ?? false;

    private static Type GetBaseType(Type type)
    {
        while (type.BaseType != null && IsOwnAssemblyType(type.BaseType))
        {
            type = type.BaseType;
        }

        return type;
    }

    public override bool CanConvert(Type typeToConvert) =>
        Types.Contains(typeToConvert);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) =>
        Activator.CreateInstance(typeof(PolymorphicConverter<>).MakeGenericType(typeToConvert)) as JsonConverter;

    private sealed class PolymorphicConverter<T> : JsonConverter<T>
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(ref reader, options);
            }
            catch (Exception e)
            {
                throw new JsonException(e.Message, e);
            }
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value, value!.GetType(), options);
    }
}