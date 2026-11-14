namespace DevWinUI;

public sealed partial class WorldMapCountry
{
    internal WorldMapCountry(string code, string name, Rect mainBounds, List<WorldMapShapeData> shapes)
    {
        Code = code;
        Name = name;
        MainBounds = mainBounds;
        Shapes = shapes;
    }

    public string Code { get; }
    public string Name { get; }
    public Rect MainBounds { get; }
    public Rect Bounds { get; internal set; }

    internal List<WorldMapShapeData> Shapes { get; }
    internal CanvasGeometry? Geometry { get; set; }
    internal CanvasCachedGeometry? CachedFill { get; set; }
    internal CanvasCachedGeometry? CachedStroke { get; set; }
    internal float CachedStrokeScale { get; set; }
    internal float CachedStrokePx { get; set; }
    internal WorldMapBorderStyle CachedStrokeStyle { get; set; }
    internal CanvasCachedGeometry? CachedRim { get; set; }
    internal float CachedRimScale { get; set; }
    internal float CachedRimPx { get; set; }
}
