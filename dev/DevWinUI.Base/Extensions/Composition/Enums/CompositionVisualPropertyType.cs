namespace DevWinUI;


[Flags]
public enum CompositionVisualPropertyType
{
    None = 0,
    Opacity = 1 << 0,
    Offset = 1 << 1,
    Scale = 1 << 2,
    Size = 1 << 3,
    RotationAngleInDegrees = 1 << 4,
    All = ~0
}
