namespace DevWinUI;

public partial class EdgeLighting
{
    public bool IsLighting
    {
        get { return (bool)GetValue(IsLightingProperty); }
        set { SetValue(IsLightingProperty, value); }
    }

    public static readonly DependencyProperty IsLightingProperty =
        DependencyProperty.Register(nameof(IsLighting), typeof(bool), typeof(EdgeLighting), new PropertyMetadata(true, OnIsLightingChanged));

    private static void OnIsLightingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateLightingState();
        }
    }

    public double StrokeThickness
    {
        get { return (double)GetValue(StrokeThicknessProperty); }
        set { SetValue(StrokeThicknessProperty, value); }
    }

    public static readonly DependencyProperty StrokeThicknessProperty =
        DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(EdgeLighting), new PropertyMetadata(10d, OnStrokeThicknessChanged));

    private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateStrokeThickness();
        }
    }

    public new double CornerRadius
    {
        get { return (double)GetValue(CornerRadiusProperty); }
        set { SetValue(CornerRadiusProperty, value); }
    }

    public static new readonly DependencyProperty CornerRadiusProperty =
        DependencyProperty.Register(nameof(CornerRadius), typeof(double), typeof(EdgeLighting), new PropertyMetadata(8.0d, OnCornerRadiusChanged));

    private static void OnCornerRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.Build();
            ctl.UpdateBorderCornerRadius();
        }
    }
    public Color BorderStrokeColor
    {
        get { return (Color)GetValue(BorderStrokeColorProperty); }
        set { SetValue(BorderStrokeColorProperty, value); }
    }

    public static readonly DependencyProperty BorderStrokeColorProperty =
        DependencyProperty.Register(nameof(BorderStrokeColor), typeof(Color), typeof(EdgeLighting), new PropertyMetadata(Windows.UI.Color.FromArgb(60, 20, 20, 20), OnBorderBackgroundChanged));

    private static void OnBorderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateBorderBackground();
        }
    }

    public Color HighlightColor
    {
        get { return (Color)GetValue(HighlightColorProperty); }
        set { SetValue(HighlightColorProperty, value); }
    }

    public static readonly DependencyProperty HighlightColorProperty =
        DependencyProperty.Register(nameof(HighlightColor), typeof(Color), typeof(EdgeLighting), new PropertyMetadata(Windows.UI.Color.FromArgb(255, 255, 255, 255), OnHighlightColorChanged));

    private static void OnHighlightColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateBrush();
        }
    }

    public double HighlightOffset
    {
        get { return (double)GetValue(HighlightOffsetProperty); }
        set { SetValue(HighlightOffsetProperty, value); }
    }

    public static readonly DependencyProperty HighlightOffsetProperty =
        DependencyProperty.Register(nameof(HighlightOffset), typeof(double), typeof(EdgeLighting), new PropertyMetadata(0.0d, OnHighlightOffsetChanged));

    private static void OnHighlightOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateBrush();
        }
    }

    public Color FadeColor
    {
        get { return (Color)GetValue(FadeColorProperty); }
        set { SetValue(FadeColorProperty, value); }
    }

    public static readonly DependencyProperty FadeColorProperty =
        DependencyProperty.Register(nameof(FadeColor), typeof(Color), typeof(EdgeLighting), new PropertyMetadata(Windows.UI.Color.FromArgb(0, 0, 120, 215), OnFadeColorChanged));

    private static void OnFadeColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateBrush();
        }
    }

    public double FadeOffset
    {
        get { return (double)GetValue(FadeOffsetProperty); }
        set { SetValue(FadeOffsetProperty, value); }
    }

    public static readonly DependencyProperty FadeOffsetProperty =
        DependencyProperty.Register(nameof(FadeOffset), typeof(double), typeof(EdgeLighting), new PropertyMetadata(0.4d, OnFadeOffsetChanged));

    private static void OnFadeOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateBrush();
        }
    }

    public Color MidColor
    {
        get { return (Color)GetValue(MidColorProperty); }
        set { SetValue(MidColorProperty, value); }
    }

    public static readonly DependencyProperty MidColorProperty =
        DependencyProperty.Register(nameof(MidColor), typeof(Color), typeof(EdgeLighting), new PropertyMetadata(Windows.UI.Color.FromArgb(255, 0, 120, 215), OnMidColorChanged));

    private static void OnMidColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateBrush();
        }
    }

    public double MidOffset
    {
        get { return (double)GetValue(MidOffsetProperty); }
        set { SetValue(MidOffsetProperty, value); }
    }

    public static readonly DependencyProperty MidOffsetProperty =
        DependencyProperty.Register(nameof(MidOffset), typeof(double), typeof(EdgeLighting), new PropertyMetadata(0.3d, OnMidOffsetChanged));
    private static void OnMidOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateBrush();
        }
    }

    public Color TailColor
    {
        get { return (Color)GetValue(TailColorProperty); }
        set { SetValue(TailColorProperty, value); }
    }

    public static readonly DependencyProperty TailColorProperty =
        DependencyProperty.Register(nameof(TailColor), typeof(Color), typeof(EdgeLighting), new PropertyMetadata(Colors.Transparent, OnTailColorChanged));

    private static void OnTailColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.UpdateBrush();
        }
    }
    public TimeSpan Duration
    {
        get { return (TimeSpan)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }


    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(EdgeLighting), new PropertyMetadata(TimeSpan.FromSeconds(4), OnDurationChanged));

    private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.Build();
        }
    }

    public int IterationCount
    {
        get { return (int)GetValue(IterationCountProperty); }
        set { SetValue(IterationCountProperty, value); }
    }

    public static readonly DependencyProperty IterationCountProperty =
        DependencyProperty.Register(nameof(IterationCount), typeof(int), typeof(EdgeLighting), new PropertyMetadata(1, OnIterationCountChanged));

    private static void OnIterationCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.Build();
        }
    }

    public AnimationIterationBehavior IterationBehavior
    {
        get { return (AnimationIterationBehavior)GetValue(IterationBehaviorProperty); }
        set { SetValue(IterationBehaviorProperty, value); }
    }

    public static readonly DependencyProperty IterationBehaviorProperty =
        DependencyProperty.Register(nameof(IterationBehavior), typeof(AnimationIterationBehavior), typeof(EdgeLighting), new PropertyMetadata(AnimationIterationBehavior.Forever, OnIterationBehaviorChanged));
    private static void OnIterationBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.Build();
        }
    }
    public bool IsRounded
    {
        get { return (bool)GetValue(IsRoundedProperty); }
        set { SetValue(IsRoundedProperty, value); }
    }

    public static readonly DependencyProperty IsRoundedProperty =
        DependencyProperty.Register(nameof(IsRounded), typeof(bool), typeof(EdgeLighting), new PropertyMetadata(false, IsRoundedChanged));

    private static void IsRoundedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (EdgeLighting)d;
        if (ctl != null)
        {
            ctl.Build();
            ctl.UpdateBorderCornerRadius();
        }
    }
}
