namespace DevWinUI;

internal sealed partial class RoadmapRenderState
{
    public RoadmapPanel Panel = null!;
    public RoadmapMetrics Metrics;
    public RoadmapMode Mode;
    public Color AxisColor;
    public Color StemColor;
    public Color FillColor;
    public Color TextColor;
    public CanvasTextFormat LabelFormat = null!;
    public CanvasTextFormat GlyphFormat = null!;
    public CanvasStrokeStyle Dashed = null!;
    public float Pulse;
    public float Boundary;
    public float Width;
    public float Height;
    public float Time;
    public bool Flow;
    public Color EffectColor;
    public float BootHead;
}
