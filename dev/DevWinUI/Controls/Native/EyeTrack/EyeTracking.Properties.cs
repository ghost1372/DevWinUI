namespace DevWinUI;

public sealed partial class EyeTracking
{
    public double EyeSize
    {
        get { return (double)GetValue(EyeSizeProperty); }
        set { SetValue(EyeSizeProperty, value); }
    }

    public static readonly DependencyProperty EyeSizeProperty =
        DependencyProperty.Register(nameof(EyeSize), typeof(double), typeof(EyeTracking), new PropertyMetadata(120d, OnChanged));

    public double Gap
    {
        get { return (double)GetValue(GapProperty); }
        set { SetValue(GapProperty, value); }
    }

    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(nameof(Gap), typeof(double), typeof(EyeTracking), new PropertyMetadata(40d, OnChanged));

    public Brush ScleraBrush
    {
        get { return (Brush)GetValue(ScleraBrushProperty); }
        set { SetValue(ScleraBrushProperty, value); }
    }

    public static readonly DependencyProperty ScleraBrushProperty =
        DependencyProperty.Register(nameof(ScleraBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush CyberScleraBrush
    {
        get { return (Brush)GetValue(CyberScleraBrushProperty); }
        set { SetValue(CyberScleraBrushProperty, value); }
    }

    public static readonly DependencyProperty CyberScleraBrushProperty =
        DependencyProperty.Register(nameof(CyberScleraBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush IrisBrush
    {
        get { return (Brush)GetValue(IrisBrushProperty); }
        set { SetValue(IrisBrushProperty, value); }
    }

    public static readonly DependencyProperty IrisBrushProperty =
        DependencyProperty.Register(nameof(IrisBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush IrisAccentBrush
    {
        get { return (Brush)GetValue(IrisAccentBrushProperty); }
        set { SetValue(IrisAccentBrushProperty, value); }
    }

    public static readonly DependencyProperty IrisAccentBrushProperty =
        DependencyProperty.Register(nameof(IrisAccentBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush PupilBrush
    {
        get { return (Brush)GetValue(PupilBrushProperty); }
        set { SetValue(PupilBrushProperty, value); }
    }

    public static readonly DependencyProperty PupilBrushProperty =
        DependencyProperty.Register(nameof(PupilBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush OutlineBrush
    {
        get { return (Brush)GetValue(OutlineBrushProperty); }
        set { SetValue(OutlineBrushProperty, value); }
    }

    public static readonly DependencyProperty OutlineBrushProperty =
        DependencyProperty.Register(nameof(OutlineBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush RimBrush
    {
        get { return (Brush)GetValue(RimBrushProperty); }
        set { SetValue(RimBrushProperty, value); }
    }

    public static readonly DependencyProperty RimBrushProperty =
        DependencyProperty.Register(nameof(RimBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush VeinBrush
    {
        get { return (Brush)GetValue(VeinBrushProperty); }
        set { SetValue(VeinBrushProperty, value); }
    }

    public static readonly DependencyProperty VeinBrushProperty =
        DependencyProperty.Register(nameof(VeinBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush HighlightBrush
    {
        get { return (Brush)GetValue(HighlightBrushProperty); }
        set { SetValue(HighlightBrushProperty, value); }
    }

    public static readonly DependencyProperty HighlightBrushProperty =
        DependencyProperty.Register(nameof(HighlightBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush ShadeBrush
    {
        get { return (Brush)GetValue(ShadeBrushProperty); }
        set { SetValue(ShadeBrushProperty, value); }
    }

    public static readonly DependencyProperty ShadeBrushProperty =
        DependencyProperty.Register(nameof(ShadeBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush UpperEyelidShadeBrush
    {
        get { return (Brush)GetValue(UpperEyelidShadeBrushProperty); }
        set { SetValue(UpperEyelidShadeBrushProperty, value); }
    }

    public static readonly DependencyProperty UpperEyelidShadeBrushProperty =
        DependencyProperty.Register(nameof(UpperEyelidShadeBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public Brush LowerEyelidShadeBrush
    {
        get { return (Brush)GetValue(LowerEyelidShadeBrushProperty); }
        set { SetValue(LowerEyelidShadeBrushProperty, value); }
    }

    public static readonly DependencyProperty LowerEyelidShadeBrushProperty =
        DependencyProperty.Register(nameof(LowerEyelidShadeBrush), typeof(Brush), typeof(EyeTracking), new PropertyMetadata(null, OnChanged));

    public double PupilRange
    {
        get { return (double)GetValue(PupilRangeProperty); }
        set { SetValue(PupilRangeProperty, value); }
    }

    public static readonly DependencyProperty PupilRangeProperty =
        DependencyProperty.Register(nameof(PupilRange), typeof(double), typeof(EyeTracking), new PropertyMetadata(0.7d, OnChanged));

    public bool ShowReflection
    {
        get { return (bool)GetValue(ShowReflectionProperty); }
        set { SetValue(ShowReflectionProperty, value); }
    }

    public static readonly DependencyProperty ShowReflectionProperty =
        DependencyProperty.Register(nameof(ShowReflection), typeof(bool), typeof(EyeTracking), new PropertyMetadata(true, OnChanged));

    public bool ShowIrisDetail
    {
        get { return (bool)GetValue(ShowIrisDetailProperty); }
        set { SetValue(ShowIrisDetailProperty, value); }
    }

    public static readonly DependencyProperty ShowIrisDetailProperty =
        DependencyProperty.Register(nameof(ShowIrisDetail), typeof(bool), typeof(EyeTracking), new PropertyMetadata(true, OnChanged));

    public bool IdleAnimation
    {
        get { return (bool)GetValue(IdleAnimationProperty); }
        set { SetValue(IdleAnimationProperty, value); }
    }

    public static readonly DependencyProperty IdleAnimationProperty =
        DependencyProperty.Register(nameof(IdleAnimation), typeof(bool), typeof(EyeTracking), new PropertyMetadata(true, OnChanged));

    public double BlinkInterval
    {
        get { return (double)GetValue(BlinkIntervalProperty); }
        set { SetValue(BlinkIntervalProperty, value); }
    }

    public static readonly DependencyProperty BlinkIntervalProperty =
        DependencyProperty.Register(nameof(BlinkInterval), typeof(double), typeof(EyeTracking), new PropertyMetadata(4000d, OnChanged));

    public int EyeCount
    {
        get { return (int)GetValue(EyeCountProperty); }
        set { SetValue(EyeCountProperty, value); }
    }

    public static readonly DependencyProperty EyeCountProperty =
        DependencyProperty.Register(nameof(EyeCount), typeof(int), typeof(EyeTracking), new PropertyMetadata(2, OnChanged));

    public EyeKind Kind
    {
        get { return (EyeKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(EyeKind), typeof(EyeTracking), new PropertyMetadata(EyeKind.Realistic, OnChanged));

    public bool ReactivePupil
    {
        get { return (bool)GetValue(ReactivePupilProperty); }
        set { SetValue(ReactivePupilProperty, value); }
    }

    public static readonly DependencyProperty ReactivePupilProperty =
        DependencyProperty.Register(nameof(ReactivePupil), typeof(bool), typeof(EyeTracking), new PropertyMetadata(true, OnChanged));

    public bool ShowEyelids
    {
        get { return (bool)GetValue(ShowEyelidsProperty); }
        set { SetValue(ShowEyelidsProperty, value); }
    }

    public static readonly DependencyProperty ShowEyelidsProperty =
        DependencyProperty.Register(nameof(ShowEyelids), typeof(bool), typeof(EyeTracking), new PropertyMetadata(true, OnChanged));

    public bool GlobalMouseTracking
    {
        get { return (bool)GetValue(GlobalMouseTrackingProperty); }
        set { SetValue(GlobalMouseTrackingProperty, value); }
    }

    public static readonly DependencyProperty GlobalMouseTrackingProperty =
        DependencyProperty.Register(nameof(GlobalMouseTracking), typeof(bool), typeof(EyeTracking), new PropertyMetadata(true, OnGlobalMouseTrackingChanged));
}
