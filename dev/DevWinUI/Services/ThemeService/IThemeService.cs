namespace DevWinUI;
public interface IThemeService
{
    delegate void ActualThemeChangedEventHandler(FrameworkElement sender, object args);
    event ActualThemeChangedEventHandler ActualThemeChanged;

    Window Window { get; set; }
    void AutoInitialize(Window window);
    void Initialize(Window window, bool useAutoSave = true, string filename = null);
    void ConfigBackdrop(BackdropType backdropType = BackdropType.Mica, bool force = false);
    void ConfigTintColor(Color color, bool force = false);
    void ConfigTintColor();
    void ConfigFallbackColor(Color color, bool force = false);
    void ConfigFallbackColor();
    void ConfigElementTheme(ElementTheme elementTheme = ElementTheme.Default, bool force = false);

    SystemBackdrop GetSystemBackdrop();
    SystemBackdrop GetSystemBackdrop(BackdropType backdropType);
    BackdropType GetBackdropType();
    BackdropType GetBackdropType(SystemBackdrop systemBackdrop);
    ElementTheme GetElementTheme();
    ElementTheme GetActualTheme();

    void SetBackdropType(BackdropType backdropType);
    void SetBackdropTintColor(Color color);
    void SetBackdropFallbackColor(Color color);

    void SetElementTheme(ElementTheme elementTheme);
    void SetElementThemeWithoutSave(ElementTheme elementTheme);

    bool IsDarkTheme();

    void OnThemeComboBoxSelectionChanged(object sender);
    void SetThemeComboBoxDefaultItem(ComboBox themeComboBox);
    void OnBackdropComboBoxSelectionChanged(object sender);
    void SetBackdropComboBoxDefaultItem(ComboBox backdropComboBox);
    void OnThemeRadioButtonChecked(object sender);
    void SetThemeRadioButtonDefaultItem(Panel ThemePanel);
    void OnBackdropRadioButtonChecked(object sender);
    void SetBackdropRadioButtonDefaultItem(Panel BackdropPanel);
    void UpdateCaptionButtons();
    void UpdateCaptionButtons(Window window);
}
