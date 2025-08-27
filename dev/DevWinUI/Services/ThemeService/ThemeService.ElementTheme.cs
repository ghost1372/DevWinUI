namespace DevWinUI;
public partial class ThemeService
{
    public async Task<bool> SetElementThemeWithoutSaveAsync()
    {
        ElementTheme requestedTheme = ElementTheme.Default;
        if (ActualTheme == ElementTheme.Light)
        {
            requestedTheme = ElementTheme.Dark;
        }
        else if (ActualTheme == ElementTheme.Dark)
        {
            requestedTheme |= ElementTheme.Light;
        }
        return await SetElementThemeWithoutSaveAsync(requestedTheme);
    }
    public async Task<bool> SetElementThemeWithoutSaveAsync(ElementTheme theme)
    {
        var cachedAutoSave = _useAutoSave;
        if (_useAutoSave)
            _useAutoSave = false;

        var result = await SetElementThemeAsync(theme);

        if (cachedAutoSave)
            _useAutoSave = true;

        return result;
    }
    public async Task<bool> SetElementThemeAsync(ElementTheme theme)
    {
        if (_initialization is null)
        {
            throw new NullReferenceException(
                $"Theme service not initialized, {nameof(InitializeAsync)} needs to complete before SetThemeAsync can be called");
        }

        // Make sure initialization completes before attempting to set new theme
        await _initialization.Task;

        GeneralHelper.SetPreferredAppMode(theme);

        return await InternalSetElementThemeAsync(theme);
    }

    private async Task<bool> InternalSetElementThemeAsync(ElementTheme theme)
    {
        bool result = false;

        foreach (var item in WindowHelper.ActiveWindows)
        {
            if (item.Dispatcher.HasThreadAccess ||
                (!(_initialization?.Task.IsCompleted ?? false)))
            {
                result = InternalSetElementThemeOnUIThread(item.Window, theme);
                UpdateCaptionButtons(item.Window);
            }
            else
            {
                result = await item.Dispatcher.ExecuteAsync(async (ct) =>
                {
                    var result2 = InternalSetElementThemeOnUIThread(item.Window, theme);
                    UpdateCaptionButtons(item.Window);
                    return result2;
                });
            }
        }

        return result;
    }

    private bool InternalSetElementThemeOnUIThread(Window window, ElementTheme theme)
    {
        if (!_rootElements.TryGetValue(window, out var rootElement))
            return false;

        var existingIsDark = rootElement.ActualTheme == ElementTheme.Dark;

        rootElement.RequestedTheme = theme switch
        {
            ElementTheme.Default => ElementTheme.Default,
            ElementTheme.Dark => ElementTheme.Dark,
            ElementTheme.Light => ElementTheme.Light,
            _ => ElementTheme.Default,
        };

        if (_useAutoSave)
        {
            SaveDesiredElementTheme(theme);
        }

        if (existingIsDark != (rootElement.ActualTheme == ElementTheme.Dark))
        {
            ThemeChanged?.Invoke(this, theme);
        }
        return true;
    }

    private void SaveDesiredElementTheme(ElementTheme theme)
    {
        try
        {
            GlobalData.Config.ElementTheme = theme;
            GlobalData.Save();
        }
        catch { }
    }

    private ElementTheme GetSavedElementTheme()
    {
        try
        {
            return GeneralHelper.GetEnum<ElementTheme>(GlobalData.Config.ElementTheme.ToString());
        }
        catch { }

        return ElementTheme.Default;
    }
}
