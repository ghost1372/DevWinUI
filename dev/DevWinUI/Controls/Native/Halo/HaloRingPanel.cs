//https://github.com/benthorner/radial_controls

namespace DevWinUI;

public partial class HaloRingPanel : Panel
{
    public static void SetAngle(DependencyObject o, double value)
    {
        o.SetValue(HaloRingPanel.AngleProperty, value);
    }

    public static double GetAngle(DependencyObject o)
    {
        return (double)o.GetValue(HaloRingPanel.AngleProperty);
    }
    public static readonly DependencyProperty AngleProperty =
        DependencyProperty.RegisterAttached("Angle", typeof(double), typeof(HaloRingPanel), new PropertyMetadata(0.0, Refresh));

    public static double GetOffset(DependencyObject o)
    {
        return (double)o.GetValue(HaloRingPanel.OffsetProperty);
    }

    public static void SetOffset(DependencyObject o, double value)
    {
        o.SetValue(HaloRingPanel.OffsetProperty, value);
    }
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.RegisterAttached("Offset", typeof(double), typeof(HaloRingPanel), new PropertyMetadata(0.0, Refresh));

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
        }

        SetValue(HaloPanel.ThicknessProperty, RingThickness());

        return RingSize(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var thickness = RingThickness();
        var size = RingSize(finalSize);

        var radius = (Math.Min(size.Width, size.Height) - thickness) / 2;

        foreach (var child in Children)
        {
            ArrangeChild(child, size);
            TransformChild(child, radius);
        }

        return size;
    }

    private void ArrangeChild(UIElement child, Size size)
    {
        var topLeft = new Point(
            (size.Width - child.DesiredSize.Width) / 2,
            (size.Height - child.DesiredSize.Height) / 2
        );

        child.Arrange(new Rect(topLeft, child.DesiredSize));
    }

    private void TransformChild(UIElement child, double radius)
    {
        var offset = GetOffset(child).ToRadians();

        child.RenderTransform = new TransformGroup
        {
            Children = new TransformCollection
            {
                new TranslateTransform
                {
                    X = radius * Math.Sin(offset),
                    Y = -radius * Math.Cos(offset)
                },
                new RotateTransform { Angle = GetAngle(child) }
            }
        };

        child.RenderTransformOrigin = new Point(0.5, 0.5);
    }

    private Size RingSize(Size size)
    {
        var thickness = RingThickness();

        return new Size(
            size.Width < thickness ? thickness : size.Width,
            size.Height < thickness ? thickness : size.Height
        );
    }

    private double RingThickness()
    {
        if (Children.Count == 0) return 0.0;

        return Children.Max(child =>
        {
            return Math.Max(
                child.DesiredSize.Width, child.DesiredSize.Height
            );
        });
    }

    private static void Refresh(object o, DependencyPropertyChangedEventArgs e)
    {
        var element = o as FrameworkElement;
        if (element == null) return;

        var parent = element.Parent as HaloRingPanel;
        if (parent == null) return;

        parent.InvalidateMeasure();
        parent.UpdateLayout();
    }
}
