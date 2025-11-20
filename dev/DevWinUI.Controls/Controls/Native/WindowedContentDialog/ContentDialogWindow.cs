//https://github.com/SuGar0218/WindowedContentDialog

using System.ComponentModel;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

/// <summary>
/// The window that contains ContentDialogContent.
/// <br/>
/// Never show window immediately after construction,
/// <br/>
/// because the size and position have not been computed.
/// Please use Open() when handling Loaded event.
/// Don't use Activate() only,
/// it will not make window modal even if OverlappedPresenter.IsModal is true.
/// </summary>
[ContentProperty(Name = nameof(DialogContent))]
public partial class ContentDialogWindow : Window
{
    public ContentDialogWindow()
    {
        Content = new ContentDialogContent();
        ContentDialogContent!.PrimaryButtonClick += OnPrimaryButtonClick;
        ContentDialogContent.SecondaryButtonClick += OnSecondaryButtonClick;
        ContentDialogContent.CloseButtonClick += OnCloseButtonClick;
        ContentDialogContent.Loaded += OnContentLoaded;
        ContentDialogContent.Loaded += (o, e) => DetermineTitleBarButtonForegroundColor();
        ContentDialogContent.ActualThemeChanged += (o, e) => DetermineTitleBarButtonForegroundColor();
        InitializeWindow();
    }

    private ContentDialogWindow(ContentDialogContent? content)
    {
        Content = content;
        InitializeWindow();
    }

    internal static ContentDialogWindow CreateWithoutComponent() => new(null);

    private ContentDialogContent? ContentDialogContent => (ContentDialogContent)Content;

    internal void InitializeComponent(ContentDialogContent component)
    {
        component.PrimaryButtonClick += OnPrimaryButtonClick;
        component.SecondaryButtonClick += OnSecondaryButtonClick;
        component.CloseButtonClick += OnCloseButtonClick;
        component.Loaded += OnContentLoaded;
        component.Loaded += DetermineTitleBarButtonForegroundColor;
        component.ActualThemeChanged += DetermineTitleBarButtonForegroundColor;

        Content = component;

        Closed += (sender, args) =>
        {
            ContentDialogContent!.PrimaryButtonClick -= OnPrimaryButtonClick;
            ContentDialogContent.SecondaryButtonClick -= OnSecondaryButtonClick;
            ContentDialogContent.CloseButtonClick -= OnCloseButtonClick;
            ContentDialogContent.Loaded -= OnContentLoaded;
            ContentDialogContent.Loaded -= DetermineTitleBarButtonForegroundColor;
            ContentDialogContent.ActualThemeChanged -= DetermineTitleBarButtonForegroundColor;
        };
    }

    private void InitializeWindow()
    {
        ExtendsContentIntoTitleBar = true;
        _presenter = OverlappedPresenter.CreateForDialog();
        _presenter.IsResizable = true;
        AppWindow.SetPresenter(_presenter);
        AppWindow.Closing += (appWindow, e) => OnClosingRequestedBySystem();
        Activated += OnActivated;
        Closed += OnClosed;
    }

    private void DetermineTitleBarButtonForegroundColor()
    {
        switch (ContentDialogContent!.ActualTheme)
        {
            case ElementTheme.Light:
                AppWindow.TitleBar.ButtonForegroundColor = Colors.Black;
                break;
            case ElementTheme.Dark:
                AppWindow.TitleBar.ButtonForegroundColor = Colors.White;
                break;
        }
    }

    private void DetermineTitleBarButtonForegroundColor(object sender, object args) => DetermineTitleBarButtonForegroundColor();

    public event TypedEventHandler<ContentDialogWindow, CancelEventArgs>? PrimaryButtonClick;
    public event TypedEventHandler<ContentDialogWindow, CancelEventArgs>? SecondaryButtonClick;
    public event TypedEventHandler<ContentDialogWindow, CancelEventArgs>? CloseButtonClick;

    public IList<KeyboardAccelerator> PrimaryButtonKeyboardAccelerators => ContentDialogContent?.PrimaryButtonKeyboardAccelerators ?? [];
    public IList<KeyboardAccelerator> SecondaryButtonKeyboardAccelerators => ContentDialogContent?.SecondaryButtonKeyboardAccelerators ?? [];
    public IList<KeyboardAccelerator> CloseButtonKeyboardAccelerators => ContentDialogContent?.CloseButtonKeyboardAccelerators ?? [];

    public event TypedEventHandler<ContentDialogWindow, EventArgs>? Loaded;
    public event TypedEventHandler<ContentDialogWindow, EventArgs>? Opened;

    public bool IsLoaded { get; private set; }

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
        if (!ContentDialogContent!.IsLoaded)
            return;

        if (args.WindowActivationState is WindowActivationState.Deactivated)
        {
            ContentDialogContent.AfterLostFocus();
        }
        else
        {
            ContentDialogContent.AfterGotFocus();
        }
    }

    private void OnClosingRequestedBySystem()
    {
        _parent?.Activate();
        AppWindow.Hide();
    }

    private void OnClosingRequstedByCode()
    {
        _parent?.Activate();
        AppWindow.Hide();
    }

    private void OnClosed(object sender, WindowEventArgs args)
    {
        _parent?.Closed -= OnParentClosed;
    }

    private void OnParentClosed(object sender, WindowEventArgs args)
    {
        _parent = null;
    }

    /// <summary>
    /// Set parent window, whether modal, whether to show at center of parent.
    /// </summary>
    public void SetParent(Window? parent, bool modal = true, bool center = true)
    {
        _center = center;

        if (_parent == parent)
            return;

        _parent?.Closed -= OnParentClosed;
        _parent = parent;
        _parent?.Closed += OnParentClosed;

        if (!modal || parent is null)
            return;

        IntPtr ownerHwnd = parent is null ? IntPtr.Zero : Win32Interop.GetWindowFromWindowId(parent.AppWindow.Id);
        IntPtr selfHwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);
        NativeMethods.SetWindowLong(selfHwnd, -8, ownerHwnd);

        // IsModal must be set after set parent window; otherwise, it will cause exception.
        _presenter.IsModal = true;
    }

    public ElementTheme RequestedTheme
    {
        get => ContentDialogContent!.RequestedTheme;
        set
        {
            ContentDialogContent!.RequestedTheme = value;
            AppWindow.TitleBar.PreferredTheme = value switch
            {
                ElementTheme.Light => TitleBarTheme.Light,
                ElementTheme.Dark => TitleBarTheme.Dark,
                _ => TitleBarTheme.UseDefaultAppMode,
            };
        }
    }

    public ContentDialogResult Result { get; private set; }

    public object? DialogTitle
    {
        get => ContentDialogContent.Title;
        set => ContentDialogContent.Title = value;
    }

    public object? DialogContent
    {
        get => ContentDialogContent.Content;
        set => ContentDialogContent.Content = value;
    }

    #region ContentDialogContent properties
    public Brush? Foreground
    {
        get => ContentDialogContent.Foreground;
        set => ContentDialogContent.Foreground = value;
    }

    public Brush? Background
    {
        get => ContentDialogContent.Background;
        set => ContentDialogContent.Background = value;
    }

    public Brush? BorderBrush
    {
        get => ContentDialogContent.BorderBrush;
        set => ContentDialogContent.BorderBrush = value;
    }

    public Thickness BorderThickness
    {
        get => ContentDialogContent.BorderThickness;
        set => ContentDialogContent.BorderThickness = value;
    }

    public FlowDirection FlowDirection
    {
        get => ContentDialogContent.FlowDirection;
        set => ContentDialogContent.FlowDirection = value;
    }

    public DataTemplate? TitleTemplate
    {
        get => ContentDialogContent.TitleTemplate;
        set => ContentDialogContent.TitleTemplate = value;
    }

    public DataTemplate? ContentTemplate
    {
        get => ContentDialogContent.ContentTemplate;
        set => ContentDialogContent.ContentTemplate = value;
    }

    public string? PrimaryButtonText
    {
        get => ContentDialogContent.PrimaryButtonText;
        set => ContentDialogContent.PrimaryButtonText = value;
    }

    public string? SecondaryButtonText
    {
        get => ContentDialogContent.SecondaryButtonText;
        set => ContentDialogContent.SecondaryButtonText = value;
    }

    public string? CloseButtonText
    {
        get => ContentDialogContent.CloseButtonText;
        set => ContentDialogContent.CloseButtonText = value;
    }

    public bool IsPrimaryButtonEnabled
    {
        get => ContentDialogContent.IsPrimaryButtonEnabled;
        set => ContentDialogContent.IsPrimaryButtonEnabled = value;
    }

    public bool IsSecondaryButtonEnabled
    {
        get => ContentDialogContent.IsSecondaryButtonEnabled;
        set => ContentDialogContent.IsSecondaryButtonEnabled = value;
    }

    public ContentDialogButton DefaultButton
    {
        get => ContentDialogContent.DefaultButton;
        set => ContentDialogContent.DefaultButton = value;
    }

    public Style? PrimaryButtonStyle
    {
        get => ContentDialogContent.PrimaryButtonStyle;
        set => ContentDialogContent.PrimaryButtonStyle = value;
    }

    public Style? SecondaryButtonStyle
    {
        get => ContentDialogContent.SecondaryButtonStyle;
        set => ContentDialogContent.SecondaryButtonStyle = value;
    }

    public Style? CloseButtonStyle
    {
        get => ContentDialogContent.CloseButtonStyle;
        set => ContentDialogContent.CloseButtonStyle = value;
    }


    #endregion

    private void OnContentLoaded(object sender, RoutedEventArgs e)
    {
        AppWindow.ResizeClient(new Windows.Graphics.SizeInt32(
            (int)((ContentDialogContent!.DesiredSize.Width + 1) * ContentDialogContent.XamlRoot.RasterizationScale) + 1,
            (int)((ContentDialogContent.DesiredSize.Height - 30) * ContentDialogContent.XamlRoot.RasterizationScale) + 1));
        SetTitleBar(ContentDialogContent.TitleArea);

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

        DispatcherQueue.TryEnqueue(() => Loaded?.Invoke(this, EventArgs.Empty));
    }

    public void Open()
    {
        AppWindow.Show();
        DispatcherQueue.TryEnqueue(() => Opened?.Invoke(this, EventArgs.Empty));
    }

    public void OpenAfterLoaded()
    {
        if (IsLoaded)
        {
            Open();
        }
        else
        {
            Loaded += (sender, args) => Open();
        }
    }

    private void OnPrimaryButtonClick(ContentDialogContent sender, EventArgs e)
    {
        Result = ContentDialogResult.Primary;
        CancelEventArgs args = new();
        PrimaryButtonClick?.Invoke(this, args);
        AfterCommandSpaceButtonClick(args);
    }

    private void OnSecondaryButtonClick(ContentDialogContent sender, EventArgs e)
    {
        Result = ContentDialogResult.Secondary;
        CancelEventArgs args = new();
        SecondaryButtonClick?.Invoke(this, args);
        AfterCommandSpaceButtonClick(args);
    }

    private void OnCloseButtonClick(ContentDialogContent sender, EventArgs e)
    {
        Result = ContentDialogResult.None;
        CancelEventArgs args = new();
        CloseButtonClick?.Invoke(this, args);
        AfterCommandSpaceButtonClick(args);
    }

    private void AfterCommandSpaceButtonClick(CancelEventArgs args)
    {
        if (args.Cancel)
        {
            Result = ContentDialogResult.None;
            return;
        }
        OnClosingRequstedByCode();
        Close();
    }

    protected static void SizeToXamlRoot(FrameworkElement element, XamlRoot root)
    {
        element.Width = root.Size.Width;
        element.Height = root.Size.Height;
    }

    protected static Style DefaultButtonStyle => (Style)Application.Current.Resources["DefaultButtonStyle"];
    protected static Color SmokeFillColor => (Color)Application.Current.Resources["SmokeFillColorDefault"];

    private OverlappedPresenter _presenter = null!;

    private Window? _parent;

    private bool _center;

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
}
