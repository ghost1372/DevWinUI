using Microsoft.UI.Xaml.Media;
using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class MessageBoxPage : Page
{
    public ObservableCollection<MessageBoxIcon> MessageBoxIconItems { get; set; } = new ObservableCollection<MessageBoxIcon>(Enum.GetValues<MessageBoxIcon>());
    public ObservableCollection<MessageBoxButtons> MessageBoxButtonItems { get; set; } = new ObservableCollection<MessageBoxButtons>(Enum.GetValues<MessageBoxButtons>());
    public ObservableCollection<MessageBoxDefaultButton> MessageBoxDefaultButtonItems { get; set; } = new ObservableCollection<MessageBoxDefaultButton>(Enum.GetValues<MessageBoxDefaultButton>());
    public ObservableCollection<BackdropType> BackdropItems { get; set; } = new ObservableCollection<BackdropType>(Enum.GetValues<BackdropType>());
    public ObservableCollection<FlowDirection> FlowDirectionItems { get; set; } = new ObservableCollection<FlowDirection>(Enum.GetValues<FlowDirection>());
    public ObservableCollection<WindowStartupLocation> WindowStartupLocationItems { get; set; } = new ObservableCollection<WindowStartupLocation>(Enum.GetValues<WindowStartupLocation>());

    public MessageBoxPage()
    {
        InitializeComponent();
    }

    private async void OnMessageBox(object sender, RoutedEventArgs e)
    {
        var element = sender as Button;
        var underlayBackdrop = CmbUnderlayBackdrops.SelectedItem.As<BackdropType>();

        var button = CmbMessageBoxButtons.SelectedItem.As<MessageBoxButtons>();
        var defaultButton = CmbMessageBoxDefaultButton.SelectedItem.As<MessageBoxDefaultButton>();
        var icon = CmbMessageBoxIcon.SelectedItem.As<MessageBoxIcon>();
        var flow = CmbFlow.SelectedItem.As<FlowDirection>();
        var location = CmbWindowStartupLocation.SelectedItem.As<WindowStartupLocation>();

        var ownerWindow = TGHasOwnerWindow.IsOn ? MainWindow.Instance : null;

        switch (underlayBackdrop)
        {
            case BackdropType.None:
                MessageBox.SystemBackdrop = null;
                break;
            case BackdropType.Mica:
                MessageBox.SystemBackdrop = new MicaBackdrop();
                break;
            case BackdropType.MicaAlt:
                MessageBox.SystemBackdrop = new MicaBackdrop() { Kind = Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt };
                break;
            case BackdropType.Acrylic:
                MessageBox.SystemBackdrop = new AcrylicSystemBackdrop();
                break;
            case BackdropType.AcrylicThin:
                MessageBox.SystemBackdrop = new AcrylicSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicKind.Thin);
                break;
            case BackdropType.Transparent:
                MessageBox.SystemBackdrop = new TransparentBackdrop();
                break;
        }

        MessageBox.FlowDirection = flow;
        MessageBox.StartupLocation = location;
        MessageBox.CanResize = TGCanResize.IsOn;
        MessageBox.HasTitleBar = TGHasTitleBar.IsOn;

        MessageBoxResult result = MessageBoxResult.None;
        switch (element.Tag.ToString())
        {
            case "None":
                result = await MessageBox.ShowAsync(ownerWindow, txtContent.Text?.ToString(), txtTitle.Text, button, icon, defaultButton);
                break;

            case "Info":
                result = await MessageBox.ShowInfoAsync(ownerWindow, txtContent.Text?.ToString(), txtTitle.Text, button, defaultButton);
                break;
            case "Warning":
                result = await MessageBox.ShowWarningAsync(ownerWindow, txtContent.Text?.ToString(), txtTitle.Text, button, defaultButton);
                break;
            case "Error":
                result = await MessageBox.ShowErrorAsync(ownerWindow, txtContent.Text?.ToString(), txtTitle.Text, button, defaultButton);
                break;
            case "Success":
                result = await MessageBox.ShowSuccessAsync(ownerWindow, txtContent.Text?.ToString(), txtTitle.Text, button, defaultButton);
                break;
            case "Question":
                result = await MessageBox.ShowQuestionAsync(ownerWindow, txtContent.Text?.ToString(), txtTitle.Text, button, defaultButton);
                break;

        }
        TxtResult.Text = result.ToString();
    }
}
