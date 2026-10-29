namespace DevWinUI;

public enum DiffUnifiedMode
{
    Inline,
    Before,
    After,
}

[Flags]
public enum DiffUnifiedModes
{
    None = 0,
    Inline = 1,
    Before = 2,
    After = 4,
    All = Inline | Before | After,
}
