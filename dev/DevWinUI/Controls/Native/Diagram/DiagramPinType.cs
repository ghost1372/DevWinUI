namespace DevWinUI;

public sealed partial class DiagramPinType
{
    public string? Name { get; set; }

    public Color Color { get; set; } = Color.FromArgb(255, 0x9A, 0xA0, 0xA6);

    public DiagramPinShape? Shape { get; set; }

    public string? Glyph { get; set; }

    public static IEnumerable<DiagramPinType> CreateDefaults()
    {
        yield return Make("exec", 0xC8, 0xCA, 0xD0, "\uE768");
        yield return Make("bool", 0xD9, 0xA8, 0x1C);
        yield return Make("time", 0xA3, 0x5D, 0xEB);
        yield return Make("int", 0x3B, 0x82, 0xF6);
        yield return Make("float", 0x22, 0xC5, 0x5E);
        yield return Make("str", 0xF9, 0x73, 0x16);
        yield return Make("duration", 0x7C, 0x2A, 0xD9);
        yield return Make("list", 0x63, 0x66, 0xF1, "\uE8FD");
        yield return Make("map", 0x0E, 0xA5, 0xE9);
        yield return Make("tuple", 0xEC, 0x48, 0x99);
        yield return Make("struct", 0x14, 0xB8, 0xA6);
    }

    private static DiagramPinType Make(string name, byte r, byte g, byte b, string? glyph = null) =>
        new() { Name = name, Color = Color.FromArgb(255, r, g, b), Glyph = glyph };
}
