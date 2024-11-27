using WinRT.Interop;
using WinRT;

namespace DevWinUI;
public partial class ThemeService
{
    private void ConfigBackdropBase(BackdropType backdropType, bool force)
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

    private void ConfigTintColorBase(Color color, bool force)
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
    private void ConfigTintColorBase()
    {
        var systemBackdrop = Window.SystemBackdrop;
        if (systemBackdrop != null)
        {
            if (IsDarkTheme())
            {
                switch (GetBackdropType())
                {
                    case BackdropType.Mica:
                        ConfigTintColorBase(MicaSystemBackdrop.Default_TintColor_Dark, false);
                        break;
                    case BackdropType.MicaAlt:
                        ConfigTintColorBase(MicaSystemBackdrop.Default_TintColor_MicaAlt_Dark, false);
                        break;
                    case BackdropType.AcrylicThin:
                    case BackdropType.AcrylicBase:
                        ConfigTintColorBase(AcrylicSystemBackdrop.Default_TintColor_Dark, false);
                        break;
                }
            }
            else
            {
                switch (GetBackdropType())
                {
                    case BackdropType.Mica:
                        ConfigTintColorBase(MicaSystemBackdrop.Default_TintColor_Light, false);
                        break;
                    case BackdropType.MicaAlt:
                        ConfigTintColorBase(MicaSystemBackdrop.Default_TintColor_MicaAlt_Light, false);
                        break;
                    case BackdropType.AcrylicThin:
                    case BackdropType.AcrylicBase:
                        ConfigTintColorBase(AcrylicSystemBackdrop.Default_TintColor_Light, false);
                        break;
                }
            }

        }
    }
    private void ConfigFallbackColorBase(Color color, bool force)
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

    private void ConfigFallbackColorBase()
    {
        var systemBackdrop = Window.SystemBackdrop;
        if (systemBackdrop != null)
        {
            if (IsDarkTheme())
            {
                switch (GetBackdropType())
                {
                    case BackdropType.MicaAlt:
                        ConfigFallbackColorBase(MicaSystemBackdrop.Default_FallbackColor_MicaAlt_Dark, false);
                        break;
                    case BackdropType.AcrylicThin:
                    case BackdropType.AcrylicBase:
                        ConfigFallbackColorBase(AcrylicSystemBackdrop.Default_FallbackColor_Dark, false);
                        break;
                }
            }
            else
            {
                switch (GetBackdropType())
                {
                    case BackdropType.MicaAlt:
                        ConfigFallbackColorBase(MicaSystemBackdrop.Default_FallbackColor_MicaAlt_Light, false);
                        break;
                    case BackdropType.AcrylicThin:
                    case BackdropType.AcrylicBase:
                        ConfigFallbackColorBase(AcrylicSystemBackdrop.Default_FallbackColor_Light, false);
                        break;
                }
            }
        }
    }
    private void ConfigElementThemeBase(ElementTheme elementTheme, bool force)
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

    private void EnableRequestedThemeBase()
    {
        try
        {
            unsafe
            {
                *(bool*)(((IWinRTObject)this).NativeObject.As<IUnknownVftbl>(IID).ThisPtr + 0x118U) = true;
            }
        }
        catch (Exception)
        {
        }
    }

    private static ref readonly Guid IID
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return ref Unsafe.As<byte, Guid>(ref MemoryMarshal.GetReference((ReadOnlySpan<byte>)new byte[16]
            {
            231, 244, 168, 6, 70, 17, 175, 85, 130, 13,
            235, 213, 86, 67, 176, 33
            }));
        }
    }
}
