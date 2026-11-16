namespace DevWinUI;

public sealed partial class DiagramEdgeEventArgs : EventArgs
{
    public DiagramEdgeEventArgs(DiagramEdge edge) => Edge = edge;

    public DiagramEdge Edge { get; }
}
