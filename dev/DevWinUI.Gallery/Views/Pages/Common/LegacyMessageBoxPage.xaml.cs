namespace DevWinUIGallery.Views;

public sealed partial class LegacyMessageBoxPage : Page
{
    public LegacyMessageBoxPage()
    {
        this.InitializeComponent();
    }

    private void BtnMessageBox_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(App.MainWindow, "Hello WinUI, This is a Legacy MessageBox", "MessageBox Title", MessageBoxStyle.IconInformation);
    }
}
