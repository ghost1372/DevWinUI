namespace DevWinUI;

public sealed partial class MotionWallTileEventArgs : EventArgs
{
    public MotionWallTileEventArgs(object? item, int index)
    {
        Item = item;
        Index = index;
    }

    public object? Item { get; }

    public int Index { get; }
}
