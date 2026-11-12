namespace DevWinUIGallery.Views;

public sealed partial class AntigravityPage : Page
{
    public AntigravityPage()
    {
        InitializeComponent();
    }

    private void Replay_Click(object sender, RoutedEventArgs e) => AntigravitySample.Replay();
}
