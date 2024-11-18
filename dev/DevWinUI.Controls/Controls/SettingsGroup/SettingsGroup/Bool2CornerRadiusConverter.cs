using Microsoft.UI.Xaml.Data;

namespace DevWinUI;
internal partial class Bool2CornerRadiusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var itemsCount = System.Convert.ToInt32(value);
        if (itemsCount > 0)
        {
            return new CornerRadius(8, 8, 0, 0);
        }
        return new CornerRadius(8);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
