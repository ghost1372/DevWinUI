namespace DevWinUI;
public partial class Clock
{
    public event EventHandler<DateTime> SelectedTimeChanged;
    
    public static readonly DependencyProperty SelectedTimeProperty = DependencyProperty.Register(
       nameof(SelectedTime), typeof(DateTime), typeof(Clock), new PropertyMetadata(DateTime.Now, OnSelectedTimeChanged));
    public DateTime SelectedTime
    {
        get => (DateTime)GetValue(SelectedTimeProperty);
        set => SetValue(SelectedTimeProperty, value);
    }
    private static void OnSelectedTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (Clock)d;
        if (ctl != null)
        {
            ctl.OnSelectedTimeChanged((DateTime)e.NewValue);
        }
    }

    private void OnSelectedTimeChanged(DateTime dateTime)
    {
        SelectedTimeChanged?.Invoke(this, dateTime);
    }

    public static readonly DependencyProperty TimeFormatProperty = DependencyProperty.Register(
        nameof(TimeFormat), typeof(string), typeof(Clock), new PropertyMetadata("HH:mm:ss"));

    public string TimeFormat
    {
        get => (string)GetValue(TimeFormatProperty);
        set => SetValue(TimeFormatProperty, value);
    }
    

    public static readonly DependencyProperty CenterPointFillProperty =
        DependencyProperty.Register(nameof(CenterPointFill), typeof(SolidColorBrush), typeof(Clock), new PropertyMetadata(default(SolidColorBrush)));
    public SolidColorBrush CenterPointFill
    {
        get { return (SolidColorBrush)GetValue(CenterPointFillProperty); }
        set { SetValue(CenterPointFillProperty, value); }
    }
    

    public static readonly DependencyProperty CenterPointStrokeProperty =
        DependencyProperty.Register(nameof(CenterPointStroke), typeof(SolidColorBrush), typeof(Clock), new PropertyMetadata(default(SolidColorBrush)));
    public SolidColorBrush CenterPointStroke
    {
        get { return (SolidColorBrush)GetValue(CenterPointStrokeProperty); }
        set { SetValue(CenterPointStrokeProperty, value); }
    }

    public static readonly DependencyProperty CenterPointHeightProperty =
        DependencyProperty.Register(nameof(CenterPointHeight), typeof(double), typeof(Clock), new PropertyMetadata(8.0));
    public double CenterPointHeight
    {
        get { return (double)GetValue(CenterPointHeightProperty); }
        set { SetValue(CenterPointHeightProperty, value); }
    }

    public static readonly DependencyProperty CenterPointWidthProperty =
        DependencyProperty.Register(nameof(CenterPointWidth), typeof(double), typeof(Clock), new PropertyMetadata(8.0));
    public double CenterPointWidth
    {
        get { return (double)GetValue(CenterPointWidthProperty); }
        set { SetValue(CenterPointWidthProperty, value); }
    }

    public static readonly DependencyProperty MinuteHandBackgroundProperty =
        DependencyProperty.Register(nameof(MinuteHandBackground), typeof(SolidColorBrush), typeof(Clock), new PropertyMetadata(default(SolidColorBrush)));
    public SolidColorBrush MinuteHandBackground
    {
        get { return (SolidColorBrush)GetValue(MinuteHandBackgroundProperty); }
        set { SetValue(MinuteHandBackgroundProperty, value); }
    }

    public static readonly DependencyProperty TitleBorderBackgroundProperty =
        DependencyProperty.Register(nameof(TitleBorderBackground), typeof(SolidColorBrush), typeof(Clock), new PropertyMetadata(default(SolidColorBrush)));
    public SolidColorBrush TitleBorderBackground
    {
        get { return (SolidColorBrush)GetValue(TitleBorderBackgroundProperty); }
        set { SetValue(TitleBorderBackgroundProperty, value); }
    }

    public static readonly DependencyProperty TitleBorderCornerRadiusProperty =
        DependencyProperty.Register(nameof(TitleBorderCornerRadius), typeof(CornerRadius), typeof(Clock), new PropertyMetadata(default(CornerRadius)));
    public CornerRadius TitleBorderCornerRadius
    {
        get { return (CornerRadius)GetValue(TitleBorderCornerRadiusProperty); }
        set { SetValue(TitleBorderCornerRadiusProperty, value); }
    }


    public static readonly DependencyProperty ClockWidthProperty =
        DependencyProperty.Register(nameof(ClockWidth), typeof(double), typeof(Clock), new PropertyMetadata(250.0));
    public double ClockWidth
    {
        get { return (double)GetValue(ClockWidthProperty); }
        set { SetValue(ClockWidthProperty, value); }
    }
    

    public static readonly DependencyProperty ClockHeightProperty =
        DependencyProperty.Register(nameof(ClockHeight), typeof(double), typeof(Clock), new PropertyMetadata(250.0));
    public double ClockHeight
    {
        get { return (double)GetValue(ClockHeightProperty); }
        set { SetValue(ClockHeightProperty, value); }
    }

    public static readonly DependencyProperty ClockBackgroundProperty =
        DependencyProperty.Register(nameof(ClockBackground), typeof(SolidColorBrush), typeof(Clock), new PropertyMetadata(default(SolidColorBrush)));
    public SolidColorBrush ClockBackground
    {
        get { return (SolidColorBrush)GetValue(ClockBackgroundProperty); }
        set { SetValue(ClockBackgroundProperty, value); }
    }
    
    public static readonly DependencyProperty OffsetAngleProperty = DependencyProperty.Register(
        nameof(OffsetAngle), typeof(double), typeof(Clock), new PropertyMetadata(0.0));
    public double OffsetAngle
    {
        get => (double)GetValue(OffsetAngleProperty);
        set => SetValue(OffsetAngleProperty, value);
    }

    
    public static readonly DependencyProperty DiameterProperty =
        DependencyProperty.Register(nameof(Diameter), typeof(double), typeof(Clock), new PropertyMetadata(170.0));
    public double Diameter
    {
        get => (double)GetValue(DiameterProperty);
        set => SetValue(DiameterProperty, value);
    }

    public static readonly DependencyProperty HeaderMarginProperty =
        DependencyProperty.Register(nameof(HeaderMargin), typeof(Thickness), typeof(Clock), new PropertyMetadata(new Thickness(4)));
    public Thickness HeaderMargin
    {
        get { return (Thickness)GetValue(HeaderMarginProperty); }
        set { SetValue(HeaderMarginProperty, value); }
    }
}
