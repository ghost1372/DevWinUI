using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUIGallery.Views;

public sealed partial class QRCodePage : Page
{
    public QRCodePage()
    {
        InitializeComponent();
        CmbError.Value = ErrorCorrectionLevel.High;
        Loaded += QRCodePage_Loaded;
    }

    private void QRCodePage_Loaded(object sender, RoutedEventArgs e)
    {
        Generate();
    }

    private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        Generate();
    }

    private void Generate()
    {
        if (string.IsNullOrEmpty(TxtInput.Text) || CmbError.Value is not ErrorCorrectionLevel level)
            return;

        var standardQr = QRCode.Create(TxtInput.Text, level);

        var png = standardQr.ToPng();
        using (var stream = new MemoryStream(png))
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream.AsRandomAccessStream());
            OutPutImg.Source = bitmapImage;
        }
    }

    private void CmbError_ValueChanged(ChoiceOption sender, object value)
    {
        Generate();
    }
}
