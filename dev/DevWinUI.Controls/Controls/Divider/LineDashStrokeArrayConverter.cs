using Microsoft.UI.Xaml.Data;

namespace DevWinUI;
internal partial class LineDashStrokeArrayConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is DoubleCollection collection)
        {
            var doubleCollection = new DoubleCollection();
            foreach (var item in collection)
            {
                doubleCollection.Add(item);
            }
            return doubleCollection;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
