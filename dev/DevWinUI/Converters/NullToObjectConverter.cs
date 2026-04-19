namespace DevWinUI;
public partial class NullToObjectConverter : DependencyObject, IValueConverter
{
    public static readonly DependencyProperty NullValueProperty =
        DependencyProperty.Register(nameof(NullValue), typeof(object), typeof(NullToObjectConverter), new PropertyMetadata(null));

    public static readonly DependencyProperty NotNullValueProperty =
        DependencyProperty.Register(nameof(NotNullValue), typeof(object), typeof(NullToObjectConverter), new PropertyMetadata(null));

    /// <summary>
    /// Value returned when the source is null
    /// </summary>
    public object NullValue
    {
        get => GetValue(NullValueProperty);
        set => SetValue(NullValueProperty, value);
    }

    /// <summary>
    /// Value returned when the source is not null
    /// </summary>
    public object NotNullValue
    {
        get => GetValue(NotNullValueProperty);
        set => SetValue(NotNullValueProperty, value);
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool isNull = value is null;

        // Invert if parameter is true
        if (ConverterTools.TryParseBool(parameter))
        {
            isNull = !isNull;
        }

        return ConverterTools.Convert(isNull ? NullValue : NotNullValue, targetType);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        bool result = Equals(value, ConverterTools.Convert(NullValue, value.GetType()));

        if (ConverterTools.TryParseBool(parameter))
        {
            result = !result;
        }

        return result ? null : DependencyProperty.UnsetValue;
    }
}
