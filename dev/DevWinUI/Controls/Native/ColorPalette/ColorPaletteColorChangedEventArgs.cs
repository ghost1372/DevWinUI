namespace DevWinUI;

public class ColorPaletteColorChangedEventArgs : EventArgs
{
    public ColorPaletteItem ColorPaletteItem { get; }
    public Color Color { get; }
    public string ColorName { get; }
    public ColorPaletteColorChangedEventArgs(Color color, string colorName, ColorPaletteItem colorPaletteItem)
    {
        this.Color = color;
        ColorName = colorName;
        ColorPaletteItem = colorPaletteItem;
    }
}
