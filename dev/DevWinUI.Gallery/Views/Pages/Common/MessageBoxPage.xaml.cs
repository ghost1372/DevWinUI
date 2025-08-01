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
        var button = (MessageBoxButtons)CmbMessageBoxButtons.SelectedItem;
        MessageBoxDefaultButton defaultButton = (MessageBoxDefaultButton)CmbMessageBoxDefaultBUtton.SelectedItem;

        var ownerWindow = TGHasOwnerWindow.IsOn ? MainWindow.Instance : null;
        var result = await MessageBox.ShowAsync(TGIsModal.IsOn, ownerWindow, txtContent.Text?.ToString(), txtTitle.Text?.ToString(), button, defaultButton);
        TxtResult.Text = result.ToString();
    }
}
