using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery.Views;

public sealed partial class WindowedContentDialogPage : Page
{
    public WindowedContentDialogPage()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var underlayBackdrop = (BackdropType)CmbUnderlayBackdrops.Value;
        var flow = (FlowDirection)CmbFlow.Value;
        var orientation = (Orientation)CmbButtonOrientation.Value;

        WindowedContentDialog dialog = new()
        {
            Header = txtTitle.Text,
            Content = txtContent.Text,
            PrimaryButtonContent = "PrimaryButtonText",
            SecondaryButtonContent = "SecondaryButtonText",
            CloseButtonContent = "CloseButtonText",
            Owner = MainWindow.Instance,
            HasTitleBar = TGHasTitleBar.IsOn,
            CanResize = TGIsResizable.IsOn,
            FlowDirection = flow,
            ButtonOrientation = orientation,
            CanDragMoveWindow = TGDragMove.IsOn
        };

        switch (underlayBackdrop)
        {
            case BackdropType.None:
                dialog.SystemBackdrop = null;
                break;
            case BackdropType.Mica:
                dialog.SystemBackdrop = new MicaBackdrop();
                break;
            case BackdropType.MicaAlt:
                dialog.SystemBackdrop = new MicaBackdrop() { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt };
                break;
            case BackdropType.Acrylic:
                dialog.SystemBackdrop = new AcrylicSystemBackdrop();
                break;
            case BackdropType.AcrylicThin:
                dialog.SystemBackdrop = new AcrylicSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicKind.Thin);
                break;
            case BackdropType.Transparent:
                dialog.SystemBackdrop = new TransparentBackdrop();
                break;
        }

        ContentDialogResult result = await dialog.ShowAsync();
        TxtResult.Text = result.ToString();
    }
}
