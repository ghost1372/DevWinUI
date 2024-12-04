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
        App.Current.GetThemeService.SetBackdropTintColor(args.NewColor);
    }

    private void ColorPalette_ItemClick(object sender, ItemClickEventArgs e)
    {
        var color = e.ClickedItem as ColorPaletteItem;
        if (color != null)
        {
            if (color.Hex.Contains("#000000"))
            {
                App.Current.GetThemeService.ResetBackdropProperties();
            }
            else
            {
                App.Current.GetThemeService.SetBackdropTintColor(color.Color);
            }
            TintBox.Fill = new SolidColorBrush(color.Color);
        }
    }
}


