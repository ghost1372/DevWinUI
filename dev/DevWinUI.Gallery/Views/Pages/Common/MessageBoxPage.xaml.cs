namespace DevWinUIGallery.Views;

public sealed partial class MessageBoxPage : Page
{
    public MessageBoxPage()
    {
        InitializeComponent();
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        await MessageBox.ShowAsync(txtContent.Text?.ToString(), txtTitle.Text?.ToString(), MessageBoxButtons.OKCancel);
    }
}
