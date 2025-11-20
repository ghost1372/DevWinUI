namespace DevWinUI;

internal partial class TimeMinutesConverter : IValueConverter
{
    private readonly HaloTimePicker _picker;

    public TimeMinutesConverter(HaloTimePicker picker)
    {
        _picker = picker;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var minutes = _picker.Time.TotalMinutes;
        return (minutes % 60 / 60) * 360;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        var newValue = AngleFor((double)value);

        var newMinutes = (newValue / 360) * 60;
        var seconds = (newMinutes * 60) % 60;

        var hours = WrapHours(_picker.Time, newMinutes);

        return new TimeSpan(
            hours, (int)newMinutes, (int)seconds
        );
    }

    private int WrapHours(TimeSpan span, double newMinutes)
    {
        var oldMinutes = span.Minutes;

        if ((oldMinutes > 45) && (newMinutes < 16))
        {
            return (span.Hours + 1) % 24;
        }

        if ((oldMinutes < 16) && (newMinutes > 45))
        {
            return (24 + span.Hours - 1) % 24;
        }

        return span.Hours;
    }

    private double AngleFor(double value)
    {
        return ((value % 360) + 360) % 360;
    }
}
