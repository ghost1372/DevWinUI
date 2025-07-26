namespace DevWinUIGallery.Views;

public sealed partial class GifImagePage : Page
{
    public GifImagePage()
    {
        InitializeComponent();
    }

    private void OnStop(object sender, RoutedEventArgs e)
    {
        GifImageSample.Stop();
    }

    private void OnPlay(object sender, RoutedEventArgs e)
    {
        GifImageSample.Play();
    }

    private void OnPause(object sender, RoutedEventArgs e)
    {
        GifImageSample.Pause();
    }
}
