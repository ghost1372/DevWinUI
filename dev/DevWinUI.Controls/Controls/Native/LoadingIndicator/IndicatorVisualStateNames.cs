using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;
internal partial class IndicatorVisualStateNames : MarkupExtension
{
    private static IndicatorVisualStateNames _activeState;
    private static IndicatorVisualStateNames _inactiveState;

    public static IndicatorVisualStateNames ActiveState =>
        _activeState ?? (_activeState = new IndicatorVisualStateNames("Active"));

    public static IndicatorVisualStateNames InactiveState =>
        _inactiveState ?? (_inactiveState = new IndicatorVisualStateNames("Inactive"));

    private IndicatorVisualStateNames(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        Name = name;
    }

    public string Name { get; }

    protected override object ProvideValue()
    {
        return Name;
    }
}

