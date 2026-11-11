using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

public sealed partial class Diagram
{
    private const int BackgroundBudget = 30000;
    private const double MinCell = 10;

    private double _bgPeriod;

    private void BuildBackground()
    {
        if (_back == null) return;

        _back.Children.Clear();
        _bgPeriod = 0;

        var mode = BackgroundPattern;
        double w = ActualWidth, h = ActualHeight;
        double spacing = Math.Max(4, BackgroundSpacing);
        double cell = BackgroundScalesWithZoom ? spacing * _zoom : spacing;
        int major = MajorEvery > 1 ? MajorEvery : 0;

        if (mode == DiagramBackground.None || w <= 0 || h <= 0) return;

        bool minor = cell >= MinCell;
        if (!minor && (major == 0 || cell * major < MinCell)) return;

        int stride = minor ? 1 : major;
        double period = cell * Math.Max(1, major);
        _bgPeriod = period;

        int nx = (int)Math.Ceiling((w + period) / cell) + 1;
        int ny = (int)Math.Ceiling((h + period) / cell) + 1;

        double size = double.IsNaN(PatternSize)
            ? mode switch { DiagramBackground.Dots => 2.4, DiagramBackground.Cross => 8, DiagramBackground.Diamonds => 5, _ => 0 }
            : Math.Max(0.5, PatternSize);
        double thickness = Math.Max(0.25, PatternThickness);
        bool filled = mode is DiagramBackground.Dots or DiagramBackground.Diamonds;
        double opacity = double.IsNaN(PatternOpacity)
            ? mode is DiagramBackground.Grid or DiagramBackground.Lines ? 0.35 : 0.7
            : Math.Clamp(PatternOpacity, 0, 1);

        var minorGroup = new GeometryGroup();
        var majorGroup = new GeometryGroup();
        int budget = BackgroundBudget;

        bool IsMajor(int k) => major > 0 && k % major == 0;

        if (mode is DiagramBackground.Grid or DiagramBackground.Lines)
        {
            if (mode == DiagramBackground.Grid)
            {
                for (int i = 0; i <= nx && budget-- > 0; i += stride)
                {
                    double x = i * cell;
                    (IsMajor(i) ? majorGroup : minorGroup).Children.Add(
                        new LineGeometry { StartPoint = new Point(x, 0), EndPoint = new Point(x, h + period) });
                }
            }

            for (int j = 0; j <= ny && budget-- > 0; j += stride)
            {
                double y = j * cell;
                (IsMajor(j) ? majorGroup : minorGroup).Children.Add(
                    new LineGeometry { StartPoint = new Point(0, y), EndPoint = new Point(w + period, y) });
            }
        }
        else
        {
            for (int i = 0; i <= nx && budget > 0; i += stride)
            {
                for (int j = 0; j <= ny && budget-- > 0; j += stride)
                {
                    bool isMajor = IsMajor(i) && IsMajor(j);
                    AddMark(isMajor ? majorGroup : minorGroup, mode, i * cell, j * cell, isMajor ? size * 1.6 : size);
                }
            }
        }

        double majorOpacity = major > 0 ? Math.Min(1, opacity * 1.6) : opacity;
        AddBackgroundPath(minorGroup, filled, thickness, opacity);
        AddBackgroundPath(majorGroup, filled, major > 0 && !filled ? thickness * 1.6 : thickness, majorOpacity);

        UpdateBackgroundOffset();
    }

    private static void AddMark(GeometryGroup group, DiagramBackground mode, double x, double y, double size)
    {
        double r = size / 2;

        switch (mode)
        {
            case DiagramBackground.Cross:
                group.Children.Add(new LineGeometry { StartPoint = new Point(x - r, y), EndPoint = new Point(x + r, y) });
                group.Children.Add(new LineGeometry { StartPoint = new Point(x, y - r), EndPoint = new Point(x, y + r) });
                break;

            case DiagramBackground.Diamonds:
                {
                    var fig = new PathFigure { StartPoint = new Point(x, y - r), IsClosed = true, IsFilled = true };
                    fig.Segments.Add(new LineSegment { Point = new Point(x + r, y) });
                    fig.Segments.Add(new LineSegment { Point = new Point(x, y + r) });
                    fig.Segments.Add(new LineSegment { Point = new Point(x - r, y) });
                    var geometry = new PathGeometry();
                    geometry.Figures.Add(fig);
                    group.Children.Add(geometry);
                    break;
                }

            default:
                group.Children.Add(new EllipseGeometry { Center = new Point(x, y), RadiusX = r, RadiusY = r });
                break;
        }
    }

    private void AddBackgroundPath(GeometryGroup group, bool filled, double thickness, double opacity)
    {
        if (group.Children.Count == 0) return;

        var path = new Path { Data = group, Opacity = opacity, IsHitTestVisible = false };
        if (filled)
        {
            path.Fill = PatternBrush;
        }
        else
        {
            path.Stroke = PatternBrush;
            path.StrokeThickness = thickness;
        }

        _back!.Children.Add(path);
    }

    private void UpdateBackgroundOffset()
    {
        double p = _bgPeriod;
        if (p <= 0) return;

        _bgXf.X = ((_panX % p) + p) % p - p;
        _bgXf.Y = ((_panY % p) + p) % p - p;
    }

    public double BackgroundSpacing
    {
        get { return (double)GetValue(BackgroundSpacingProperty); }
        set { SetValue(BackgroundSpacingProperty, value); }
    }

    public static readonly DependencyProperty BackgroundSpacingProperty =
        DependencyProperty.Register(nameof(BackgroundSpacing), typeof(double), typeof(Diagram),
            new PropertyMetadata(24d, (d, e) => ((Diagram)d).BuildBackground()));

    public double PatternSize
    {
        get { return (double)GetValue(PatternSizeProperty); }
        set { SetValue(PatternSizeProperty, value); }
    }

    public static readonly DependencyProperty PatternSizeProperty =
        DependencyProperty.Register(nameof(PatternSize), typeof(double), typeof(Diagram),
            new PropertyMetadata(double.NaN, (d, e) => ((Diagram)d).BuildBackground()));

    public double PatternThickness
    {
        get { return (double)GetValue(PatternThicknessProperty); }
        set { SetValue(PatternThicknessProperty, value); }
    }

    public static readonly DependencyProperty PatternThicknessProperty =
        DependencyProperty.Register(nameof(PatternThickness), typeof(double), typeof(Diagram),
            new PropertyMetadata(1d, (d, e) => ((Diagram)d).BuildBackground()));

    public double PatternOpacity
    {
        get { return (double)GetValue(PatternOpacityProperty); }
        set { SetValue(PatternOpacityProperty, value); }
    }

    public static readonly DependencyProperty PatternOpacityProperty =
        DependencyProperty.Register(nameof(PatternOpacity), typeof(double), typeof(Diagram),
            new PropertyMetadata(double.NaN, (d, e) => ((Diagram)d).BuildBackground()));

    public int MajorEvery
    {
        get { return (int)GetValue(MajorEveryProperty); }
        set { SetValue(MajorEveryProperty, value); }
    }

    public static readonly DependencyProperty MajorEveryProperty =
        DependencyProperty.Register(nameof(MajorEvery), typeof(int), typeof(Diagram),
            new PropertyMetadata(0, (d, e) => ((Diagram)d).BuildBackground()));

    public bool BackgroundScalesWithZoom
    {
        get { return (bool)GetValue(BackgroundScalesWithZoomProperty); }
        set { SetValue(BackgroundScalesWithZoomProperty, value); }
    }

    public static readonly DependencyProperty BackgroundScalesWithZoomProperty =
        DependencyProperty.Register(nameof(BackgroundScalesWithZoom), typeof(bool), typeof(Diagram),
            new PropertyMetadata(true, (d, e) => ((Diagram)d).BuildBackground()));
}
