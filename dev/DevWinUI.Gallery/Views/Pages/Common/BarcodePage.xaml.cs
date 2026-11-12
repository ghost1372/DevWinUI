using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUIGallery.Views;

public sealed partial class BarcodePage : Page
{
    public BarcodePage()
    {
        InitializeComponent();
        CmbType.Value = BarcodeType.Code128;
        Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Generate();
    }

    private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
    {
        Generate();
    }

    private void CmbType_ValueChanged(ChoiceOption sender, object value)
    {
        Generate();
    }

    private void Generate()
    {
        if (string.IsNullOrEmpty(TxtInput?.Text) || CmbType?.Value is not BarcodeType type)
            return;

        var text = TxtInput.Text;
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
            return;

        using var stream = new MemoryStream(barcode.ToPng());
        var bitmapImage = new BitmapImage();
        bitmapImage.SetSource(stream.AsRandomAccessStream());
        OutPutImg.Source = bitmapImage;
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
