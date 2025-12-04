using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUIGallery.Views;

public sealed partial class ColorAnalyzerPage : Page
{
    int imageIndex = 1;

    public ColorAnalyzerPage()
    {
        InitializeComponent();
    }

    private void BtnChange_Click(object sender, RoutedEventArgs e)
    {
        if (imageIndex == 11)
        {
            imageIndex = 1;
        }

        ImageSample.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Carousel/{imageIndex}.jpg"));
        imageIndex++;
    }

    private void ImageSample_ImageOpened(object sender, RoutedEventArgs e)
    {
        ColorAnalyzerSample.UpdateAnalyzer();
    }
}
