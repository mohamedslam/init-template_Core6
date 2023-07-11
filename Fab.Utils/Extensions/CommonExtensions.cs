using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Fab.Utils.Extensions;

public static class CommonExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    public static T As<T>(this object obj) =>
        (T)obj;

    [MethodImpl(MethodImplOptions.AggressiveInlining), DebuggerStepThrough]
    public static T? AsOrDefault<T>(this object obj)
    {
        try
        {
            return (T) obj;
        }
        catch (Exception)
        {
            return default;
        }
    }
}