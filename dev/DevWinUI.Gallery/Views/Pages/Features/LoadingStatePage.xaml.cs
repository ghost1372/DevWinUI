namespace DevWinUIGallery.Views;

public sealed partial class LoadingStatePage : Page
{
    public LoadingStatePage()
    {
        InitializeComponent();
    }

    private void ControlExample_ResetRequested(object sender, EventArgs e)
    {
        MyLoadingState.Restart();
    }
}
