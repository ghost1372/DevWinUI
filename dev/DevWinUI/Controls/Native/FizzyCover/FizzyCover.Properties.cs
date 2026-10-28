namespace DevWinUI;

public sealed partial class FizzyCover
{
    public object? InvertedContent
    {
        get { return GetValue(InvertedContentProperty); }
        set { SetValue(InvertedContentProperty, value); }
    }

    public static readonly DependencyProperty InvertedContentProperty =
        DependencyProperty.Register(nameof(InvertedContent), typeof(object), typeof(FizzyCover), new PropertyMetadata(null, (d, _) => ((FizzyCover)d).UpdateMirror()));

    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register(nameof(Radius), typeof(double), typeof(FizzyCover), new PropertyMetadata(999.0, (d, _) => ((FizzyCover)d).ApplyShape()));

    public FizzyTrigger Trigger
    {
        get { return (FizzyTrigger)GetValue(TriggerProperty); }
        set { SetValue(TriggerProperty, value); }
    }

    public static readonly DependencyProperty TriggerProperty =
        DependencyProperty.Register(nameof(Trigger), typeof(FizzyTrigger), typeof(FizzyCover), new PropertyMetadata(FizzyTrigger.Hover, OnKickChanged));

    public bool Pressed
    {
        get { return (bool)GetValue(PressedProperty); }
        set { SetValue(PressedProperty, value); }
    }

    public static readonly DependencyProperty PressedProperty =
        DependencyProperty.Register(nameof(Pressed), typeof(bool), typeof(FizzyCover), new PropertyMetadata(false, OnKickChanged));

    public double Proximity
    {
        get { return (double)GetValue(ProximityProperty); }
        set { SetValue(ProximityProperty, value); }
    }

    public static readonly DependencyProperty ProximityProperty =
        DependencyProperty.Register(nameof(Proximity), typeof(double), typeof(FizzyCover), new PropertyMetadata(160.0, OnRangeChanged));

    public double WakeMargin
    {
        get { return (double)GetValue(WakeMarginProperty); }
        set { SetValue(WakeMarginProperty, value); }
    }

    public static readonly DependencyProperty WakeMarginProperty =
        DependencyProperty.Register(nameof(WakeMargin), typeof(double), typeof(FizzyCover), new PropertyMetadata(60.0, OnRangeChanged));

    public FizzyShape ParticleShape
    {
        get { return (FizzyShape)GetValue(ParticleShapeProperty); }
        set { SetValue(ParticleShapeProperty, value); }
    }

    public static readonly DependencyProperty ParticleShapeProperty =
        DependencyProperty.Register(nameof(ParticleShape), typeof(FizzyShape), typeof(FizzyCover), new PropertyMetadata(FizzyShape.Mixed, OnParticlesChanged));

    public int Count
    {
        get { return (int)GetValue(CountProperty); }
        set { SetValue(CountProperty, value); }
    }

    public static readonly DependencyProperty CountProperty =
        DependencyProperty.Register(nameof(Count), typeof(int), typeof(FizzyCover), new PropertyMetadata(80, OnParticlesChanged));

    public double Size
    {
        get { return (double)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(double), typeof(FizzyCover), new PropertyMetadata(1.5, OnParticlesChanged));

    public double Variation
    {
        get { return (double)GetValue(VariationProperty); }
        set { SetValue(VariationProperty, value); }
    }

    public static readonly DependencyProperty VariationProperty =
        DependencyProperty.Register(nameof(Variation), typeof(double), typeof(FizzyCover), new PropertyMetadata(0.65, OnParticlesChanged));

    public double Drift
    {
        get { return (double)GetValue(DriftProperty); }
        set { SetValue(DriftProperty, value); }
    }

    public static readonly DependencyProperty DriftProperty =
        DependencyProperty.Register(nameof(Drift), typeof(double), typeof(FizzyCover), new PropertyMetadata(1.0));

    public double Spin
    {
        get { return (double)GetValue(SpinProperty); }
        set { SetValue(SpinProperty, value); }
    }

    public static readonly DependencyProperty SpinProperty =
        DependencyProperty.Register(nameof(Spin), typeof(double), typeof(FizzyCover), new PropertyMetadata(1.0));

    public double Crest
    {
        get { return (double)GetValue(CrestProperty); }
        set { SetValue(CrestProperty, value); }
    }

    public static readonly DependencyProperty CrestProperty =
        DependencyProperty.Register(nameof(Crest), typeof(double), typeof(FizzyCover), new PropertyMetadata(1.0));

    public double RestAlpha
    {
        get { return (double)GetValue(RestAlphaProperty); }
        set { SetValue(RestAlphaProperty, value); }
    }

    public static readonly DependencyProperty RestAlphaProperty =
        DependencyProperty.Register(nameof(RestAlpha), typeof(double), typeof(FizzyCover), new PropertyMetadata(0.18));

    public double ActiveAlpha
    {
        get { return (double)GetValue(ActiveAlphaProperty); }
        set { SetValue(ActiveAlphaProperty, value); }
    }

    public static readonly DependencyProperty ActiveAlphaProperty =
        DependencyProperty.Register(nameof(ActiveAlpha), typeof(double), typeof(FizzyCover), new PropertyMetadata(0.58));

    public double IdleSpeed
    {
        get { return (double)GetValue(IdleSpeedProperty); }
        set { SetValue(IdleSpeedProperty, value); }
    }

    public static readonly DependencyProperty IdleSpeedProperty =
        DependencyProperty.Register(nameof(IdleSpeed), typeof(double), typeof(FizzyCover), new PropertyMetadata(0.35));

    public double ActiveSpeed
    {
        get { return (double)GetValue(ActiveSpeedProperty); }
        set { SetValue(ActiveSpeedProperty, value); }
    }

    public static readonly DependencyProperty ActiveSpeedProperty =
        DependencyProperty.Register(nameof(ActiveSpeed), typeof(double), typeof(FizzyCover), new PropertyMetadata(1.5));

    public double FillDuration
    {
        get { return (double)GetValue(FillDurationProperty); }
        set { SetValue(FillDurationProperty, value); }
    }

    public static readonly DependencyProperty FillDurationProperty =
        DependencyProperty.Register(nameof(FillDuration), typeof(double), typeof(FizzyCover), new PropertyMetadata(0.7));

    public Color InkColor
    {
        get { return (Color)GetValue(InkColorProperty); }
        set { SetValue(InkColorProperty, value); }
    }

    public static readonly DependencyProperty InkColorProperty =
        DependencyProperty.Register(nameof(InkColor), typeof(Color), typeof(FizzyCover), new PropertyMetadata(Colors.Transparent, OnColorsChanged));

    public Color InvertColor
    {
        get { return (Color)GetValue(InvertColorProperty); }
        set { SetValue(InvertColorProperty, value); }
    }

    public static readonly DependencyProperty InvertColorProperty =
        DependencyProperty.Register(nameof(InvertColor), typeof(Color), typeof(FizzyCover), new PropertyMetadata(Colors.Transparent, OnColorsChanged));

    private static void OnKickChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FizzyCover)d).Kick();

    private static void OnRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var self = (FizzyCover)d;
        self.Kick();
    }

    private static void OnParticlesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FizzyCover)d).QueueParticles();

    private static void OnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((FizzyCover)d).ApplyColors();
}
