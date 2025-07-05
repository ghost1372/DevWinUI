namespace DevWinUIGallery.Views;

public sealed partial class HeaderCarouselPage : Page
{
    public HeaderCarouselPage()
    {
        InitializeComponent();
    }

    private void HeaderCarousel_ItemClick(object sender, HeaderCarouselEventArgs e)
    {
        TxtResult.Text = e.HeaderCarouselItem.Header;
    }
}
