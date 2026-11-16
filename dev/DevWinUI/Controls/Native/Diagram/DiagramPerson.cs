namespace DevWinUI;

public sealed partial class DiagramPerson
{
    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public override string ToString() => Name;
}
