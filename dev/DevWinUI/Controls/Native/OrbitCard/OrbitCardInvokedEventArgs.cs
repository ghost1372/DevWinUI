namespace DevWinUI;

public sealed partial class OrbitCardInvokedEventArgs : EventArgs
{
    internal OrbitCardInvokedEventArgs(OrbitCardItem item, int index)
    {
        Item = item;
        Index = index;
    }

    public OrbitCardItem Item { get; }

    public int Index { get; }
}
