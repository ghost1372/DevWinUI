namespace DevWinUI;

public partial class BoolNegationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return !(value is bool && (bool)value);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return !(value is bool && (bool)value);
    }
}
