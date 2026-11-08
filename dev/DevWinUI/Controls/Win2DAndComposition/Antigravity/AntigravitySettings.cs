namespace DevWinUI;

internal sealed partial class AntigravitySettings
{
    public int Count { get; init; }
    public float Magnet { get; init; } = 170f;
    public float Ring { get; init; } = 120f;
    public float WaveSpeed { get; init; }
    public float WaveAmp { get; init; }
    public int WaveCount { get; init; }
    public float Size { get; init; } = 0.9f;
    public float Lerp { get; init; } = 0.05f;
    public float ColorSpeed { get; init; }
    public float Variance { get; init; }
    public float Rot { get; init; }
    public float Depth { get; init; }
    public float Pulse { get; init; }
    public float FieldStrength { get; init; }
    public float Ambient { get; init; }
    public AntigravityColorMode Mode { get; init; }
    public AntigravityParticleShape Shape { get; init; }
    public bool Auto { get; init; }
    public bool Interactive { get; init; }
    public bool ClickBurst { get; init; }
    public bool Intro { get; init; }
    public Color[] LutLinear { get; init; } = Array.Empty<Color>();
    public Color[] LutCyclic { get; init; } = Array.Empty<Color>();
}
