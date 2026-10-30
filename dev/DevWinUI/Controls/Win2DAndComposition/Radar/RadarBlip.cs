namespace DevWinUI;

public sealed partial class RadarBlip : DependencyObject
{
    internal double Origin;
    internal bool Reset = true;
    internal event Action? Changed;

    public void Restart()
    {
        Reset = true;
        Changed?.Invoke();
    }

    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }

    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(RadarBlip), new PropertyMetadata(0.0, OnMotionChanged));

    public double Distance
    {
        get { return (double)GetValue(DistanceProperty); }
        set { SetValue(DistanceProperty, value); }
    }

    public static readonly DependencyProperty DistanceProperty =
        DependencyProperty.Register(nameof(Distance), typeof(double), typeof(RadarBlip), new PropertyMetadata(0.5, OnMotionChanged));

    public RadarMotion Motion
    {
        get { return (RadarMotion)GetValue(MotionProperty); }
        set { SetValue(MotionProperty, value); }
    }

    public static readonly DependencyProperty MotionProperty =
        DependencyProperty.Register(nameof(Motion), typeof(RadarMotion), typeof(RadarBlip), new PropertyMetadata(RadarMotion.None, OnMotionChanged));

    public double EndAngle
    {
        get { return (double)GetValue(EndAngleProperty); }
        set { SetValue(EndAngleProperty, value); }
    }

    public static readonly DependencyProperty EndAngleProperty =
        DependencyProperty.Register(nameof(EndAngle), typeof(double), typeof(RadarBlip), new PropertyMetadata(double.NaN, OnMotionChanged));

    public double EndDistance
    {
        get { return (double)GetValue(EndDistanceProperty); }
        set { SetValue(EndDistanceProperty, value); }
    }

    public static readonly DependencyProperty EndDistanceProperty =
        DependencyProperty.Register(nameof(EndDistance), typeof(double), typeof(RadarBlip), new PropertyMetadata(double.NaN, OnMotionChanged));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(RadarBlip), new PropertyMetadata(10.0, OnMotionChanged));

    public RadarRepeat Repeat
    {
        get { return (RadarRepeat)GetValue(RepeatProperty); }
        set { SetValue(RepeatProperty, value); }
    }

    public static readonly DependencyProperty RepeatProperty =
        DependencyProperty.Register(nameof(Repeat), typeof(RadarRepeat), typeof(RadarBlip), new PropertyMetadata(RadarRepeat.Loop, OnMotionChanged));

    public double Heading
    {
        get { return (double)GetValue(HeadingProperty); }
        set { SetValue(HeadingProperty, value); }
    }

    public static readonly DependencyProperty HeadingProperty =
        DependencyProperty.Register(nameof(Heading), typeof(double), typeof(RadarBlip), new PropertyMetadata(double.NaN, OnVisualChanged));

    public RadarVisibility Visibility
    {
        get { return (RadarVisibility)GetValue(VisibilityProperty); }
        set { SetValue(VisibilityProperty, value); }
    }

    public static readonly DependencyProperty VisibilityProperty =
        DependencyProperty.Register(nameof(Visibility), typeof(RadarVisibility), typeof(RadarBlip), new PropertyMetadata(RadarVisibility.Inherit, OnVisualChanged));

    public string? Label
    {
        get { return (string?)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(RadarBlip), new PropertyMetadata(null, OnVisualChanged));

    public RadarBlipState State
    {
        get { return (RadarBlipState)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(RadarBlipState), typeof(RadarBlip), new PropertyMetadata(RadarBlipState.Normal, OnVisualChanged));

    public double TrailLength
    {
        get { return (double)GetValue(TrailLengthProperty); }
        set { SetValue(TrailLengthProperty, value); }
    }

    public static readonly DependencyProperty TrailLengthProperty =
        DependencyProperty.Register(nameof(TrailLength), typeof(double), typeof(RadarBlip), new PropertyMetadata(0.0, OnVisualChanged));

    public RadarMarker Marker
    {
        get { return (RadarMarker)GetValue(MarkerProperty); }
        set { SetValue(MarkerProperty, value); }
    }

    public static readonly DependencyProperty MarkerProperty =
        DependencyProperty.Register(nameof(Marker), typeof(RadarMarker), typeof(RadarBlip), new PropertyMetadata(RadarMarker.Dot, OnVisualChanged));

    public double Size
    {
        get { return (double)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(double), typeof(RadarBlip), new PropertyMetadata(3.0, OnVisualChanged));

    public Brush? Foreground
    {
        get { return (Brush?)GetValue(ForegroundProperty); }
        set { SetValue(ForegroundProperty, value); }
    }

    public static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(RadarBlip), new PropertyMetadata(null, OnVisualChanged));

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((RadarBlip)d).Changed?.Invoke();
    }

    private static void OnMotionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var blip = (RadarBlip)d;
        blip.Reset = true;
        blip.Changed?.Invoke();
    }
}
