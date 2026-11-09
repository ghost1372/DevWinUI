namespace DevWinUI;

public sealed partial class MagicKeyboard
{
    private bool UsesThemeResources => Skin is MagicKeyboardSkin.System or MagicKeyboardSkin.Custom;

    private string ThemeKey => IsHighContrast ? "HighContrast" : ActualTheme == ElementTheme.Dark ? "Dark" : "Light";

    private MagicKeyboardColors LoadColors(MagicKeyboardSkin skin)
    {
        string prefix = "MagicKeyboard" + skin;
        var f = MagicKeyboardColors.Fallback;
        return new MagicKeyboardColors(
            Find(prefix + "ChassisTop", f.ChassisTop),
            Find(prefix + "ChassisBottom", f.ChassisBottom),
            Find(prefix + "ChassisStroke", f.ChassisStroke),
            Find(prefix + "FaceTop", f.FaceTop),
            Find(prefix + "FaceBottom", f.FaceBottom),
            Find(prefix + "PressTop", f.PressTop),
            Find(prefix + "PressBottom", f.PressBottom),
            Find(prefix + "Edge", f.Edge),
            Find(prefix + "Legend", f.Legend),
            Find(prefix + "Dim", f.Dim),
            Find(prefix + "LedActive", f.LedActive));
    }

    private Color Find(string key, Color fallback)
    {
        string theme = ThemeKey;

        for (DependencyObject? d = this; d is not null; d = VisualTreeHelper.GetParent(d))
            if (d is FrameworkElement fe && TryFind(fe.Resources, key, theme, out var v))
                return ToColor(v, fallback);

        if (Application.Current?.Resources is { } app && TryFind(app, key, theme, out var a))
            return ToColor(a, fallback);

        return fallback;
    }

    private static bool TryFind(ResourceDictionary rd, string key, string theme, out object? value)
    {
        if (rd.ThemeDictionaries.TryGetValue(theme, out var t) && t is ResourceDictionary td && td.TryGetValue(key, out value)) return true;
        if (rd.ThemeDictionaries.TryGetValue("Default", out var df) && df is ResourceDictionary dd && dd.TryGetValue(key, out value)) return true;
        if (rd.TryGetValue(key, out value)) return true;

        foreach (var merged in rd.MergedDictionaries)
            if (TryFind(merged, key, theme, out value)) return true;

        value = null;
        return false;
    }

    private static Color ToColor(object? value, Color fallback) => value switch
    {
        Color c => c,
        SolidColorBrush b => b.Color,
        _ => fallback
    };
}
