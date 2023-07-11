namespace Fab.Utils.Extensions;

public static class ArrayDestructuringExtensions
{
    public static void Deconstruct<T>(this T?[] array, out T? item)
    {
        item = array is { Length: > 0 }
            ? array[0]
            : default;
    }

    public static void Deconstruct<T>(this T?[] array, out T? item1, out T? item2)
    {
        item1 = array is { Length: > 0 }
            ? array[0]
            : default;

        item2 = array is { Length: > 1 }
            ? array[1]
            : default;
    }

    public static void Deconstruct<T>(this T?[] array, out T? item1, out T? item2, out T? item3)
    {
        item1 = array is { Length: > 0 }
            ? array[0]
            : default;

        item2 = array is { Length: > 1 }
            ? array[1]
            : default;

        item3 = array is { Length: > 2 }
            ? array[2]
            : default;
    }

    public static void Deconstruct<T>(this T?[] array, out T? item1, out T? item2, out T? item3, out T? item4)
    {
        item1 = array is { Length: > 0 }
            ? array[0]
            : default;

        item2 = array is { Length: > 1 }
            ? array[1]
            : default;

        item3 = array is { Length: > 2 }
            ? array[2]
            : default;

        item4 = array is { Length: > 3 }
            ? array[3]
            : default;
    }

    public static void Deconstruct<T>(this T?[] array,
                                      out T? item1, out T? item2, out T? item3, out T? item4, out T? item5)
    {
        item1 = array is { Length: > 0 }
            ? array[0]
            : default;

        item2 = array is { Length: > 1 }
            ? array[1]
            : default;

        item3 = array is { Length: > 2 }
            ? array[2]
            : default;

        item4 = array is { Length: > 3 }
            ? array[3]
            : default;

        item5 = array is { Length: > 4 }
            ? array[4]
            : default;
    }
}