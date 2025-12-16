using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class MessageBoxPage : Page
{
    public ObservableCollection<MessageBoxIcon> MessageBoxIconItems { get; set; } = new ObservableCollection<MessageBoxIcon>(Enum.GetValues<MessageBoxIcon>());
    public ObservableCollection<UnderlayCoverMode> UnderlayCoverModeItems { get; set; } = new ObservableCollection<UnderlayCoverMode>(Enum.GetValues<UnderlayCoverMode>());
    public ObservableCollection<UnderlayMode> UnderlayModeItems { get; set; } = new ObservableCollection<UnderlayMode>(Enum.GetValues<UnderlayMode>());
    public ObservableCollection<MessageBoxButtons> MessageBoxButtonItems { get; set; } = new ObservableCollection<MessageBoxButtons>(Enum.GetValues<MessageBoxButtons>());
    public ObservableCollection<MessageBoxDefaultButton> MessageBoxDefaultButtonItems { get; set; } = new ObservableCollection<MessageBoxDefaultButton>(Enum.GetValues<MessageBoxDefaultButton>());
    public ObservableCollection<BackdropType> BackdropItems { get; set; } = new ObservableCollection<BackdropType>(Enum.GetValues<BackdropType>());

    public MessageBoxPage()
    {
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
