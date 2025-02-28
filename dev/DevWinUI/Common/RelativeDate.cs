namespace DevWinUI;

[StructLayout(LayoutKind.Auto)]
public readonly struct RelativeDate : IComparable, IComparable<RelativeDate>, IEquatable<RelativeDate>
{
    public readonly ResourceHelper resourceHelper;
    private TimeProvider TimeProvider { get; }
    private DateTime DateTime { get; }

    public RelativeDate(DateTime dateTime, TimeProvider? timeProvider)
    {
        if (dateTime.Kind == DateTimeKind.Unspecified)
            throw new ArgumentException("Cannot determine if the argument is a local datetime or UTC datetime", nameof(dateTime));

        resourceHelper = new ResourceHelper();
        DateTime = dateTime.ToUniversalTime();
        TimeProvider = timeProvider ?? TimeProvider.System;
    }

    public RelativeDate(DateTime dateTime)
        : this(dateTime, timeProvider: null)
    {
    }

    public static RelativeDate Get(DateTime dateTime) => new(dateTime);

    public static RelativeDate Get(DateTimeOffset dateTime) => new(dateTime.UtcDateTime);

    public static RelativeDate Get(DateTime dateTime, TimeProvider? timeProvider) => new(dateTime, timeProvider);

    public static RelativeDate Get(DateTimeOffset dateTime, TimeProvider? timeProvider) => new(dateTime.UtcDateTime, timeProvider);

    public override string ToString() => ToString("en-US");

    public string ToString(string language)
    {
        var now = TimeProvider.GetUtcNow().UtcDateTime;
        var delta = now - DateTime;

        if (delta < TimeSpan.Zero)
        {
            delta = -delta;
            if (delta < TimeSpan.FromMinutes(1))
            {
                return delta.Seconds <= 1 ?
                    GetString("InOneSecond", language) :
                    GetString("InManySeconds", language, delta.Seconds);
            }

            if (delta < TimeSpan.FromMinutes(2))
                return GetString("InAMinute", language);

            if (delta < TimeSpan.FromMinutes(45))
                return GetString("InManyMinutes", language, delta.Minutes);

            if (delta < TimeSpan.FromMinutes(90))
                return GetString("InAnHour", language);

            if (delta < TimeSpan.FromHours(24))
                return GetString("InManyHours", language, delta.Hours);

            if (delta < TimeSpan.FromHours(48))
                return GetString("Tomorrow", language);

            if (delta < TimeSpan.FromDays(30))
                return GetString("InManyDays", language, delta.Days);

            if (delta < TimeSpan.FromDays(365)) // We don't care about leap year
            {
                var months = Convert.ToInt32(Math.Floor((double)delta.Days / 30));
                return months <= 1 ?
                    GetString("InOneMonth", language) :
                    GetString("InManyMonths", language, months);
            }
            else
            {
                var years = Convert.ToInt32(Math.Floor((double)delta.Days / 365));
                return years <= 1 ?
                    GetString("InOneYear", language) :
                    GetString("InManyYears", language, years);
            }
        }

        if (delta == TimeSpan.Zero)
            return GetString("Now", language);

        if (delta < TimeSpan.FromMinutes(1))
        {
            return delta.Seconds <= 1 ?
                GetString("OneSecondAgo", language) :
                GetString("ManySecondsAgo", language, delta.Seconds);
        }

        if (delta < TimeSpan.FromMinutes(2))
            return GetString("AMinuteAgo", language);

        if (delta < TimeSpan.FromMinutes(45))
            return GetString("ManyMinutesAgo", language, delta.Minutes);

        if (delta < TimeSpan.FromMinutes(90))
            return GetString("AnHourAgo", language);

        if (delta < TimeSpan.FromHours(24))
            return GetString("ManyHoursAgo", language, delta.Hours);

        if (delta < TimeSpan.FromHours(48))
            return GetString("Yesterday", language);

        if (delta < TimeSpan.FromDays(30))
            return GetString("ManyDaysAgo", language, delta.Days);

        if (delta < TimeSpan.FromDays(365)) // We don't care about leap year
        {
            var months = Convert.ToInt32(Math.Floor((double)delta.Days / 30));
            return months <= 1 ?
                GetString("OneMonthAgo", language) :
                GetString("ManyMonthsAgo", language, months);
        }
        else
        {
            var years = Convert.ToInt32(Math.Floor((double)delta.Days / 365));
            return years <= 1 ?
                GetString("OneYearAgo", language) :
                GetString("ManyYearsAgo", language, years);
        }
    }
    private string GetString(string key, string language)
    {
        return resourceHelper.GetStringFromResource(key, language, "DevWinUI/Resources");
    }
    private string GetString(string key, string language, int value)
    {
        var candidate = resourceHelper.GetStringFromResource(key, language, "DevWinUI/Resources");

        return candidate != null ? string.Format(candidate, value) : key;
    }
    int IComparable.CompareTo(object? obj)
    {
        if (obj is RelativeDate rd)
            return CompareTo(rd);

        return CompareTo(default);
    }

    public int CompareTo(RelativeDate other) => DateTime.CompareTo(other.DateTime);

    public override bool Equals(object? obj) => obj is RelativeDate date && Equals(date);

    public bool Equals(RelativeDate other) => DateTime == other.DateTime;

    public override int GetHashCode() => -10323184 + DateTime.GetHashCode();

    public static bool operator ==(RelativeDate date1, RelativeDate date2) => date1.Equals(date2);
    public static bool operator !=(RelativeDate date1, RelativeDate date2) => !(date1 == date2);
    public static bool operator <(RelativeDate left, RelativeDate right) => left.CompareTo(right) < 0;
    public static bool operator <=(RelativeDate left, RelativeDate right) => left.CompareTo(right) <= 0;
    public static bool operator >(RelativeDate left, RelativeDate right) => left.CompareTo(right) > 0;
    public static bool operator >=(RelativeDate left, RelativeDate right) => left.CompareTo(right) >= 0;
}
