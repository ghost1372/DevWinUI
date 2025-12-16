namespace DevWinUIGallery.Views;

public sealed partial class ColorPalettePage : Page
{
    public ObservableCollection<ColorSetType> ColorSetItems { get; set; } = new ObservableCollection<ColorSetType>(Enum.GetValues<ColorSetType>());
    public ObservableCollection<ColorItemShape> ColorItemShapeItems { get; set; } = new ObservableCollection<ColorItemShape>(Enum.GetValues<ColorItemShape>());

    public ColorPalettePage()
    {
        InitializeComponent();
    }

    private void ColorPalette_ColorChanged(object sender, ColorPaletteColorChangedEventArgs e)
    {
        TxtResult.Text = e.Color.ToString();
    }
}
