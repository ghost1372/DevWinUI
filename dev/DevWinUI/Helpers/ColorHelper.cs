namespace DevWinUI;
public static partial class ColorHelper
{
    /// <summary>
    /// Converts a hexadecimal color string into a Color object, supporting both 6 and 8 character formats.
    /// </summary>
    /// <param name="hexColor">The hexadecimal string representation of a color, which may include an alpha channel.</param>
    /// <returns>A Color object representing the specified hexadecimal color.</returns>
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

    /// <summary>
    /// Converts a color represented in RGB format to an unsigned integer value.
    /// </summary>
    /// <param name="color">The input color is represented in terms of its red, green, and blue components.</param>
    /// <returns>An unsigned integer that encodes the color in a specific bit format.</returns>
    public static uint ColorToUInt(Color color)
    {
        return (uint)((color.B << 16) | (color.G << 8) | (color.R << 0));
    }

    /// <summary>
    /// Converts a color object into its hexadecimal string representation.
    /// </summary>
    /// <param name="color">The color object containing alpha, red, green, and blue components.</param>
    /// <returns>A string formatted as a hexadecimal color code.</returns>
    public static string GetHexFromColor(Color color)
    {
        return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}",
                     color.A,
                     color.R,
                     color.G,
                     color.B);
    }

    /// <summary>
    /// Creates a SolidColorBrush from a hexadecimal color string.
    /// </summary>
    /// <param name="hex">The hexadecimal string represents a color, which may include an alpha channel.</param>
    /// <returns>A SolidColorBrush object representing the specified color.</returns>
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

    /// <summary>
    /// Tints the color by the given percent.
    /// </summary>
    /// <param name="color">The color being tinted.</param>
    /// <param name="percent">The percent to tint. Ex: 0.1 will make the color 10% lighter.</param>
    /// <returns>The new tinted color.</returns>
    public static Color LightenColor(Color color, float percent)
    {
        var lighting = GetBrightnessFromColor(color);
        lighting = lighting + lighting * percent;
        if (lighting > 1.0)
        {
            lighting = 1;
        }
        else if (lighting <= 0)
        {
            lighting = 0.1f;
        }
        var tintedColor = GetColorFromHsl(color.A, GetHueFromColor(color), GetSaturationFromColor(color), lighting);

        return tintedColor;
    }

    /// <summary>
    /// Tints the color by the given percent.
    /// </summary>
    /// <param name="color">The color being tinted.</param>
    /// <param name="percent">The percent to tint. Ex: 0.1 will make the color 10% darker.</param>
    /// <returns>The new tinted color.</returns>
    public static Color DarkenColor(Color color, float percent)
    {
        var lighting = GetBrightnessFromColor(color);
        lighting = lighting - lighting * percent;
        if (lighting > 1.0)
        {
            lighting = 1;
        }
        else if (lighting <= 0)
        {
            lighting = 0;
        }
        var tintedColor = GetColorFromHsl(color.A, GetHueFromColor(color), GetSaturationFromColor(color), lighting);

        return tintedColor;
    }

    /// <summary>
    /// Converts the HSL values to a Color.
    /// </summary>
    /// <param name="alpha">The alpha.</param>
    /// <param name="hue">The hue.</param>
    /// <param name="saturation">The saturation.</param>
    /// <param name="lighting">The lighting.</param>
    /// <returns></returns>
    public static Color GetColorFromHsl(byte alpha, float hue, float saturation, float lighting)
    {
        //if (alpha is < 0 or > 255) {
        //    throw new ArgumentOutOfRangeException("alpha");
        //}
        if (hue is < 0f or > 360f)
        {
            throw new ArgumentOutOfRangeException("hue");
        }
        if (saturation is < 0f or > 1f)
        {
            throw new ArgumentOutOfRangeException("saturation");
        }
        if (lighting is < 0f or > 1f)
        {
            throw new ArgumentOutOfRangeException("lighting");
        }

        if (0 == saturation)
        {
            return Color.FromArgb(alpha, Convert.ToByte(lighting * 255), Convert.ToByte(lighting * 255), Convert.ToByte(lighting * 255));
        }

        float fMax, fMid, fMin;
        int iSextant;

        if (0.5 < lighting)
        {
            fMax = lighting - (lighting * saturation) + saturation;
            fMin = lighting + (lighting * saturation) - saturation;
        }
        else
        {
            fMax = lighting + (lighting * saturation);
            fMin = lighting - (lighting * saturation);
        }

        iSextant = (int)Math.Floor(hue / 60f);
        if (300f <= hue)
        {
            hue -= 360f;
        }
        hue /= 60f;
        hue -= 2f * (float)Math.Floor(((iSextant + 1f) % 6f) / 2f);
        fMid = 0 == iSextant % 2
            ? hue * (fMax - fMin) + fMin
            : fMin - hue * (fMax - fMin);

        byte bMax = Convert.ToByte(fMax * 255);
        byte bMid = Convert.ToByte(fMid * 255);
        byte bMin = Convert.ToByte(fMin * 255);

        switch (iSextant)
        {
            case 1:
                return Color.FromArgb(alpha, bMid, bMax, bMin);
            case 2:
                return Color.FromArgb(alpha, bMin, bMax, bMid);
            case 3:
                return Color.FromArgb(alpha, bMin, bMid, bMax);
            case 4:
                return Color.FromArgb(alpha, bMid, bMin, bMax);
            case 5:
                return Color.FromArgb(alpha, bMax, bMin, bMid);
            default:
                return Color.FromArgb(alpha, bMax, bMid, bMin);
        }
    }

    /// <summary>
    /// Gets the brightness of the color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The brightness value.</returns>
    public static float GetBrightnessFromColor(Color color)
    {
        return (color.R * 0.299f + color.G * 0.587f + color.B * 0.114f) / 255f;
    }

    /// <summary>
    /// Gets the hue of the color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The hue value.</returns>
    public static float GetHueFromColor(Color color)
    {
        float r = color.R / 255f;
        float g = color.G / 255f;
        float b = color.B / 255f;

        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));

        float hue = 0f;

        if (max == min)
        {
            hue = 0f;
        }
        else if (max == r)
        {
            hue = (60f * ((g - b) / (max - min)) + 360f) % 360f;
        }
        else if (max == g)
        {
            hue = (60f * ((b - r) / (max - min)) + 120f) % 360f;
        }
        else if (max == b)
        {
            hue = (60f * ((r - g) / (max - min)) + 240f) % 360f;
        }

        return hue;
    }

    /// <summary>
    /// Gets the saturation of the color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The saturation value.</returns>
    public static float GetSaturationFromColor(Color color)
    {
        float r = color.R / 255f;
        float g = color.G / 255f;
        float b = color.B / 255f;

        float max = Math.Max(r, Math.Max(g, b));
        float min = Math.Min(r, Math.Min(g, b));

        float saturation = 0f;

        if (max != 0)
        {
            saturation = (max - min) / max;
        }

        return saturation;
    }

    /// <summary>
    /// Finds the best contrasting color (black or white)
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public static Color ContrastColorBlackWhite(Color color)
    {
        double luma = ((0.299 * color.R) + (0.587 * color.G) + (0.114 * color.B)) / (double)255;
        return luma > 0.5 ? Colors.Black : Colors.White;
    }
}
