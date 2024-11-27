namespace DevWinUI;
public partial class ThemeService
{
    private bool _isInitialized;
    public ThemeService AutoInitialize(Window window)
    {
        AutoInitializeBase(window);

        _isInitialized = true;

        return this; // Enable chaining
    }
    public ThemeService Initialize(Window window, bool useAutoSave, string filename)
    {
        InitializeBase(window, useAutoSave, filename);

        _isInitialized = true;

        return this; // Enable chaining
    }

    public ThemeService Initialize(Window window, bool useAutoSave)
    {
        InitializeBase(window, useAutoSave, null);

        _isInitialized = true;

        return this; // Enable chaining
    }
    public ThemeService Initialize(Window window, string filename)
    {
        InitializeBase(window, true, filename);

        _isInitialized = true;

        return this; // Enable chaining
    }
    public ThemeService Initialize(Window window)
    {
        InitializeBase(window, true, null);

        _isInitialized = true;

        return this; // Enable chaining
    }

    public ThemeService ConfigureBackdrop(BackdropType backdropType, bool force)
    {
        EnsureInitialized();
        ConfigBackdropBase(backdropType, force);
        return this;
    }
    public ThemeService ConfigureBackdrop(BackdropType backdropType)
    {
        EnsureInitialized();
        ConfigBackdropBase(backdropType, false);
        return this;
    }
    public ThemeService ConfigureBackdrop()
    {
        EnsureInitialized();
        ConfigBackdropBase(BackdropType.Mica, false);
        return this;
    }
    public ThemeService ConfigureTintColor(Color color, bool force)
    {
        EnsureInitialized();
        ConfigTintColorBase(color, force);
        return this;
    }
    public ThemeService ConfigureTintColor(Color color)
    {
        EnsureInitialized();
        ConfigTintColorBase(color, false);
        return this;
    }
    public ThemeService ConfigureTintColor()
    {
        EnsureInitialized();
        ConfigTintColorBase();
        return this;
    }
    public ThemeService ConfigureFallbackColor(Color color, bool force)
    {
        EnsureInitialized();
        ConfigFallbackColorBase(color, force);
        return this;
    }
    public ThemeService ConfigureFallbackColor(Color color)
    {
        EnsureInitialized();
        ConfigFallbackColorBase(color, false);
        return this;
    }
    public ThemeService ConfigureFallbackColor()
    {
        EnsureInitialized();
        ConfigFallbackColorBase();
        return this;
    }
    public ThemeService ConfigureElementTheme(ElementTheme elementTheme, bool force)
    {
        EnsureInitialized();
        ConfigElementThemeBase(elementTheme, force);
        return this;
    }
    public ThemeService ConfigureElementTheme(ElementTheme elementTheme)
    {
        EnsureInitialized();
        ConfigElementThemeBase(elementTheme, false);
        return this;
    }
    public ThemeService ConfigureElementTheme()
    {
        EnsureInitialized();
        ConfigElementThemeBase(ElementTheme.Default, false);
        return this;
    }

    /// <summary>
    /// This method enables the ability to use the Application.Current.RequestedTheme.
    /// </summary>
    /// <returns></returns>
    public ThemeService EnableRequestedTheme()
    {
        EnableRequestedThemeBase();
        return this;
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service must be initialized before configuration.");
    }
}

