using Microsoft.UI.Xaml.Data;

namespace DevWinUI;
internal partial class CornerRadiusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is CornerRadius cornerRadius)
        {
            return new CornerRadius(0, 0, cornerRadius.BottomRight, cornerRadius.BottomLeft);
        }
        else
        {
            return value;
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value;
    }
}
