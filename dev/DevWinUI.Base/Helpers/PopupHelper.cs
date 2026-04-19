using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;
public partial class PopupHelper
{
    public static Popup CreatePopup(XamlRoot xamlRoot, BackdropType backdropType, bool isFullCover, double verticalOffset)
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
            Height = isFullCover ? xamlRoot.Size.Height : xamlRoot.Size.Height - verticalOffset,
            Opacity = 0.0
        };

        if (isFullCover)
        {
            overlay.CornerRadius = new CornerRadius(8);
            popup.Margin = new Thickness(0);
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

            popup.Margin = new Thickness(0, verticalOffset, 0,0);
        }

        new PopupBackdropManager().TrySetSystemBackdrop(popup, backdropType);

        popup.Child = overlay;
        return popup;
    }
}
