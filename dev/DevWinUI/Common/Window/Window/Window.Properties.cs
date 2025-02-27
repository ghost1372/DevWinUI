namespace DevWinUI;
public partial class Window
{
    #region Base
    public new AppWindow AppWindow => base.AppWindow;
    public new string Title
    {
        get => base.Title;
        set
        {
            base.Title = value;
            base.AppWindow.Title = value;
        }
    }

    public new UIElement Content
    {
        get => base.Content;
        set => base.Content = value;
    }
    #endregion

    #region Only Get Property
    public double RasterizationScale
    {
        get
        {
            if (Content != null && Content.XamlRoot != null)
            {
                return Content.XamlRoot.RasterizationScale;
            }
            return 0.0;
        }
    }

    public DisplayMonitorDetails DisplayMonitor
    {
        get => DisplayMonitorHelper.GetMonitorInfo(Hwnd);
    }
    public IntPtr Hwnd
    {
        get => WindowNative.GetWindowHandle(this);
    }
    public WindowId WindowId
    {
        get => AppWindow.Id;
    }

    public bool IsVisible
    {
        get => AppWindow.IsVisible;
    }
    public RainbowFrame GetRainbowFrame
    {
        get => rainbowFrame;
    }
    public ModernSystemMenu GetModernSystemMenu
    {
        get => modernSystemMenu;
    }
    public SizeInt32 ClientSize
    {
        get => AppWindow.ClientSize;
    }
    public SizeInt32 Size
    {
        get => AppWindow.Size;
    }

    #endregion

    #region Only Set Property
    public WindowLayoutOption WindowLayout
    {
        set
        {
            switch (value)
            {
                case WindowLayoutOption.RightToLeft:
                    GeneralHelper.SetApplicationLayoutRTL(Hwnd);
                    break;
                case WindowLayoutOption.LeftToRight:
                    GeneralHelper.SetApplicationLayoutLTR(Hwnd);
                    break;
                default:
                    GeneralHelper.SetApplicationLayoutLTR(Hwnd);
                    break;
            }
        }
    }

    public ElementTheme LegacySystemMenuTheme
    {
        set => GeneralHelper.SetPreferredAppMode(value);
    }

    #endregion

    #region Get/Set Property
    public bool IsShownInSwitchers
    {
        get => AppWindow.IsShownInSwitchers;
        set => AppWindow.IsShownInSwitchers = value;
    }
    public int Width
    {
        get => AppWindow.Size.Width;
        set => AppWindow.Resize(new SizeInt32(value, Height));
    }
    public int Height
    {
        get => AppWindow.Size.Height;
        set => AppWindow.Resize(new SizeInt32(Width, value));
    }
    public int ClientWidth
    {
        get => AppWindow.ClientSize.Width;
        set => AppWindow.Resize(new SizeInt32(value, ClientHeight));
    }
    public int ClientHeight
    {
        get => AppWindow.ClientSize.Height;
        set => AppWindow.Resize(new SizeInt32(ClientWidth, value));
    }
    public BackdropType BackdropType
    {
        get => ThemeService.GetBackdropTypeFromSystemBackdrop(SystemBackdrop);
        set => SystemBackdrop = ThemeService.GetSystemBackdropFromType(value);
    }
    public NativeValues.DWM_WINDOW_CORNER_PREFERENCE CornerRadius
    {
        get => WindowHelper.GetWindowCornerRadius(this);
        set
        {
            WindowHelper.SetWindowCornerRadius(this, value);
        }
    }

    public bool UseModernSystemMenu
    {
        get => modernSystemMenu.IsModernSystemMenuEnabled;
        set
        {
            modernSystemMenu.IsModernSystemMenuEnabled = value;
        }
    }
    public bool IsAlwaysOnTop
    {
        get => ((OverlappedPresenter)AppWindow.Presenter).IsAlwaysOnTop;
        set
        {
            switch (AppWindow.Presenter.Kind)
            {
                case AppWindowPresenterKind.Overlapped:
                case AppWindowPresenterKind.Default:
                    ((OverlappedPresenter)AppWindow.Presenter).IsAlwaysOnTop = value;
                    break;
            }
        }
    }

    public bool IsMinimizable
    {
        get => ((OverlappedPresenter)AppWindow.Presenter).IsMinimizable;
        set
        {
            switch (AppWindow.Presenter.Kind)
            {
                case AppWindowPresenterKind.Overlapped:
                case AppWindowPresenterKind.Default:
                    ((OverlappedPresenter)AppWindow.Presenter).IsMinimizable = value;
                    break;
            }
        }
    }

    public bool IsMaximizable
    {
        get => ((OverlappedPresenter)AppWindow.Presenter).IsMaximizable;
        set
        {
            switch (AppWindow.Presenter.Kind)
            {
                case AppWindowPresenterKind.Overlapped:
                case AppWindowPresenterKind.Default:
                    ((OverlappedPresenter)AppWindow.Presenter).IsMaximizable = value;
                    break;
            }
        }
    }

    public bool IsResizable
    {
        get => ((OverlappedPresenter)AppWindow.Presenter).IsResizable;
        set
        {
            switch (AppWindow.Presenter.Kind)
            {
                case AppWindowPresenterKind.Overlapped:
                case AppWindowPresenterKind.Default:
                    ((OverlappedPresenter)AppWindow.Presenter).IsResizable = value;
                    break;
            }
        }
    }

    public bool HasTitleBar
    {
        get => ((OverlappedPresenter)AppWindow.Presenter).HasTitleBar;
        set
        {
            switch (AppWindow.Presenter.Kind)
            {
                case AppWindowPresenterKind.Overlapped:
                case AppWindowPresenterKind.Default:
                    ((OverlappedPresenter)AppWindow.Presenter).SetBorderAndTitleBar(true, value);
                    break;
            }
        }
    }

    #endregion

    #region Full Property
    private FrameworkElement titleBar;
    public FrameworkElement TitleBar
    {
        get { return titleBar; }
        set { titleBar = value; }
    }

    private bool useRainbowFrame;
    public bool UseRainbowFrame
    {
        get
        {
            rainbowFrame.StopRainbowFrame();
            return useRainbowFrame;
        }
        set
        {
            useRainbowFrame = true;
            rainbowFrame.StartRainbowFrame();
        }
    }

    private string icon = "Assets/icon.ico";
    public string Icon
    {
        get => icon;
        set
        {
            icon = value;
            AppWindow.SetIcon(value);
        }
    }

    private OverlappedPresenterState windowState;
    public OverlappedPresenterState WindowState
    {
        get => windowState;
        set
        {
            windowState = value;
            switch (value)
            {
                case OverlappedPresenterState.Restored:
                    ((OverlappedPresenter)AppWindow.Presenter).Restore();
                    break;
                case OverlappedPresenterState.Minimized:
                    ((OverlappedPresenter)AppWindow.Presenter).Minimize();
                    break;
                case OverlappedPresenterState.Maximized:
                    ((OverlappedPresenter)AppWindow.Presenter).Maximize();
                    break;
            }
        }
    }
    private bool autoTrackWindow;
    public bool AutoTrackWindow
    {
        get => autoTrackWindow;
        set
        {
            autoTrackWindow = value;
            if (value)
            {
                WindowHelper.TrackWindow(this);
            }
            else
            {
                WindowHelper.RemoveWindowFromTrack(this);
            }
        }
    }
    #endregion
}
