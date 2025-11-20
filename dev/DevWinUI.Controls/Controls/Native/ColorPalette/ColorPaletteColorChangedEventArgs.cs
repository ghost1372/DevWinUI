namespace DevWinUI;
public class ColorPaletteColorChangedEventArgs : EventArgs
{
    public ColorPaletteItem ColorPaletteItem { get; set; }
    public Color Color { get; set; }
    public string ColorName { get; set; }
    public ColorPaletteColorChangedEventArgs(Color color, string colorName, ColorPaletteItem colorPaletteItem)
    {
        this.Color = color;
        ColorName = colorName;
        ColorPaletteItem = colorPaletteItem;
    }
}
