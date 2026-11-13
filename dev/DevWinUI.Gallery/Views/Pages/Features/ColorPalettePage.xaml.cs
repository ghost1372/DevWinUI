namespace DevWinUIGallery.Views;

public sealed partial class ColorPalettePage : Page
{
    public ColorPalettePage()
    {
        InitializeComponent();
    }

    private void ColorPalette_ColorChanged(object sender, ColorPaletteColorChangedEventArgs e)
    {
        TxtResult.Text = e.Color.ToString();
    }
}
