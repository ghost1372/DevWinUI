namespace DevWinUI;

[Flags]
public enum ParticleGlobeSelectionEffects
{
    None = 0,
    Highlight = 1,
    Pulse = 2,
    Outline = 4,
    DimOthers = 8,
    All = Highlight | Pulse | Outline | DimOthers,
}
