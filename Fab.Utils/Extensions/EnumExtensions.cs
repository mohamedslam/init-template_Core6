using System.Reflection;

namespace Fab.Utils.Extensions;

public static class EnumExtensions
{
    public static T? GetEnumCustomAttribute<T>(this Enum e)
        where T : Attribute =>
        e.GetType()
         .GetField(e.ToString())!
         .GetCustomAttribute<T>();

    public static IEnumerable<T> GetEnumCustomAttributes<T>(this Enum e)
        where T : Attribute =>
        e.GetType()
         .GetField(e.ToString())!
         .GetCustomAttributes<T>();
}