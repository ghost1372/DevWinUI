namespace DevWinUI;

public static partial class DateTimeHelper
{
    public static unsafe string GetWindowsDateTimePattern(DateTimePattern pattern)
    {
        uint localeType = pattern switch
        {
            DateTimePattern.ShortTime => PInvoke.LOCALE_SSHORTTIME,
            DateTimePattern.LongTime => PInvoke.LOCALE_SLONGDATE,
            DateTimePattern.ShortDate => PInvoke.LOCALE_SSHORTDATE,
            DateTimePattern.LongDate => PInvoke.LOCALE_SLONGDATE,
            _ => throw new ArgumentOutOfRangeException(nameof(pattern))
        };

        // Null locale name means the user's default locale.
        int length = PInvoke.GetLocaleInfoEx(null, localeType, null, 0);

        if (length <= 0)
            throw new InvalidOperationException(
                "Failed to retrieve the Windows time pattern.");

        char* buffer = stackalloc char[length];

        int result = PInvoke.GetLocaleInfoEx(null, localeType, buffer, length);

        if (result <= 0)
            throw new InvalidOperationException(
                "Failed to retrieve the Windows time pattern.");

        return new string(buffer, 0, result - 1);
    }
    public static bool Is24HourFormat()
    {
        return GetWindowsDateTimePattern(DateTimePattern.ShortTime).Contains('H');
    }
}
