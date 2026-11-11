namespace DevWinUI;

public sealed partial class DiagramEdge
{
    public string? From { get; set; }

    public string? To { get; set; }

    public string? Label { get; set; }

    public bool IsDashed { get; set; }

    public bool IsAnimated { get; set; }

    public string? FromPin { get; set; }

    public string? ToPin { get; set; }

    public string? Conflict { get; internal set; }
}
