namespace DevWinUI;

public partial class AnimationPath
{
    public TimeSpan Duration
    {
        get { return (TimeSpan)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(AnimationPath), new PropertyMetadata(TimeSpan.FromSeconds(6), OnDurationChanged));

    private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null)
            ctl.AnimateStroke(ctl._length);
    }

    public AnimationIterationBehavior RepeatBehavior
    {
        get { return (AnimationIterationBehavior)GetValue(RepeatBehaviorProperty); }
        set { SetValue(RepeatBehaviorProperty, value); }
    }

    public static readonly DependencyProperty RepeatBehaviorProperty =
        DependencyProperty.Register(nameof(RepeatBehavior), typeof(AnimationIterationBehavior), typeof(AnimationPath), new PropertyMetadata(AnimationIterationBehavior.Forever, OnRepeatBehaviorChanged));

    private static void OnRepeatBehaviorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null)
            ctl.AnimateStroke(ctl._length);
    }

    public string Data
    {
        get { return (string)GetValue(DataProperty); }
        set { SetValue(DataProperty, value); }
    }

    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register(nameof(Data), typeof(string), typeof(AnimationPath), new PropertyMetadata(null, OnDataChanged));

    private static void OnDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl.hostBorder != null)
            ctl.Init();
    }

    public double StrokeThickness
    {
        get { return (double)GetValue(StrokeThicknessProperty); }
        set { SetValue(StrokeThicknessProperty, value); }
    }

    public static readonly DependencyProperty StrokeThicknessProperty =
        DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(AnimationPath), new PropertyMetadata(10.0d, OnStrokeThicknessChanged));

    private static void OnStrokeThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null)
            ctl._shape.StrokeThickness = (float)(double)e.NewValue;
    }

    public new Brush Foreground
    {
        get { return (Brush)GetValue(ForegroundProperty); }
        set { SetValue(ForegroundProperty, value); }
    }

    public new static readonly DependencyProperty ForegroundProperty =
        DependencyProperty.Register(nameof(Foreground), typeof(Brush), typeof(AnimationPath), new PropertyMetadata(null, OnForegroundChanged));

    private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null && ctl._compositor != null)
        {
            Color color = ctl.GetColorFromBrush();
            ctl._shape.StrokeBrush = ctl._compositor.CreateColorBrush(color);
        }
    }

    public CompositionStrokeCap StrokeStartCap
    {
        get { return (CompositionStrokeCap)GetValue(StrokeStartCapProperty); }
        set { SetValue(StrokeStartCapProperty, value); }
    }

    public static readonly DependencyProperty StrokeStartCapProperty =
        DependencyProperty.Register(nameof(StrokeStartCap), typeof(CompositionStrokeCap), typeof(AnimationPath), new PropertyMetadata(CompositionStrokeCap.Flat, OnStrokeStartCapChanged));

    private static void OnStrokeStartCapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null && ctl._compositor != null)
        {
            ctl._shape.StrokeStartCap = (CompositionStrokeCap)e.NewValue;
        }
    }

    public double StrokeMiterLimit
    {
        get { return (double)GetValue(StrokeMiterLimitProperty); }
        set { SetValue(StrokeMiterLimitProperty, value); }
    }

    public static readonly DependencyProperty StrokeMiterLimitProperty =
        DependencyProperty.Register(nameof(StrokeMiterLimit), typeof(double), typeof(AnimationPath), new PropertyMetadata(1.0d, OnStrokeMiterLimitChanged));

    private static void OnStrokeMiterLimitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null && ctl._compositor != null)
        {
            ctl._shape.StrokeMiterLimit = (float)(double)e.NewValue;
        }
    }

    public CompositionStrokeCap StrokeEndCap
    {
        get { return (CompositionStrokeCap)GetValue(StrokeEndCapProperty); }
        set { SetValue(StrokeEndCapProperty, value); }
    }

    public static readonly DependencyProperty StrokeEndCapProperty =
        DependencyProperty.Register(nameof(StrokeEndCap), typeof(CompositionStrokeCap), typeof(AnimationPath), new PropertyMetadata(CompositionStrokeCap.Flat, OnStrokeEndCapChanged));

    private static void OnStrokeEndCapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null && ctl._compositor != null)
        {
            ctl._shape.StrokeEndCap = (CompositionStrokeCap)e.NewValue;
        }
    }

    public double StrokeDashOffset
    {
        get { return (double)GetValue(StrokeDashOffsetProperty); }
        set { SetValue(StrokeDashOffsetProperty, value); }
    }

    public static readonly DependencyProperty StrokeDashOffsetProperty =
        DependencyProperty.Register(nameof(StrokeDashOffset), typeof(double), typeof(AnimationPath), new PropertyMetadata(default(double), OnStrokeDashOffsetChanged));

    private static void OnStrokeDashOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null && ctl._compositor != null)
        {
            ctl._shape.StrokeDashOffset = (float)(double)e.NewValue;
        }
    }

    public CompositionStrokeCap StrokeDashCap
    {
        get { return (CompositionStrokeCap)GetValue(StrokeDashCapProperty); }
        set { SetValue(StrokeDashCapProperty, value); }
    }

    public static readonly DependencyProperty StrokeDashCapProperty =
        DependencyProperty.Register(nameof(StrokeDashCap), typeof(CompositionStrokeCap), typeof(AnimationPath), new PropertyMetadata(CompositionStrokeCap.Flat, OnStrokeDashCapChanged));

    private static void OnStrokeDashCapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null && ctl._compositor != null)
        {
            ctl._shape.StrokeDashCap = (CompositionStrokeCap)e.NewValue;
        }
    }

    public CompositionStrokeLineJoin StrokeLineJoin
    {
        get { return (CompositionStrokeLineJoin)GetValue(StrokeLineJoinProperty); }
        set { SetValue(StrokeLineJoinProperty, value); }
    }

    public static readonly DependencyProperty StrokeLineJoinProperty =
        DependencyProperty.Register(nameof(StrokeLineJoin), typeof(CompositionStrokeLineJoin), typeof(AnimationPath), new PropertyMetadata(CompositionStrokeLineJoin.Miter, OnStrokeLineJoinChanged));

    private static void OnStrokeLineJoinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimationPath ctl && ctl._shape != null && ctl._compositor != null)
        {
            ctl._shape.StrokeLineJoin = (CompositionStrokeLineJoin)e.NewValue;
        }
    }

    public bool IsPlaying
    {
        get => (bool)GetValue(IsPlayingProperty);
        set => SetValue(IsPlayingProperty, value);
    }
    public static readonly DependencyProperty IsPlayingProperty =
        DependencyProperty.Register(nameof(IsPlaying), typeof(bool), typeof(AnimationPath), new PropertyMetadata(true, OnIsPlayingChanged));

    private static void OnIsPlayingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AnimationPath)d;
        ctl.HandleAnimationState();
    }
}
