namespace DevWinUI;

public class DropdownColorPickerColorChangedEventArgs : EventArgs
{
    public ColorPaletteItem ColorPaletteItem { get; }
    public Color Color { get; }
    public string ColorName { get; }

    public DropdownColorPickerColorChangedEventArgs(Color color, string colorName, ColorPaletteItem colorPaletteItem)
    {
        Color = color;
        ColorName = colorName;
        ColorPaletteItem = colorPaletteItem;
    }
}
