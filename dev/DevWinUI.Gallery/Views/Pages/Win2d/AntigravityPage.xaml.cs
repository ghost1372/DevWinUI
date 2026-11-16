namespace DevWinUIGallery.Views;

public sealed partial class AntigravityPage : Page
{
    public AntigravityPage()
    {
        InitializeComponent();
    }

    private void ControlExample_ResetRequested(object sender, EventArgs e)
    {
        AntigravitySample.Replay();
    }
}
