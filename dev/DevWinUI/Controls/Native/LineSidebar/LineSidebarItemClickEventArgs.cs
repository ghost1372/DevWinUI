namespace DevWinUI;

public sealed partial class LineSidebarItemClickEventArgs : EventArgs
{
    public LineSidebarItemClickEventArgs(int index, string label)
    {
        Index = index;
        Label = label;
    }

    public int Index { get; }
    public string Label { get; }
}
