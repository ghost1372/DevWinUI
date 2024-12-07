
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Dispatching;

namespace DevWinUI;
public partial class ThemeService
{
    private void SetWindowSystemBackdrop(BackdropType backdropType)
    {
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            foreach (var window in WindowHelper.ActiveWindows)
            {
                window.SystemBackdrop = GetSystemBackdrop(backdropType);
            }
        });
    }

    public void SetBackdropType(BackdropType backdropType)
    {
        if (useAutoSave && GlobalData.Config != null)
        {
            if (GlobalData.Config.BackdropType != backdropType)
            {
                SetWindowSystemBackdrop(backdropType);

                GlobalData.Config.BackdropType = backdropType;
                GlobalData.Save();
            }
        }
        else
        {
            SetWindowSystemBackdrop(backdropType);
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
        foreach (var window in WindowHelper.ActiveWindows)
        {
            var systemBackdrop = window.SystemBackdrop;
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
    }

    public void SetBackdropFallbackColor(Color color)
    {
        foreach (var window in WindowHelper.ActiveWindows)
        {
            var systemBackdrop = window.SystemBackdrop;
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
}
