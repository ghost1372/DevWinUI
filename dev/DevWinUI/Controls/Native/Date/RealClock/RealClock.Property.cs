namespace DevWinUI;
public partial class RealClock
{
    public CornerRadius ClockCornerRadius
    {
        get { return (CornerRadius)GetValue(ClockCornerRadiusProperty); }
        set { SetValue(ClockCornerRadiusProperty, value); }
    }
    public static readonly DependencyProperty ClockCornerRadiusProperty =
        DependencyProperty.Register(nameof(ClockCornerRadius), typeof(CornerRadius), typeof(RealClock), new PropertyMetadata(new CornerRadius(0)));

    public string TimeZoneId
    {
        get => (string)GetValue(TimeZoneIdProperty);
        set => SetValue(TimeZoneIdProperty, value);
    }
    public static readonly DependencyProperty TimeZoneIdProperty = DependencyProperty.Register(
        nameof(TimeZoneId), typeof(string), typeof(RealClock), new PropertyMetadata(null, OnTimeZoneIdChanged));
    
    private static void OnTimeZoneIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RealClock)d;
        if (ctl != null)
        {
            ctl.Update();
        }
    }

    public string TimeFormat
    {
        get => (string)GetValue(TimeFormatProperty);
        set => SetValue(TimeFormatProperty, value);
    }
    public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register(
        nameof(TimeFormat), typeof(string), typeof(RealClock), new PropertyMetadata("HH:mm", OnTimeFormatChanged));
    private static void OnTimeFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (RealClock)d;
        if (ctl != null)
        {
            ctl.Update();
        }
    }

    public Brush CenterPointFill
    {
        get { return (Brush)GetValue(CenterPointFillProperty); }
        set { SetValue(CenterPointFillProperty, value); }
    }
    public static readonly DependencyProperty CenterPointFillProperty =
        DependencyProperty.Register(nameof(CenterPointFill), typeof(Brush), typeof(RealClock), new PropertyMetadata(default(Brush)));

    public Brush CenterPointStroke
    {
        get { return (Brush)GetValue(CenterPointStrokeProperty); }
        set { SetValue(CenterPointStrokeProperty, value); }
    }
    public static readonly DependencyProperty CenterPointStrokeProperty =
        DependencyProperty.Register(nameof(CenterPointStroke), typeof(Brush), typeof(RealClock), new PropertyMetadata(default(Brush)));

    public double CenterPointHeight
    {
        get { return (double)GetValue(CenterPointHeightProperty); }
        set { SetValue(CenterPointHeightProperty, value); }
    }
    public static readonly DependencyProperty CenterPointHeightProperty =
        DependencyProperty.Register(nameof(CenterPointHeight), typeof(double), typeof(RealClock), new PropertyMetadata(8.0));

    public double CenterPointWidth
    {
        get { return (double)GetValue(CenterPointWidthProperty); }
        set { SetValue(CenterPointWidthProperty, value); }
    }
    public static readonly DependencyProperty CenterPointWidthProperty =
        DependencyProperty.Register(nameof(CenterPointWidth), typeof(double), typeof(RealClock), new PropertyMetadata(8.0));

    public Brush SecondHandBackground
    {
        get { return (Brush)GetValue(SecondHandBackgroundProperty); }
        set { SetValue(SecondHandBackgroundProperty, value); }
    }

    public static readonly DependencyProperty SecondHandBackgroundProperty =
        DependencyProperty.Register(nameof(SecondHandBackground), typeof(Brush), typeof(RealClock), new PropertyMetadata(default(Brush)));

    public Brush MinuteHandBackground
    {
        get { return (Brush)GetValue(MinuteHandBackgroundProperty); }
        set { SetValue(MinuteHandBackgroundProperty, value); }
    }
    public static readonly DependencyProperty MinuteHandBackgroundProperty =
        DependencyProperty.Register(nameof(MinuteHandBackground), typeof(Brush), typeof(RealClock), new PropertyMetadata(default(Brush)));

    public Brush TitleBorderBackground
    {
        get { return (Brush)GetValue(TitleBorderBackgroundProperty); }
        set { SetValue(TitleBorderBackgroundProperty, value); }
    }
    public static readonly DependencyProperty TitleBorderBackgroundProperty =
        DependencyProperty.Register(nameof(TitleBorderBackground), typeof(Brush), typeof(RealClock), new PropertyMetadata(default(Brush)));

    public CornerRadius TitleBorderCornerRadius
    {
        get { return (CornerRadius)GetValue(TitleBorderCornerRadiusProperty); }
        set { SetValue(TitleBorderCornerRadiusProperty, value); }
    }
    public static readonly DependencyProperty TitleBorderCornerRadiusProperty =
        DependencyProperty.Register(nameof(TitleBorderCornerRadius), typeof(CornerRadius), typeof(RealClock), new PropertyMetadata(default(CornerRadius)));

    public double ClockWidth
    {
        get { return (double)GetValue(ClockWidthProperty); }
        set { SetValue(ClockWidthProperty, value); }
    }
    public static readonly DependencyProperty ClockWidthProperty =
        DependencyProperty.Register(nameof(ClockWidth), typeof(double), typeof(RealClock), new PropertyMetadata(250.0));

    public double ClockHeight
    {
        get { return (double)GetValue(ClockHeightProperty); }
        set { SetValue(ClockHeightProperty, value); }
    }
    public static readonly DependencyProperty ClockHeightProperty =
        DependencyProperty.Register(nameof(ClockHeight), typeof(double), typeof(RealClock), new PropertyMetadata(250.0));

    public Brush ClockBackground
    {
        get { return (Brush)GetValue(ClockBackgroundProperty); }
        set { SetValue(ClockBackgroundProperty, value); }
    }
    public static readonly DependencyProperty ClockBackgroundProperty =
        DependencyProperty.Register(nameof(ClockBackground), typeof(Brush), typeof(RealClock), new PropertyMetadata(default(Brush)));

    public double OffsetAngle
    {
        get => (double)GetValue(OffsetAngleProperty);
        set => SetValue(OffsetAngleProperty, value);
    }
    public static readonly DependencyProperty OffsetAngleProperty = DependencyProperty.Register(
        nameof(OffsetAngle), typeof(double), typeof(RealClock), new PropertyMetadata(0.0));

    public double Diameter
    {
        get => (double)GetValue(DiameterProperty);
        set => SetValue(DiameterProperty, value);
    }
    public static readonly DependencyProperty DiameterProperty =
        DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(RealClock), new PropertyMetadata(170.0));

    public Thickness HeaderMargin
    {
        get { return (Thickness)GetValue(HeaderMarginProperty); }
        set { SetValue(HeaderMarginProperty, value); }
    }

    public static readonly DependencyProperty HeaderMarginProperty =
        DependencyProperty.Register(nameof(HeaderMargin), typeof(Thickness), typeof(RealClock), new PropertyMetadata(new Thickness(4)));
}
