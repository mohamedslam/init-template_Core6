using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fab.Utils.Extensions;

public static class KotlinLikeExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    public static TSource Also<TSource>(this TSource source, Action<TSource> action)
    {
        action(source);
        return source;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    public static TResult Let<TSource, TResult>(this TSource source, Func<TSource, TResult> func) =>
        func(source);
}