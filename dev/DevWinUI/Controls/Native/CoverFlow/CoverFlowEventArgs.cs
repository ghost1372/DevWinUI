namespace DevWinUI;

public partial class CoverFlowEventArgs : EventArgs
{
    public CoverFlowEventArgs(int index, object item)
    {
        Index = index;
        Item = item;
    }

    public int Index { get; }
    public object Item { get; }
}
