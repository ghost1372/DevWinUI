namespace DevWinUI;

internal readonly struct RoadmapMetrics
{
    public double ItemWidth { get; init; }
    public double MarkerSize { get; init; }
    public double AxisThickness { get; init; }
    public double StemThickness { get; init; }
    public double StemLength { get; init; }
    public double LabelFontSize { get; init; }
    public double LabelOffset { get; init; }
    public RoadmapAxisCap StartCap { get; init; }
    public RoadmapAxisCap EndCap { get; init; }

    public double LabelHeight
    {
        get { return LabelFontSize * 1.4; }
    }

    public double BadgeHeight
    {
        get { return LabelFontSize * 1.4 + 14; }
    }

    public double SecondStemLength
    {
        get { return Math.Max(16, StemLength * 0.6); }
    }
}
