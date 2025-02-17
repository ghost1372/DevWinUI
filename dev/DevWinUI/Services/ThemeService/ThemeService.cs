using System.Collections.Specialized;

namespace DevWinUI;
public partial class ThemeService : IThemeService
{
    public readonly string ConfigFilePath = "CoreAppConfigV8.1.0.json";
    public event IThemeService.ActualThemeChangedEventHandler ActualThemeChanged;
    private bool changeThemeWithoutSave = false;
    private bool useAutoSave;
    public ElementTheme ActualTheme
    {
        get
        {
            foreach (Window window in WindowHelper.ActiveWindows)
            {
                if (window.Content is FrameworkElement rootElement)
                {
                    if (rootElement.RequestedTheme != ElementTheme.Default)
                    {
                        return rootElement.RequestedTheme;
                    }
                }
            }

            return GeneralHelper.GetEnum<ElementTheme>(Application.Current.RequestedTheme.ToString());
        }
    }
    public ElementTheme RootTheme
    {
        get
        {
            foreach (Window window in WindowHelper.ActiveWindows)
            {
                if (window.Content is FrameworkElement rootElement)
                {
                    return rootElement.RequestedTheme;
                }
            }

            return ElementTheme.Default;
        }
        set
        {
            foreach (Window window in WindowHelper.ActiveWindows)
            {
                if (window.Content is FrameworkElement rootElement)
                {
                    rootElement.RequestedTheme = value;
                }
            }

            if (!changeThemeWithoutSave)
            {
                if (this.useAutoSave && GlobalData.Config != null)
                {
                    GlobalData.Config.ElementTheme = value;
                    GlobalData.Save();
                }
            }
        }
    }

    public ThemeService() { }
    public ThemeService(Window window)
    {
        InitializeBase(window);
        ConfigElementThemeBase(ElementTheme.Default, false);
        ConfigBackdropBase(BackdropType.Mica, false);
    }
    private void AutoInitializeBase(Window window)
    {
        InitializeBase(window);
        ConfigElementThemeBase(ElementTheme.Default, false);
        ConfigBackdropBase(BackdropType.Mica, false);
    }

    private void InitializeBase(Window window, bool useAutoSave = true, string filename = null)
    {
        if (window == null)
        {
            return;
        }

        WindowHelper.TrackWindow(window);
        WindowHelper.ActiveWindows.CollectionChanged -= OnActiveWindowsChanged;
        WindowHelper.ActiveWindows.CollectionChanged += OnActiveWindowsChanged;

        foreach (var windowItem in WindowHelper.ActiveWindows)
        {
            if (windowItem.Content is FrameworkElement element)
            {
                GeneralHelper.SetPreferredAppMode(element.ActualTheme);
                element.ActualThemeChanged -= OnActualThemeChanged;
                element.ActualThemeChanged += OnActualThemeChanged;
            }
        }

        string RootPath = Path.Combine(PathHelper.GetAppDataFolderPath(), ProcessInfoHelper.ProductNameAndVersion);
        string AppConfigPath = Path.Combine(RootPath, ConfigFilePath);

        this.useAutoSave = useAutoSave;

        if (useAutoSave)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                AppConfigPath = filename;
            }

            GlobalData.SavePath = AppConfigPath;
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }
            GlobalData.Init();
        }
    }
    private void OnActiveWindowsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null)
        {
            foreach (Window window in e.NewItems)
            {
                InitializeWindowPropertiesOnce(window);
            }
        }
    }
    private void InitializeWindowPropertiesOnce(Window window)
    {
        RootTheme = GetElementTheme();
        window.SystemBackdrop = GetSystemBackdrop();
        if (window.SystemBackdrop is MicaSystemBackdrop mica)
        {
            mica.TintColor = GlobalData.Config?.BackdropTintColor ?? GetDefaultTintColor();
            mica.FallbackColor = GlobalData.Config?.BackdropFallBackColor ?? GetDefaultFallbackTintColor();
        }
        else if (window.SystemBackdrop is AcrylicSystemBackdrop acrylic)
        {
            acrylic.TintColor = GlobalData.Config?.BackdropTintColor ?? GetDefaultTintColor();
            acrylic.FallbackColor = GlobalData.Config?.BackdropFallBackColor ?? GetDefaultFallbackTintColor();
        }
    }
    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        GeneralHelper.SetPreferredAppMode(sender.ActualTheme);
        ActualThemeChanged?.Invoke(sender, args);
    }

    public void UpdateCaptionButtons()
    {
        foreach (var window in WindowHelper.ActiveWindows)
        {
            UpdateCaptionButtons(window);
        }
    }

    public void UpdateCaptionButtons(Window window)
    {
        if (window == null)
            return;

        var res = Application.Current.Resources;
        Windows.UI.Color buttonForegroundColor;
        Windows.UI.Color buttonHoverForegroundColor;

        Windows.UI.Color buttonHoverBackgroundColor;
        if (ActualTheme == ElementTheme.Dark)
        {
            buttonForegroundColor = DevWinUI.ColorHelper.GetColorFromHex("#FFFFFF");
            buttonHoverForegroundColor = DevWinUI.ColorHelper.GetColorFromHex("#FFFFFF");

            buttonHoverBackgroundColor = DevWinUI.ColorHelper.GetColorFromHex("#0FFFFFFF");
        }
        else
        {
            buttonForegroundColor = DevWinUI.ColorHelper.GetColorFromHex("#191919");
            buttonHoverForegroundColor = DevWinUI.ColorHelper.GetColorFromHex("#191919");

            buttonHoverBackgroundColor = DevWinUI.ColorHelper.GetColorFromHex("#09000000");
        }
        res["WindowCaptionForeground"] = buttonForegroundColor;

        window.AppWindow.TitleBar.ButtonForegroundColor = buttonForegroundColor;
        window.AppWindow.TitleBar.ButtonHoverForegroundColor = buttonHoverForegroundColor;

        window.AppWindow.TitleBar.ButtonHoverBackgroundColor = buttonHoverBackgroundColor;
    }
    
    public bool IsDarkTheme()
    {
        return RootTheme == ElementTheme.Default
            ? Application.Current.RequestedTheme == ApplicationTheme.Dark
            : RootTheme == ElementTheme.Dark;
    }

    public static void ChangeThemeWithoutSave(Window window)
    {
        var element = window?.Content as FrameworkElement;

        if (element == null)
        {
            return;
        }

        if (element.ActualTheme == ElementTheme.Light)
        {
            element.RequestedTheme = ElementTheme.Dark;
        }
        else if (element.ActualTheme == ElementTheme.Dark)
        {
            element.RequestedTheme = ElementTheme.Light;
        }
    }

    public void ResetBackdropProperties()
    {
        var backdrop = GetSystemBackdrop();
        if (backdrop != null)
        {
            if (backdrop is MicaSystemBackdrop mica)
            {
                mica.micaController.ResetProperties();
                if (useAutoSave && GlobalData.Config != null)
                {
                    GlobalData.Config.BackdropFallBackColor = null;
                    GlobalData.Config.BackdropTintColor = null;
                    GlobalData.Save();
                }
            }
            else if (backdrop is AcrylicSystemBackdrop acrylic)
            {
                acrylic.acrylicController.ResetProperties();
                if (useAutoSave && GlobalData.Config != null)
                {
                    GlobalData.Config.BackdropFallBackColor = null;
                    GlobalData.Config.BackdropTintColor = null;
                    GlobalData.Save();
                }
            }
        }
    }
}
