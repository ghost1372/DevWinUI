namespace DevWinUI;

public sealed partial class BranchedMenuToggledEventArgs : EventArgs
{
    public BranchedMenuToggledEventArgs(int index, bool isOpen)
    {
        Index = index;
        IsOpen = isOpen;
    }

    public int Index { get; }

    public bool IsOpen { get; }
}
