namespace DevWinUIGallery.Views;

public sealed partial class ColorSlideControlPage : Page
{
    public ColorSlideControlPage()
    {
        InitializeComponent();
    }
    private void OnClick(object sender, RoutedEventArgs e)
    {
        var button = (Button)sender;
        ColorSlideSample.StartTransition(button, this.RenderSize);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ColorSlideSample.ApplyClipping(e.NewSize);
    }
}
