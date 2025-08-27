using System.Collections.Specialized;
using Microsoft.UI.Composition.SystemBackdrops;

namespace DevWinUI;
public partial class ThemeService : IThemeService, IDisposable
{
    public event EventHandler<ElementTheme>? ThemeChanged;
    public event EventHandler<BackdropType>? BackdropChanged;

    private readonly Dictionary<Window, FrameworkElement> _rootElements = new();
    private TaskCompletionSource<bool>? _initialization;

    private readonly string ConfigFilePath = "CoreAppConfigV9.0.0.json";

    private bool _useAutoSave = true;
    private bool _isBackdropEnabled = true;

    private ElementTheme _userDefinedTheme = ElementTheme.Default;
    private BackdropType _userDefinedBackdrop = BackdropType.Mica;
    private string _userDefinedFileName = null;
    public ThemeService() { }

    public ThemeService Initialize(Window window)
    {
        string RootPath = Path.Combine(PathHelper.GetAppDataFolderPath(), ProcessInfoHelper.ProductNameAndVersion);
        string AppConfigPath = Path.Combine(RootPath, ConfigFilePath);

        if (_useAutoSave)
        {
            if (!string.IsNullOrEmpty(_userDefinedFileName))
            {
                AppConfigPath = _userDefinedFileName;
            }

            GlobalData.SavePath = AppConfigPath;
            if (!Directory.Exists(RootPath))
            {
                Directory.CreateDirectory(RootPath);
            }
            GlobalData.Init();
        }

        WindowHelper.TrackWindow(window);
        WindowHelper.ActiveWindows.CollectionChanged -= ActiveWindows_CollectionChanged;
        WindowHelper.ActiveWindows.CollectionChanged += ActiveWindows_CollectionChanged;

        foreach (var item in WindowHelper.ActiveWindows)
        {
            if (!item.Dispatcher.HasThreadAccess)
                _ = item.Dispatcher.ExecuteAsync(ct => InitWindow(item.Window, ct));
            else
                _ = InitWindow(item.Window, CancellationToken.None);
        }

        return this;
    }

    private async void ActiveWindows_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
        {
            foreach (TrackWindowItem item in e.NewItems)
            {
                RegisterRootElement(item.Window);
                await SetElementThemeWithoutSaveAsync(ElementTheme);
                await SetBackdropTypeWithoutSaveAsync(BackdropType);
            }
        }
        else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
        {
            foreach (TrackWindowItem item in e.OldItems)
            {
                UnregisterRootElement(item.Window);
            }
        }
    }

    private void UnregisterRootElement(Window window)
    {
        if (_rootElements.TryGetValue(window, out var root))
        {
            root.ActualThemeChanged -= ElementThemeChanged;
            _rootElements.Remove(window);
        }
    }

    private void RegisterRootElement(Window window)
    {
        if (window.Content is FrameworkElement root)
        {
            if (!_rootElements.ContainsKey(window))
            {
                root.ActualThemeChanged -= ElementThemeChanged;
                root.ActualThemeChanged += ElementThemeChanged;
                _rootElements[window] = root;
            }
        }
    }

    private async ValueTask InitWindow(Window window, CancellationToken ct)
    {
        RegisterRootElement(window);
        await InitializeAsync();
    }

    public bool IsDark => _rootElements?.Values.FirstOrDefault()?.ActualTheme == ElementTheme.Dark;
    public ElementTheme ActualTheme => _rootElements.Values.FirstOrDefault().ActualTheme;
    public ElementTheme ElementTheme => GetInternalElementTheme();
    public BackdropType BackdropType => GetInternalBackdropType();

    private ElementTheme GetInternalElementTheme()
    {
        if (_rootElements?.Values?.FirstOrDefault() != null)
        {
            return _rootElements.Values.FirstOrDefault().RequestedTheme;
        }

        return ElementTheme.Default;
    }
    private BackdropType GetInternalBackdropType()
    {
        return GetInternalBackdropType(_rootElements?.Keys.FirstOrDefault().SystemBackdrop);
    }

    private BackdropType GetInternalBackdropType(SystemBackdrop systemBackdrop)
    {
        return CreateBackdropType(systemBackdrop);
    }
    private static BackdropType CreateBackdropType(SystemBackdrop systemBackdrop)
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
            return acrylic.Kind == (DesktopAcrylicKind.Base | DesktopAcrylicKind.Default) ? BackdropType.Acrylic : BackdropType.AcrylicThin;
        }
        else
        {
            return BackdropType.None;
        }
    }
    private void ElementThemeChanged(FrameworkElement sender, object args)
    {
        ThemeChanged?.Invoke(this, GetInternalElementTheme());
    }

    private async Task InitializeAsync()
    {
        if (_initialization is not null)
        {
            await _initialization.Task.ConfigureAwait(false);
            return;
        }

        _initialization = new TaskCompletionSource<bool>();

        ElementTheme theme = _useAutoSave ? GetSavedElementTheme() : _userDefinedTheme;
        BackdropType backdrop = _useAutoSave ? GetSavedBackdropType() : _userDefinedBackdrop;

        var success = await InternalSetElementThemeAsync(theme);
        var successBackdrop = await InternalSetBackdropTypeAsync(backdrop);

        if (!success)
        {
            // If theme not set immediately, wait until any window is loaded
            foreach (var root in _rootElements.Values)
            {
                if (root is FrameworkElement fe)
                {
                    async void OnLoaded(object sender, RoutedEventArgs args)
                    {
                        fe.Loaded -= OnLoaded;
                        var themeSet = await InternalSetElementThemeAsync(theme);
                        CompleteInitialization(themeSet);

                        if (!successBackdrop)
                        {
                            var backdropSet = await InternalSetBackdropTypeAsync(backdrop);
                            CompleteInitialization(backdropSet);
                        }
                    }

                    fe.Loaded += OnLoaded;
                    await _initialization.Task;
                    return;
                }
            }

            CompleteInitialization(false);
        }
        else
        {
            CompleteInitialization(true);
        }
    }

    private void CompleteInitialization(bool success)
    {
        var init = _initialization;
        if (init is null) return;
        init.TrySetResult(success);
    }

    public void Dispose()
    {
        WindowHelper.ActiveWindows.CollectionChanged -= ActiveWindows_CollectionChanged;

        foreach (var kvp in _rootElements)
        {
            kvp.Value.ActualThemeChanged -= ElementThemeChanged;
        }
        _rootElements.Clear();
    }

    private void UpdateCaptionButtons(Microsoft.UI.Xaml.Window window)
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
}

