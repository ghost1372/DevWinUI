
namespace DevWinUI;
public partial class StringToStyleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string styleKey && !string.IsNullOrEmpty(styleKey))
        {
            if (Application.Current.Resources.TryGetValue(styleKey, out var resource) && resource is Style style)
            {
                return style;
            }
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
