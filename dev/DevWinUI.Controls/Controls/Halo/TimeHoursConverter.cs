namespace DevWinUI;

internal partial class TimeHoursConverter : IValueConverter
{
    private readonly HaloTimePicker _picker;

    public TimeHoursConverter(HaloTimePicker picker)
    {
        _picker = picker;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var hours = _picker.Time.TotalHours;
        return ((hours % 12) / 12) * 360;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        var span = _picker.Time;

        var newValue = AngleFor((double)value);
        var newHours = newValue / 360 * 12;

        var offset = WrapPeriod(span, newHours);

        return new TimeSpan(
            (int)newHours + offset, span.Minutes, span.Seconds
        );
    }

    private int WrapPeriod(TimeSpan span, double newHours)
    {
        var oldHours = span.Hours % 12;
        var offset = (span.Hours / 12) * 12;

        if ((oldHours > 9) ^ (newHours > 9))
        {
            if ((oldHours < 4) ^ (newHours < 4))
            {
                return (offset + 12) % 24;
            }
        }

        return offset;
    }

    private double AngleFor(double value)
    {
        return ((value % 360) + 360) % 360;
    }
}
