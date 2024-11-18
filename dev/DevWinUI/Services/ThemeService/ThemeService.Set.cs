namespace DevWinUI;
public partial class ThemeService
{
    private void SetWindowSystemBackdrop(SystemBackdrop systemBackdrop)
    {
        Window.SystemBackdrop = systemBackdrop;
    }

    public void SetBackdropType(BackdropType backdropType)
    {
        var systemBackdrop = GetSystemBackdrop(backdropType);

        if (useAutoSave && GlobalData.Config != null)
        {
            if (GlobalData.Config.BackdropType != backdropType)
            {
                SetWindowSystemBackdrop(systemBackdrop);

                GlobalData.Config.BackdropType = backdropType;
                GlobalData.Save();
            }
        }
        else
        {
            SetWindowSystemBackdrop(systemBackdrop);
        }
    }
    
    public void SetElementTheme(ElementTheme elementTheme)
    {
        changeThemeWithoutSave = false;
        if (RootTheme != elementTheme)
        {
            RootTheme = elementTheme;
        }
    }

    public void SetElementThemeWithoutSave(ElementTheme elementTheme)
    {
        changeThemeWithoutSave = true;
        if (RootTheme != elementTheme)
        {
            RootTheme = elementTheme;
        }
    }

    public void SetBackdropTintColor(Color color)
    {
        var systemBackdrop = Window.SystemBackdrop;
        if (systemBackdrop != null)
        {
            if (systemBackdrop is MicaSystemBackdrop mica)
            {
                mica.TintColor = color;
            }
            else if (systemBackdrop is AcrylicSystemBackdrop acrylic)
            {
                acrylic.TintColor = color;
            }

            if (useAutoSave && GlobalData.Config != null && GlobalData.Config.BackdropTintColor != color)
            {
                GlobalData.Config.BackdropTintColor = color;
                GlobalData.Save();
            }
        }
    }

    public void SetBackdropFallbackColor(Color color)
    {
        var systemBackdrop = Window.SystemBackdrop;
        if (systemBackdrop != null)
        {
            if (systemBackdrop is MicaSystemBackdrop mica)
            {
                mica.FallbackColor = color;
            }
            else if (systemBackdrop is AcrylicSystemBackdrop acrylic)
            {
                acrylic.FallbackColor = color;
            }

            if (useAutoSave && GlobalData.Config != null && GlobalData.Config.BackdropFallBackColor != color)
            {
                GlobalData.Config.BackdropFallBackColor = color;
                GlobalData.Save();
            }
        }
    }
}
