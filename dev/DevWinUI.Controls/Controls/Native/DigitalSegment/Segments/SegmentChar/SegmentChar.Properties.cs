namespace DevWinUI;

public partial class SegmentChar
{
    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }

    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(SegmentChar), new PropertyMetadata(0.0));

    public Brush SegmentForeground
    {
        get { return (Brush)GetValue(SegmentForegroundProperty); }
        set { SetValue(SegmentForegroundProperty, value); }
    }

    public static readonly DependencyProperty SegmentForegroundProperty =
        DependencyProperty.Register(nameof(SegmentForeground), typeof(Brush), typeof(SegmentChar), new PropertyMetadata(null, (d, e) => ((SegmentChar)d).UpdateCharacter()));

    public Brush SegmentBackground
    {
        get { return (Brush)GetValue(SegmentBackgroundProperty); }
        set { SetValue(SegmentBackgroundProperty, value); }
    }

    public static readonly DependencyProperty SegmentBackgroundProperty =
        DependencyProperty.Register(nameof(SegmentBackground), typeof(Brush), typeof(SegmentChar), new PropertyMetadata(null, (d, e) => ((SegmentChar)d).UpdateCharacter()));


    public Brush ColonForeground
    {
        get { return (Brush)GetValue(ColonForegroundProperty); }
        set { SetValue(ColonForegroundProperty, value); }
    }

    public static readonly DependencyProperty ColonForegroundProperty =
        DependencyProperty.Register(nameof(ColonForeground), typeof(Brush), typeof(SegmentChar), new PropertyMetadata(null, (d, e) => ((SegmentChar)d).UpdateCharacter()));

    public Brush ColonBackground
    {
        get { return (Brush)GetValue(ColonBackgroundProperty); }
        set { SetValue(ColonBackgroundProperty, value); }
    }

    public static readonly DependencyProperty ColonBackgroundProperty =
        DependencyProperty.Register(nameof(ColonBackground), typeof(Brush), typeof(SegmentChar), new PropertyMetadata(null, (d, e) => ((SegmentChar)d).UpdateCharacter()));


    public double StrokeThickness
    {
        get { return (double)GetValue(StrokeThicknessProperty); }
        set { SetValue(StrokeThicknessProperty, value); }
    }

    public static readonly DependencyProperty StrokeThicknessProperty =
        DependencyProperty.Register(nameof(StrokeThickness), typeof(double), typeof(SegmentChar), new PropertyMetadata(0.0));
    
    public Brush Stroke
    {
        get { return (Brush)GetValue(StrokeProperty); }
        set { SetValue(StrokeProperty, value); }
    }

    public static readonly DependencyProperty StrokeProperty =
        DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(SegmentChar), new PropertyMetadata(null));

    public string Character
    {
        get => (string)GetValue(CharacterProperty);
        set => SetValue(CharacterProperty, value);
    }
    public static readonly DependencyProperty CharacterProperty = 
        DependencyProperty.Register(nameof(Character), typeof(string), typeof(SegmentChar),new PropertyMetadata("", (d, e) => ((SegmentChar)d).UpdateCharacter()));

    public double MatrixDotSize
    {
        get => (double)GetValue(MatrixDotSizeProperty);
        set => SetValue(MatrixDotSizeProperty, value);
    }
    public static readonly DependencyProperty MatrixDotSizeProperty =
        DependencyProperty.Register(nameof(MatrixDotSize), typeof(double), typeof(SegmentChar),new PropertyMetadata(12.0, (d, e) => ((SegmentChar)d).ReDrawMatrix()));

    public double MatrixDotGap
    {
        get => (double)GetValue(MatrixDotGapProperty);
        set => SetValue(MatrixDotGapProperty, value);
    }

    public static readonly DependencyProperty MatrixDotGapProperty =
        DependencyProperty.Register(nameof(MatrixDotGap), typeof(double), typeof(SegmentChar),new PropertyMetadata(4.0, (d, e) => ((SegmentChar)d).ReDrawMatrix()));

    public bool IsMatrixSquare
    {
        get => (bool)GetValue(IsMatrixSquareProperty);
        set => SetValue(IsMatrixSquareProperty, value);
    }
    public static readonly DependencyProperty IsMatrixSquareProperty =
        DependencyProperty.Register(nameof(IsMatrixSquare), typeof(bool), typeof(SegmentChar),new PropertyMetadata(false, (d, e) => ((SegmentChar)d).ReDrawMatrix()));
    
    public bool IsColonBlink
    {
        get { return (bool)GetValue(IsColonBlinkProperty); }
        set { SetValue(IsColonBlinkProperty, value); }
    }

    public static readonly DependencyProperty IsColonBlinkProperty =
        DependencyProperty.Register(nameof(IsColonBlink), typeof(bool), typeof(SegmentChar), new PropertyMetadata(false, OnIsBlinkChanged));

    private static void OnIsBlinkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = d as SegmentChar;
        if (ctl != null)
        {
            if ((bool)e.NewValue)
                ctl.StartColonBlink();
            else
                ctl.StopColonBlink();
        }
    }

    public bool IsPatternMakerEnabled
    {
        get { return (bool)GetValue(IsPatternMakerEnabledProperty); }
        set { SetValue(IsPatternMakerEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsPatternMakerEnabledProperty =
        DependencyProperty.Register(nameof(IsPatternMakerEnabled), typeof(bool), typeof(SegmentChar), new PropertyMetadata(false, OnIsPatternMakerEnabledChanged));

    private static void OnIsPatternMakerEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SegmentChar)d;
        if (ctl != null)
        {
            if ((bool)e.NewValue)
            {
                ctl.RegisterPointerEvents();
            }
            else
            {
                ctl.UnRegisterPointerEvent();
            }
        }
    }
}
