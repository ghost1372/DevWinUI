using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using WinRT;

namespace DevWinUIGallery.Views;

public sealed partial class QRCodePage : Page
{
    public ObservableCollection<ErrorCorrectionLevel> ErrorCorrectionLevelItems { get; set; } = new ObservableCollection<ErrorCorrectionLevel>(Enum.GetValues<ErrorCorrectionLevel>());

    public QRCodePage()
    {
        InitializeComponent();
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
        if (string.IsNullOrEmpty(TxtInput.Text))
            return;

        var level = CmbError.SelectedItem.As<ErrorCorrectionLevel>();
        var standardQr = QRCode.Create(TxtInput.Text, level);

        var png = standardQr.ToPng();
        using (var stream = new MemoryStream(png))
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream.AsRandomAccessStream());
            OutPutImg.Source = bitmapImage;
        }
    }

    private void CmbError_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Generate();
    }
}
