namespace DevWinUI;

internal sealed partial class ScrambledTextLetter
{
    public Grid Cell = null!;
    public TextBlock Glyph = null!;
    public CompositeTransform Transform = null!;
    public TextBlock[]? Ghosts;
    public TranslateTransform[]? GhostTransforms;
    public string Real = string.Empty;
    public bool IsSpace;
    public bool ShowingScramble;
    public double ScrambleUntil;
    public double NextSwap;
    public double PulseUntil;
    public double PointerTarget;
    public double Intensity;
    public double PendingAt = double.PositiveInfinity;
}
