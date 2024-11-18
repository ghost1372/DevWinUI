using System.Collections.ObjectModel;
using DevWinUIGallery.Models;

namespace DevWinUIGallery.Views;
public sealed partial class StylesPage : Page
{
    public ObservableCollection<ColorFormatModel> ColorFormats { get; set; } = new ObservableCollection<ColorFormatModel>();
    public StylesPage()
    {
        this.InitializeComponent();

        var hexFormatName = "HEX";
        var rgbFormatName = "RGB";
        var hslFormatName = "HSL";
        var hsvFormatName = "HSV";
        var cmykFormatName = "CMYK";
        var hsbFormatName = "HSB";
        var hsiFormatName = "HSI";
        var hwbFormatName = "HWB";
        var ncolFormatName = "NCol";

        ColorFormats.Add(new ColorFormatModel(hexFormatName, "#EF68FF", true));
        ColorFormats.Add(new ColorFormatModel(rgbFormatName, "rgb(239, 104, 255)", true));
        ColorFormats.Add(new ColorFormatModel(hslFormatName, "hsl(294, 100%, 70%)", true));
        ColorFormats.Add(new ColorFormatModel(hsvFormatName, "hsv(294, 59%, 100%)", true));
        ColorFormats.Add(new ColorFormatModel(cmykFormatName, "cmyk(6%, 59%, 0%, 0%)", true));
        ColorFormats.Add(new ColorFormatModel(hsbFormatName, "hsb(100, 50%, 75%)", true));
        ColorFormats.Add(new ColorFormatModel(hsiFormatName, "hsi(100, 50%, 75%)", true));
        ColorFormats.Add(new ColorFormatModel(hwbFormatName, "hwb(100, 50%, 75%)", true));
        ColorFormats.Add(new ColorFormatModel(ncolFormatName, "R10, 50%, 75%", true));
    }
}
