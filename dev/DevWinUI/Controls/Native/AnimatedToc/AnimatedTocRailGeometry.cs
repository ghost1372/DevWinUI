namespace DevWinUI;

internal sealed partial class AnimatedTocRailGeometry
{
    public readonly PathGeometry Geometry = new();
    public readonly PathFigure Figure = new() { IsClosed = false };
    public readonly PolyLineSegment Segment = new();

    public AnimatedTocRailGeometry()
    {
        Figure.Segments.Add(Segment);
        Geometry.Figures.Add(Figure);
    }
}
