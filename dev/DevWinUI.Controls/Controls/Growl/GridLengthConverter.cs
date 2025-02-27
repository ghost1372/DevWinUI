namespace DevWinUI;
public partial class GridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is Visibility buttonVisibility)
        {
            if (buttonVisibility == Visibility.Visible)
            {
                return new GridLength(1, GridUnitType.Star);
            }
            else
            {
                return new GridLength(1, GridUnitType.Auto);
            }
        }
        return new GridLength();
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
