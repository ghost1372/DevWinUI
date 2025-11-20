using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUIGallery.Views;

public sealed partial class FrostedGlassPage : Page
{
    public FrostedGlassPage()
    {
        InitializeComponent();
    }

    private void CalligraphyPenButton_OnClick(object sender, RoutedEventArgs e)
    {
        FlyoutBase.ShowAttachedFlyout(FlyoutButton);
    }
}
