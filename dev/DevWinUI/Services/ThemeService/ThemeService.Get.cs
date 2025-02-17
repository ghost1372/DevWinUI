using Microsoft.UI.Composition.SystemBackdrops;

namespace DevWinUI;
public partial class ThemeService
{
    public ElementTheme GetElementTheme()
    {
        return RootTheme;
    }
    public ApplicationTheme GetApplicationTheme()
    {
        switch (RootTheme)
        {
            default:
            case ElementTheme.Default:
                return Application.Current.RequestedTheme == ApplicationTheme.Dark
                ? ApplicationTheme.Dark
                : ApplicationTheme.Light;
            case ElementTheme.Light:
                return ApplicationTheme.Light;
            case ElementTheme.Dark:
                return ApplicationTheme.Dark;
        }
    }

    public ElementTheme GetActualTheme()
    {
        return ActualTheme;
    }

    public SystemBackdrop GetSystemBackdrop()
    {
        return WindowHelper.ActiveWindows[0].SystemBackdrop;
    }

    public SystemBackdrop GetSystemBackdrop(BackdropType backdropType)
    {
        switch (backdropType)
        {
            case BackdropType.None:
                return null;
            case BackdropType.Mica:
                return new MicaSystemBackdrop(MicaKind.Base);
            case BackdropType.MicaAlt:
                return new MicaSystemBackdrop(MicaKind.BaseAlt);
            case BackdropType.DesktopAcrylic:
                return new DesktopAcrylicBackdrop();
            case BackdropType.AcrylicBase:
                return new AcrylicSystemBackdrop(DesktopAcrylicKind.Base);
            case BackdropType.AcrylicThin:
                return new AcrylicSystemBackdrop(DesktopAcrylicKind.Thin);
            case BackdropType.Transparent:
                return new TransparentBackdrop();
            default:
                return null;
        }
    }

    public BackdropType GetBackdropType()
    {
        return GetBackdropType(WindowHelper.ActiveWindows[0].SystemBackdrop);
    }

    public BackdropType GetBackdropType(SystemBackdrop systemBackdrop)
    {
        if (systemBackdrop is MicaSystemBackdrop mica)
        {
            return mica.Kind == MicaKind.Base ? BackdropType.Mica : BackdropType.MicaAlt;
        }
        else if (systemBackdrop is TransparentBackdrop)
        {
            return BackdropType.Transparent;
        }
        else if (systemBackdrop is AcrylicSystemBackdrop acrylic)
        {
            return acrylic.Kind == DesktopAcrylicKind.Base ? BackdropType.AcrylicBase : BackdropType.AcrylicThin;
        }
        else if (systemBackdrop is DesktopAcrylicBackdrop)
        {
            return BackdropType.DesktopAcrylic;
        }
        else
        {
            return BackdropType.None;
        }
    }

    private BackdropType GetSystemBackdropFromLocalConfig(BackdropType backdropType, bool ForceBackdrop)
    {
        BackdropType currentBackdrop = backdropType;
        if (this.useAutoSave && GlobalData.Config != null)
        {
            currentBackdrop = GlobalData.Config.BackdropType;
        }

        return ForceBackdrop ? backdropType : currentBackdrop;
    }

    private Color? GetBackdropTintColorFromLocalConfig(Color? color, bool ForceTintColor)
    {
        Color? tintColor = color;
        if (this.useAutoSave && GlobalData.Config != null)
        {
            tintColor = GlobalData.Config.BackdropTintColor;
        }

        return ForceTintColor ? color : tintColor;
    }

    private Color? GetBackdropFallbackColorFromLocalConfig(Color? color, bool ForceFallbackColor)
    {
        Color? fallbackColor = color;
        if (this.useAutoSave && GlobalData.Config != null)
        {
            fallbackColor = GlobalData.Config.BackdropFallBackColor;
        }

        return ForceFallbackColor ? color : fallbackColor;
    }

    private ElementTheme GetElementThemeFromLocalConfig(ElementTheme theme, bool forceTheme)
    {
        ElementTheme currentTheme = theme;
        if (this.useAutoSave && GlobalData.Config != null)
        {
            currentTheme = GlobalData.Config.ElementTheme;
        }
        return forceTheme ? theme : currentTheme;
    }
}
