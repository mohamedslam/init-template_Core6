namespace Fab.Utils.Extensions;

public static class DateTimeExtensions
{
    private static readonly DateTime Epoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static long ToUnixTimestamp(this DateTime date)
    {
        var time = date.ToUniversalTime().Subtract(Epoch);
        return time.Ticks / TimeSpan.TicksPerSecond;
    }
}