namespace DevWinUI;

internal sealed partial class MotionWallSettings
{
    public Orientation Orientation { get; init; }
    public int Lanes { get; init; }
    public bool AddEdgeLanes { get; init; }
    public int ItemsPerLane { get; init; }
    public MotionWallDistribution Distribution { get; init; }
    public double Gap { get; init; }
    public double TileWidth { get; init; }
    public double TileHeight { get; init; }
    public double AspectRatio { get; init; }
    public double CornerRadius { get; init; }
    public double Speed { get; init; }
    public double LoopDuration { get; init; }
    public MotionWallDirection Direction { get; init; }
    public double Variance { get; init; }
    public MotionWallVarianceStyle VarianceStyle { get; init; }
    public bool RandomStart { get; init; }
    public bool PauseOnHover { get; init; }
    public double Tilt { get; init; }
    public double Turn { get; init; }
    public double Roll { get; init; }
    public double Perspective { get; init; }
    public double Depth { get; init; }
    public bool Interactive { get; init; }
    public double Parallax { get; init; }
    public double PointerTilt { get; init; }
    public double Spotlight { get; init; }
    public double Dim { get; init; }
    public bool Grayscale { get; init; }
    public double Lift { get; init; }
    public Color OverlayColor { get; init; }
    public double OverlayOpacity { get; init; }
    public bool ShowLines { get; init; }
    public Color LineColor { get; init; }
    public Color TileColor { get; init; }
    public Color TextColor { get; init; }
    public bool FadeEdges { get; init; }
    public double EdgeFadeSize { get; init; }
    public double EdgeFadeStrength { get; init; }
    public Color? EdgeFadeColor { get; init; }
    public Color BackgroundColor { get; init; }

    public int TotalLanes => Math.Max(1, Lanes) + (AddEdgeLanes ? 2 : 0);

    public bool NeedsPointer => Spotlight > 0 || Parallax > 0 || PointerTilt > 0 || Interactive || PauseOnHover;
}
