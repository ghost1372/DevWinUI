namespace DevWinUIGallery.Views;

public sealed partial class ColorPalettePage : Page
{
    public BaseViewModel ViewModel { get; }
    public ColorPalettePage()
    {
        ViewModel = App.GetService<BaseViewModel>();
        InitializeComponent();
    }

    private void ColorPalette_ColorChanged(object sender, ColorPaletteColorChangedEventArgs e)
    {
        TxtResult.Text = e.Color.ToString();
    }
}
