namespace DevWinUI;
public class DropdownColorPickerColorChangedEventArgs : EventArgs
{
    public ColorPaletteItem ColorPaletteItem { get; set; }
    public Color Color { get; set; }
    public string ColorName { get; set; }
    public DropdownColorPickerColorChangedEventArgs(Color color, string colorName, ColorPaletteItem colorPaletteItem)
    {
        this.Color = color;
        ColorName = colorName;
        ColorPaletteItem = colorPaletteItem;
    }
}
