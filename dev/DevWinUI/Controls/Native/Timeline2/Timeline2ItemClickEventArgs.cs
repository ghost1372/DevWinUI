namespace DevWinUI;

public sealed partial class Timeline2ItemClickEventArgs : EventArgs
{
    public Timeline2ItemClickEventArgs(object? item, int index)
    {
        Item = item;
        Index = index;
    }

    public object? Item { get; }

    public int Index { get; }
}
