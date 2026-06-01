namespace DevWinUI;

public partial class EmptyObjectToObjectConverter : DependencyObject, IValueConverter
{
    public static readonly DependencyProperty NotEmptyValueProperty =
        DependencyProperty.Register(nameof(NotEmptyValue), typeof(object), typeof(EmptyObjectToObjectConverter), new PropertyMetadata(null));

    public static readonly DependencyProperty EmptyValueProperty =
        DependencyProperty.Register(nameof(EmptyValue), typeof(object), typeof(EmptyObjectToObjectConverter), new PropertyMetadata(null));

    public object NotEmptyValue
    {
        get { return GetValue(NotEmptyValueProperty); }
        set { SetValue(NotEmptyValueProperty, value); }
    }

    public object EmptyValue
    {
        get { return GetValue(EmptyValueProperty); }
        set { SetValue(EmptyValueProperty, value); }
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var isEmpty = CheckValueIsEmpty(value);

        if (ConverterTools.TryParseBool(parameter))
        {
            isEmpty = !isEmpty;
        }

        return ConverterTools.Convert(isEmpty ? EmptyValue : NotEmptyValue, targetType);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }

    protected virtual bool CheckValueIsEmpty(object value)
    {
        return value == null;
    }
}
