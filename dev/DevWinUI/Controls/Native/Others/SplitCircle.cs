namespace DevWinUI;

[TemplatePart(Name = nameof(PART_FirstFigure), Type = typeof(PathFigure))]
[TemplatePart(Name = nameof(PART_FirstArc), Type = typeof(ArcSegment))]
[TemplatePart(Name = nameof(PART_SecondFigure), Type = typeof(PathFigure))]
[TemplatePart(Name = nameof(PART_SecondArc), Type = typeof(ArcSegment))]
public sealed partial class SplitCircle : Control
{
    private const string PART_FirstFigure = "PART_FirstFigure";
    private const string PART_FirstArc = "PART_FirstArc";
    private const string PART_SecondFigure = "PART_SecondFigure";
    private const string PART_SecondArc = "PART_SecondArc";

    private PathFigure firstFigure;
    private PathFigure secondFigure;
    private ArcSegment firstArc;
    private ArcSegment secondArc;

    public bool ShowMicaLayer
    {
        get => (bool)GetValue(ShowMicaLayerProperty);
        set => SetValue(ShowMicaLayerProperty, value);
    }

    public static readonly DependencyProperty ShowMicaLayerProperty =
        DependencyProperty.Register(nameof(ShowMicaLayer), typeof(bool), typeof(SplitCircle), new PropertyMetadata(true));
    public Color FirstColor
    {
        get => (Color)GetValue(FirstColorProperty);
        set => SetValue(FirstColorProperty, value);
    }

    public static readonly DependencyProperty FirstColorProperty =
        DependencyProperty.Register(nameof(FirstColor), typeof(Color), typeof(SplitCircle), new PropertyMetadata(null, OnFirstColorChanged));
    private static void OnFirstColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SplitCircle)d;
        if (ctl != null)
        {
            ctl.FirstColorBrush = new SolidColorBrush((Color)e.NewValue);
        }
    }
    internal Brush FirstColorBrush
    {
        get { return (Brush)GetValue(FirstColorBrushProperty); }
        set { SetValue(FirstColorBrushProperty, value); }
    }

    internal static readonly DependencyProperty FirstColorBrushProperty =
        DependencyProperty.Register(nameof(FirstColorBrush), typeof(Brush), typeof(SplitCircle), new PropertyMetadata(null));

    public Color SecondColor
    {
        get => (Color)GetValue(SecondColorProperty);
        set => SetValue(SecondColorProperty, value);
    }

    public static readonly DependencyProperty SecondColorProperty =
        DependencyProperty.Register(nameof(SecondColor), typeof(Color), typeof(SplitCircle), new PropertyMetadata(null, OnSecondColorChanged));

    private static void OnSecondColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SplitCircle)d;
        if (ctl != null)
        {
            ctl.SecondColorBrush = new SolidColorBrush((Color)e.NewValue);
        }
    }

    internal Brush SecondColorBrush
    {
        get { return (Brush)GetValue(SecondColorBrushProperty); }
        set { SetValue(SecondColorBrushProperty, value); }
    }

    internal static readonly DependencyProperty SecondColorBrushProperty =
        DependencyProperty.Register(nameof(SecondColorBrush), typeof(Brush), typeof(SplitCircle), new PropertyMetadata(null));


    public Orientation SplitOrientation
    {
        get => (Orientation)GetValue(SplitOrientationProperty);
        set => SetValue(SplitOrientationProperty, value);
    }

    public static readonly DependencyProperty SplitOrientationProperty =
        DependencyProperty.Register(nameof(SplitOrientation), typeof(Orientation), typeof(SplitCircle), new PropertyMetadata(Orientation.Horizontal, OnSplitOrientationChanged));

    private static void OnSplitOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SplitCircle)d;
        if (ctl != null)
        {
            ctl.UpdateSplitOrientation();
        }
    }

    public SplitCircle()
    {
        this.DefaultStyleKey = typeof(SplitCircle);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        firstFigure = GetTemplateChild(PART_FirstFigure) as PathFigure;
        firstArc = GetTemplateChild(PART_FirstArc) as ArcSegment;

        secondFigure = GetTemplateChild(PART_SecondFigure) as PathFigure;
        secondArc = GetTemplateChild(PART_SecondArc) as ArcSegment;

        UpdateSplitOrientation();
    }

    private void UpdateSplitOrientation()
    {
        if (firstFigure == null || firstArc == null || secondFigure == null || secondArc == null)
            return;

        if (SplitOrientation == Orientation.Vertical)
        {
            // Horizontal Split (Left/Right)
            firstFigure.StartPoint = new Point(50, 0);
            firstArc.Point = new Point(50, 100);
            firstArc.SweepDirection = SweepDirection.Clockwise;

            secondFigure.StartPoint = new Point(50, 0);
            secondArc.Point = new Point(50, 100);
            secondArc.SweepDirection = SweepDirection.Counterclockwise;
        }
        else
        {
            // Vertical Split (Top/Bottom)
            firstFigure.StartPoint = new Point(0, 50);
            firstArc.Point = new Point(100, 50);
            firstArc.SweepDirection = SweepDirection.Clockwise;

            secondFigure.StartPoint = new Point(0, 50);
            secondArc.Point = new Point(100, 50);
            secondArc.SweepDirection = SweepDirection.Counterclockwise;
        }
    }
}
