namespace DevWinUIGallery.Views;

public sealed partial class CheckUpdateControlPage : Page
{
    public CheckUpdateControlPage()
    {
        InitializeComponent();
    }

    private async void CheckUpdateControl_Click(object sender, RoutedEventArgs e)
    {
        await MessageBox.ShowAsync("Downloading...");
    }
}
