namespace DevWinUI;

public static partial class MaterialAttach
{
    private static void HandleMediaTransportControls(MediaTransportControls transport, bool newValue)
    {
        if (newValue)
        {
            var style = Application.Current.Resources["MediaTransportControlsWithSystemBackdropElementStyle"] as Style;
            transport.Style = style;
        }
        else
        {
            var style = Application.Current.Resources["DefaultMediaTransportControlsStyle"] as Style;
            transport.Style = style;
        }
    }
}
