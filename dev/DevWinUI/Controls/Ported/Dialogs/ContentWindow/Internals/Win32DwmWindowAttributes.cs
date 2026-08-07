using System.Runtime.InteropServices;

namespace DevWinUI;

internal unsafe partial class Win32DwmWindowAttributes
{
    public const uint DWMWA_BORDER_COLOR = 34;
    public const uint DWMWA_WINDOW_CORNER_PREFERENCE = 33;

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmGetWindowAttribute(nint hwnd, uint dwAttribute, void* pvAttribute, uint cbAttribute);

    [LibraryImport("dwmapi.dll")]
    internal static partial int DwmSetWindowAttribute(nint hwnd, uint dwAttribute, void* pvAttribute, uint cbAttribute);

    public static unsafe Color? GetDwmColorAttribute(nint hwnd)
    {
        Span<byte> bytes = stackalloc byte[4];

        fixed (byte* pBytes = bytes)
        {
            DwmGetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, pBytes, (uint)bytes.Length);
        }

        if (bytes[0] == 0xFF &&
            bytes[1] == 0xFF &&
            bytes[2] == 0xFF &&
            bytes[3] == 0xFF)
        {
            return null;
        }

        return Color.FromArgb(255, bytes[0], bytes[1], bytes[2]);
    }

    public static unsafe void SetDwmColorAttribute(nint hwnd, Color? color)
    {
        Span<byte> bytes = stackalloc byte[4];

        if (color is Color value)
        {
            bytes[0] = value.R;
            bytes[1] = value.G;
            bytes[2] = value.B;
            bytes[3] = 0;
        }
        else
        {
            bytes.Fill(0xFF);
        }

        fixed (byte* pBytes = bytes)
        {
            DwmSetWindowAttribute(hwnd, DWMWA_BORDER_COLOR, pBytes, (uint)bytes.Length);
        }
    }
    public static int GetDwmIntAttribute(nint hwnd)
    {
        int value = 0;

        DwmGetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, &value, sizeof(int));

        return value;
    }

    public static void SetDwmIntAttribute(nint hwnd, int value)
    {
        DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, &value, sizeof(int));
    }
    public static Color? GetBorderColor(nint hwnd) => GetDwmColorAttribute(hwnd);
    public static void SetBorderColor(nint hwnd, Color? color) => SetDwmColorAttribute(hwnd, color);

    public static WindowCornerRoundness GetCornerRoundness(nint hwnd) => (WindowCornerRoundness)GetDwmIntAttribute(hwnd);
    public static void SetCornerRoundness(nint hwnd, WindowCornerRoundness cornerRoundness) => SetDwmIntAttribute(hwnd, (int)cornerRoundness);
}
