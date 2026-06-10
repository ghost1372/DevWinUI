namespace DevWinUI;

public static partial class MaterialAttach
{
    private static void HandleFlyout(Flyout flyout, bool newValue)
    {
        if (newValue)
        {
            var presenterStyle = Application.Current.Resources["FlyoutPresenterWithSystemBackdropElementStyle"] as Style;
            flyout.FlyoutPresenterStyle = presenterStyle;
        }
        else
        {
            var presenterStyle = Application.Current.Resources["DefaultFlyoutPresenterStyle"] as Style;
            flyout.FlyoutPresenterStyle = presenterStyle;
        }
    }
}
