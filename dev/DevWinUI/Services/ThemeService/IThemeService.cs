namespace DevWinUI;
public interface IThemeService
{
    delegate void ActualThemeChangedEventHandler(FrameworkElement sender, object args);
    event ActualThemeChangedEventHandler ActualThemeChanged;

    SystemBackdrop GetSystemBackdrop();
    SystemBackdrop GetSystemBackdrop(BackdropType backdropType);
    BackdropType GetBackdropType();
    BackdropType GetBackdropType(SystemBackdrop systemBackdrop);
    ElementTheme GetElementTheme();
    ApplicationTheme GetApplicationTheme();
    ElementTheme GetActualTheme();

    void SetBackdropType(BackdropType backdropType);
    void SetBackdropTintColor(Color? color);
    void SetBackdropFallbackColor(Color? color);

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
    void ResetBackdropProperties();

    ThemeService AutoInitialize(Window window);
    ThemeService Initialize(Window window, bool useAutoSave, string filename);
    ThemeService Initialize(Window window, bool useAutoSave);
    ThemeService Initialize(Window window, string filename);
    ThemeService Initialize(Window window);
    ThemeService ConfigureBackdrop(BackdropType backdropType, bool force);
    ThemeService ConfigureBackdrop(BackdropType backdropType);
    ThemeService ConfigureBackdrop();
    ThemeService ConfigureTintColor(Color? color, bool force);
    ThemeService ConfigureTintColor(Color? color);
    ThemeService ConfigureTintColor();
    ThemeService ConfigureFallbackColor(Color? color, bool force);
    ThemeService ConfigureFallbackColor(Color? color);
    ThemeService ConfigureFallbackColor();
    ThemeService ConfigureElementTheme(ElementTheme elementTheme, bool force);
    ThemeService ConfigureElementTheme(ElementTheme elementTheme);
    ThemeService ConfigureElementTheme();
    ThemeService EnableRequestedTheme();
}
