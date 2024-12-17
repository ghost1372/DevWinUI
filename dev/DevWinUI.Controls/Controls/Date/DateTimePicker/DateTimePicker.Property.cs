namespace DevWinUI;
public partial class DateTimePicker
{
    public DateTimeOffset SelectedDate
    {
        get { return (DateTimeOffset)GetValue(SelectedDateProperty); }
        set { SetValue(SelectedDateProperty, value); }
    }
    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(nameof(SelectedDate), typeof(DateTimeOffset), typeof(DateTimePicker), new PropertyMetadata(DateTimeOffset.Now, OnSelectedDateChanged));

    private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DateTimePicker ctl && ctl != null)
        {
            ctl.UpdateSelectedDate();
        }
    }

    public TimePickerDisplayMode TimePickerDisplayMode
    {
        get { return (TimePickerDisplayMode)GetValue(TimePickerDisplayModeProperty); }
        set { SetValue(TimePickerDisplayModeProperty, value); }
    }

    public static readonly DependencyProperty TimePickerDisplayModeProperty =
        DependencyProperty.Register(nameof(TimePickerDisplayMode), typeof(TimePickerDisplayMode), typeof(DateTimePicker), new PropertyMetadata(TimePickerDisplayMode.Right));

    public CornerRadius CalendarViewCornerRadius
    {
        get { return (CornerRadius)GetValue(CalendarViewCornerRadiusProperty); }
        set { SetValue(CalendarViewCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty CalendarViewCornerRadiusProperty =
        DependencyProperty.Register(nameof(CalendarViewCornerRadius), typeof(CornerRadius), typeof(DateTimePicker), new PropertyMetadata(new CornerRadius(8)));

    public string PlaceholderText
    {
        get { return (string)GetValue(PlaceholderTextProperty); }
        set { SetValue(PlaceholderTextProperty, value); }
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(DateTimePicker), new PropertyMetadata(default(string)));
    public object Description
    {
        get { return (object)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(object), typeof(DateTimePicker), new PropertyMetadata(default(object)));
    public object Header
    {
        get { return (object)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(DateTimePicker), new PropertyMetadata(default(object)));
    public DataTemplate HeaderTemplate
    {
        get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        set { SetValue(HeaderTemplateProperty, value); }
    }

    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(DateTimePicker), new PropertyMetadata(default(DataTemplate)));
    public object ConfirmButtonContent
    {
        get { return (object)GetValue(ConfirmButtonContentProperty); }
        set { SetValue(ConfirmButtonContentProperty, value); }
    }

    public static readonly DependencyProperty ConfirmButtonContentProperty =
        DependencyProperty.Register(nameof(ConfirmButtonContent), typeof(object), typeof(DateTimePicker), new PropertyMetadata("Confirm"));

    public Style ConfirmButtonStyle
    {
        get { return (Style)GetValue(ConfirmButtonStyleProperty); }
        set { SetValue(ConfirmButtonStyleProperty, value); }
    }

    public static readonly DependencyProperty ConfirmButtonStyleProperty =
        DependencyProperty.Register(nameof(ConfirmButtonStyle), typeof(Style), typeof(DateTimePicker), new PropertyMetadata(default(Style)));

    public bool IsConfirmButtonShow
    {
        get { return (bool)GetValue(IsConfirmButtonShowProperty); }
        set { SetValue(IsConfirmButtonShowProperty, value); }
    }

    public static readonly DependencyProperty IsConfirmButtonShowProperty =
        DependencyProperty.Register(nameof(IsConfirmButtonShow), typeof(bool), typeof(DateTimePicker), new PropertyMetadata(true));

    public TimeSpan? SelectedTime
    {
        get { return (TimeSpan?)GetValue(SelectedTimeProperty); }
        set { SetValue(SelectedTimeProperty, value); }
    }

    public static readonly DependencyProperty SelectedTimeProperty =
        DependencyProperty.Register(nameof(SelectedTime), typeof(TimeSpan?), typeof(DateTimeBase), new PropertyMetadata(TimeNow, OnSelectedTimeChanged));

    private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is DateTimePicker ctl && ctl != null)
        {
            ctl.UpdateSelectedTime();
        }
    }
}
