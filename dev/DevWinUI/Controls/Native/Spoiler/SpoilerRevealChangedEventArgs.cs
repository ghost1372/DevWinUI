namespace DevWinUI;

public sealed partial class SpoilerRevealChangedEventArgs : EventArgs
{
    public bool IsRevealed { get; }

    public SpoilerRevealChangedEventArgs(bool isRevealed)
    {
        IsRevealed = isRevealed;
    }
}
