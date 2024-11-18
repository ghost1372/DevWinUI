namespace DevWinUI;
public static partial class ColorHelper
{
    public static Color GetColorFromHex(string hexColor)
    {
        if (hexColor.Length == 7) // 6-digit hex color
        {
            hexColor = hexColor.Insert(1, "FF"); // insert FF as alpha value
        }
        return
            Color.FromArgb(
              Convert.ToByte(hexColor.Substring(1, 2), 16),
                Convert.ToByte(hexColor.Substring(3, 2), 16),
                Convert.ToByte(hexColor.Substring(5, 2), 16),
                Convert.ToByte(hexColor.Substring(7, 2), 16)
            );
    }
    public static uint ColorToUInt(Color color)
    {
        return (uint)((color.B << 16) | (color.G << 8) | (color.R << 0));
    }

    public static string GetHexFromColor(Color color)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}",
                     color.A,
                     color.R,
                     color.G,
                     color.B);
    }

    public static SolidColorBrush GetSolidColorBrush(string hex)
    {
        hex = hex.Replace("#", string.Empty);

        byte a = 255;
        int index = 0;

        if (hex.Length == 8)
        {
            a = (byte)(Convert.ToUInt32(hex.Substring(index, 2), 16));
            index += 2;
        }

        byte r = (byte)(Convert.ToUInt32(hex.Substring(index, 2), 16));
        index += 2;
        byte g = (byte)(Convert.ToUInt32(hex.Substring(index, 2), 16));
        index += 2;
        byte b = (byte)(Convert.ToUInt32(hex.Substring(index, 2), 16));
        SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(a, r, g, b));
        return myBrush;
    }
}
