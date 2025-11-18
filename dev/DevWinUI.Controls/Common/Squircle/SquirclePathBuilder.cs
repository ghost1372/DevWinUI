//https://github.com/cnbluefire/Squircle.Windows

namespace DevWinUI;

internal abstract partial class SquirclePathBuilder
{
    private List<Element>? elements;

    protected IReadOnlyList<Element> Elements => elements ?? [];

    public void ArcTo(bool isRelative, double radiusX, double radiusY, double angle, bool isLargeFlag, bool sweepDirectionClockwise, Vector2 endPoint)
    {
        EnsureElements().Add(new ArcElement(isRelative, radiusX, radiusY, angle, isLargeFlag, sweepDirectionClockwise, endPoint));
    }

    public void CubicBezierTo(bool isRelative, Vector2 controlPoint1, Vector2 controlPoint2, Vector2 endPoint)
    {
        EnsureElements().Add(new CubicBezierElement(isRelative, controlPoint1, controlPoint2, endPoint));
    }

    public void LineTo(bool isRelative, Vector2 lineTo)
    {
        EnsureElements().Add(new LineElement(isRelative, lineTo));
    }

    public void MoveTo(bool isRelative, Vector2 point)
    {
        EnsureElements().Add(new MoveElement(isRelative, point));
    }

    protected void CopyTo(SquirclePathBuilder pathBuilder)
    {
        pathBuilder.elements = null;

        for (int i = 0; i < Elements.Count; i++)
        {
            pathBuilder.EnsureElements().Add(Elements[i]);
        }
    }

    private List<Element> EnsureElements() => elements ??= [];

    protected record Element(bool IsRelative);

    protected record MoveElement(bool IsRelative, Vector2 MoveTo) : Element(IsRelative);

    protected record LineElement(bool IsRelative, Vector2 LineTo) : Element(IsRelative);

    protected record CubicBezierElement(bool IsRelative, Vector2 ControlPoint1, Vector2 ControlPoint2, Vector2 EndPoint) : Element(IsRelative);

    protected record ArcElement(bool IsRelative, double RadiusX, double RadiusY, double Angle, bool IsLargeFlag, bool SweepDirectionClockwise, Vector2 EndPoint) : Element(IsRelative);
}
