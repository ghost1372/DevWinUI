namespace DevWinUI;

public partial class HaloSlice : Microsoft.UI.Xaml.Shapes.Path
{
    private PathGeometry path = new PathGeometry();

    private LineSegment sliceStart = new LineSegment();
    private LineSegment sliceEnd = new LineSegment();

    private ArcSegment arcSegment = new ArcSegment();
    private PathFigure arcFigure = new PathFigure();

    public double Angle
    {
        get { return (double)GetValue(AngleProperty); }
        set { SetValue(AngleProperty, value); }
    }
    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.Register(nameof(Angle), typeof(double), typeof(HaloSlice), new PropertyMetadata(0.0, Refresh));

    public double Offset
    {
        get { return (double)GetValue(OffsetProperty); }
        set { SetValue(OffsetProperty, value); }
    }
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register(nameof(Offset), typeof(double), typeof(HaloSlice), new PropertyMetadata(0.0, Refresh));

    public double Spread
    {
        get { return (double)GetValue(SpreadProperty); }
        set { SetValue(SpreadProperty, value); }
    }
    public static readonly DependencyProperty SpreadProperty =
        DependencyProperty.Register(nameof(Spread), typeof(double), typeof(HaloSlice), new PropertyMetadata(360.0, Refresh));

    public HaloSlice()
    {
        arcSegment.SweepDirection = SweepDirection.Clockwise;

        arcFigure.Segments = new PathSegmentCollection
        {
            sliceStart, arcSegment, sliceEnd
        };

        path.Figures = new PathFigureCollection { arcFigure };

        Data = path;
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
        ((HaloSlice)o).InvalidateMeasure();
        ((HaloSlice)o).UpdateLayout();
    }

    private void ArrangePath(HaloCircleHelper circle)
    {
        arcFigure.StartPoint = circle.Center;

        sliceStart.Point = circle.PointAt(Angle + Offset);
        sliceEnd.Point = circle.Center;

        arcSegment.Point = circle.PointAt(Angle + Offset + Spread);

        arcSegment.Size = circle.Size();
        arcSegment.IsLargeArc = (Spread > 180);
    }
}
