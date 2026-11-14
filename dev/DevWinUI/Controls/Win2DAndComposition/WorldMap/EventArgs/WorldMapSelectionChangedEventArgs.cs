namespace DevWinUI;

public sealed partial class WorldMapSelectionChangedEventArgs : EventArgs
{
    internal WorldMapSelectionChangedEventArgs(IReadOnlyList<string> added, IReadOnlyList<string> removed)
    {
        Added = added;
        Removed = removed;
    }

    public IReadOnlyList<string> Added { get; }
    public IReadOnlyList<string> Removed { get; }
}
