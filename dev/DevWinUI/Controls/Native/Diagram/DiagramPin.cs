namespace DevWinUI;

public sealed partial class DiagramPin
{
    public string? Id { get; set; }

    public string? Name { get; set; }

    public string? Label { get; set; }

    public string? Type { get; set; }

    public string Key => Id ?? Name ?? string.Empty;

    public string DisplayLabel => Label ?? Name ?? Id ?? string.Empty;

    public bool IsExec => string.Equals(Type, "exec", StringComparison.OrdinalIgnoreCase);

    internal bool IsOutput { get; set; }

    internal bool Connected { get; set; }
}
