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
        var underlay = CmbUnderlayMode.SelectedItem.As<UnderlayMode>();
        var underlayBackdrop = CmbUnderlayBackdrops.SelectedItem.As<BackdropType>();
        var underlayCoverMode = CmbUnderlayCoverModes.SelectedItem.As<UnderlayCoverMode>();

        var button = CmbMessageBoxButtons.SelectedItem.As<MessageBoxButtons>();
        var defaultButton = CmbMessageBoxDefaultBUtton.SelectedItem.As<MessageBoxDefaultButton>();
        var icon = CmbMessageBoxIcon.SelectedItem.As<MessageBoxIcon>();

        var ownerWindow = TGHasOwnerWindow.IsOn ? MainWindow.Instance : null;

        MessageBoxOptions options = MessageBoxOptions.Default;

        options.Underlay = underlay;
        options.UnderlaySystemBackdrop = new UnderlaySystemBackdropOptions
        {
            Backdrop = underlayBackdrop,
            CoverMode = underlayCoverMode
        };

        var result = await MessageBox.ShowAsync(TGIsModal.IsOn, ownerWindow, txtContent.Text?.ToString(), txtTitle.Text, button, icon, defaultButton, options);
        
        TxtResult.Text = result.ToString();
    }
}
