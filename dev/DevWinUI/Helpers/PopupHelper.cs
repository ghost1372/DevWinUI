using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;
public partial class PopupHelper
{
    public static Popup CreatePopup(XamlRoot xamlRoot, PopupBackdropType backdropType, PopupCoverMode coverMode, double verticalOffset)
    {
        var popup = new Popup
        {
            IsLightDismissEnabled = false,
            XamlRoot = xamlRoot,
            ShouldConstrainToRootBounds = false,
        };

        var overlay = new Border
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch,
            Width = xamlRoot.Size.Width,
            Height = xamlRoot.Size.Height
        };

        if (coverMode == PopupCoverMode.Full)
        {
            overlay.CornerRadius = new CornerRadius(8);
            popup.VerticalOffset = 0;
        }
        else
        {
            if (verticalOffset == 0)
            {
                overlay.CornerRadius = new CornerRadius(8);
            }
            else
            {
                overlay.CornerRadius = new CornerRadius(0, 0, 8, 8);
            }

            popup.VerticalOffset = verticalOffset;
        }

        new PopupBackdropManager().TrySetSystemBackdrop(popup, backdropType);

        popup.Child = overlay;
        return popup;
    }
}
public enum PopupBackdropType
{
    Mica,
    MicaAlt,
    Acrylic,
    AcrylicThin
}
public enum PopupCoverMode
{
    Full,
    ClientArea
}
