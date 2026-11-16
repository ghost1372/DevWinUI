namespace DevWinUI;

public sealed partial class DiagramConflict
{
    public DiagramConflict(DiagramEdge? edge, string message, string detail)
    {
        Edge = edge;
        Message = message;
        Detail = detail;
    }

    public DiagramEdge? Edge { get; }

    public string Message { get; }

    public string Detail { get; }
}
