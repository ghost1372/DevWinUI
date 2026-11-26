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

    public BorderBeamMode Mode
    {
        get { return (BorderBeamMode)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(BorderBeamMode), typeof(BorderBeam), new PropertyMetadata(BorderBeamMode.Chase, OnTimingChanged));

    public BorderBeamOrigin MirrorOrigin
    {
        get { return (BorderBeamOrigin)GetValue(MirrorOriginProperty); }
        set { SetValue(MirrorOriginProperty, value); }
    }

    public static readonly DependencyProperty MirrorOriginProperty =
        DependencyProperty.Register(nameof(MirrorOrigin), typeof(BorderBeamOrigin), typeof(BorderBeam), new PropertyMetadata(BorderBeamOrigin.Top, OnGeometryChanged));

    public BorderBeamEdge MirrorEdge
    {
        get { return (BorderBeamEdge)GetValue(MirrorEdgeProperty); }
        set { SetValue(MirrorEdgeProperty, value); }
    }

    public static readonly DependencyProperty MirrorEdgeProperty =
        DependencyProperty.Register(nameof(MirrorEdge), typeof(BorderBeamEdge), typeof(BorderBeam), new PropertyMetadata(BorderBeamEdge.Center, OnGeometryChanged));

    public BorderBeamStart MirrorStart
    {
        get { return (BorderBeamStart)GetValue(MirrorStartProperty); }
        set { SetValue(MirrorStartProperty, value); }
    }

    public static readonly DependencyProperty MirrorStartProperty =
        DependencyProperty.Register(nameof(MirrorStart), typeof(BorderBeamStart), typeof(BorderBeam), new PropertyMetadata(BorderBeamStart.Both, OnTimingChanged));

    public double MirrorLag
    {
        get { return (double)GetValue(MirrorLagProperty); }
        set { SetValue(MirrorLagProperty, value); }
    }

    public static readonly DependencyProperty MirrorLagProperty =
        DependencyProperty.Register(nameof(MirrorLag), typeof(double), typeof(BorderBeam), new PropertyMetadata(0.3, OnPaintChanged));

    public BorderBeamOrigin ChaseOrigin
    {
        get { return (BorderBeamOrigin)GetValue(ChaseOriginProperty); }
        set { SetValue(ChaseOriginProperty, value); }
    }

    public static readonly DependencyProperty ChaseOriginProperty =
        DependencyProperty.Register(nameof(ChaseOrigin), typeof(BorderBeamOrigin), typeof(BorderBeam), new PropertyMetadata(BorderBeamOrigin.Top, OnGeometryChanged));

    public BorderBeamEdge ChaseEdge
    {
        get { return (BorderBeamEdge)GetValue(ChaseEdgeProperty); }
        set { SetValue(ChaseEdgeProperty, value); }
    }

    public static readonly DependencyProperty ChaseEdgeProperty =
        DependencyProperty.Register(nameof(ChaseEdge), typeof(BorderBeamEdge), typeof(BorderBeam), new PropertyMetadata(BorderBeamEdge.LeftEdge, OnGeometryChanged));

    public BorderBeamDirection ChaseDirection
    {
        get { return (BorderBeamDirection)GetValue(ChaseDirectionProperty); }
        set { SetValue(ChaseDirectionProperty, value); }
    }

    public static readonly DependencyProperty ChaseDirectionProperty =
        DependencyProperty.Register(nameof(ChaseDirection), typeof(BorderBeamDirection), typeof(BorderBeam), new PropertyMetadata(BorderBeamDirection.Clockwise, OnPaintChanged));

    public int BeamCount
    {
        get { return (int)GetValue(BeamCountProperty); }
        set { SetValue(BeamCountProperty, value); }
    }

    public static readonly DependencyProperty BeamCountProperty =
        DependencyProperty.Register(nameof(BeamCount), typeof(int), typeof(BorderBeam), new PropertyMetadata(1, OnPaintChanged));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(BorderBeam), new PropertyMetadata(15.0, OnTimingChanged));

    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(BorderBeam), new PropertyMetadata(0.0, OnTimingChanged));

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

    public bool IsActive
    {
        get { return (bool)GetValue(IsActiveProperty); }
        set { SetValue(IsActiveProperty, value); }
    }

    public static readonly DependencyProperty IsActiveProperty =
        DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(BorderBeam), new PropertyMetadata(true, OnActiveChanged));

    public bool FadeOut
    {
        get { return (bool)GetValue(FadeOutProperty); }
        set { SetValue(FadeOutProperty, value); }
    }

    public static readonly DependencyProperty FadeOutProperty =
        DependencyProperty.Register(nameof(FadeOut), typeof(bool), typeof(BorderBeam), new PropertyMetadata(true, OnPaintChanged));

    public int CycleCount
    {
        get { return (int)GetValue(CycleCountProperty); }
        set { SetValue(CycleCountProperty, value); }
    }

    public static readonly DependencyProperty CycleCountProperty =
        DependencyProperty.Register(nameof(CycleCount), typeof(int), typeof(BorderBeam), new PropertyMetadata(0, OnPaintChanged));

    public TimeSpan FadeOutDuration
    {
        get { return (TimeSpan)GetValue(FadeOutDurationProperty); }
        set { SetValue(FadeOutDurationProperty, value); }
    }

    public static readonly DependencyProperty FadeOutDurationProperty =
        DependencyProperty.Register(nameof(FadeOutDuration), typeof(TimeSpan), typeof(BorderBeam), new PropertyMetadata(TimeSpan.FromSeconds(0.5), OnPaintChanged));

    private static void OnActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BorderBeam)d).PublishStyle((bool)e.NewValue);

    private static void OnPaintChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BorderBeam)d).PublishStyle(false);

    private static void OnTimingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BorderBeam)d).PublishStyle(true);

    private static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((BorderBeam)d).RebuildPath();
}
