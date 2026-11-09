namespace DevWinUI;

internal static partial class MagicKeyboardLayoutHelper
{
    public const double U = 60, FnH = 38, FnGap = 18, Pad = 18;
    public static double Width(MagicKeyboardFormFactor form) => (form == MagicKeyboardFormFactor.Full ? 22.5 : 18.25) * U + 2 * Pad;
    public static double Height => Pad + FnH + FnGap + 5 * U + Pad;

    public static List<KeyDef> Build(MagicKeyboardLayout layout, MagicKeyboardFormFactor form, MagicKeyboardPlatform platform)
    {
        var keys = new List<KeyDef>();
        bool iso = layout == MagicKeyboardLayout.Iso;
        bool mac = platform == MagicKeyboardPlatform.Mac;
        string enterLabel = mac ? "return" : "enter";
        double yF = Pad, y1 = yF + FnH + FnGap, y2 = y1 + U, y3 = y2 + U, y4 = y3 + U, y5 = y4 + U;

        void Add(string code, string top, string bottom, MagicKeyboardKeyStyle style, MagicKeyboardKeyAlign align,
                 double xu, double y, double wu, double h = U, (double, double)[]? poly = null)
            => keys.Add(new KeyDef(code, top, bottom, style, align, Pad + xu * U, y, wu * U, h, poly));

        void Letter(char ch, double xu, double y)
            => Add("Key" + char.ToUpperInvariant(ch), "", ch.ToString(), MagicKeyboardKeyStyle.Letter, MagicKeyboardKeyAlign.Center, xu, y, 1);

        Add("Escape", "", "esc", MagicKeyboardKeyStyle.Function, MagicKeyboardKeyAlign.Center, 0, yF, 1, FnH);
        double[] fx = { 2, 3, 4, 5, 6.5, 7.5, 8.5, 9.5, 11, 12, 13, 14 };
        for (int i = 0; i < fx.Length; i++)
            Add($"F{i + 1}", "", $"F{i + 1}", MagicKeyboardKeyStyle.Function, MagicKeyboardKeyAlign.Center, fx[i], yF, 1, FnH);
        if (mac)
        {
            Add("F13", "", "F13", MagicKeyboardKeyStyle.Function, MagicKeyboardKeyAlign.Center, 15.25, yF, 1, FnH);
            Add("F14", "", "F14", MagicKeyboardKeyStyle.Function, MagicKeyboardKeyAlign.Center, 16.25, yF, 1, FnH);
            Add("F15", "", "F15", MagicKeyboardKeyStyle.Function, MagicKeyboardKeyAlign.Center, 17.25, yF, 1, FnH);
        }
        else
        {
            Add("PrintScreen", "prtscr", "sysrq", MagicKeyboardKeyStyle.Function, MagicKeyboardKeyAlign.Center, 15.25, yF, 1, FnH);
            Add("ScrollLock", "scroll", "lock", MagicKeyboardKeyStyle.Function, MagicKeyboardKeyAlign.Center, 16.25, yF, 1, FnH);
            Add("Pause", "pause", "break", MagicKeyboardKeyStyle.Function, MagicKeyboardKeyAlign.Center, 17.25, yF, 1, FnH);
        }

        string[] nc = { "Backquote", "Digit1", "Digit2", "Digit3", "Digit4", "Digit5", "Digit6", "Digit7", "Digit8", "Digit9", "Digit0", "Minus", "Equal" };
        string[] nm = { "`", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "-", "=" };
        string[] ns = { "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "_", "+" };
        if (iso) { nm[0] = "§"; ns[0] = "±"; }
        for (int i = 0; i < 13; i++) Add(nc[i], ns[i], nm[i], MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, i, y1, 1);
        Add("Backspace", "", mac ? "delete" : "backspace", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Right, 13, y1, 2);

        Add("Tab", "", "tab", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 0, y2, 1.5);
        const string qrow = "qwertyuiop";
        for (int i = 0; i < qrow.Length; i++) Letter(qrow[i], 1.5 + i, y2);
        Add("BracketLeft", "{", "[", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, 11.5, y2, 1);
        Add("BracketRight", "}", "]", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, 12.5, y2, 1);
        if (!iso)
        {
            Add("Backslash", "|", "\\", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, 13.5, y2, 1.5);
        }
        else
        {
            const double inset = 7, width = 1.5 * U;
            Add("Enter", "", enterLabel, MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Right, 13.5, y2, 1.5, 2 * U, new (double, double)[]
            {
                (inset, inset),
                (width - inset, inset),
                (width - inset, 2 * U - inset),
                (0.25 * U + inset, 2 * U - inset),
                (0.25 * U + inset, U - inset),
                (inset, U - inset)
            });
        }

        Add("CapsLock", "", "caps lock", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 0, y3, 1.75);
        const string homeRow = "asdfghjkl";
        for (int i = 0; i < homeRow.Length; i++) Letter(homeRow[i], 1.75 + i, y3);
        Add("Semicolon", ":", ";", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, 10.75, y3, 1);
        Add("Quote", "\"", "'", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, 11.75, y3, 1);
        if (!iso) Add("Enter", "", enterLabel, MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Right, 12.75, y3, 2.25);
        else Add("Backslash", "~", "#", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, 12.75, y3, 1);

        double cursor = iso ? 1.25 : 2.25;
        Add("ShiftLeft", "", "shift", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 0, y4, cursor);
        if (iso)
        {
            Add("IntlBackslash", "|", "\\", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, cursor, y4, 1);
            cursor += 1;
        }
        foreach (char ch in "zxcvbnm")
        {
            Letter(ch, cursor, y4);
            cursor += 1;
        }
        Add("Comma", "<", ",", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, cursor++, y4, 1);
        Add("Period", ">", ".", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, cursor++, y4, 1);
        Add("Slash", "?", "/", MagicKeyboardKeyStyle.Dual, MagicKeyboardKeyAlign.Center, cursor++, y4, 1);
        Add("ShiftRight", "", "shift", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Right, cursor, y4, 15 - cursor);

        if (mac)
        {
            Add("Fn", "", "fn", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 0, y5, 1.25);
            Add("ControlLeft", "\u2303", "control", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 1.25, y5, 1.25);
            Add("AltLeft", "\u2325", "option", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 2.5, y5, 1.25);
            Add("MetaLeft", "\u2318", "command", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Right, 3.75, y5, 1.75);
            Add("Space", "", "space", MagicKeyboardKeyStyle.Blank, MagicKeyboardKeyAlign.Center, 5.5, y5, 6.5);
            Add("MetaRight", "\u2318", "command", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 12, y5, 1.75);
            Add("AltRight", "\u2325", "option", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 13.75, y5, 1.25);
        }
        else
        {
            Add("Fn", "", "fn", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 0, y5, 1.25);
            Add("ControlLeft", "", "ctrl", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 1.25, y5, 1.25);
            Add("MetaLeft", "\u229E", "win", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 2.5, y5, 1.25);
            Add("AltLeft", "", "alt", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 3.75, y5, 1.25);
            Add("Space", "", "space", MagicKeyboardKeyStyle.Blank, MagicKeyboardKeyAlign.Center, 5, y5, 5);
            Add("AltRight", "", "alt", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 10, y5, 1.25);
            Add("MetaRight", "\u229E", "win", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 11.25, y5, 1.25);
            Add("ContextMenu", "", "menu", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 12.5, y5, 1.25);
            Add("ControlRight", "", "ctrl", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Left, 13.75, y5, 1.25);
        }

        Add("Insert", "", "ins", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Center, 15.25, y1, 1);
        Add("Home", "", "home", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Center, 16.25, y1, 1);
        Add("PageUp", "", "pg up", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Center, 17.25, y1, 1);
        Add("Delete", "", "del", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Center, 15.25, y2, 1);
        Add("End", "", "end", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Center, 16.25, y2, 1);
        Add("PageDown", "", "pg dn", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Center, 17.25, y2, 1);

        if (mac)
        {
            Add("ArrowLeft", "", "\u25C0", MagicKeyboardKeyStyle.Arrow, MagicKeyboardKeyAlign.Center, 15.25, y5, 1);
            Add("ArrowUp", "", "\u25B2", MagicKeyboardKeyStyle.Arrow, MagicKeyboardKeyAlign.Center, 16.25, y5, 1, U / 2);
            Add("ArrowDown", "", "\u25BC", MagicKeyboardKeyStyle.Arrow, MagicKeyboardKeyAlign.Center, 16.25, y5 + U / 2, 1, U / 2);
            Add("ArrowRight", "", "\u25B6", MagicKeyboardKeyStyle.Arrow, MagicKeyboardKeyAlign.Center, 17.25, y5, 1);
        }
        else
        {
            Add("ArrowUp", "", "\u25B2", MagicKeyboardKeyStyle.Arrow, MagicKeyboardKeyAlign.Center, 16.25, y4, 1);
            Add("ArrowLeft", "", "\u25C0", MagicKeyboardKeyStyle.Arrow, MagicKeyboardKeyAlign.Center, 15.25, y5, 1);
            Add("ArrowDown", "", "\u25BC", MagicKeyboardKeyStyle.Arrow, MagicKeyboardKeyAlign.Center, 16.25, y5, 1);
            Add("ArrowRight", "", "\u25B6", MagicKeyboardKeyStyle.Arrow, MagicKeyboardKeyAlign.Center, 17.25, y5, 1);
        }

        if (form == MagicKeyboardFormFactor.Full)
        {
            const double nx = 18.5;
            Add("NumLock", mac ? "" : "num", mac ? "clear" : "lock", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Center, nx, y1, 1);
            Add("NumpadDivide", "", "/", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 1, y1, 1);
            Add("NumpadMultiply", "", "*", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 2, y1, 1);
            Add("NumpadSubtract", "", "-", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 3, y1, 1);

            Add("Numpad7", "home", "7", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx, y2, 1);
            Add("Numpad8", "\u2191", "8", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 1, y2, 1);
            Add("Numpad9", "pg up", "9", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 2, y2, 1);
            Add("NumpadAdd", "", "+", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 3, y2, 1, 2 * U);

            Add("Numpad4", "\u2190", "4", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx, y3, 1);
            Add("Numpad5", "", "5", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 1, y3, 1);
            Add("Numpad6", "\u2192", "6", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 2, y3, 1);

            Add("Numpad1", "end", "1", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx, y4, 1);
            Add("Numpad2", "\u2193", "2", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 1, y4, 1);
            Add("Numpad3", "pg dn", "3", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 2, y4, 1);
            Add("NumpadEnter", "", "enter", MagicKeyboardKeyStyle.Modifier, MagicKeyboardKeyAlign.Center, nx + 3, y4, 1, 2 * U);

            Add("Numpad0", "ins", "0", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx, y5, 2);
            Add("NumpadDecimal", "del", ".", MagicKeyboardKeyStyle.Numpad, MagicKeyboardKeyAlign.Center, nx + 2, y5, 1);
        }

        return keys;
    }
}
