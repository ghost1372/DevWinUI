namespace DevWinUI;
public partial class ThemeService
{
    public ThemeService ConfigureAutoSave(bool isEnabled, string? fileName = null)
    {
        _useAutoSave = isEnabled;
        _userDefinedFileName = fileName;
        return this;
    }

    public ThemeService ConfigureElementTheme(ElementTheme theme)
    {
        _userDefinedTheme = theme;
        return this;
    }
    public ThemeService ConfigureBackdrop(BackdropType backdropType)
    {
        _userDefinedBackdrop = backdropType;
        return this;
    }
    public ThemeService ConfigureBackdrop(BackdropType backdropType, bool isEnabled)
    {
        _isBackdropEnabled = isEnabled;
        _userDefinedBackdrop = backdropType;
        return this;
    }
    public ThemeService ConfigureBackdrop(bool isEnabled)
    {
        _isBackdropEnabled = isEnabled;
        return this;
    }
}

