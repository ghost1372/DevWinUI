namespace DevWinUI;

public interface IThemeService
{
    bool IsDark { get; }
    ElementTheme ElementTheme { get; }
    ElementTheme ActualTheme { get; }
    BackdropType BackdropType { get; }

    Task<bool> SetElementThemeAsync(ElementTheme theme);
    Task<bool> SetElementThemeWithoutSaveAsync(ElementTheme theme);
    Task<bool> SetElementThemeWithoutSaveAsync();
    Task<bool> SetBackdropTypeAsync(BackdropType backdropType);
    Task<bool> SetBackdropTypeWithoutSaveAsync(BackdropType backdropType);
    Task<bool> SetBackdropTypeWithoutSaveAsync();

    AcrylicSystemBackdrop GetAcrylicSystemBackdrop();
    MicaSystemBackdrop GetMicaSystemBackdrop();
    SystemBackdrop GetSystemBackdrop();

    event EventHandler<ElementTheme> ThemeChanged;
    event EventHandler<BackdropType> BackdropChanged;

    void OnThemeComboBoxSelectionChanged(object sender);
    void SetThemeComboBoxDefaultItem(ComboBox themeComboBox);
    void OnBackdropComboBoxSelectionChanged(object sender);
    void SetBackdropComboBoxDefaultItem(ComboBox backdropComboBox);
    void OnThemeRadioButtonChecked(object sender);
    void SetThemeRadioButtonDefaultItem(Panel ThemePanel);
    void OnBackdropRadioButtonChecked(object sender);
    void SetBackdropRadioButtonDefaultItem(Panel BackdropPanel);
    void Dispose();
    ThemeService ConfigureAutoSave(bool isEnabled, string? fileName = null);
    ThemeService ConfigureElementTheme(ElementTheme theme);
    ThemeService ConfigureBackdrop(BackdropType backdropType);
    ThemeService ConfigureBackdrop(BackdropType backdropType, bool isEnabled);
    ThemeService ConfigureBackdrop(bool isEnabled);
    ThemeService Initialize(Window window);
}
