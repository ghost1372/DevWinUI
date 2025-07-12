namespace DevWinUIGallery.Views;

public sealed partial class ColorBloomControlPage : Page
{
    public ColorBloomControlPage()
    {
        this.InitializeComponent();
    }

    private void OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        ColorBloomSample.StartTransition(button, this.RenderSize);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ColorBloomSample.ApplyClipping(e.NewSize);
    }
}
