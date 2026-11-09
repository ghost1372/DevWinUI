namespace DevWinUI;

public sealed partial class BorderBeam
{
    public double Size
    {
        get { return (double)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(double), typeof(BorderBeam), new PropertyMetadata(200.0, OnPaintChanged));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(BorderBeam), new PropertyMetadata(15.0, OnTimingChanged));

    public double Delay
    {
        get { return (double)GetValue(DelayProperty); }
        set { SetValue(DelayProperty, value); }
    }

    public static readonly DependencyProperty DelayProperty =
        DependencyProperty.Register(nameof(Delay), typeof(double), typeof(BorderBeam), new PropertyMetadata(0.0, OnTimingChanged));

    public double BorderWidth
    {
        get { return (double)GetValue(BorderWidthProperty); }
        set { SetValue(BorderWidthProperty, value); }
    }

    public static readonly DependencyProperty BorderWidthProperty =
        DependencyProperty.Register(nameof(BorderWidth), typeof(double), typeof(BorderBeam), new PropertyMetadata(1.5, OnGeometryChanged));

    public Color ColorFrom
    {
        get { return (Color)GetValue(ColorFromProperty); }
        set { SetValue(ColorFromProperty, value); }
    }

    public static readonly DependencyProperty ColorFromProperty =
        DependencyProperty.Register(nameof(ColorFrom), typeof(Color), typeof(BorderBeam), new PropertyMetadata(Color.FromArgb(255, 0xFF, 0xAA, 0x40), OnPaintChanged));

    public Color ColorTo
    {
        get { return (Color)GetValue(ColorToProperty); }
        set { SetValue(ColorToProperty, value); }
    }

    public static readonly DependencyProperty ColorToProperty =
        DependencyProperty.Register(nameof(ColorTo), typeof(Color), typeof(BorderBeam), new PropertyMetadata(Color.FromArgb(255, 0x9C, 0x40, 0xFF), OnPaintChanged));

    private static void OnPaintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BorderBeam)d).PublishStyle(false);

    private static void OnTimingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BorderBeam)d).PublishStyle(true);

    private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BorderBeam)d).RebuildPath();
}
