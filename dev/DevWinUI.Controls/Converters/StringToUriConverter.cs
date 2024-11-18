using Microsoft.UI.Xaml.Data;

namespace DevWinUI;

internal partial class StringToUriConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value != null && value is string uri)
        {
            if (uri.StartsWith("ms-appx:///"))
            {
                return uri;
            }
            if (uri.StartsWith("/"))
            {
                uri = uri.Substring(1);
                return new Uri($"ms-appx:///{uri}");
            }

            return new Uri(uri);
        }
        return new Uri("ms-appx:///Assets/NOT_FOUND_IMAGE.png");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
