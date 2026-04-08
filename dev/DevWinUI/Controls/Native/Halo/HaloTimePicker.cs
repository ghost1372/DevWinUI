namespace DevWinUI;

public partial class HaloTimePicker : Control
{
    private const string PART_Hours = "PART_Hours";
    private const string PART_Minutes = "PART_Minutes";
    private const string PART_Display = "PART_Display";

    private HaloArcSlider hours;
    private HaloArcSlider minutes;
    private TextBlock display;

    public event EventHandler TimeChanged;

    public Brush MinutesArcBrush
    {
        get { return (Brush)GetValue(MinutesArcBrushProperty); }
        set { SetValue(MinutesArcBrushProperty, value); }
    }

    public static readonly DependencyProperty MinutesArcBrushProperty =
        DependencyProperty.Register(nameof(MinutesArcBrush), typeof(Brush), typeof(HaloTimePicker), new PropertyMetadata(null));

    public Brush HoursArcBrush
    {
        get { return (Brush)GetValue(HoursArcBrushProperty); }
        set { SetValue(HoursArcBrushProperty, value); }
    }

    public static readonly DependencyProperty HoursArcBrushProperty =
        DependencyProperty.Register(nameof(HoursArcBrush), typeof(Brush), typeof(HaloTimePicker), new PropertyMetadata(null));

    public int MinutesBand
    {
        get { return (int)GetValue(MinutesBandProperty); }
        set { SetValue(MinutesBandProperty, value); }
    }

    public static readonly DependencyProperty MinutesBandProperty =
        DependencyProperty.Register(nameof(MinutesBand), typeof(int), typeof(HaloTimePicker), new PropertyMetadata(2));

    public int HoursBand
    {
        get { return (int)GetValue(HoursBandProperty); }
        set { SetValue(HoursBandProperty, value); }
    }

    public static readonly DependencyProperty HoursBandProperty =
        DependencyProperty.Register(nameof(HoursBand), typeof(int), typeof(HaloTimePicker), new PropertyMetadata(1));

    public Brush MinutesBrush
    {
        get { return (Brush)GetValue(MinutesBrushProperty); }
        set { SetValue(MinutesBrushProperty, value); }
    }

    public static readonly DependencyProperty MinutesBrushProperty =
        DependencyProperty.Register(nameof(MinutesBrush), typeof(Brush), typeof(HaloTimePicker), new PropertyMetadata(null));

    public Brush HoursBrush
    {
        get { return (Brush)GetValue(HoursBrushProperty); }
        set { SetValue(HoursBrushProperty, value); }
    }

    public static readonly DependencyProperty HoursBrushProperty =
        DependencyProperty.Register(nameof(HoursBrush), typeof(Brush), typeof(HaloTimePicker), new PropertyMetadata(null));

    public TimeSpan Time
    {
        get { return (TimeSpan)GetValue(TimeProperty); }
        set { SetValue(TimeProperty, value); }
    }
    public static readonly DependencyProperty TimeProperty =
        DependencyProperty.Register(nameof(Time), typeof(TimeSpan), typeof(HaloTimePicker), new PropertyMetadata(new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second), OnTimeChanged));

    private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HaloTimePicker)d;
        if (ctl != null)
        {
            ctl.TimeChanged?.Invoke(ctl, EventArgs.Empty);
        }
    }

    public HaloTimePicker()
    {
        DefaultStyleKey = typeof(HaloTimePicker);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        hours = GetTemplateChild(PART_Hours) as HaloArcSlider;
        minutes = GetTemplateChild(PART_Minutes) as HaloArcSlider;
        display = GetTemplateChild(PART_Display) as TextBlock;

        BindingOperations.SetBinding(hours, HaloArcSlider.AngleProperty, new Binding
        {
            Source = this, Path = new PropertyPath("Time"),
            Converter = new TimeHoursConverter(this), Mode = BindingMode.TwoWay
        });

        BindingOperations.SetBinding(minutes, HaloArcSlider.AngleProperty, new Binding
        {
            Source = this, Path = new PropertyPath("Time"),
            Converter = new TimeMinutesConverter(this), Mode = BindingMode.TwoWay
        });

        BindingOperations.SetBinding(display, TextBlock.TextProperty, new Binding
        {
            Source = this, Path = new PropertyPath("Time"),
            Converter = new TimeDisplayConverter()
        });
    }
}
