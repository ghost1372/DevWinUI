namespace DevWinUI;

internal partial class UserPolygon
{
    public List<Vector2> Points { get; set; } = new();
    public float OffsetX { get; set; } = 0;
    public float CurrentY { get; set; }
    public string Key { get; set; }
    public bool IsRounded { get; set; }
}
