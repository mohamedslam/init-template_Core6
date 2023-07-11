using System.Runtime.CompilerServices;

namespace Fab.Utils.Extensions;

public static class StringExtensions
{
    public static string Capitalize(this string s) =>
        string.Create(s.Length, s, (chars, str) =>
        {
            chars[0] = char.ToUpper(str[0]);

            for (var i = 1; i < chars.Length; i++)
            {
                chars[i] = str[i];
            }
        });

    public static string Uncapitalize(this string s) =>
        string.Create(s.Length, s, (chars, str) =>
        {
            chars[0] = char.ToLower(str[0]);

            for (var i = 1; i < chars.Length; i++)
            {
                chars[i] = str[i];
            }
        });

    public static string ToHexString(this byte[] bytes) =>
        string.Create(bytes.Length * 2, bytes, (span, array) =>
        {
            for (var i = 0; i < array.Length; i++)
            {
                var b = (byte)(array[i] >> 4);
                span[i * 2] = (char)(b > 9 ? b + 0x37 : b + 0x30);
                b = (byte)(array[i] & 0xF);
                span[i * 2 + 1] = (char)(b > 9 ? b + 0x37 : b + 0x30);
            }
        });

    public static string Pluralize(this int count, string one, string two, string many)
    {
        if (count % 10 == 1 &&
            count % 100 != 11)
        {
            return one;
        }

        if (count % 10 >= 2 && count % 10 <= 4 &&
            (count % 100 < 10 || count % 100 >= 20))
        {
            return two;
        }

        return many;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<TSource>(this IEnumerable<TSource> source, string? separator) =>
        string.Join(separator, source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Join<TSource>(this IEnumerable<TSource> source, char separator) =>
        string.Join(separator, source);
}