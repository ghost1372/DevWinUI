using Microsoft.UI.Windowing;

namespace DevWinUI;

public sealed partial class ContentDialogWindow : Window
{
    public ContentDialogWindow() : base()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        _presenter = OverlappedPresenter.CreateForDialog();
        _presenter.IsResizable = true;
        AppWindow.SetPresenter(_presenter);

        AppWindow.Closing += (appWindow, e) => OnClosingRequestedBySystem();
        Activated += OnActivated;

        // When showing accent color in title bar is enabled,
        // title bar buttons in the default custom title bar in WinUI3
        // will become white like system title bar.
        // But there is no accent color background here.
        void DetermineTitleBarButtonForegroundColor()
        {
            switch (content.ActualTheme)
            {
                case ElementTheme.Light:
                    AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                    break;
                case ElementTheme.Dark:
                    AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
                    break;
            }
            ;
        }
        content.Loaded += (o, e) => DetermineTitleBarButtonForegroundColor();
        content.ActualThemeChanged += (o, e) => DetermineTitleBarButtonForegroundColor();
    }

    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? PrimaryButtonClick;
    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? SecondaryButtonClick;
    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? CloseButtonClick;

    public event TypedEventHandler<ContentDialogWindow, EventArgs>? Loaded;
    public event TypedEventHandler<ContentDialogWindow, EventArgs>? Opened;
    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (!content.IsLoaded)
            return;

        if (args.WindowActivationState is WindowActivationState.Deactivated)
        {
            content.AfterLostFocus();
        }
        else
        {
            content.AfterGotFocus();
        }
    }

    /// <summary>
    /// AppWindow.Closing event happens when title bar close button clicked or ALT+F4 pressed.
    /// </summary>
    private void OnClosingRequestedBySystem()
    {
        _parent?.Activate();
        AppWindow.Hide();
    }

    /// <summary>
    /// Close() will not make AppWindow.Closing event happen.
    /// </summary>
    private void OnClosingRequestedByCode()
    {
        _parent?.Activate();
        AppWindow.Hide();
    }

    private void OnParentClosed(object sender, WindowEventArgs args)
    {
        if (_parent != null)
            _parent.Closed -= OnParentClosed;

        _parent = null;
        DispatcherQueue.TryEnqueue(Close);
    }

    /// <summary>
    /// Set parent window, whether modal, whether to show at center of parent.
    /// </summary>
    public void SetParent(Window? parent, bool modal = true, bool center = true)
    {
        _center = center;

        if (_parent == parent)
            return;

        if (_parent != null)
            _parent.Closed -= OnParentClosed;

        _parent = parent;

        if (_parent != null)
            _parent.Closed += OnParentClosed;

        IntPtr ownerHwnd = parent is null ? IntPtr.Zero : Win32Interop.GetWindowFromWindowId(parent.AppWindow.Id);
        IntPtr ownedHwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);
        NativeMethods.SetWindowLong(ownedHwnd, -8, ownerHwnd);

        _presenter.IsModal = parent is not null && modal;
    }

    public ElementTheme RequestedTheme
    {
        get => content.RequestedTheme;
        set
        {
            content.RequestedTheme = value;
            AppWindow.TitleBar.PreferredTheme = value switch
            {
                ElementTheme.Light => TitleBarTheme.Light,
                ElementTheme.Dark => TitleBarTheme.Dark,
                _ => TitleBarTheme.UseDefaultAppMode,
            };
        }
    }

    #region ContentDialogContent properties

    public Brush? Foreground
    {
        get => content.Foreground;
        set => content.Foreground = value;
    }

    public Brush? Background
    {
        get => content.Background;
        set => content.Background = value;
    }

    public Brush? BorderBrush
    {
        get => content.BorderBrush;
        set => content.BorderBrush = value;
    }

    public Thickness BorderThickness
    {
        get => content.BorderThickness;
        set => content.BorderThickness = value;
    }

    public FlowDirection FlowDirection
    {
        get => content.FlowDirection;
        set => content.FlowDirection = value;
    }

    public DataTemplate? TitleTemplate
    {
        get => content.TitleTemplate;
        set => content.TitleTemplate = value;
    }

    public DataTemplate? ContentTemplate
    {
        get => content.ContentTemplate;
        set => content.ContentTemplate = value;
    }

    public string? PrimaryButtonText
    {
        get => content.PrimaryButtonText;
        set => content.PrimaryButtonText = value;
    }

    public string? SecondaryButtonText
    {
        get => content.SecondaryButtonText;
        set => content.SecondaryButtonText = value;
    }

    public string? CloseButtonText
    {
        get => content.CloseButtonText;
        set => content.CloseButtonText = value;
    }

    public bool IsPrimaryButtonEnabled
    {
        get => content.IsPrimaryButtonEnabled;
        set => content.IsPrimaryButtonEnabled = value;
    }

    public bool IsSecondaryButtonEnabled
    {
        get => content.IsSecondaryButtonEnabled;
        set => content.IsSecondaryButtonEnabled = value;
    }

    public ContentDialogButton DefaultButton
    {
        get => content.DefaultButton;
        set => content.DefaultButton = value;
    }

    public Style? PrimaryButtonStyle
    {
        get => content.PrimaryButtonStyle;
        set => content.PrimaryButtonStyle = value;
    }

    public Style? SecondaryButtonStyle
    {
        get => content.SecondaryButtonStyle;
        set => content.SecondaryButtonStyle = value;
    }

    public Style? CloseButtonStyle
    {
        get => content.CloseButtonStyle;
        set => content.CloseButtonStyle = value;
    }

    public object? DialogTitle
    {
        get => content.Title;
        set => content.Title = value;
    }
    #endregion

    public ContentDialogResult Result { get; private set; }

    public object? DialogContent
    {
        get => content.Content;
        set => content.Content = value;
    }

    private bool _hasTitleBar = true;
    public bool HasTitleBar
    {
        get => _hasTitleBar;
        set
        {
            _hasTitleBar = value;
            _presenter?.SetBorderAndTitleBar(true, value);
        }
    }

    private bool _isResizable;
    public bool IsResizable
    {
        get => _isResizable;
        set
        {
            _isResizable = value;
            if (_presenter != null)
            {
                _presenter.IsResizable = value;
            }
        }
    }

    private void OnContentLoaded(object sender, RoutedEventArgs e)
    {
        // AppWindow.Resize is inaccurate.
        // AppWindow.ResizeCilent is inaccurate when ExtendsContentInfoTitleBar = false.
        // AppWindow.ResizeCilent is accurate in width but there is an extra height of title bar (30 DIP) when ExtendsContentInfoTitleBar = true.
        // No matter whether ExtendsContentInfoTitleBar, the size is the same after use AppWindow.ResizeCilent.
        AppWindow.ResizeClient(new Windows.Graphics.SizeInt32(
            (int)((content.DesiredSize.Width + 1) * content.XamlRoot.RasterizationScale) + 1,
            (int)((content.DesiredSize.Height - 30) * content.XamlRoot.RasterizationScale) + 1));

        SetTitleBar(content.TitleArea);

        if (_center)
        {
            if (_parent is not null)
            {
                AppWindow.Move(new Windows.Graphics.PointInt32(
                    _parent.AppWindow.Position.X + (_parent.AppWindow.Size.Width - AppWindow.Size.Width) / 2,
                    _parent.AppWindow.Position.Y + (_parent.AppWindow.Size.Height - AppWindow.Size.Height) / 2));
            }
            else
            {
                DisplayArea displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
                AppWindow.Move(new Windows.Graphics.PointInt32(
                    (displayArea.OuterBounds.Width - AppWindow.Size.Width) / 2,
                    (displayArea.OuterBounds.Height - AppWindow.Size.Height) / 2));
            }
        }

        if (SystemBackdrop is null)
        {
            Background = RequestedTheme switch
            {
                ElementTheme.Light => new SolidColorBrush(Colors.White),
                ElementTheme.Dark => new SolidColorBrush(Color.FromArgb(0xFF, 0x20, 0x20, 0x20)),
                _ => null
            };
        }
        if (SystemBackdrop is null || SystemBackdrop is DesktopAcrylicBackdrop)
        {
            content.CommandSpace.Background.Opacity = 1.0;
        }
        else
        {
            content.CommandSpace.Background.Opacity = 0.5;
        }

        DispatcherQueue.TryEnqueue(() => Loaded?.Invoke(this, EventArgs.Empty));
    }

    public void Open()
    {
        AppWindow.Show();
        DispatcherQueue.TryEnqueue(() => Opened?.Invoke(this, EventArgs.Empty));
    }

    private void OnPrimaryButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.Primary;
        ContentDialogWindowButtonClickEventArgs args = new() { Cancel = false };
        PrimaryButtonClick?.Invoke(this, args);
        AfterCommandBarButtonClick(args);
    }

    private void OnSecondaryButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.Secondary;
        ContentDialogWindowButtonClickEventArgs args = new() { Cancel = false };
        SecondaryButtonClick?.Invoke(this, args);
        AfterCommandBarButtonClick(args);
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.None;
        ContentDialogWindowButtonClickEventArgs args = new() { Cancel = false };
        CloseButtonClick?.Invoke(this, args);
        AfterCommandBarButtonClick(args);
    }

    private void AfterCommandBarButtonClick(ContentDialogWindowButtonClickEventArgs args)
    {
        if (args.Cancel)
            return;

        OnClosingRequestedByCode();
        DispatcherQueue.TryEnqueue(Close);
    }

    private readonly OverlappedPresenter _presenter;

    private Window? _parent;

    private bool _center;
}
