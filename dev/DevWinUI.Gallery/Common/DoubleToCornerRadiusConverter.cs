using Microsoft.UI.Xaml.Data;

namespace DevWinUIGallery.Common;

internal partial class DoubleToCornerRadiusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double vDouble)
        {
            return new CornerRadius(vDouble);
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
