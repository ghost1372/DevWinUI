using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;

[MarkupExtensionReturnType(ReturnType = typeof(string))]
public sealed partial class ResourceStringExtension : MarkupExtension
{
    private static readonly ResourceLoader resourceLoader = new();

    public string Name { get; set; } = string.Empty;

    protected override object ProvideValue() => resourceLoader.GetString(Name);
}
