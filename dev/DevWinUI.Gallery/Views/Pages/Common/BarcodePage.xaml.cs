using System.Data;
using Microsoft.UI.Xaml.Media.Imaging;
using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class BarcodePage : Page
{
    public ObservableCollection<BarcodeType> BarcodeTypeItems { get; set; } = new ObservableCollection<BarcodeType>(Enum.GetValues<BarcodeType>());

    public BarcodePage()
    {
        InitializeComponent();
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

    private void Generate()
    {
        if (string.IsNullOrEmpty(TxtInput.Text))
            return;

        Barcode barcode = null;
        var item = CmbType.SelectedItem.As<BarcodeType>();
        switch (item)
        {
            case BarcodeType.Code128:
                barcode = Barcode.CreateCode128(TxtInput.Text);
                break;
            case BarcodeType.Code93:
                barcode = Barcode.CreateCode93(TxtInput.Text);
                break;
            case BarcodeType.Code39:
                barcode = Barcode.CreateCode39(TxtInput.Text);
                break;
            case BarcodeType.Ean13:
                barcode = Barcode.CreateEan13(TxtInput.Text);
                break;
            case BarcodeType.Ean8:
                barcode = Barcode.CreateEan8(TxtInput.Text);
                break;
            case BarcodeType.Itf:
                barcode = Barcode.CreateItf(TxtInput.Text);
                break;
            case BarcodeType.Codabar:
                barcode = Barcode.CreateCodabar(TxtInput.Text);
                break;
            case BarcodeType.UpcA:
                barcode = Barcode.CreateUpcA(TxtInput.Text);
                break;
        }

        var png = barcode.ToPng();
        using (var stream = new MemoryStream(png))
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream.AsRandomAccessStream());
            OutPutImg.Source = bitmapImage;
        }
    }

    private void CmbType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Generate();
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
