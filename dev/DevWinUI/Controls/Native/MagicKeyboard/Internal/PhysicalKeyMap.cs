using Windows.System;

namespace DevWinUI;

internal static partial class PhysicalKeyMap
{
    private const uint Ext = 0x100;

    private static readonly Dictionary<uint, string> ScanToCode = BuildScan();

    private static readonly Dictionary<string, uint> CodeToScan =
        ScanToCode.Where(kv => (kv.Key & Ext) == 0).ToDictionary(kv => kv.Value, kv => kv.Key);

    public static readonly string[] CharacterCodes = CodeToScan.Keys
        .Where(c => c.StartsWith("Key") || c.StartsWith("Digit") ||
                    c is "Backquote" or "Minus" or "Equal" or "BracketLeft" or "BracketRight" or
                         "Backslash" or "Semicolon" or "Quote" or "Comma" or "Period" or "Slash" or "IntlBackslash")
        .ToArray();

    private static Dictionary<uint, string> BuildScan()
    {
        var d = new Dictionary<uint, string>
        {
            [0x01] = "Escape",
            [0x0C] = "Minus",
            [0x0D] = "Equal",
            [0x0E] = "Backspace",
            [0x0F] = "Tab",
            [0x1A] = "BracketLeft",
            [0x1B] = "BracketRight",
            [0x1C] = "Enter",
            [0x1D] = "ControlLeft",
            [0x27] = "Semicolon",
            [0x28] = "Quote",
            [0x29] = "Backquote",
            [0x2A] = "ShiftLeft",
            [0x2B] = "Backslash",
            [0x33] = "Comma",
            [0x34] = "Period",
            [0x35] = "Slash",
            [0x36] = "ShiftRight",
            [0x37] = "NumpadMultiply",
            [0x38] = "AltLeft",
            [0x39] = "Space",
            [0x3A] = "CapsLock",
            [0x45] = "Pause",
            [0x46] = "ScrollLock",
            [0x47] = "Numpad7",
            [0x48] = "Numpad8",
            [0x49] = "Numpad9",
            [0x4A] = "NumpadSubtract",
            [0x4B] = "Numpad4",
            [0x4C] = "Numpad5",
            [0x4D] = "Numpad6",
            [0x4E] = "NumpadAdd",
            [0x4F] = "Numpad1",
            [0x50] = "Numpad2",
            [0x51] = "Numpad3",
            [0x52] = "Numpad0",
            [0x53] = "NumpadDecimal",
            [0x54] = "PrintScreen",
            [0x56] = "IntlBackslash",
            [0x57] = "F11",
            [0x58] = "F12",
            [Ext | 0x1C] = "NumpadEnter",
            [Ext | 0x1D] = "ControlRight",
            [Ext | 0x35] = "NumpadDivide",
            [Ext | 0x37] = "PrintScreen",
            [Ext | 0x38] = "AltRight",
            [Ext | 0x45] = "NumLock",
            [Ext | 0x46] = "Pause",
            [Ext | 0x47] = "Home",
            [Ext | 0x48] = "ArrowUp",
            [Ext | 0x49] = "PageUp",
            [Ext | 0x4B] = "ArrowLeft",
            [Ext | 0x4D] = "ArrowRight",
            [Ext | 0x4F] = "End",
            [Ext | 0x50] = "ArrowDown",
            [Ext | 0x51] = "PageDown",
            [Ext | 0x52] = "Insert",
            [Ext | 0x53] = "Delete",
            [Ext | 0x5B] = "MetaLeft",
            [Ext | 0x5C] = "MetaRight",
            [Ext | 0x5D] = "ContextMenu",
        };
        for (uint i = 0; i < 10; i++) d[0x02 + i] = "Digit" + ((i + 1) % 10);
        for (uint i = 0; i < 10; i++) d[0x3B + i] = "F" + (i + 1);
        const string q = "QWERTYUIOP", a = "ASDFGHJKL", z = "ZXCVBNM";
        for (uint i = 0; i < q.Length; i++) d[0x10 + i] = "Key" + q[(int)i];
        for (uint i = 0; i < a.Length; i++) d[0x1E + i] = "Key" + a[(int)i];
        for (uint i = 0; i < z.Length; i++) d[0x2C + i] = "Key" + z[(int)i];
        return d;
    }

    public static string? FromScan(uint scanCode, bool extended) =>
        ScanToCode.TryGetValue(scanCode | (extended ? Ext : 0), out var c) ? c : null;

    public static uint ScanFor(string code) => CodeToScan.TryGetValue(code, out var sc) ? sc : 0;

    public static string? Resolve(VirtualKey key, uint scanCode, bool extended)
    {
        switch ((int)key)
        {
            case 3:
            case 19: return "Pause";
            case 44: return "PrintScreen";
            case 144: return "NumLock";
            case 145: return "ScrollLock";
        }
        return FromScan(scanCode, extended) ?? Map(key);
    }

    public static string? Map(VirtualKey key)
    {
        int v = (int)key;
        if (v >= 65 && v <= 90) return "Key" + (char)v;
        if (v >= 48 && v <= 57) return "Digit" + (char)v;
        if (v >= 96 && v <= 105) return "Numpad" + (v - 96);
        if (v >= 112 && v <= 126) return "F" + (v - 111);
        return v switch
        {
            27 => "Escape",
            8 => "Backspace",
            9 => "Tab",
            20 => "CapsLock",
            13 => "Enter",
            32 => "Space",
            33 => "PageUp",
            34 => "PageDown",
            35 => "End",
            36 => "Home",
            37 => "ArrowLeft",
            38 => "ArrowUp",
            39 => "ArrowRight",
            40 => "ArrowDown",
            45 => "Insert",
            46 => "Delete",
            93 => "ContextMenu",
            106 => "NumpadMultiply",
            107 => "NumpadAdd",
            109 => "NumpadSubtract",
            110 => "NumpadDecimal",
            111 => "NumpadDivide",
            192 => "Backquote",
            189 => "Minus",
            187 => "Equal",
            219 => "BracketLeft",
            221 => "BracketRight",
            220 => "Backslash",
            186 => "Semicolon",
            222 => "Quote",
            188 => "Comma",
            190 => "Period",
            191 => "Slash",
            226 => "IntlBackslash",
            91 => "MetaLeft",
            92 => "MetaRight",
            _ => null
        };
    }
}
