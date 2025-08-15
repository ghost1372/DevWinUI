using WinRT;

namespace DevWinUI;
public partial class ThemeService
{
    private void ConfigBackdropBase(BackdropType backdropType, bool force)
    {
        if (WindowHelper.ActiveWindows.Count == 0)
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
            backdropType = GetSystemBackdropFromLocalConfig(backdropType, force);

            SetWindowSystemBackdrop(backdropType);
        }
    }

    private void ConfigTintColorBase(Color? color, bool force)
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
    private Color? GetDefaultTintColor()
    {
        switch (GetBackdropType())
        {
            case BackdropType.Mica:
                return new MicaSystemBackdrop().TintColor;
            case BackdropType.MicaAlt:
                return new MicaSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt).TintColor;
            case BackdropType.Acrylic:
            case BackdropType.AcrylicThin:
                return new AcrylicSystemBackdrop().TintColor;
            default:
                return null;
        }
    }

    private Color? GetDefaultFallbackTintColor()
    {
        switch (GetBackdropType())
        {
            case BackdropType.Mica:
                return new MicaSystemBackdrop().FallbackColor;
            case BackdropType.MicaAlt:
                return new MicaSystemBackdrop(Microsoft.UI.Composition.SystemBackdrops.MicaKind.BaseAlt).FallbackColor;
            case BackdropType.Acrylic:
            case BackdropType.AcrylicThin:
                return new AcrylicSystemBackdrop().FallbackColor;
            default:
                return null;
        }
    }
    private void ConfigTintColorBase()
    {
        ConfigTintColorBase(GetDefaultTintColor(), false);
    }
    private void ConfigFallbackColorBase(Color? color, bool force)
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
        ConfigFallbackColorBase(GetDefaultFallbackTintColor(), false);
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
