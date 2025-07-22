using Windows.UI;

namespace DevWinUIGallery.Views;

public sealed partial class ThemeSettingPage : Page
{
    public ThemeSettingPage()
    {
        this.InitializeComponent();
    }

    private void OnColorPaletteColorChanged(object sender, ColorPaletteColorChangedEventArgs e)
    {
        SetTintColor(e.Color);

        MainDropdownColorPicker.Color = e.Color;
    }

    private void MainDropdownColorPicker_ColorChanged(object sender, DropdownColorPickerColorChangedEventArgs e)
    {
        SetTintColor(e.Color);
    }

    private void SetTintColor(Color color)
    {
        if (color.ToString().Contains("#FF000000") || color.ToString().Contains("#000000"))
        {
            App.Current.ThemeService.ResetBackdropProperties();
        }
        else
        {
            App.Current.ThemeService.SetBackdropTintColor(color);
        }
    }
}


