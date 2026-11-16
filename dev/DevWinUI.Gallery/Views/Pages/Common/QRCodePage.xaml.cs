using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUIGallery.Views;

public sealed partial class QRCodePage : Page
{
    public QRCodePage()
    {
        InitializeComponent();
    }

    private BitmapImage GenerateImage(string text, object value)
    {
        if (string.IsNullOrEmpty(text) || value is not ErrorCorrectionLevel level)
            return null;

        var png = QRCode.Create(text, level).ToPng();
        using var stream = new MemoryStream(png);
        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(stream.AsRandomAccessStream());
        return bitmapImage;
    }
}
