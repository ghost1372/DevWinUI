namespace DevWinUI;

internal sealed record MagicKeyboardColors(
    Color ChassisTop,
    Color ChassisBottom,
    Color ChassisStroke,
    Color FaceTop,
    Color FaceBottom,
    Color PressTop,
    Color PressBottom,
    Color Edge,
    Color Legend,
    Color Dim,
    Color LedActive)
{
    public static MagicKeyboardColors Fallback { get; } = new(
        C(0xE4, 0xE5, 0xE8),
        C(0xCF, 0xD1, 0xD5),
        C(0xB5, 0xB7, 0xBB),
        C(0xFB, 0xFB, 0xFC),
        C(0xEE, 0xEF, 0xF1),
        C(0xE2, 0xE3, 0xE6),
        C(0xD6, 0xD8, 0xDC),
        C(0xA9, 0xAC, 0xB2),
        C(0x2B, 0x2B, 0x2E),
        C(0x6E, 0x70, 0x76),
        C(0x34, 0xC7, 0x59));

    private static Color C(byte r, byte g, byte b) => Color.FromArgb(255, r, g, b);
}
