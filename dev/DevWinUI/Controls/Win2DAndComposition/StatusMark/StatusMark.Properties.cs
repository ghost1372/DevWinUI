namespace DevWinUI;

public sealed partial class StatusMark
{
    public StatusMarkStatus Status
    {
        get { return (StatusMarkStatus)GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(StatusMarkStatus), typeof(StatusMark), new PropertyMetadata(StatusMarkStatus.Pending, OnPropertyChanged));

    public double Progress
    {
        get { return (double)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(nameof(Progress), typeof(double), typeof(StatusMark), new PropertyMetadata(double.NaN, OnPropertyChanged));

    public double Maximum
    {
        get { return (double)GetValue(MaximumProperty); }
        set { SetValue(MaximumProperty, value); }
    }

    public static readonly DependencyProperty MaximumProperty =
        DependencyProperty.Register(nameof(Maximum), typeof(double), typeof(StatusMark), new PropertyMetadata(100.0, OnPropertyChanged));

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(StatusMark), new PropertyMetadata(string.Empty, OnPropertyChanged));

    public Color DoneColor
    {
        get { return (Color)GetValue(DoneColorProperty); }
        set { SetValue(DoneColorProperty, value); }
    }

    public static readonly DependencyProperty DoneColorProperty =
        DependencyProperty.Register(nameof(DoneColor), typeof(Color), typeof(StatusMark), new PropertyMetadata(Color.FromArgb(255, 0x22, 0xC5, 0x5E), OnPropertyChanged));

    public Color ErrorColor
    {
        get { return (Color)GetValue(ErrorColorProperty); }
        set { SetValue(ErrorColorProperty, value); }
    }

    public static readonly DependencyProperty ErrorColorProperty =
        DependencyProperty.Register(nameof(ErrorColor), typeof(Color), typeof(StatusMark), new PropertyMetadata(Color.FromArgb(255, 0xEF, 0x44, 0x44), OnPropertyChanged));

    /// <summary>
    /// Glyph size in pixels.
    /// </summary>
    public double GlyphSize
    {
        get { return (double)GetValue(GlyphSizeProperty); }
        set { SetValue(GlyphSizeProperty, value); }
    }

    public static readonly DependencyProperty GlyphSizeProperty =
        DependencyProperty.Register(nameof(GlyphSize), typeof(double), typeof(StatusMark), new PropertyMetadata(20.0, OnPropertyChanged));

    /// <summary>
    /// Stroke width in a 24-unit coordinate space.
    /// </summary>
    public double StrokeWidth
    {
        get { return (double)GetValue(StrokeWidthProperty); }
        set { SetValue(StrokeWidthProperty, value); }
    }

    public static readonly DependencyProperty StrokeWidthProperty =
        DependencyProperty.Register(nameof(StrokeWidth), typeof(double), typeof(StatusMark), new PropertyMetadata(2.0, OnPropertyChanged));

    public int Dashes
    {
        get { return (int)GetValue(DashesProperty); }
        set { SetValue(DashesProperty, value); }
    }

    public static readonly DependencyProperty DashesProperty =
        DependencyProperty.Register(nameof(Dashes), typeof(int), typeof(StatusMark), new PropertyMetadata(8, OnPropertyChanged));

    /// <summary>
    /// Milliseconds per rotation while running without progress.
    /// </summary>
    public double SpinDuration
    {
        get { return (double)GetValue(SpinDurationProperty); }
        set { SetValue(SpinDurationProperty, value); }
    }

    public static readonly DependencyProperty SpinDurationProperty =
        DependencyProperty.Register(nameof(SpinDuration), typeof(double), typeof(StatusMark), new PropertyMetadata(1100.0, OnPropertyChanged));

    /// <summary>
    /// Share of the ring covered by the spinning arc, from 0 to 1.
    /// </summary>
    public double ArcLength
    {
        get { return (double)GetValue(ArcLengthProperty); }
        set { SetValue(ArcLengthProperty, value); }
    }

    public static readonly DependencyProperty ArcLengthProperty =
        DependencyProperty.Register(nameof(ArcLength), typeof(double), typeof(StatusMark), new PropertyMetadata(0.68, OnPropertyChanged));

    /// <summary>
    /// Milliseconds taken to draw the check or cross.
    /// </summary>
    public double DrawDuration
    {
        get { return (double)GetValue(DrawDurationProperty); }
        set { SetValue(DrawDurationProperty, value); }
    }

    public static readonly DependencyProperty DrawDurationProperty =
        DependencyProperty.Register(nameof(DrawDuration), typeof(double), typeof(StatusMark), new PropertyMetadata(240.0, OnPropertyChanged));

    /// <summary>
    /// Tint opacity inside the ring when done or failed.
    /// </summary>
    public double FillOpacity
    {
        get { return (double)GetValue(FillOpacityProperty); }
        set { SetValue(FillOpacityProperty, value); }
    }

    public static readonly DependencyProperty FillOpacityProperty =
        DependencyProperty.Register(nameof(FillOpacity), typeof(double), typeof(StatusMark), new PropertyMetadata(0.06, OnPropertyChanged));

    /// <summary>
    /// Draws a strike-through over the label when done.
    /// </summary>
    public bool Strike
    {
        get { return (bool)GetValue(StrikeProperty); }
        set { SetValue(StrikeProperty, value); }
    }

    public static readonly DependencyProperty StrikeProperty =
        DependencyProperty.Register(nameof(Strike), typeof(bool), typeof(StatusMark), new PropertyMetadata(true, OnPropertyChanged));

    /// <summary>
    /// Extra delay before the strike-through, in milliseconds.
    /// </summary>
    public double StrikeDelay
    {
        get { return (double)GetValue(StrikeDelayProperty); }
        set { SetValue(StrikeDelayProperty, value); }
    }

    public static readonly DependencyProperty StrikeDelayProperty =
        DependencyProperty.Register(nameof(StrikeDelay), typeof(double), typeof(StatusMark), new PropertyMetadata(60.0, OnPropertyChanged));

    /// <summary>
    /// Replaces the spin and draw animations with a breathing ring and fades.
    /// </summary>
    public bool ReduceMotion
    {
        get { return (bool)GetValue(ReduceMotionProperty); }
        set { SetValue(ReduceMotionProperty, value); }
    }

    public static readonly DependencyProperty ReduceMotionProperty =
        DependencyProperty.Register(nameof(ReduceMotion), typeof(bool), typeof(StatusMark), new PropertyMetadata(false, OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((StatusMark)d).OnPropsChanged();
    }
}
