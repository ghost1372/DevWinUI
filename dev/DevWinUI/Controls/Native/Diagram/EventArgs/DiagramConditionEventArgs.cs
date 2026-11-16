namespace DevWinUI;

public sealed partial class DiagramConditionEventArgs : EventArgs
{
    public DiagramConditionEventArgs(DiagramNode node, DiagramCondition condition)
    {
        Node = node;
        Condition = condition;
    }

    public DiagramNode Node { get; }

    public DiagramCondition Condition { get; }
}
