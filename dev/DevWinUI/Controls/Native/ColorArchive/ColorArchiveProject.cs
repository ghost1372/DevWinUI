using System.Globalization;

namespace DevWinUI;

public sealed partial class ColorArchiveProject
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;

    public Windows.UI.Color BgColor { get; set; } = Microsoft.UI.Colors.White;

    public string? Image { get; set; }

    public static ColorArchiveProject Make(string id, string name, string badge, string date, string hex, string? image = null)
    {
        return new ColorArchiveProject
        {
            Id = id,
            Name = name,
            Badge = badge,
            Date = date,
            Image = image,
            BgColor = ParseHex(hex)
        };
    }

    private static Windows.UI.Color ParseHex(string? hex)
    {
        string s = (hex ?? string.Empty).TrimStart('#');
        if (s.Length != 6 || !int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int v))
        {
            return Microsoft.UI.Colors.White;
        }
        return Windows.UI.Color.FromArgb(255, (byte)(v >> 16), (byte)(v >> 8), (byte)v);
    }
}
