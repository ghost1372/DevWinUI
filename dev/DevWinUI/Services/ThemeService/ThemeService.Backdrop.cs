namespace DevWinUI;
public partial class ThemeService
{
    public async Task<bool> SetBackdropTypeWithoutSaveAsync()
    {
        BackdropType requestedBackdrop = BackdropType.None;

        switch (BackdropType)
        {
            case BackdropType.None:
                requestedBackdrop = BackdropType.Mica;
                break;
            case BackdropType.Mica:
                requestedBackdrop = BackdropType.MicaAlt;
                break;
            case BackdropType.MicaAlt:
                requestedBackdrop = BackdropType.Acrylic;
                break;
            case BackdropType.Acrylic:
                requestedBackdrop = BackdropType.AcrylicThin;
                break;
            case BackdropType.AcrylicThin:
                requestedBackdrop = BackdropType.Transparent;
                break;
            case BackdropType.Transparent:
                requestedBackdrop = BackdropType.None;
                break;
        }

        return await SetBackdropTypeWithoutSaveAsync(requestedBackdrop);
    }
    public async Task<bool> SetBackdropTypeWithoutSaveAsync(BackdropType backdropType)
    {
        var cachedAutoSave = _useAutoSave;
        if (_useAutoSave)
            _useAutoSave = false;

        var result = await SetBackdropTypeAsync(backdropType);

        if (cachedAutoSave)
            _useAutoSave = true;

        return result;
    }
    public async Task<bool> SetBackdropTypeAsync(BackdropType backdropType)
    {
        if (_initialization is null)
        {
            throw new NullReferenceException(
                $"Theme service not initialized, {nameof(InitializeAsync)} needs to complete before SetBackdropAsync can be called");
        }

        // Make sure initialization completes before attempting to set new theme
        await _initialization.Task;

        return await InternalSetBackdropTypeAsync(backdropType);
    }
    private async Task<bool> InternalSetBackdropTypeAsync(BackdropType backdropType)
    {
        bool result = false;

        if (_isBackdropEnabled)
        {
            foreach (var item in WindowHelper.ActiveWindows)
            {
                if (item.Dispatcher.HasThreadAccess ||
                    (!(_initialization?.Task.IsCompleted ?? false)))
                {
                    result = InternalSetBackdropTypeOnUIThread(item.Window, backdropType);
                }
                else
                {
                    result = await item.Dispatcher.ExecuteAsync(async (ct) =>
                    {
                        return InternalSetBackdropTypeOnUIThread(item.Window, backdropType);
                    });
                }
            }
        }

        return result;
    }
    private bool InternalSetBackdropTypeOnUIThread(Window window, BackdropType backdropType)
    {
        switch (backdropType)
        {
            case BackdropType.None:
                window.SystemBackdrop = null;
                break;
            case BackdropType.Mica:
                window.SystemBackdrop = new MicaSystemBackdrop();
                break;
            case BackdropType.MicaAlt:
                window.SystemBackdrop = new MicaSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt);
                break;
            case BackdropType.Acrylic:
                window.SystemBackdrop = new AcrylicSystemBackdrop();
                break;
            case BackdropType.AcrylicThin:
                window.SystemBackdrop = new AcrylicSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicKind.Thin);
                break;
            case BackdropType.Transparent:
                window.SystemBackdrop = new TransparentBackdrop();
                break;
        }

        if (_useAutoSave)
        {
            SaveDesiredBackdropType(backdropType);
        }

        BackdropChanged?.Invoke(this, backdropType);

        return true;
    }
    private void SaveDesiredBackdropType(BackdropType backdropType)
    {
        try
        {
            GlobalData.Config.BackdropType = backdropType;
            GlobalData.Save();
        }
        catch { }
    }
    private BackdropType? GetSavedBackdropType()
    {
        try
        {
            return GlobalData.Config.BackdropType;
        }
        catch { }

        return null;
    }

    public SystemBackdrop GetSystemBackdrop()
    {
        return _rootElements?.Keys.FirstOrDefault().SystemBackdrop;
    }
    public MicaSystemBackdrop GetMicaSystemBackdrop()
    {
        if (GetSystemBackdrop() is MicaSystemBackdrop micaSystemBackdrop)
        {
            return micaSystemBackdrop;
        }

        return null;
    }

    public AcrylicSystemBackdrop GetAcrylicSystemBackdrop()
    {
        if (GetSystemBackdrop() is AcrylicSystemBackdrop acrylicSystemBackdrop)
        {
            return acrylicSystemBackdrop;
        }

        return null;
    }
}
