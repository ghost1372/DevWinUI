using System.Globalization;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace DevWinUI;

internal static partial class SystemKeyLabels
{
    private const int VkShift = 0x10;
    private const uint ToUnicodeDoNotChangeKeyState = 4;

    public static IntPtr CurrentLayout() => PInvoke.GetKeyboardLayout(0);

    public static Dictionary<string, (string Base, string Shift)> Read(IntPtr hkl)
    {
        var result = new Dictionary<string, (string, string)>();

        foreach (var code in PhysicalKeyMap.CharacterCodes)
        {
            uint sc = PhysicalKeyMap.ScanFor(code);
            if (sc == 0) continue;
            uint vk = ScanToVirtualKey(sc, hkl);
            if (vk == 0) continue;
            result[code] = (Clean(Translate(vk, sc, false, hkl)), Clean(Translate(vk, sc, true, hkl)));
        }
        return result;
    }
    public static uint ScanToVirtualKey(uint scanCode, nint layout) =>
        PInvoke.MapVirtualKeyEx(scanCode, MAP_VIRTUAL_KEY_TYPE.MAPVK_VSC_TO_VK_EX, new HKL(layout));
    public unsafe static string Translate(uint virtualKey, uint scanCode, bool shift, nint layout)
    {
        Span<byte> state = stackalloc byte[256];
        state.Clear();
        if (shift) state[VkShift] = 0x80;

        Span<char> buffer = stackalloc char[8];
        int n;
        fixed (byte* pState = state)
        fixed (char* pBuffer = buffer)
        {
            n = PInvoke.ToUnicodeEx(virtualKey, scanCode, pState, pBuffer, buffer.Length, ToUnicodeDoNotChangeKeyState, new HKL(layout));
        }

        if (n == 0) return "";
        if (n < 0) return buffer[0].ToString();
        return new string(buffer.Slice(0, Math.Min(n, buffer.Length)));
    }

    private static string Clean(string s)
    {
        if (s.Length == 0) return s;
        switch (s)
        {
            case "\u200C": return "ZWNJ";
            case "\u200D": return "ZWJ";
            case "\u200E": return "LRM";
            case "\u200F": return "RLM";
        }
        switch (char.GetUnicodeCategory(s[0]))
        {
            case UnicodeCategory.Control:
            case UnicodeCategory.Format:
            case UnicodeCategory.SpaceSeparator:
            case UnicodeCategory.OtherNotAssigned:
                return "";
            case UnicodeCategory.NonSpacingMark:
            case UnicodeCategory.EnclosingMark:
            case UnicodeCategory.SpacingCombiningMark:
                return "\u25CC" + s;
        }
        return s;
    }
}
