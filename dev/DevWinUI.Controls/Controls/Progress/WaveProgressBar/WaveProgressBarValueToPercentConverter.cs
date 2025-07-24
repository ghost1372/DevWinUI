namespace DevWinUI;

public partial class WaveProgressBarValueToPercentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value == null)
        {
            return value;
        }
        var percent = string.Format("{0:F0} %", (double)value);
        return percent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
