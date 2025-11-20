namespace DevWinUI;

public partial class DigitalSegment
{
    public double MatrixDotSize
    {
        get => (double)GetValue(MatrixDotSizeProperty);
        set => SetValue(MatrixDotSizeProperty, value);
    }
    public static readonly DependencyProperty MatrixDotSizeProperty =
        DependencyProperty.Register(nameof(MatrixDotSize), typeof(double), typeof(DigitalSegment), new PropertyMetadata(12.0, OnRedrawDigits));

    public double MatrixDotGap
    {
        get => (double)GetValue(MatrixDotGapProperty);
        set => SetValue(MatrixDotGapProperty, value);
    }
    public static readonly DependencyProperty MatrixDotGapProperty =
        DependencyProperty.Register(nameof(MatrixDotGap), typeof(double), typeof(DigitalSegment), new PropertyMetadata(4.0, OnRedrawDigits));

    public bool IsMatrixSquare
    {
        get => (bool)GetValue(IsMatrixSquareProperty);
        set => SetValue(IsMatrixSquareProperty, value);
    }
    public static readonly DependencyProperty IsMatrixSquareProperty =
        DependencyProperty.Register(nameof(IsMatrixSquare), typeof(bool), typeof(DigitalSegment), new PropertyMetadata(false, OnRedrawDigits));

    public TimeSpan ScrollSpeed
    {
        get { return (TimeSpan)GetValue(ScrollSpeedProperty); }
        set { SetValue(ScrollSpeedProperty, value); }
    }

    public static readonly DependencyProperty ScrollSpeedProperty =
        DependencyProperty.Register(nameof(ScrollSpeed), typeof(TimeSpan), typeof(DigitalSegment), new PropertyMetadata(TimeSpan.FromMilliseconds(300)));

    public bool IsScrolling
    {
        get => (bool)GetValue(IsScrollingProperty);
        set => SetValue(IsScrollingProperty, value);
    }

    public static readonly DependencyProperty IsScrollingProperty =
        DependencyProperty.Register(nameof(IsScrolling), typeof(bool), typeof(DigitalSegment), new PropertyMetadata(false, OnScrollingChanged));

    public DigitalSegmentScrollDirection ScrollDirection
    {
        get => (DigitalSegmentScrollDirection)GetValue(ScrollDirectionProperty);
        set => SetValue(ScrollDirectionProperty, value);
    }

    public static readonly DependencyProperty ScrollDirectionProperty =
        DependencyProperty.Register(nameof(ScrollDirection), typeof(DigitalSegmentScrollDirection), typeof(DigitalSegment), new PropertyMetadata(DigitalSegmentScrollDirection.RightToLeft));

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(DigitalSegment), new PropertyMetadata(0.0));

    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }

    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(DigitalSegment), new PropertyMetadata(-5.0, OnRedrawDigits));

    public Brush SegmentForeground
    {
        get { return (Brush)GetValue(SegmentForegroundProperty); }
        set { SetValue(SegmentForegroundProperty, value); }
    }

    public static readonly DependencyProperty SegmentForegroundProperty =
        DependencyProperty.Register(nameof(SegmentForeground), typeof(Brush), typeof(DigitalSegment), new PropertyMetadata(null, OnRedrawDigits));

    public Brush SegmentBackground
    {
        get { return (Brush)GetValue(SegmentBackgroundProperty); }
        set { SetValue(SegmentBackgroundProperty, value); }
    }

    public static readonly DependencyProperty SegmentBackgroundProperty =
        DependencyProperty.Register(nameof(SegmentBackground), typeof(Brush), typeof(DigitalSegment), new PropertyMetadata(null, OnRedrawDigits));

    public Brush ColonForeground
    {
        get { return (Brush)GetValue(ColonForegroundProperty); }
        set { SetValue(ColonForegroundProperty, value); }
    }

    public static readonly DependencyProperty ColonForegroundProperty =
        DependencyProperty.Register(nameof(ColonForeground), typeof(Brush), typeof(DigitalSegment), new PropertyMetadata(null, OnRedrawDigits));

    public Brush ColonBackground
    {
        get { return (Brush)GetValue(ColonBackgroundProperty); }
        set { SetValue(ColonBackgroundProperty, value); }
    }

    public static readonly DependencyProperty ColonBackgroundProperty =
        DependencyProperty.Register(nameof(ColonBackground), typeof(Brush), typeof(DigitalSegment), new PropertyMetadata(null, OnRedrawDigits));

    public double StrokeThickness
    {
        get { return (double)GetValue(StrokeThicknessProperty); }
        set { SetValue(StrokeThicknessProperty, value); }
    }

    public static readonly DependencyProperty StrokeThicknessProperty =
        DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(DigitalSegment), new PropertyMetadata(0.0, OnRedrawDigits));
    public Brush Stroke
    {
        get { return (Brush)GetValue(StrokeProperty); }
        set { SetValue(StrokeProperty, value); }
    }

    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(DigitalSegment), new PropertyMetadata(null, OnRedrawDigits));

    public SegmentChar Model
    {
        get { return (SegmentChar)GetValue(ModelProperty); }
        set { SetValue(ModelProperty, value); }
    }

    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register(nameof(Model), typeof(SegmentChar), typeof(DigitalSegment), new PropertyMetadata(null, OnRedrawDigits));

    public int SymbolCount
    {
        get { return (int)GetValue(SymbolCountProperty); }
        set { SetValue(SymbolCountProperty, value); }
    }

    public static readonly DependencyProperty SymbolCountProperty =
        DependencyProperty.Register(nameof(SymbolCount), typeof(int), typeof(DigitalSegment), new PropertyMetadata(0, OnRedrawDigits));

    public bool IsColonBlink
    {
        get { return (bool)GetValue(IsColonBlinkProperty); }
        set { SetValue(IsColonBlinkProperty, value); }
    }

    public static readonly DependencyProperty IsColonBlinkProperty =
        DependencyProperty.Register(nameof(IsColonBlink), typeof(bool), typeof(DigitalSegment), new PropertyMetadata(false, OnRedrawDigits));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(nameof(Text), typeof(string), typeof(DigitalSegment), new PropertyMetadata(null, OnRedrawDigits));

    private static void OnRedrawDigits(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = d as DigitalSegment;
        if (ctl != null)
        {
            ctl.UpdateText();
        }
    }
}
