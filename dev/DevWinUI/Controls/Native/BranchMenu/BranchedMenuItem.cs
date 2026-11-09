namespace DevWinUI;

public partial class BranchedMenuItem
{
    public string Label { get; set; } = "";

    public string? Value { get; set; }

    public IList<BranchedMenuChild> Children { get; set; } = new List<BranchedMenuChild>();
}
