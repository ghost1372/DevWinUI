namespace DevWinUIGallery.Views;

public sealed partial class SystemTrayIconPage : Page
{
    public SystemTrayIconPage()
    {
        InitializeComponent();
    }

    private void TGIsVisible_Toggled(object sender, RoutedEventArgs e)
    {
        if (TGIsVisible.IsOn)
        {
            MainWindow.Instance.AddTrayIcon(TxtToolTip.Text);
        }
        else
        {
            MainWindow.Instance.RemoveTrayIcon();
        }
    }

    private void TxtToolTip_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not Microsoft.UI.Xaml.Controls.TextBox textBox)
            return;

        MainWindow.Instance.TrayIcon?.Tooltip = textBox.Text;
    }
}
