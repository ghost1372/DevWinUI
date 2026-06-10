namespace DevWinUI;

public static partial class MaterialAttach
{
    public static bool GetUseAcrylicBackground(DependencyObject obj)
    {
        return (bool)obj.GetValue(UseAcrylicBackgroundProperty);
    }

    public static void SetUseAcrylicBackground(DependencyObject obj, bool value)
    {
        obj.SetValue(UseAcrylicBackgroundProperty, value);
    }

    public static readonly DependencyProperty UseAcrylicBackgroundProperty =
        DependencyProperty.RegisterAttached("UseAcrylicBackground", typeof(bool), typeof(MaterialAttach), new PropertyMetadata(false, OnUseAcrylicBackgroundChanged));

    private static void OnUseAcrylicBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        switch (d)
        {
            case AutoSuggestBox autoSuggestBox:
                HandleAutoSuggestBox(autoSuggestBox, (bool)e.NewValue);
                break;

            case ComboBox comboBox:
                HandleComboBox(comboBox, (bool)e.NewValue);
                break;

            case CommandBar commandBar:
                HandleCommandBar(commandBar, (bool)e.NewValue);
                break;

            case Flyout flyout:
                HandleFlyout(flyout, (bool)e.NewValue);
                break;

            case ToolTip toolTip:
                HandleToolTip(toolTip, (bool)e.NewValue);
                break;
            case MediaTransportControls mediaTransportControls:
                HandleMediaTransportControls(mediaTransportControls, (bool)e.NewValue);
                break;
        }
    }
}
