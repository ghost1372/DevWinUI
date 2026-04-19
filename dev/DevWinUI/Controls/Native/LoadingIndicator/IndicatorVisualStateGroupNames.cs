using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;
internal partial class IndicatorVisualStateGroupNames : MarkupExtension
{
    private static IndicatorVisualStateGroupNames _internalActiveStates;
    private static IndicatorVisualStateGroupNames _sizeStates;

    public static IndicatorVisualStateGroupNames ActiveStates =>
        _internalActiveStates ?? (_internalActiveStates = new IndicatorVisualStateGroupNames("ActiveStates"));

    public static IndicatorVisualStateGroupNames SizeStates =>
        _sizeStates ?? (_sizeStates = new IndicatorVisualStateGroupNames("SizeStates"));

    private IndicatorVisualStateGroupNames(string name)
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

