namespace DevWinUI;

public partial class ClockPicker
{
    public CornerRadius FlyoutCornerRadius
    {
        get { return (CornerRadius)GetValue(FlyoutCornerRadiusProperty); }
        set { SetValue(FlyoutCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty FlyoutCornerRadiusProperty =
        DependencyProperty.Register(nameof(FlyoutCornerRadius), typeof(CornerRadius), typeof(ClockPicker), new PropertyMetadata(new CornerRadius(0)));

    public Thickness FlyoutBorderThickness
    {
        get { return (Thickness)GetValue(FlyoutBorderThicknessProperty); }
        set { SetValue(FlyoutBorderThicknessProperty, value); }
    }

    public static readonly DependencyProperty FlyoutBorderThicknessProperty =
        DependencyProperty.Register(nameof(FlyoutBorderThickness), typeof(Thickness), typeof(ClockPicker), new PropertyMetadata(new Thickness(1)));

    public TimeSpan? SelectedTime
    {
        get { return (TimeSpan?)GetValue(SelectedTimeProperty); }
        set { SetValue(SelectedTimeProperty, value); }
    }

    public static readonly DependencyProperty SelectedTimeProperty =
        DependencyProperty.Register(nameof(SelectedTime), typeof(TimeSpan?), typeof(ClockPicker), new PropertyMetadata(DateTimeBase.TimeNow, OnSelectedTimeChanged));

    private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ClockPicker ctl)
        {
            ctl.OnSelectedTimeChanged();
        }
    }

    public TimeOnly? SelectedTimeOnly
    {
        get { return (TimeOnly?)GetValue(SelectedTimeOnlyProperty); }
        set { SetValue(SelectedTimeOnlyProperty, value); }
    }

    public static readonly DependencyProperty SelectedTimeOnlyProperty =
        DependencyProperty.Register(nameof(SelectedTimeOnly), typeof(TimeOnly?), typeof(ClockPicker), new PropertyMetadata(TimeOnly.FromDateTime(DateTime.Now), OnSelectedTimeOnlyChanged));

    private static void OnSelectedTimeOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ClockPicker ctl)
        {
            ctl.OnSelectedTimeOnlyChanged();
        }
    }

    public TimeSpan Time
    {
        get { return (TimeSpan)GetValue(TimeProperty); }
        set { SetValue(TimeProperty, value); }
    }

    public static readonly DependencyProperty TimeProperty =
        DependencyProperty.Register(nameof(Time), typeof(TimeSpan), typeof(ClockPicker), new PropertyMetadata(TimeSpan.Zero, OnTimeChanged));

    private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ClockPicker ctl)
        {
            ctl.OnTimeChanged();
        }
    }

    public int MinuteIncrement
    {
        get { return (int)GetValue(MinuteIncrementProperty); }
        set { SetValue(MinuteIncrementProperty, value); }
    }

    public static readonly DependencyProperty MinuteIncrementProperty =
        DependencyProperty.Register(nameof(MinuteIncrement), typeof(int), typeof(ClockPicker), new PropertyMetadata(1, OnMinuteIncrementChanged));

    private static void OnMinuteIncrementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ClockPicker ctl && ctl.clock != null)
        {
            ctl.clock.MinuteIncrement = (int)e.NewValue;
        }
    }

    public LightDismissOverlayMode LightDismissOverlayMode
    {
        get { return (LightDismissOverlayMode)GetValue(LightDismissOverlayModeProperty); }
        set { SetValue(LightDismissOverlayModeProperty, value); }
    }

    public static readonly DependencyProperty LightDismissOverlayModeProperty =
        DependencyProperty.Register(nameof(LightDismissOverlayMode), typeof(LightDismissOverlayMode), typeof(ClockPicker), new PropertyMetadata(LightDismissOverlayMode.Auto));

    public string PlaceholderText
    {
        get { return (string)GetValue(PlaceholderTextProperty); }
        set { SetValue(PlaceholderTextProperty, value); }
    }

    public static readonly DependencyProperty PlaceholderTextProperty =
        DependencyProperty.Register(nameof(PlaceholderText), typeof(string), typeof(ClockPicker), new PropertyMetadata(default(string)));

    public object Description
    {
        get { return (object)GetValue(DescriptionProperty); }
        set { SetValue(DescriptionProperty, value); }
    }

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(object), typeof(ClockPicker), new PropertyMetadata(default(object), OnDescriptionChanged));

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ClockPicker ctl)
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
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(ClockPicker), new PropertyMetadata(default(object), OnHeaderChanged));

    private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ClockPicker ctl)
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
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(ClockPicker), new PropertyMetadata(default(DataTemplate)));

    public object ConfirmButtonContent
    {
        get { return (object)GetValue(ConfirmButtonContentProperty); }
        set { SetValue(ConfirmButtonContentProperty, value); }
    }

    public static readonly DependencyProperty ConfirmButtonContentProperty =
        DependencyProperty.Register(nameof(ConfirmButtonContent), typeof(object), typeof(ClockPicker), new PropertyMetadata("Confirm"));

    public Style ConfirmButtonStyle
    {
        get { return (Style)GetValue(ConfirmButtonStyleProperty); }
        set { SetValue(ConfirmButtonStyleProperty, value); }
    }

    public static readonly DependencyProperty ConfirmButtonStyleProperty =
        DependencyProperty.Register(nameof(ConfirmButtonStyle), typeof(Style), typeof(ClockPicker), new PropertyMetadata(default(Style)));

    public bool ShowConfirmButton
    {
        get { return (bool)GetValue(ShowConfirmButtonProperty); }
        set { SetValue(ShowConfirmButtonProperty, value); }
    }

    public static readonly DependencyProperty ShowConfirmButtonProperty =
        DependencyProperty.Register(nameof(ShowConfirmButton), typeof(bool), typeof(ClockPicker), new PropertyMetadata(true, OnShowConfirmButtonChanged));

    private static void OnShowConfirmButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ClockPicker ctl)
        {
            ctl.UpdateTemplate();
        }
    }

    public string TimeFormat
    {
        get { return (string)GetValue(TimeFormatProperty); }
        set { SetValue(TimeFormatProperty, value); }
    }

    public static readonly DependencyProperty TimeFormatProperty =
        DependencyProperty.Register(nameof(TimeFormat), typeof(string), typeof(ClockPicker), new PropertyMetadata("HH:mm:ss", OnTimeFormatChanged));

    private static void OnTimeFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ClockPicker ctl && ctl.clock != null)
        {
            ctl.clock.TimeFormat = (string)e.NewValue;
        }
    }

    public Thickness ClockMargin
    {
        get { return (Thickness)GetValue(ClockMarginProperty); }
        set { SetValue(ClockMarginProperty, value); }
    }

    public static readonly DependencyProperty ClockMarginProperty =
        DependencyProperty.Register(nameof(ClockMargin), typeof(Thickness), typeof(ClockPicker), new PropertyMetadata(new Thickness(0)));

    public Thickness ClockBorderThickness
    {
        get { return (Thickness)GetValue(ClockBorderThicknessProperty); }
        set { SetValue(ClockBorderThicknessProperty, value); }
    }

    public static readonly DependencyProperty ClockBorderThicknessProperty =
        DependencyProperty.Register(nameof(ClockBorderThickness), typeof(Thickness), typeof(ClockPicker), new PropertyMetadata(new Thickness(0)));

    public CornerRadius ClockCornerRadius
    {
        get { return (CornerRadius)GetValue(ClockCornerRadiusProperty); }
        set { SetValue(ClockCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty ClockCornerRadiusProperty =
        DependencyProperty.Register(nameof(ClockCornerRadius), typeof(CornerRadius), typeof(ClockPicker), new PropertyMetadata(new CornerRadius(0, 4, 4, 0)));
}
