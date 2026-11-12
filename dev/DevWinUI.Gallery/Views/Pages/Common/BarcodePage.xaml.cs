using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUIGallery.Views;

public sealed partial class BarcodePage : Page
{
    public BarcodePage()
    {
        InitializeComponent();
    }

    private BitmapImage GenerateImage(string text, object value)
    {
        if (string.IsNullOrEmpty(text) || value is not BarcodeType type)
            return null;

        var barcode = type switch
        {
            BarcodeType.Code128 => Barcode.CreateCode128(text),
            BarcodeType.Code93 => Barcode.CreateCode93(text),
            BarcodeType.Code39 => Barcode.CreateCode39(text),
            BarcodeType.Ean13 => Barcode.CreateEan13(text),
            BarcodeType.Ean8 => Barcode.CreateEan8(text),
            BarcodeType.Itf => Barcode.CreateItf(text),
            BarcodeType.Codabar => Barcode.CreateCodabar(text),
            BarcodeType.UpcA => Barcode.CreateUpcA(text),
            _ => null
        };

        if (barcode is null)
            return null;

        using var stream = new MemoryStream(barcode.ToPng());
        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(stream.AsRandomAccessStream());
        return bitmapImage;
    }
}

public enum BarcodeType
{
    Code128,
    Code93,
    Code39,
    Ean13,
    Ean8,
    Itf,
    Codabar,
    UpcA
}
