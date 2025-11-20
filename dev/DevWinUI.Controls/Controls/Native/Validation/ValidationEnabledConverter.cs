using System.ComponentModel;

namespace DevWinUI;

public sealed partial class ValidationEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is INotifyDataErrorInfo ? true : (object)false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
