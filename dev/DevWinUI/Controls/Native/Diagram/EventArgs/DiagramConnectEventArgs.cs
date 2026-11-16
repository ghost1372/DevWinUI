namespace DevWinUI;

public sealed partial class DiagramConnectEventArgs : EventArgs
{
    public DiagramConnectEventArgs(DiagramNode sourceNode, DiagramPin sourcePin, DiagramNode targetNode, DiagramPin targetPin)
    {
        SourceNode = sourceNode;
        SourcePin = sourcePin;
        TargetNode = targetNode;
        TargetPin = targetPin;
    }

    public DiagramNode SourceNode { get; }

    public DiagramPin SourcePin { get; }

    public DiagramNode TargetNode { get; }

    public DiagramPin TargetPin { get; }

    public bool Cancel { get; set; }
}
