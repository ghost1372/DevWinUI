using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Options))]
public sealed partial class DiagramCondition
{
    public string? Label { get; set; } = "If";

    public List<string> Options { get; } = new();

    public string? Value { get; set; }

    public string? Placeholder { get; set; }
}
