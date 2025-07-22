using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery.Views;

public sealed partial class ThemeSettingPage : Page
{
    public ThemeSettingPage()
    {
        this.InitializeComponent();
    }
    private void ColorPicker_ColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        TintBox.Fill = new SolidColorBrush(args.NewColor);
        App.Current.ThemeService.SetBackdropTintColor(args.NewColor);
    }

    private void OnColorPaletteColorChanged(object sender, ColorPaletteColorChangedEventArgs e)
    {
        if (e.Color.ToString().Contains("#FF000000") || e.Color.ToString().Contains("#000000"))
        {
            App.Current.ThemeService.ResetBackdropProperties();
        }
        else
        {
            App.Current.ThemeService.SetBackdropTintColor(e.Color);
        }

        TintBox.Fill = new SolidColorBrush(e.Color);
    }
}


