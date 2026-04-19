namespace DevWinUI;
public partial class DateTimePicker
{
    internal CornerRadius FlyoutCornerRadius
    {
        get { return (CornerRadius)GetValue(FlyoutCornerRadiusProperty); }
        set { SetValue(FlyoutCornerRadiusProperty, value); }
    }

    internal static readonly DependencyProperty FlyoutCornerRadiusProperty =
        DependencyProperty.Register(nameof(FlyoutCornerRadius), typeof(CornerRadius), typeof(DateTimePicker), new PropertyMetadata(new CornerRadius(0)));

    internal Thickness FlyoutBorderThickness
    {
        get { return (Thickness)GetValue(FlyoutBorderThicknessProperty); }
        set { SetValue(FlyoutBorderThicknessProperty, value); }
    }

    internal static readonly DependencyProperty FlyoutBorderThicknessProperty =
        DependencyProperty.Register(nameof(FlyoutBorderThickness), typeof(Thickness), typeof(DateTimePicker), new PropertyMetadata(new Thickness(0)));

    public bool ShowAccentBorderOnHeader
    {
        get { return (bool)GetValue(ShowAccentBorderOnHeaderProperty); }
        set { SetValue(ShowAccentBorderOnHeaderProperty, value); }
    }

    public static readonly DependencyProperty ShowAccentBorderOnHeaderProperty =
        DependencyProperty.Register(nameof(ShowAccentBorderOnHeader), typeof(bool), typeof(DateTimePicker), new PropertyMetadata(true));

    public ClockMode ClockMode
    {
        get { return (ClockMode)GetValue(ClockModeProperty); }
        set { SetValue(ClockModeProperty, value); }
    }

    public static readonly DependencyProperty ClockModeProperty =
        DependencyProperty.Register(nameof(ClockMode), typeof(ClockMode), typeof(DateTimePicker), new PropertyMetadata(ClockMode.AnalogClock, OnClockModeChanged));

    private static void OnClockModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DateTimePicker)d;
        if (ctl != null)
        {
            ctl.UpdateTemplate();
        }
    }

    public DateTimeOffset SelectedDateTime
    {
        get { return (DateTimeOffset)GetValue(SelectedDateTimeProperty); }
        set { SetValue(SelectedDateTimeProperty, value); }
    }
    public static readonly DependencyProperty SelectedDateTimeProperty =
        DependencyProperty.Register(nameof(SelectedDateTime), typeof(DateTimeOffset), typeof(DateTimePicker), new PropertyMetadata(DateTimeOffset.Now, OnSelectedDateChanged));

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
        DependencyProperty.Register(nameof(Description), typeof(object), typeof(DateTimePicker), new PropertyMetadata(default(object), OnDescriptionChanged));
    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DateTimePicker)d;
        if (ctl != null)
        {
            ctl.UpdateDescriptionVisibility();
        }
    }
    public object Header
    {
        get { return (object)GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(DateTimePicker), new PropertyMetadata(default(object), OnHeaderChanged));
    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DateTimePicker)d;
        if (ctl != null)
        {
            ctl.UpdateHeaderVisibility();
        }
    }
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

    public bool ShowConfirmButton
    {
        get { return (bool)GetValue(ShowConfirmButtonProperty); }
        set { SetValue(ShowConfirmButtonProperty, value); }
    }

    public static readonly DependencyProperty ShowConfirmButtonProperty =
        DependencyProperty.Register(nameof(ShowConfirmButton), typeof(bool), typeof(DateTimePicker), new PropertyMetadata(true, OnShowConfirmButtonChanged));

    private static void OnShowConfirmButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DateTimePicker)d;
        if (ctl != null)
        {
            ctl.UpdateTemplate();
        }
    }

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
