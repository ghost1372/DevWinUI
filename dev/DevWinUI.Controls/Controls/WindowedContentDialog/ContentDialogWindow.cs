using Microsoft.UI.Windowing;
using WinRT;

namespace DevWinUI;

/// <summary>
/// Handles the activation behavior of ContentDialog.
/// <br/>
/// If the window is not activated before showing the dialog, calling ShowAsync may result in the dialog not being displayed properly.
/// <br/>
/// To ensure proper display:
/// <br/>
/// Activate the window first; especially in situations where the ContentDialog is being shown immediately after navigation or when the dialog is being shown at startup, ensure the window is active to avoid display issues.
/// The dialog must be shown only after the window is fully ready; calling ShowAsync too early may cause layout or rendering issues. It's recommended to delay the dialog until the visual tree is fully ready.
/// </summary>
public sealed partial class ContentDialogWindow : Window
{
    public ContentDialogWindow() : base()
    {
        ExtendsContentIntoTitleBar = true;
        Activated += OnActivated;
        presenter = AppWindow.Presenter.As<OverlappedPresenter>();
        presenter.IsMinimizable = false;
        presenter.IsMaximizable = false;

        Closed += (o, e) =>
        {
            if (content != null)
            {
                content.Content = null;
            }
        };
    }

    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs> PrimaryButtonClick;
    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs> SecondaryButtonClick;
    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs> CloseButtonClick;

    private void OnActivated(object sender, WindowActivatedEventArgs args)
    {
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
    /// Displays a dialog and returns the user’s response.
    /// <br/>
    /// </summary>
    /// <param name="modal">Indicates whether the dialog should be modal. If set to true and OwnerWindow is null, the dialog will not function as a modal dialog and will be displayed modelessly.</param>
    /// <returns>The result of the user's interaction with the dialog.</returns>
    public async Task<ContentDialogResult> ShowAsync()
    {
        AppWindow.TitleBar.PreferredTheme = RequestedTheme switch
        {
            ElementTheme.Default => TitleBarTheme.UseDefaultAppMode,
            ElementTheme.Light => TitleBarTheme.Light,
            ElementTheme.Dark => TitleBarTheme.Dark,
            _ => TitleBarTheme.UseDefaultAppMode,
        };

        content = new ContentDialogContent()
        {
            Title = Title,
            Content = Content,

            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,

            PrimaryButtonText = PrimaryButtonText,
            SecondaryButtonText = SecondaryButtonText,
            CloseButtonText = CloseButtonText,
            DefaultButton = DefaultButton,
            IsPrimaryButtonEnabled = IsPrimaryButtonEnabled,
            IsSecondaryButtonEnabled = IsSecondaryButtonEnabled,

            PrimaryButtonStyle = PrimaryButtonStyle,
            SecondaryButtonStyle = SecondaryButtonStyle,
            CloseButtonStyle = CloseButtonStyle,

            RequestedTheme = RequestedTheme,

            MinWidth = (double) Application.Current.Resources["ContentDialogMinWidth"],
            MinHeight = (double) Application.Current.Resources["ContentDialogMinHeight"],
            MaxWidth = (double) Application.Current.Resources["ContentDialogMaxWidth"],
            MaxHeight = (double) Application.Current.Resources["ContentDialogMaxHeight"],
        };
        content.Loaded += DialogLoaded;
        content.CloseButtonClick += OnCloseButtonClick;
        content.PrimaryButtonClick += OnPrimaryButtonClick;
        content.SecondaryButtonClick += OnSecondaryButtonClick;
        base.Content = content;
        using SemaphoreSlim closed = new(0, 1);
        void ClosedEventHandler(object sender, WindowEventArgs args) => closed.Release();
        Closed += ClosedEventHandler;
        await closed.WaitAsync();
        Closed -= ClosedEventHandler;
        return Result;
    }

    public bool IsModal { get; set; }

    private Window _ownerWindow;
    public Window OwnerWindow
    {
        get => _ownerWindow;
        set
        {
            if (_ownerWindow == value || value == null)
                return;

            _ownerWindow = value;

            IntPtr ownerHwnd = Win32Interop.GetWindowFromWindowId(_ownerWindow.AppWindow.Id);
            IntPtr ownedHwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);

            NativeMethods.SetWindowLong(ownedHwnd, -8, ownerHwnd);

            void OnClosed(object sender, WindowEventArgs args) => _ownerWindow.Activate();
            Closed += OnClosed;

            _ownerWindow.Closed += (o, e) =>
            {
                Closed -= OnClosed;
                Close();
            };
        }
    }

    public ElementTheme RequestedTheme { get; set; } = ElementTheme.Default;
    public Brush Foreground { get; set; } = (Brush) Application.Current.Resources["ApplicationForegroundThemeBrush"];
    public Brush Background { get; set; }
    public Brush BorderBrush { get; set; }
    public Thickness BorderThickness { get; set; }
    public CornerRadius CornerRadius { get; set; }
    public FlowDirection FlowDirection { get; set; }
    public DataTemplate TitleTemplate { get; set; }
    public DataTemplate ContentTemplate { get; set; }
    public string PrimaryButtonText { get; set; }
    public string SecondaryButtonText { get; set; }
    public string CloseButtonText { get; set; }
    public bool IsPrimaryButtonEnabled { get; set; }
    public bool IsSecondaryButtonEnabled { get; set; }
    public ContentDialogButton DefaultButton { get; set; } = ContentDialogButton.Close;
    public Style PrimaryButtonStyle { get; set; }
    public Style SecondaryButtonStyle { get; set; }
    public Style CloseButtonStyle { get; set; }

    public ContentDialogResult Result { get; private set; }

    public new object? Content { get; set; }

    /// <summary>
    /// When the window is activated, if Min/Max Width/Height are set but layout hasn't stabilized yet, it may result in incorrect sizing when showing a ContentDialog.
    /// This can cause the dialog to appear with improper dimensions if the layout hasn't fully completed or if Max Width/Height haven't been properly applied yet.
    /// </summary>
    private void DialogLoaded(object sender, RoutedEventArgs e)
    {
        AppWindow.ResizeClient(new Windows.Graphics.SizeInt32(
            (int) (content.ActualWidth * content.XamlRoot.RasterizationScale),
            (int) (content.ActualHeight * content.XamlRoot.RasterizationScale) - 30));
        content.HorizontalAlignment = HorizontalAlignment.Stretch;
        content.VerticalAlignment = VerticalAlignment.Stretch;
        content.MaxHeight = double.PositiveInfinity;
        content.MaxWidth = double.PositiveInfinity;
        SetTitleBar(content.TitleArea);

        if (OwnerWindow is null)
        {
            presenter.IsModal = false;
            DisplayArea displayArea = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Primary);
            AppWindow.Move(new Windows.Graphics.PointInt32(
                (displayArea.OuterBounds.Width - AppWindow.Size.Width) / 2,
                (displayArea.OuterBounds.Height - AppWindow.Size.Height) / 2));
        }
        else
        {
            presenter.IsModal = IsModal;
            AppWindow.Move(new Windows.Graphics.PointInt32(
                OwnerWindow.AppWindow.Position.X + (OwnerWindow.AppWindow.Size.Width - AppWindow.Size.Width) / 2,
                OwnerWindow.AppWindow.Position.Y + (OwnerWindow.AppWindow.Size.Height - AppWindow.Size.Height) / 2));
        }
        AppWindow.Show();
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.None;
        ContentDialogWindowButtonClickEventArgs args = new() { Cancel = false };
        CloseButtonClick?.Invoke(this, args);
        if (args.Cancel)
        {
            return;
        }
        Close();
    }

    private void OnPrimaryButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.Primary;
        ContentDialogWindowButtonClickEventArgs args = new() { Cancel = false };
        PrimaryButtonClick?.Invoke(this, args);
        if (args.Cancel)
        {
            return;
        }
        Close();
    }

    private void OnSecondaryButtonClick(object sender, RoutedEventArgs e)
    {
        Result = ContentDialogResult.Secondary;
        ContentDialogWindowButtonClickEventArgs args = new() { Cancel = false };
        SecondaryButtonClick?.Invoke(this, args);
        if (args.Cancel)
        {
            return;
        }
        Close();
    }

    private bool _hasTitleBar = true;
    public bool HasTitleBar
    {
        get => _hasTitleBar;
        set
        {
            _hasTitleBar = value;
            presenter?.SetBorderAndTitleBar(true, value);
        }
    }

    private bool _isResizable;
    public bool IsResizable
    {
        get => _isResizable;
        set
        {
            _isResizable = value;
            if (presenter != null)
            {
                presenter.IsResizable = value;
            }
        }
    }

    private ContentDialogContent content;

    private readonly OverlappedPresenter presenter;
}
