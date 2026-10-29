using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public sealed partial class MorphingWizardStep
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public UIElement? Content { get; set; }
}
