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
    internal readonly ObservableCollection<ColorPaletteItem> ShadeColors = new()
    {
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x5F,  G = 0x16, B = 0x16  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x7F,  G = 0x1D, B = 0x1D  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x99,  G = 0x1B, B = 0x1B  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xB9,  G = 0x1C, B = 0x1C  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xEF,  G = 0x10, B = 0x10  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xEF,  G = 0x34, B = 0x34  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF8,  G = 0x71, B = 0x71  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFC,  G = 0xA5, B = 0xA5  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFE,  G = 0xCA, B = 0xCA  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFE,  G = 0xE2, B = 0xE2  }, ColorName = "Red" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x58,  G = 0x20, B = 0x0D  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x7C,  G = 0x2D, B = 0x12  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x9A,  G = 0x34, B = 0x12  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xC2,  G = 0x41, B = 0x0C  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xEA,  G = 0x58, B = 0x0C  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF9,  G = 0x73, B = 0x16  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFB,  G = 0x92, B = 0x3C  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFD,  G = 0xBA, B = 0x74  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFE,  G = 0xD7, B = 0xAA  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFF,  G = 0xED, B = 0xD5  }, ColorName = "Orange" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x5A,  G = 0x27, B = 0x0B  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x78,  G = 0x35, B = 0x0F  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x92,  G = 0x40, B = 0x0E  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xB4,  G = 0x53, B = 0x09  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xD9,  G = 0x77, B = 0x06  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF5,  G = 0x9E, B = 0x0B  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFB,  G = 0xBF, B = 0x24  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFC,  G = 0xD3, B = 0x4D  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFD,  G = 0xE6, B = 0x8A  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFE,  G = 0xF3, B = 0xC7  }, ColorName = "Amber" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x53,  G = 0x2E, B = 0x0D  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x71,  G = 0x3F, B = 0x12  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x85,  G = 0x4D, B = 0x0E  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA1,  G = 0x62, B = 0x07  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xCA,  G = 0x8A, B = 0x04  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xEA,  G = 0xB3, B = 0x08  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFA,  G = 0xCC, B = 0x15  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFD,  G = 0xE0, B = 0x47  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFE,  G = 0xF0, B = 0x8A  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFE,  G = 0xF9, B = 0xC3  }, ColorName = "Yellow" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x23,  G = 0x35, B = 0x0C  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x36,  G = 0x53, B = 0x14  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x3F,  G = 0x62, B = 0x12  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x4D,  G = 0x7C, B = 0x0F  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x65,  G = 0xA3, B = 0x0D  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x84,  G = 0xCC, B = 0x16  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA3,  G = 0xE6, B = 0x35  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xBE,  G = 0xF2, B = 0x64  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xD9,  G = 0xF9, B = 0x9D  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xEC,  G = 0xFC, B = 0xCB  }, ColorName = "Lime" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0E,  G = 0x3D, B = 0x20  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x14,  G = 0x53, B = 0x2D  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x16,  G = 0x65, B = 0x34  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x15,  G = 0x80, B = 0x3D  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x16,  G = 0xA3, B = 0x4A  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x22,  G = 0xC5, B = 0x5E  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x4A,  G = 0xDE, B = 0x80  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x86,  G = 0xEF, B = 0xAC  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xBB,  G = 0xF7, B = 0xD0  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xDC,  G = 0xFC, B = 0xE7  }, ColorName = "Green" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x04,  G = 0x3D, B = 0x2E  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x06,  G = 0x4E, B = 0x3B  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x06,  G = 0x5F, B = 0x46  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x04,  G = 0x78, B = 0x57  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x05,  G = 0x96, B = 0x69  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x10,  G = 0xB9, B = 0x81  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x34,  G = 0xD3, B = 0x99  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x6E,  G = 0xE7, B = 0xB7  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA7,  G = 0xF3, B = 0xD0  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xD1,  G = 0xFA, B = 0xE5  }, ColorName = "Emerald" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0F,  G = 0x3D, B = 0x39  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x13,  G = 0x4E, B = 0x4A  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x11,  G = 0x5E, B = 0x59  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0F,  G = 0x76, B = 0x6E  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0D,  G = 0x94, B = 0x88  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x14,  G = 0xB8, B = 0xA6  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x2D,  G = 0xD4, B = 0xBF  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x5E,  G = 0xEA, B = 0xD4  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x99,  G = 0xF6, B = 0xE4  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xCC,  G = 0xFB, B = 0xF1  }, ColorName = "Teal" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x12,  G = 0x41, B = 0x53  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x16,  G = 0x4E, B = 0x63  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x15,  G = 0x5E, B = 0x75  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0E,  G = 0x74, B = 0x90  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x08,  G = 0x91, B = 0xB2  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x06,  G = 0xB6, B = 0xD4  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x22,  G = 0xD3, B = 0xEE  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x67,  G = 0xE8, B = 0xF9  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA5,  G = 0xF3, B = 0xFC  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xCF,  G = 0xFA, B = 0xFE  }, ColorName = "Cyan" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0A,  G = 0x3D, B = 0x5B  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0C,  G = 0x4A, B = 0x6E  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x07,  G = 0x59, B = 0x85  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x03,  G = 0x69, B = 0xA1  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x02,  G = 0x84, B = 0xC7  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0E,  G = 0xA5, B = 0xE9  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x38,  G = 0xBD, B = 0xF8  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x7D,  G = 0xD3, B = 0xFC  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xBA,  G = 0xE6, B = 0xFD  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xE0,  G = 0xF2, B = 0xFE  }, ColorName = "Light Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x15,  G = 0x29, B = 0x60  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x1E,  G = 0x3A, B = 0x8A  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x1E,  G = 0x40, B = 0xAF  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x1D,  G = 0x4E, B = 0xD8  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x25,  G = 0x63, B = 0xEB  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x3B,  G = 0x82, B = 0xF6  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x60,  G = 0xA5, B = 0xFA  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x93,  G = 0xC5, B = 0xFD  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xBF,  G = 0xDB, B = 0xFE  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xDB,  G = 0xEA, B = 0xFE  }, ColorName = "Blue" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x22,  G = 0x20, B = 0x59  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x31,  G = 0x2E, B = 0x81  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x37,  G = 0x30, B = 0xA3  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x43,  G = 0x38, B = 0xCA  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x4F,  G = 0x46, B = 0xE5  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x63,  G = 0x66, B = 0xF1  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x81,  G = 0x8C, B = 0xF8  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA5,  G = 0xB4, B = 0xFC  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xC7,  G = 0xD2, B = 0xFE  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xE0,  G = 0xE7, B = 0xFF  }, ColorName = "Indigo" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x31,  G = 0x13, B = 0x61  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x4C,  G = 0x1D, B = 0x95  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x5B,  G = 0x21, B = 0xB6  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x6D,  G = 0x28, B = 0xD9  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x7C,  G = 0x3A, B = 0xED  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x8B,  G = 0x5C, B = 0xF6  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA7,  G = 0x8B, B = 0xFA  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xC4,  G = 0xB5, B = 0xFD  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xDD,  G = 0xD6, B = 0xFE  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xED,  G = 0xE9, B = 0xFE  }, ColorName = "Violet" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x36,  G = 0x11, B = 0x54  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x58,  G = 0x1C, B = 0x87  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x6B,  G = 0x21, B = 0xA8  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x7E,  G = 0x22, B = 0xCE  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x93,  G = 0x33, B = 0xEA  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA8,  G = 0x55, B = 0xF7  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xC0,  G = 0x84, B = 0xFC  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xD8,  G = 0xB4, B = 0xFE  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xE9,  G = 0xD5, B = 0xFF  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF3,  G = 0xE8, B = 0xFF  }, ColorName = "Purple" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x46,  G = 0x10, B = 0x4A  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x70,  G = 0x1A, B = 0x75  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x86,  G = 0x19, B = 0x8F  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA2,  G = 0x1C, B = 0xAF  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xC0,  G = 0x26, B = 0xD3  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xD9,  G = 0x46, B = 0xEF  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xE8,  G = 0x79, B = 0xF9  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF0,  G = 0xAB, B = 0xFC  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF5,  G = 0xD0, B = 0xFE  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFA,  G = 0xE8, B = 0xFF  }, ColorName = "Fuchsia" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x5E,  G = 0x11, B = 0x31  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x83,  G = 0x18, B = 0x43  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x9D,  G = 0x17, B = 0x4D  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xBE,  G = 0x18, B = 0x5D  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xDB,  G = 0x27, B = 0x77  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xEC,  G = 0x48, B = 0x99  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF4,  G = 0x72, B = 0xB6  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF9,  G = 0xA8, B = 0xD4  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFB,  G = 0xCF, B = 0xE8  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFC,  G = 0xE7, B = 0xF3  }, ColorName = "Pink" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x55,  G = 0x0B, B = 0x22  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x88,  G = 0x13, B = 0x37  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x9F,  G = 0x12, B = 0x39  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xBE,  G = 0x12, B = 0x3C  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xE1,  G = 0x1D, B = 0x48  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF4,  G = 0x3F, B = 0x5E  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFB,  G = 0x71, B = 0x85  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFD,  G = 0xA4, B = 0xAF  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFE,  G = 0xCD, B = 0xD3  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFF,  G = 0xE4, B = 0xE6  }, ColorName = "Rose" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x09,  G = 0x0E, B = 0x1A  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x0F,  G = 0x17, B = 0x2A  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x1E,  G = 0x29, B = 0x3B  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x33,  G = 0x41, B = 0x55  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x47,  G = 0x55, B = 0x69  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x64,  G = 0x74, B = 0x8B  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x94,  G = 0xA3, B = 0xB8  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xCB,  G = 0xD5, B = 0xE1  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xE2,  G = 0xE8, B = 0xF0  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xF1,  G = 0xF5, B = 0xF9  }, ColorName = "Blue Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x00,  G = 0x00, B = 0x00  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x17,  G = 0x17, B = 0x17  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x26,  G = 0x26, B = 0x26  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x40,  G = 0x40, B = 0x40  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x52,  G = 0x52, B = 0x52  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0x73,  G = 0x73, B = 0x73  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xA3,  G = 0xA3, B = 0xA3  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xD4,  G = 0xD4, B = 0xD4  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xE5,  G = 0xE5, B = 0xE5  }, ColorName = "Gray" },
        new ColorPaletteItem { Color = new() { A = 0xFF, R = 0xFF,  G = 0xFF, B = 0xFF }, ColorName = "Gray" },
        new ColorPaletteItem { Color = ColorHelper.GetColorFromHex("#00000000"), ColorName = "Transparent"}
    };

    public ColorPalette()
    {
        DefaultStyleKey = typeof(ColorPalette);
        if (Colors == null)
        {
            Colors = new ObservableCollection<ColorPaletteItem>();
        }
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
