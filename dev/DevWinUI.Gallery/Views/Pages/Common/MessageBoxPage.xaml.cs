using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class MessageBoxPage : Page
{
    public BaseViewModel ViewModel { get;}
    public MessageBoxPage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private async void OnMessageBox(object sender, RoutedEventArgs e)
    {
        var popupBackdropType = CmbUnderlayBackdrops.SelectedItem.As<BackdropType>();
        var popupCoverMode = CmbUnderlayCoverModes.SelectedItem.As<UnderlayCoverMode>();
        var button = (MessageBoxButtons)CmbMessageBoxButtons.SelectedItem;
        MessageBoxDefaultButton defaultButton = (MessageBoxDefaultButton)CmbMessageBoxDefaultBUtton.SelectedItem;

        var ownerWindow = TGHasOwnerWindow.IsOn ? MainWindow.Instance : null;

        var result = await MessageBox.ShowAsync(new MessageBoxOptions
        {
            IsModal = TGIsModal.IsOn,
            OwnerWindow = ownerWindow,
            Content = txtContent.Text?.ToString(),
            Title = txtTitle.Text?.ToString(),
            Buttons = button,
            DefaultButton = defaultButton,
            UnderlayBackdropCoverMode = popupCoverMode,
            UnderlayBackdrop = popupBackdropType
        });
        TxtResult.Text = result.ToString();
    }
}
