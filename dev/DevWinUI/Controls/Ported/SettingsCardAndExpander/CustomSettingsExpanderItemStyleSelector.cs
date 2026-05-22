namespace DevWinUI;

internal sealed partial class CustomSettingsExpanderItemStyleSelector : SettingsExpanderItemStyleSelector
{
    public Style ExpanderStyle { get; set; }

    protected override Style SelectStyleCore(object item, DependencyObject container)
    {
        switch (container)
        {
            case SettingsCard:
                return base.SelectStyleCore(item, container);
            case SettingsExpander:
                return ExpanderStyle;
        }

        return null;
    }
}
