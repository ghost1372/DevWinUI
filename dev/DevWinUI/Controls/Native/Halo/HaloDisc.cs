//https://github.com/benthorner/radial_controls

namespace DevWinUI;

public partial class HaloDisc : Microsoft.UI.Xaml.Shapes.Path
{
    private EllipseGeometry ellipse = new EllipseGeometry();

    public HaloDisc()
    {
        Data = ellipse;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var circle = new HaloCircleHelper(finalSize);
        circle.Radius -= StrokeThickness / 2;

        ArrangeEllipse(circle);
        return finalSize;
    }

    private void ArrangeEllipse(HaloCircleHelper circle)
    {
        ellipse.Center = circle.Center;
        ellipse.RadiusX = circle.Radius;
        ellipse.RadiusY = circle.Radius;
    }
}
