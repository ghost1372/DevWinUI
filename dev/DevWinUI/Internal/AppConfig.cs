namespace DevWinUI;
internal partial class AppConfig
{
    public ElementTheme ElementTheme { get; set; } = ElementTheme.Default;
    public BackdropType BackdropType { get; set; } = BackdropType.Mica;
    public Color? BackdropTintColor { get; set; } = null;
    public Color? BackdropFallBackColor { get; set; } = null;

    public bool IsThemeFirstRun { get; set; } = true;
    public bool IsBackdropFirstRun { get; set; } = true;
    public bool IsBackdropTintColorFirstRun { get; set; } = true;
    public bool IsBackdropFallBackColorFirstRun { get; set; } = true;
}
