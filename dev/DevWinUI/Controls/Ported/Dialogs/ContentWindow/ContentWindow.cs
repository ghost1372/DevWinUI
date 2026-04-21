// https://github.com/SuGar0218/SuGarToolkit.WinUI3

using System.ComponentModel;
using Microsoft.UI.Windowing;

namespace DevWinUI;

public partial class ContentWindow : ContentControl
{
    public ContentWindow() : this(new Window()) { }
    public ContentWindow(Window window)
    {
        DefaultStyleKey = typeof(ContentWindow);
        _window = window;
        _hwnd = Win32Interop.GetWindowFromWindowId(_window.AppWindow.Id);
        _styleHelper = new WindowStyleHelper(_hwnd);
        monitor = new WindowMessageMonitor(_hwnd);
        monitor.WindowMessageReceived += OnWindowMessageReceived;
        Loaded += OnLoaded;
        Window.AppWindow.Changed += OnAppWindowStateChanged;
        Window.AppWindow.Closing += OnAppWindowClosing;
        Window.AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
        Window.Activated += OnWindowActivated;
        Window.Content = this;
        RegisterPropertyChangedCallback(RequestedThemeProperty, OnRequestedThemeChanged);
    }

    private readonly Window _window;
    private readonly IntPtr _hwnd;
    private readonly WindowStyleHelper _styleHelper;
    private readonly WindowMessageMonitor monitor;

    #region DependencyProperty

    public bool CanMinimize
    {
        get => (bool) GetValue(CanMinimizeProperty);
        set => SetValue(CanMinimizeProperty, value);
    }

    public static readonly DependencyProperty CanMinimizeProperty = DependencyProperty.Register(
        nameof(CanMinimize),
        typeof(bool),
        typeof(ContentWindow),
        new PropertyMetadata(true, OnCanMinimizeChanged)
    );

    public bool CanMaximize
    {
        get => (bool) GetValue(CanMaximizeProperty);
        set => SetValue(CanMaximizeProperty, value);
    }

    public static readonly DependencyProperty CanMaximizeProperty = DependencyProperty.Register(
        nameof(CanMaximize),
        typeof(bool),
        typeof(ContentWindow),
        new PropertyMetadata(true, OnCanMaximizeChanged)
    );

    public bool CanResize
    {
        get => (bool) GetValue(CanResizeProperty);
        set => SetValue(CanResizeProperty, value);
    }

    public static readonly DependencyProperty CanResizeProperty = DependencyProperty.Register(
        nameof(CanResize),
        typeof(bool),
        typeof(ContentWindow),
        new PropertyMetadata(true, OnCanResizeChanged)
    );

    public new double Width
    {
        get => (double) GetValue(WidthProperty);
        set => SetValue(WidthProperty, value);
    }

    public static new readonly DependencyProperty WidthProperty = DependencyProperty.Register(
        nameof(Width),
        typeof(double),
        typeof(ContentWindow),
        new PropertyMetadata(double.NaN, OnWidthChanged)
    );

    private static void OnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (!self.IsLoaded)
            return;
        double newWidth = (double) e.NewValue;
        self.Resize(new Size(newWidth, self.Height));
    }

    public new double Height
    {
        get => (double) GetValue(HeightProperty);
        set => SetValue(HeightProperty, value);
    }

    public static new readonly DependencyProperty HeightProperty = DependencyProperty.Register(
        nameof(Height),
        typeof(double),
        typeof(ContentWindow),
        new PropertyMetadata(double.NaN, OnHeightChanged)
    );

    private static void OnHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (!self.IsLoaded)
            return;
        double newHeight = (double) e.NewValue;
        self.Resize(new Size(self.Width, newHeight));
    }

    public new double MinWidth
    {
        get => (double) GetValue(MinWidthProperty);
        set => SetValue(MinWidthProperty, value);
    }

    public static new readonly DependencyProperty MinWidthProperty = DependencyProperty.Register(
        nameof(MinWidth),
        typeof(double),
        typeof(ContentWindow),
        new PropertyMetadata(0, OnMinWidthChanged)
    );

    private static void OnMinWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        (self.Window.AppWindow.Presenter as OverlappedPresenter)?.PreferredMinimumWidth = self.DipToPixel((double) e.NewValue);
    }

    public new double MaxWidth
    {
        get => (double) GetValue(MaxWidthProperty);
        set => SetValue(MaxWidthProperty, value);
    }

    public static new readonly DependencyProperty MaxWidthProperty = DependencyProperty.Register(
        nameof(MaxWidth),
        typeof(double),
        typeof(ContentWindow),
        new PropertyMetadata(double.PositiveInfinity, OnMaxWidthChanged)
    );

    private static void OnMaxWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        (self.Window.AppWindow.Presenter as OverlappedPresenter)?.PreferredMaximumWidth = self.DipToPixel((double) e.NewValue);
    }

    public new double MinHeight
    {
        get => (double) GetValue(MinHeightProperty);
        set => SetValue(MinHeightProperty, value);
    }

    public static new readonly DependencyProperty MinHeightProperty = DependencyProperty.Register(
        nameof(MinHeight),
        typeof(double),
        typeof(ContentWindow),
        new PropertyMetadata(0, OnMinHeightChanged)
    );

    private static void OnMinHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        (self.Window.AppWindow.Presenter as OverlappedPresenter)?.PreferredMinimumHeight = self.DipToPixel((double) e.NewValue);
    }

    public new double MaxHeight
    {
        get => (double) GetValue(MaxHeightProperty);
        set => SetValue(MaxHeightProperty, value);
    }

    public static new readonly DependencyProperty MaxHeightProperty = DependencyProperty.Register(
        nameof(MaxHeight),
        typeof(double),
        typeof(ContentWindow),
        new PropertyMetadata(double.PositiveInfinity, OnMaxHeightChanged)
    );

    private static void OnMaxHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        (self.Window.AppWindow.Presenter as OverlappedPresenter)?.PreferredMaximumHeight = self.DipToPixel((double) e.NewValue);
    }

    public bool SizeToContent
    {
        get => (bool) GetValue(SizeToContentProperty);
        set => SetValue(SizeToContentProperty, value);
    }

    public static readonly DependencyProperty SizeToContentProperty = DependencyProperty.Register(
        nameof(SizeToContent),
        typeof(bool),
        typeof(ContentWindow),
        new PropertyMetadata(default(bool))
    );

    public WindowStartupLocation StartupLocation
    {
        get => (WindowStartupLocation) GetValue(StartupLocationProperty);
        set => SetValue(StartupLocationProperty, value);
    }

    public static readonly DependencyProperty StartupLocationProperty = DependencyProperty.Register(
        nameof(StartupLocation),
        typeof(WindowStartupLocation),
        typeof(ContentWindow),
        new PropertyMetadata(default(WindowStartupLocation))
    );

    public string? Title
    {
        get => (string?) GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title),
        typeof(string),
        typeof(ContentWindow),
        new PropertyMetadata(string.Empty, OnTitleChanged)
    );

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (e.NewValue is null)
        {
            self.Window.Title = string.Empty;
        }
        else
        {
            self.Window.Title = (string) e.NewValue;
        }
    }

    public SystemBackdrop? SystemBackdrop
    {
        get => (SystemBackdrop?) GetValue(SystemBackdropProperty);
        set => SetValue(SystemBackdropProperty, value);
    }

    public static readonly DependencyProperty SystemBackdropProperty = DependencyProperty.Register(
        nameof(SystemBackdrop),
        typeof(SystemBackdrop),
        typeof(ContentWindow),
        new PropertyMetadata(default(SystemBackdrop), OnSystemBackdropChanged)
    );

    private static void OnSystemBackdropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        self.Window.SystemBackdrop = (SystemBackdrop?) e.NewValue;
    }

    public bool ExtendsContentIntoTitleBar
    {
        get => (bool) GetValue(ExtendsContentIntoTitleBarProperty);
        set => SetValue(ExtendsContentIntoTitleBarProperty, value);
    }

    public static readonly DependencyProperty ExtendsContentIntoTitleBarProperty = DependencyProperty.Register(
        nameof(ExtendsContentIntoTitleBar),
        typeof(bool),
        typeof(ContentWindow),
        new PropertyMetadata(default(bool), OnExtendsContentIntoTitleBarChanged)
    );

    private static void OnExtendsContentIntoTitleBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        bool newValue = (bool) e.NewValue;
        if (newValue)
        {
            self.Window.AppWindow.TitleBar.ButtonBackgroundColor = self.TitleBarButtonBackgroundColor;
            self.Window.AppWindow.TitleBar.ButtonForegroundColor = self.TitleBarButtonForegroundColor;
            self.Window.AppWindow.TitleBar.ButtonHoverBackgroundColor = self.TitleBarButtonHoverBackgroundColor;
            self.Window.AppWindow.TitleBar.ButtonHoverForegroundColor = self.TitleBarButtonHoverForegroundColor;
            self.Window.AppWindow.TitleBar.ButtonPressedBackgroundColor = self.TitleBarButtonPressedBackgroundColor;
            self.Window.AppWindow.TitleBar.ButtonPressedForegroundColor = self.TitleBarButtonPressedForegroundColor;
        }
        else
        {
            self.Window.AppWindow.TitleBar.ButtonBackgroundColor = null;
            self.Window.AppWindow.TitleBar.ButtonForegroundColor = null;
            self.Window.AppWindow.TitleBar.ButtonHoverBackgroundColor = null;
            self.Window.AppWindow.TitleBar.ButtonHoverForegroundColor = null;
            self.Window.AppWindow.TitleBar.ButtonPressedBackgroundColor = null;
            self.Window.AppWindow.TitleBar.ButtonPressedForegroundColor = null;
        }
        self.Window.ExtendsContentIntoTitleBar = newValue;
    }

    public TitleBarHeightOption TitleBarHeightOption
    {
        get => (TitleBarHeightOption) GetValue(TitleBarHeightOptionProperty);
        set => SetValue(TitleBarHeightOptionProperty, value);
    }

    public static readonly DependencyProperty TitleBarHeightOptionProperty = DependencyProperty.Register(
        nameof(TitleBarHeightOption),
        typeof(TitleBarHeightOption),
        typeof(ContentWindow),
        new PropertyMetadata(default(TitleBarHeightOption), OnTitleBarHeightOptionChanged)
    );

    private static void OnTitleBarHeightOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        self.Window.AppWindow.TitleBar.PreferredHeightOption = (TitleBarHeightOption) e.NewValue;
    }

    public Color TitleBarButtonBackgroundColor
    {
        get => (Color) GetValue(TitleBarButtonBackgroundColorProperty);
        set => SetValue(TitleBarButtonBackgroundColorProperty, value);
    }

    public static readonly DependencyProperty TitleBarButtonBackgroundColorProperty = DependencyProperty.Register(
        nameof(TitleBarButtonBackgroundColor),
        typeof(Color),
        typeof(ContentWindow),
        new PropertyMetadata(default(Color), OnTitleBarButtonBackgroundColorChanged)
    );

    private static void OnTitleBarButtonBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (self.Window.ExtendsContentIntoTitleBar)
        {
            Color newColor = (Color) e.NewValue;
            self.Window.AppWindow.TitleBar.ButtonBackgroundColor = newColor.A == 0 ? null : newColor;
        }
    }

    public Color TitleBarButtonForegroundColor
    {
        get => (Color) GetValue(TitleBarButtonForegroundColorProperty);
        set => SetValue(TitleBarButtonForegroundColorProperty, value);
    }

    public static readonly DependencyProperty TitleBarButtonForegroundColorProperty = DependencyProperty.Register(
        nameof(TitleBarButtonForegroundColor),
        typeof(Color),
        typeof(ContentWindow),
        new PropertyMetadata(default(Color), OnTitleBarButtonForegroundColorChanged)
    );

    private static void OnTitleBarButtonForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (self.Window.ExtendsContentIntoTitleBar)
        {
            Color newColor = (Color) e.NewValue;
            self.Window.AppWindow.TitleBar.ButtonForegroundColor = newColor.A == 0 ? null : newColor;
        }
    }

    public Color TitleBarButtonHoverBackgroundColor
    {
        get => (Color) GetValue(TitleBarButtonHoverBackgroundColorProperty);
        set => SetValue(TitleBarButtonHoverBackgroundColorProperty, value);
    }

    public static readonly DependencyProperty TitleBarButtonHoverBackgroundColorProperty = DependencyProperty.Register(
        nameof(TitleBarButtonHoverBackgroundColor),
        typeof(Color),
        typeof(ContentWindow),
        new PropertyMetadata(default(Color), OnTitleBarButtonHoverBackgroundColorChanged)
    );

    private static void OnTitleBarButtonHoverBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (self.Window.ExtendsContentIntoTitleBar)
        {
            Color newColor = (Color) e.NewValue;
            self.Window.AppWindow.TitleBar.ButtonHoverBackgroundColor = newColor.A == 0 ? null : newColor;
        }
    }

    public Color TitleBarButtonHoverForegroundColor
    {
        get => (Color) GetValue(TitleBarButtonHoverForegroundColorProperty);
        set => SetValue(TitleBarButtonHoverForegroundColorProperty, value);
    }

    public static readonly DependencyProperty TitleBarButtonHoverForegroundColorProperty = DependencyProperty.Register(
        nameof(TitleBarButtonHoverForegroundColor),
        typeof(Color),
        typeof(ContentWindow),
        new PropertyMetadata(default(Color), OnTitleBarButtonHoverForegroundColorChanged)
    );

    private static void OnTitleBarButtonHoverForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (self.Window.ExtendsContentIntoTitleBar)
        {
            Color newColor = (Color) e.NewValue;
            self.Window.AppWindow.TitleBar.ButtonHoverForegroundColor = newColor.A == 0 ? null : newColor;
        }
    }

    public Color TitleBarButtonPressedBackgroundColor
    {
        get => (Color) GetValue(TitleBarButtonPressedBackgroundColorProperty);
        set => SetValue(TitleBarButtonPressedBackgroundColorProperty, value);
    }

    public static readonly DependencyProperty TitleBarButtonPressedBackgroundColorProperty = DependencyProperty.Register(
        nameof(TitleBarButtonPressedBackgroundColor),
        typeof(Color),
        typeof(ContentWindow),
        new PropertyMetadata(default(Color), OnTitleBarButtonPressedBackgroundColorChanged)
    );

    private static void OnTitleBarButtonPressedBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (self.Window.ExtendsContentIntoTitleBar)
        {
            Color newColor = (Color) e.NewValue;
            self.Window.AppWindow.TitleBar.ButtonPressedBackgroundColor = newColor.A == 0 ? null : newColor;
        }
    }

    public Color TitleBarButtonPressedForegroundColor
    {
        get => (Color) GetValue(TitleBarButtonPressedForegroundColorProperty);
        set => SetValue(TitleBarButtonPressedForegroundColorProperty, value);
    }

    public static readonly DependencyProperty TitleBarButtonPressedForegroundColorProperty = DependencyProperty.Register(
        nameof(TitleBarButtonPressedForegroundColor),
        typeof(Color),
        typeof(ContentWindow),
        new PropertyMetadata(default(Color), OnTitleBarButtonPressedForegroundColorChanged)
    );

    private static void OnTitleBarButtonPressedForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        if (self.Window.ExtendsContentIntoTitleBar)
        {
            Color newColor = (Color) e.NewValue;
            self.Window.AppWindow.TitleBar.ButtonPressedForegroundColor = newColor.A == 0 ? null : newColor;
        }
    }

    public bool ShowInTaskbar
    {
        get => (bool) GetValue(ShowInTaskbarProperty);
        set => SetValue(ShowInTaskbarProperty, value);
    }

    public static readonly DependencyProperty ShowInTaskbarProperty = DependencyProperty.Register(
        nameof(ShowInTaskbar),
        typeof(bool),
        typeof(ContentWindow),
        new PropertyMetadata(true, OnShowInTaskbarChanged)
    );

    private static void OnShowInTaskbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        self.Window.AppWindow.IsShownInSwitchers = (bool) e.NewValue;
    }

    private static void OnCanMinimizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        bool canMinimize = (bool) e.NewValue;
        self._styleHelper.CanMinimize = canMinimize;
        (self.Window.AppWindow.Presenter as OverlappedPresenter)?.IsMinimizable = canMinimize;
    }

    private static void OnCanMaximizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        bool canMaximize = (bool) e.NewValue;
        self._styleHelper.CanMaximize = canMaximize;
        (self.Window.AppWindow.Presenter as OverlappedPresenter)?.IsMaximizable = canMaximize;
    }

    private static void OnCanResizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ContentWindow self = (ContentWindow) d;
        bool canResize = (bool) e.NewValue;
        (self.Window.AppWindow.Presenter as OverlappedPresenter)?.IsResizable = canResize;
    }

    #endregion

    public event EventHandler? StateChanged;
    public event EventHandler? Activated;
    public event EventHandler? Deactivated;
    public event EventHandler? Closed;
    public event CancelEventHandler? Closing;

    public Window Window => _window;

    public WindowState WindowState
    {
        get => field;
        private set
        {
            if (field == value)
                return;

            field = value;
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public Window? Owner
    {
        get => field;
        set
        {
            if (field == value)
                return;

            field = value;
            nint ownerHwnd = Owner is null ? nint.Zero : Win32Interop.GetWindowFromWindowId(Owner.AppWindow.Id);
            NativeMethods.SetWindowLong(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWLP_HWNDPARENT, ownerHwnd);
        }
    }

    public double DpiScale => IsLoaded ? XamlRoot.RasterizationScale : (double) WindowHelper.GetDpiForWindow(_hwnd) / 96;

    public static Window? GetWindow(UIElement element) => GetContentWindow(element)?.Window;

    public static ContentWindow? GetContentWindow(UIElement element) => element.XamlRoot.Content as ContentWindow;

    private bool _shouldShow;

    private bool _hasShown;

    private void ShowImmediately()
    {
        if (!_hasShown)
        {
            _hasShown = true;
            switch (StartupLocation)
            {
                case WindowStartupLocation.CenterScreen:
                    MoveToCenterScreen();
                    break;
                case WindowStartupLocation.CenterOwner:
                    MoveToCenterOwner();
                    break;
                default:
                    break;
            }
        }
        Window.AppWindow.Show();
        Activate();
    }

    private void MoveToCenterScreen()
    {
        DisplayArea displayArea = DisplayArea.GetFromWindowId(Window.AppWindow.Id, DisplayAreaFallback.Primary);
        Window.AppWindow.Move(new PointInt32(
            (displayArea.OuterBounds.Width - Window.AppWindow.Size.Width) / 2,
            (displayArea.OuterBounds.Height - Window.AppWindow.Size.Height) / 2)
        );
    }

    private void MoveToCenterOwner()
    {
        if (Owner is null)
        {
            MoveToCenterScreen();
            return;
        }
        Window.AppWindow.Move(new PointInt32(
            Owner.AppWindow.Position.X + (Owner.AppWindow.Size.Width - Window.AppWindow.Size.Width) / 2,
            Owner.AppWindow.Position.Y + (Owner.AppWindow.Size.Height - Window.AppWindow.Size.Height) / 2)
        );
    }

    public void Show()
    {
        _shouldShow = true;
        if (IsLoaded)
        {
            ShowImmediately();
        }
    }

    public void ShowDialog()
    {
        if (Owner is not null)
        {
            (Window.AppWindow.Presenter as OverlappedPresenter)?.IsModal = true;
        }
        Show();
    }

    public void Hide()
    {
        Window.AppWindow.Hide();
    }

    public void Activate()
    {
        _shouldShow = true;
        if (IsLoaded)
        {
            Window.Activate();
        }
    }

    public bool TryMinimize()
    {
        CancelEventArgs e = new(false);
        OnMinimizing(e);
        if (e.Cancel)
            return false;
        (Window.AppWindow.Presenter as OverlappedPresenter)?.Maximize();
        return true;
    }

    public bool TryMaximize()
    {
        CancelEventArgs e = new(false);
        OnMaximizing(e);
        if (e.Cancel)
            return false;
        (Window.AppWindow.Presenter as OverlappedPresenter)?.Minimize();
        return true;
    }

    public bool TryRestore()
    {
        CancelEventArgs e = new(false);
        OnRestoring(e);
        if (e.Cancel)
            return false;
        (Window.AppWindow.Presenter as OverlappedPresenter)?.Restore();
        return true;
    }

    public bool TryClose()
    {
        CancelEventArgs e = new(false);
        OnClosing(e);
        if (e.Cancel)
            return false;
        DispatcherQueue.TryEnqueue(CloseImmediately);
        return true;
    }

    private void CloseImmediately()
    {
        Owner?.Activate();
        Window.AppWindow.Hide();
        Window.Close();
    }

    public void EnterFullscreen()
    {
        if (Window.AppWindow.Presenter.Kind is not AppWindowPresenterKind.FullScreen)
        {
            Window.AppWindow.SetPresenter(AppWindowPresenterKind.FullScreen);
        }
    }

    public void ExitFullscreen()
    {
        if (Window.AppWindow.Presenter.Kind is AppWindowPresenterKind.FullScreen)
        {
            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsMinimizable = CanMinimize;
            presenter.IsMaximizable = CanMaximize;
            presenter.IsResizable = CanResize;
            Window.AppWindow.SetPresenter(presenter);
        }
    }

    /// <summary>
    /// Resize window to specified size.
    /// If you want to keep width or height, set it double.NaN.
    /// </summary>
    /// <param name="size"></param>
    public void Resize(Size size)
    {
        if (Window.ExtendsContentIntoTitleBar)
        {
            Window.AppWindow.ResizeClient(new SizeInt32
            (
                _Width: IsValidLength(size.Width) ? DipToPixel(size.Width) : Window.AppWindow.ClientSize.Width,
                _Height: IsValidLength(size.Height) ? DipToPixel(size.Height - 30) : Window.AppWindow.ClientSize.Height
            ));
        }
        else
        {
            Window.AppWindow.ResizeClient(new SizeInt32
            (
                _Width: IsValidLength(size.Width) ? DipToPixel(size.Width) : Window.AppWindow.ClientSize.Width,
                _Height: IsValidLength(size.Height) ? DipToPixel(size.Height) : Window.AppWindow.ClientSize.Height
            ));
        }
    }

    public void ResizeToContent()
    {
        Resize(new Size(DesiredSize.Width, DesiredSize.Height));
    }

    protected virtual void OnMinimizing(CancelEventArgs e)
    {
        if (!CanMinimize)
        {
            e.Cancel = true;
            return;
        }
    }

    protected virtual void OnMaximizing(CancelEventArgs e)
    {
        if (!CanMaximize)
        {
            e.Cancel = true;
            return;
        }
    }

    protected virtual void OnRestoring(CancelEventArgs e)
    {
    }

    protected virtual void OnClosing(CancelEventArgs e)
    {
        Closing?.Invoke(this, e);
    }

    protected virtual void OnClosed()  // OnClosed is not triggered by Window.Closed because adding breakpoint in it may cause a crash
    {
        monitor?.Dispose();
        (Window.AppWindow.Presenter as OverlappedPresenter)?.IsModal = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    private void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
    {
        Owner?.Activate();
        sender.Hide();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            if (SizeToContent)
            {
                ResizeToContent();
            }
            else
            {
                Resize(new Size(Width, Height));
            }
            if (_shouldShow)
            {
                ShowImmediately();
            }
        });
    }

    private static bool IsValidLength(double length) => double.IsNormal(length) && double.IsPositive(length);

    private void OnAppWindowStateChanged(AppWindow sender, AppWindowChangedEventArgs args)
    {
        if (args.DidSizeChange)
        {
            if (sender.Presenter is OverlappedPresenter overlappedPresenter)
            {
                WindowState = overlappedPresenter.State switch
                {
                    OverlappedPresenterState.Minimized => WindowState.Minimized,
                    OverlappedPresenterState.Maximized => WindowState.Maximized,
                    OverlappedPresenterState.Restored => WindowState.Normal,
                    _ => throw new InvalidOperationException(nameof(OverlappedPresenterState))
                };
            }
        }
    }

    private void OnWindowActivated(object sender, WindowActivatedEventArgs args)
    {
        switch (args.WindowActivationState)
        {
            case WindowActivationState.Deactivated:
                Deactivated?.Invoke(this, EventArgs.Empty);
                break;

            case WindowActivationState.CodeActivated:
            case WindowActivationState.PointerActivated:
                Activated?.Invoke(this, EventArgs.Empty);
                break;

            default:
                break;
        }
    }

    private void OnRequestedThemeChanged(DependencyObject d, DependencyProperty p)
    {
        ElementTheme theme = (ElementTheme) d.GetValue(p);
        Window.AppWindow.TitleBar.PreferredTheme = theme switch
        {
            ElementTheme.Default => TitleBarTheme.UseDefaultAppMode,
            ElementTheme.Light => TitleBarTheme.Light,
            ElementTheme.Dark => TitleBarTheme.Dark,
            _ => TitleBarTheme.UseDefaultAppMode
        };
    }

    private int DipToPixel(double dip) => (int) Math.Ceiling(dip * DpiScale);

    private void OnWindowMessageReceived(object sender, WindowMessageEventArgs e)
    {
        CancelEventArgs cancelEventArg;
        switch (e.MessageType)
        {
            case (uint)NativeValues.WindowMessage.WM_SYSCOMMAND:
                switch ((e.Message.WParam & 0xFFF0))
                {
                    case SYS_COMMAND_WPARAM.SC_MINIMIZE:
                        cancelEventArg = new CancelEventArgs(false);
                        OnMinimizing(cancelEventArg);
                        if (cancelEventArg.Cancel)
                        {
                            e.Handled = true;
                            e.Result = 0;
                        }
                        break;

                    case SYS_COMMAND_WPARAM.SC_MAXIMIZE:
                        cancelEventArg = new CancelEventArgs(false);
                        OnMaximizing(cancelEventArg);
                        if (cancelEventArg.Cancel)
                        {
                            e.Handled = true;
                            e.Result = 0;
                        }
                        break;

                    case SYS_COMMAND_WPARAM.SC_RESTORE:
                        cancelEventArg = new CancelEventArgs(false);
                        OnRestoring(cancelEventArg);
                        if (cancelEventArg.Cancel)
                        {
                            e.Handled = true;
                            e.Result = 0;
                        }
                        break;
                    default:
                        break;
                }
                break;

            case (uint)NativeValues.WindowMessage.WM_CLOSE:
                cancelEventArg = new CancelEventArgs(false);
                OnClosing(cancelEventArg);
                if (cancelEventArg.Cancel)
                {
                    e.Handled = true;
                    e.Result = 0;
                }
                WindowState = WindowState.Closed;
                break;

            case (uint)NativeValues.WindowMessage.WM_DESTROY:
                OnClosed();
                break;
        }
    }
}
