namespace DevWinUI;

public readonly struct AnalyzedColor
{ 
    internal AnalyzedColor(Color color, float sampleFraction)
    {
        Color = color;
        Weight = sampleFraction;
    }

    public Color Color { get; }

    public float Weight { get; }
}
