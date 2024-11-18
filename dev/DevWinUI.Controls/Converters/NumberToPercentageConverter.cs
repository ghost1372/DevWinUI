using Microsoft.UI.Xaml.Data;

namespace DevWinUI;
public sealed partial class NumberToPercentageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
        => Math.Floor(Math.Round((double)value * 100)) + $"{((string)parameter == "WithPercentageSymbol" ? "%" : string.Empty)}";

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
