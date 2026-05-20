using System.Globalization;

namespace DevWinUI;

internal static partial class SVGColorHelper
{
    public static string ToCssString(this Color color)
    {
        if (color.A is byte.MaxValue)
        {
            return $"#{color.R:x2}{color.G:x2}{color.B:x2}";
        }

        var alpha = (color.A / 255d).ToString("0.###", CultureInfo.InvariantCulture);
        return $"rgba({color.R},{color.G},{color.B},{alpha})";
    }
}
