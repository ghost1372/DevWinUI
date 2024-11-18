namespace DevWinUI;
internal partial class AppConfig
{
    public ElementTheme ElementTheme { get; set; } = ElementTheme.Default;
    public BackdropType BackdropType { get; set; } = BackdropType.None;
    public Color BackdropTintColor { get; set; }
    public Color BackdropFallBackColor { get; set; }

    public bool IsThemeFirstRun { get; set; } = true;
    public bool IsBackdropFirstRun { get; set; } = true;
    public bool IsBackdropTintColorFirstRun { get; set; } = true;
    public bool IsBackdropFallBackColorFirstRun { get; set; } = true;
}
