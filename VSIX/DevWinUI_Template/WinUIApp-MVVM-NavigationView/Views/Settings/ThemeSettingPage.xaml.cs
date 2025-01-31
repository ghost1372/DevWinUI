using Microsoft.UI.Xaml.Media;

namespace $safeprojectname$.Views;

public sealed partial class ThemeSettingPage : Page
{
    public ThemeSettingPage()
    {
        this.InitializeComponent();
    }

    private void OnColorChanged(ColorPicker sender, ColorChangedEventArgs args)
    {
        TintBox.Fill = new SolidColorBrush(args.NewColor);
        App.Current.ThemeService.SetBackdropTintColor(args.NewColor);
    }

    private void OnColorPaletteItemClick(object sender, ItemClickEventArgs e)
    {
        var color = e.ClickedItem as ColorPaletteItem;
        if (color != null)
        {
            if (color.Hex.Contains("#000000"))
            {
                App.Current.ThemeService.ResetBackdropProperties();
            }
            else
            {
                App.Current.ThemeService.SetBackdropTintColor(color.Color);
            }
            TintBox.Fill = new SolidColorBrush(color.Color);
        }
    }
}


