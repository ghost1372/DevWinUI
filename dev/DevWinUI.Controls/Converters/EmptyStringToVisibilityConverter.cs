using Microsoft.UI.Xaml.Data;

namespace DevWinUI;
public partial class EmptyStringToVisibilityConverter : IValueConverter
{
    public Visibility EmptyValue { get; set; } = Visibility.Collapsed;
    public Visibility NonEmptyValue { get; set; } = Visibility.Visible;


    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value == null ? EmptyValue : value is string stringValue && stringValue != "" ? NonEmptyValue : (object)EmptyValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

