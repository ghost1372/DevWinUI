using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class WindowedContentDialogPage : Page
{
    public BaseViewModel ViewModel { get; }
    public WindowedContentDialogPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        var popupBackdropType = CmbUnderlayBackdrops.SelectedItem.As<BackdropType>();
        var popupCoverMode = CmbUnderlayCoverModes.SelectedItem.As<UnderlayCoverMode>();
        WindowedContentDialog dialog = new()
        {
            Title = txtTitle.Text,
            Content = txtContent.Text,
            PrimaryButtonText = "PrimaryButtonText",
            SecondaryButtonText = "SecondaryButtonText",
            CloseButtonText = "CloseButtonText",
            OwnerWindow = MainWindow.Instance,
            HasTitleBar = TGHasTitleBar.IsOn,
            IsResizable = TGIsResizable.IsOn,
            ShowUnderlayBackdrop = TGShowUnderlayBackdrop.IsOn,
            UnderlayBackdrop = popupBackdropType,
            UnderlayBackdropCoverMode = popupCoverMode,
        };

        ContentDialogResult result = await dialog.ShowAsync(TGIsModal.IsOn);
        TxtResult.Text = result.ToString();
    }
}
