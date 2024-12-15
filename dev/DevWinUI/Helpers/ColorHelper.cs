namespace DevWinUI;
public static partial class ColorHelper
{
    public static Color GetColorFromHex(string hexColor)
    {
        hexColor = hexColor.Replace("#", string.Empty);
        byte a = 255;
        byte r = 0;
        byte g = 0;
        byte b = 0;

        if (hexColor.Length == 8)
        {
            a = Convert.ToByte(hexColor.Substring(0, 2), 16);
            r = Convert.ToByte(hexColor.Substring(2, 2), 16);
            g = Convert.ToByte(hexColor.Substring(4, 2), 16);
            b = Convert.ToByte(hexColor.Substring(6, 2), 16);
        }
        else if (hexColor.Length == 6)
        {
            r = Convert.ToByte(hexColor.Substring(0, 2), 16);
            g = Convert.ToByte(hexColor.Substring(2, 2), 16);
            b = Convert.ToByte(hexColor.Substring(4, 2), 16);
        }

        return Color.FromArgb(a, r, g, b);
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
