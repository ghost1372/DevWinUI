namespace DevWinUI;

internal static partial class DateTimeService
{
    public static IClock Clock { get; set; } = new SystemClock();

    public static DateTime Now => Clock.Now;
    public static DateTime UtcNow => Clock.UtcNow;
}
