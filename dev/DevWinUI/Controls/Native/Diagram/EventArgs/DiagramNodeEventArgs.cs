namespace DevWinUI;

public sealed partial class DiagramNodeEventArgs : EventArgs
{
    public DiagramNodeEventArgs(DiagramNode node) => Node = node;

    public DiagramNode Node { get; }
}
