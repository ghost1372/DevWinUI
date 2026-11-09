namespace DevWinUI;

public sealed partial class VisualStepperItem
{
    public int Index { get; init; }
    public VisualStepperStep Step { get; init; } = null!;
}
