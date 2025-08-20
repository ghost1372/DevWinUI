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

        var result = await MessageBox.ShowAsync(new MessageBoxOptions
        {
            IsModal = TGIsModal.IsOn,
            OwnerWindow = ownerWindow,
            Content = txtContent.Text?.ToString(),
            Title = txtTitle.Text?.ToString(),
            Buttons = button,
            Icon = icon,
            DefaultButton = defaultButton,
            Underlay = underlay,
            UnderlaySystemBackdrop = new UnderlaySystemBackdropOptions
            {
                CoverMode = underlayCoverMode,
                Backdrop = underlayBackdrop
            }
        });
        TxtResult.Text = result.ToString();
    }
}
