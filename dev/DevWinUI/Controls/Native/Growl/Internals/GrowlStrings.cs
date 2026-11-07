using System.Globalization;

namespace DevWinUI;

internal static partial class GrowlStrings
{
    private static ResourceDictionary? _defaults;

    public static string Close => Get("GrowlCardCloseText", "Close");

    public static string AgeNow => Get("GrowlCardAgeNowText", "now");

    public static void SetDefaults(ResourceDictionary? defaults)
    {
        _defaults = defaults;
    }

    public static string AgeMinutes(int value) => Format(Get("GrowlCardAgeMinutesFormat", "{0}m"), value);

    public static string AgeHours(int value) => Format(Get("GrowlCardAgeHoursFormat", "{0}h"), value);

    public static string AgeDays(int value) => Format(Get("GrowlCardAgeDaysFormat", "{0}d"), value);

    public static string AgeWeeks(int value) => Format(Get("GrowlCardAgeWeeksFormat", "{0}w"), value);

    public static string AgeMonths(int value) => Format(Get("GrowlCardAgeMonthsFormat", "{0}mo"), value);

    public static string AgeYears(int value) => Format(Get("GrowlCardAgeYearsFormat", "{0}y"), value);

    public static string DurationSeconds(int seconds) => Format(Get("GrowlCardCountdownSecondsFormat", "{0}s"), seconds);

    public static string DurationMinutes(int minutes, int seconds) => Format(Get("GrowlCardCountdownMinutesFormat", "{0}:{1:00}"), minutes, seconds);

    private static string Format(string format, params object[] args)
    {
        try
        {
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }
        catch (FormatException)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}", args[0]);
        }
    }

    private static string Get(string key, string fallback)
    {
        if (TryGet(Application.Current?.Resources, key, out var value) || TryGet(_defaults, key, out value))
        {
            return value;
        }

        return fallback;
    }

    private static bool TryGet(ResourceDictionary? dictionary, string key, out string value)
    {
        value = string.Empty;

        try
        {
            if (dictionary is not null && dictionary.TryGetValue(key, out var found) && found is string text && text.Length > 0)
            {
                value = text;
                return true;
            }
        }
        catch (Exception)
        {
        }

        return false;
    }
}
