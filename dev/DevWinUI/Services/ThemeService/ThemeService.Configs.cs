namespace DevWinUI;
public partial class ThemeService
{
    public void ConfigBackdrop(BackdropType backdropType = BackdropType.Mica, bool force = false)
    {
        if (Window == null)
        {
            return;
        }

        if (useAutoSave && GlobalData.Config != null && GlobalData.Config.IsBackdropFirstRun)
        {
            GlobalData.Config.BackdropType = backdropType;
            GlobalData.Config.IsBackdropFirstRun = false;
            GlobalData.Save();
        }

        if (backdropType != BackdropType.None)
        {
            var systemBackdrop = GetSystemBackdropFromLocalConfig(backdropType, force);

            SetWindowSystemBackdrop(systemBackdrop);
        }
    }

    public void ConfigTintColor(Color color, bool force)
    {
        if (useAutoSave && GlobalData.Config != null && GlobalData.Config.IsBackdropTintColorFirstRun)
        {
            GlobalData.Config.BackdropTintColor = color;
            GlobalData.Config.IsBackdropTintColorFirstRun = false;
            GlobalData.Save();
        }

        var tintColor = GetBackdropTintColorFromLocalConfig(color, force);

        SetBackdropTintColor(tintColor);
    }
    public void ConfigTintColor()
    {
        var systemBackdrop = Window.SystemBackdrop;
        if (systemBackdrop != null)
        {
            if (IsDarkTheme())
            {
                switch (GetBackdropType())
                {
                    case BackdropType.Mica:
                        ConfigTintColor(MicaSystemBackdrop.Default_TintColor_Dark, false);
                        break;
                    case BackdropType.MicaAlt:
                        ConfigTintColor(MicaSystemBackdrop.Default_TintColor_MicaAlt_Dark, false);
                        break;
                    case BackdropType.AcrylicThin:
                    case BackdropType.AcrylicBase:
                        ConfigTintColor(AcrylicSystemBackdrop.Default_TintColor_Dark, false);
                        break;
                }
            }
            else
            {
                switch (GetBackdropType())
                {
                    case BackdropType.Mica:
                        ConfigTintColor(MicaSystemBackdrop.Default_TintColor_Light, false);
                        break;
                    case BackdropType.MicaAlt:
                        ConfigTintColor(MicaSystemBackdrop.Default_TintColor_MicaAlt_Light, false);
                        break;
                    case BackdropType.AcrylicThin:
                    case BackdropType.AcrylicBase:
                        ConfigTintColor(AcrylicSystemBackdrop.Default_TintColor_Light, false);
                        break;
                }
            }

        }
    }
    public void ConfigFallbackColor(Color color, bool force)
    {
        if (useAutoSave && GlobalData.Config != null && GlobalData.Config.IsBackdropFallBackColorFirstRun)
        {
            GlobalData.Config.BackdropFallBackColor = color;
            GlobalData.Config.IsBackdropFallBackColorFirstRun = false;
            GlobalData.Save();
        }

        var tintColor = GetBackdropFallbackColorFromLocalConfig(color, force);

        SetBackdropFallbackColor(tintColor);
    }

    public void ConfigFallbackColor()
    {
        var systemBackdrop = Window.SystemBackdrop;
        if (systemBackdrop != null)
        {
            if (IsDarkTheme())
            {
                switch (GetBackdropType())
                {
                    case BackdropType.MicaAlt:
                        ConfigFallbackColor(MicaSystemBackdrop.Default_FallbackColor_MicaAlt_Dark, false);
                        break;
                    case BackdropType.AcrylicThin:
                    case BackdropType.AcrylicBase:
                        ConfigFallbackColor(AcrylicSystemBackdrop.Default_FallbackColor_Dark, false);
                        break;
                }
            }
            else
            {
                switch (GetBackdropType())
                {
                    case BackdropType.MicaAlt:
                        ConfigFallbackColor(MicaSystemBackdrop.Default_FallbackColor_MicaAlt_Light, false);
                        break;
                    case BackdropType.AcrylicThin:
                    case BackdropType.AcrylicBase:
                        ConfigFallbackColor(AcrylicSystemBackdrop.Default_FallbackColor_Light, false);
                        break;
                }
            }
        }
    }
    public void ConfigElementTheme(ElementTheme elementTheme = ElementTheme.Default, bool force = false)
    {
        if (useAutoSave && GlobalData.Config != null && GlobalData.Config.IsThemeFirstRun)
        {
            GlobalData.Config.ElementTheme = elementTheme;
            GlobalData.Config.IsThemeFirstRun = false;
            GlobalData.Save();
        }

        if (useAutoSave && GlobalData.Config != null)
        {
            SetElementTheme(GetElementThemeFromLocalConfig(elementTheme, force));
        }
        else
        {
            SetElementTheme(elementTheme);
        }
    }
}
