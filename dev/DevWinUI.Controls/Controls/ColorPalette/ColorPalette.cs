using Windows.ApplicationModel.DataTransfer;

namespace DevWinUI;
public partial class ColorPalette : GridView
{
    public event EventHandler<ColorPaletteColorChangedEventArgs> ColorChanged;
    private ObservableCollection<ColorPaletteItem> internalColors;

    internal readonly ObservableCollection<ColorPaletteItem> BasicColors = new()
    {
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#00000000"), ColorName = "Transparent"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF000000"), ColorName = "Black"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFf44336"), ColorName = "Red"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFe91e63"), ColorName = "Pink"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF9c27b0"), ColorName = "Purple"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF673ab7"), ColorName = "Deep Purple"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF3f51b5"), ColorName = "Indigo"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF2196f3"), ColorName = "Blue"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF03a9f4"), ColorName = "Light Blue"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF00bcd4"), ColorName = "Cyan"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF009688"), ColorName = "Teal"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF4caf50"), ColorName = "Green"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF8bc34a"), ColorName = "Light Green"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFcddc39"), ColorName = "Lime"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFffeb3b"), ColorName = "Yellow"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFffc107"), ColorName = "Amber"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFff9800"), ColorName = "Orange"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFff5722"), ColorName = "Deep Orange"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF795548"), ColorName = "Brown"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF9e9e9e"), ColorName = "Grey"}
    };

    internal readonly ObservableCollection<ColorPaletteItem> ExtendedColors = new()
    {
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#00000000"), ColorName = "Transparent"},
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF000000"), ColorName = "Black"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 255, 185, 0), ColorName = "Amber"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 255, 140, 0), ColorName = "Dark Orange"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 247, 99, 12), ColorName = "Orange Red"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 202, 80, 16), ColorName = "Pumpkin"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 218, 59, 1), ColorName = "Dark Orange Red"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 239, 105, 80), ColorName = "Coral"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 209, 52, 56), ColorName = "Fire Brick"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 255, 67, 67), ColorName = "Red"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 231, 72, 86), ColorName = "Crimson"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 232, 17, 35), ColorName = "Scarlet"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 234, 0, 94), ColorName = "Rose Pink"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 195, 0, 82), ColorName = "Dark Magenta"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 227, 0, 140), ColorName = "Magenta"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 191, 0, 119), ColorName = "Deep Pink"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 194, 57, 179), ColorName = "Orchid"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 154, 0, 137), ColorName = "Purple"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 0, 120, 212), ColorName = "Royal Blue"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 0, 99, 177), ColorName = "Medium Blue"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 142, 140, 216), ColorName = "Light Slate Blue"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 107, 105, 214), ColorName = "Slate Blue"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 135, 100, 184), ColorName = "Medium Purple"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 116, 77, 169), ColorName = "Rebecca Purple"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 177, 70, 194), ColorName = "Medium Orchid"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 136, 23, 152), ColorName = "Dark Violet"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 0, 153, 188), ColorName = "Deep Sky Blue"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 45, 125, 154), ColorName = "Steel Blue"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 0, 183, 195), ColorName = "Cyan"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 3, 131, 135), ColorName = "Teal"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 0, 178, 148), ColorName = "Medium Turquoise"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 1, 133, 116), ColorName = "Dark Cyan"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 0, 204, 106), ColorName = "Medium Sea Green"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 16, 137, 62), ColorName = "Forest Green"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 122, 117, 116), ColorName = "Dim Gray"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 93, 90, 88), ColorName = "Slate Gray"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 104, 118, 138), ColorName = "Cadet Blue"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 81, 92, 107), ColorName = "Light Slate Gray"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 86, 124, 115), ColorName = "Dark Sea Green"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 72, 104, 96), ColorName = "Sea Green"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 73, 130, 5), ColorName = "Olive Drab"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 16, 124, 16), ColorName = "Green"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 118, 118, 118), ColorName = "Gray"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 76, 74, 72), ColorName = "Dark Slate Gray"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 105, 121, 126), ColorName = "Light Steel Blue"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 74, 84, 89), ColorName = "Slate Gray"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 100, 124, 100), ColorName = "Dark Olive Green"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 82, 94, 84), ColorName = "Olive Green"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 132, 117, 69), ColorName = "Tan"},
        new ColorPaletteItem { Color = Windows.UI.Color.FromArgb(255, 126, 115, 95), ColorName = "Saddle Brown" }
    };

    internal readonly ObservableCollection<ColorPaletteItem> ShadeColors = new ObservableCollection<ColorPaletteItem>(
        GenerateAllColorShades()
    );
    private static IEnumerable<ColorPaletteItem> GenerateAllColorShades()
    {
        var baseColors = new ObservableCollection<ColorPaletteItem>
        {
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFB71C1C"), ColorName = "Red" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF880E4F"), ColorName = "Pink" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF4A148C"), ColorName = "Purple" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF0D47A1"), ColorName = "Blue" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF006064"), ColorName = "Cyan" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF004D40"), ColorName = "Teal" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF1B5E20"), ColorName = "Green" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF827717"), ColorName = "Lime" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFF57F17"), ColorName = "Yellow" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FFE65100"), ColorName = "Orange" },
            new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#FF3E2723"), ColorName = "Brown" },
            new ColorPaletteItem { Color = Microsoft.UI.Colors.Gray, ColorName = "Gray" },
        };

        foreach (var pair in baseColors)
        {
            foreach (var shade in GenerateShades(pair.ColorName, pair.Color, 10))
            {
                yield return shade;
            }
        }
        yield return new ColorPaletteItem
        {
            Color = Microsoft.UI.Colors.Black,
            ColorName = "Black"
        };
        yield return new ColorPaletteItem
        {
            Color = Microsoft.UI.Colors.Transparent,
            ColorName = "Transparent"
        };
    }
    private static List<ColorPaletteItem> GenerateShades(string baseColorName, Color baseColor, int count = 10)
    {
        var list = new List<ColorPaletteItem>();

        for (int i = 0; i < count; i++)
        {
            double lightness = 0.0 + (i * (1.0 / (count - 1)));
            var shadeColor = ColorHelper.LightenColor(baseColor, (float)lightness);
            list.Add(new ColorPaletteItem
            {
                Color = shadeColor,
                ColorName = $"{baseColorName}-{(i + 1) * 20}" // e.g. Green-100, Green-200
            });
        }

        return list;
    }
    public ColorPalette()
    {
        DefaultStyleKey = typeof(ColorPalette);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ItemClick -= OnItemClick;
        ItemClick += OnItemClick;

        SelectionChanged -= OnColorPaletteSelectionChanged;
        SelectionChanged += OnColorPaletteSelectionChanged;

        UpdateItemsSource();
        UpdateSelectedColor();
    }

    private void OnColorPaletteSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SelectedItem is ColorPaletteItem item && item != null)
        {
            ColorChanged?.Invoke(this, new ColorPaletteColorChangedEventArgs(item.Color, item.ColorName, item));
            if (!SelectedColor.Equals(item.Color))
            {
                SelectedColor = item.Color;
            }
        }
    }

    private void UpdateShowHexCode()
    {
        foreach (ColorPaletteItem item in Items)
        {
            item.ShowHexCode = ShowHexCode;
        }
    }
    private void UpdateShowColorName()
    {
        foreach (ColorPaletteItem item in Items)
        {
            item.ShowColorName = ShowColorName;
        }
    }

    private void UpdateToolTip()
    {
        foreach (ColorPaletteItem item in Items)
        {
            item.ShowToolTip = ShowToolTip;
        }
    }

    private void UpdateItemsSource()
    {
        switch (ColorSet)
        {
            case ColorSetType.Custom:
                ItemsSource = Colors;
                internalColors = new(Colors);
                break;
            case ColorSetType.Basic:
                ItemsSource = BasicColors;
                internalColors = new(BasicColors);
                break;
            case ColorSetType.Extended:
                ItemsSource = ExtendedColors;
                internalColors = new(ExtendedColors);
                break;
            case ColorSetType.Shades:
                ItemsSource = ShadeColors;
                internalColors = new(ShadeColors);
                break;
        }

        UpdateItemTemplateAndSize();
        UpdateInternalColors();
        UpdateShowColorName();
        UpdateShowHexCode();
        UpdateToolTip();
    }
    private void UpdateInternalColors()
    {
        foreach (ColorPaletteItem item in Items)
        {
            switch (ColorSet)
            {
                case ColorSetType.Custom:
                    item.InternalColors = Colors;
                    break;
                case ColorSetType.Basic:
                    item.InternalColors = BasicColors;
                    break;
                case ColorSetType.Extended:
                    item.InternalColors = ExtendedColors;
                    break;
                case ColorSetType.Shades:
                    item.InternalColors = ShadeColors;
                    break;
            }
        }
    }
    private void OnItemClick(object sender, ItemClickEventArgs e)
    {
        if (IsCopyColorCodeOnClickEnabled)
        {
            if (SelectedItem is ColorPaletteItem color)
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(color.Color.ToString());
                Clipboard.SetContent(dataPackage);
                Clipboard.Flush();
            }
        }
    }

    protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
    {
        base.PrepareContainerForItemOverride(element, item);
        if (element is GridViewItem gridViewItem && item is ColorPaletteItem colorPaletteItem2)
        {
            gridViewItem.IsEnabled = colorPaletteItem2.IsEnabled;
            gridViewItem.MinHeight = 0;
            gridViewItem.MinWidth = 0;
        }
    }

    private void UpdateItemTemplateAndSize()
    {
        var width = ItemWidth;
        var height = ItemHeight;
        var minHeight = 0.0;
        var minWidth = 0.0;
        if (ItemShape == ColorItemShape.Tab)
        {
            minHeight = 100;
            minWidth = 120;
        }
        else
        {
            minHeight = 0;
            minWidth = 0;
        }
        foreach (ColorPaletteItem item in Items)
        {
            item.MinHeight = minHeight;
            item.MinWidth = minWidth;
            item.Width = width;
            item.Height = height;
            item.ItemShape = ItemShape;
        }
    }
    private void UpdateSelectedColor()
    {
        if (internalColors == null)
            return;

        var foundItem = internalColors.FirstOrDefault(c => c.Color.Equals(SelectedColor));
        if (foundItem != null && !object.Equals(SelectedItem, foundItem))
        {
            SelectedItem = foundItem;
        }
    }
}
