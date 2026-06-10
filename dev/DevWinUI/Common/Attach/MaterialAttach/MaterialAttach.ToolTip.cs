namespace DevWinUI;

public static partial class MaterialAttach
{
    private static void HandleToolTip(ToolTip toolTip, bool newValue)
    {
        if (newValue)
        {
            toolTip.Template = Application.Current.Resources["ToolTipWithSystemBackdropElementTemplate"] as ControlTemplate;
        }
        else
        {
            toolTip.Style = Application.Current.Resources["DefaultToolTipStyle"] as Style;
        }
    }
}
