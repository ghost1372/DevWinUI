namespace DevWinUI;

public partial class HaloArc : Microsoft.UI.Xaml.Shapes.Path
{
    private PathFigure figure = new PathFigure();
    private ArcSegment segment = new ArcSegment();
    private PathGeometry path = new PathGeometry();

    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }
    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(HaloArc), new PropertyMetadata(0.0, Refresh));

    public double Spread
    {
        get { return (double)GetValue(SpreadProperty); }
        set { SetValue(SpreadProperty, value); }
    }
    public static readonly DependencyProperty SpreadProperty =
        DependencyProperty.Register(nameof(Spread), typeof(double), typeof(HaloArc), new PropertyMetadata(90.0, Refresh));

    public double Offset
    {
        get { return (double)GetValue(OffsetProperty); }
        set { SetValue(OffsetProperty, value); }
    }
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register(nameof(Offset), typeof(double), typeof(HaloArc), new PropertyMetadata(0.0, Refresh));

    public double Tension
    {
        get { return (double)GetValue(TensionProperty); }
        set { SetValue(TensionProperty, value); }
    }
    public static readonly DependencyProperty TensionProperty =
        DependencyProperty.Register(nameof(Tension), typeof(double), typeof(HaloArc), new PropertyMetadata(0.0, Refresh));

    public HaloArc()
    {
        segment.SweepDirection = SweepDirection.Clockwise;
        figure.Segments = new PathSegmentCollection { segment };
        path.Figures = new PathFigureCollection { figure };

        Data = path;

        BindingOperations.SetBinding(this, Halo.ThicknessProperty,
            new Binding
            {
                Source = this, Mode = BindingMode.TwoWay,
                Path = new PropertyPath("StrokeThickness"),
            });
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var circle = new HaloCircleHelper(finalSize);
        circle.Radius -= StrokeThickness / 2;

        ArrangePath(circle);
        return finalSize;
    }

    private static void Refresh(object o, DependencyPropertyChangedEventArgs e)
    {
        ((HaloArc)o).InvalidateMeasure();
        ((HaloArc)o).UpdateLayout();
    }

    private void ArrangePath(HaloCircleHelper circle)
    {
        var tension = Tension % 1;
        var angle = Angle + Offset;

        var startAngle = angle - tension * Spread;
        var endAngle = angle + (1 - tension) * Spread;

        figure.StartPoint = circle.PointAt(startAngle);
        segment.Point = circle.PointAt(endAngle);

        segment.Size = circle.Size();
        segment.IsLargeArc = (Spread > 180);
    }
}
