using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls.Primitives;
using WinRT;

namespace DevWinUI;

public partial class WindowedContentDialog
{
    public string? Title { get; set; }
    public object? Content { get; set; }

    public ElementTheme RequestedTheme { get; set; } = ElementTheme.Default;
    public SystemBackdrop? SystemBackdrop { get; set; } = new MicaBackdrop();
    public Brush Foreground { get; set; } = (Brush) Application.Current.Resources["ApplicationForegroundThemeBrush"];
    public Brush? Background { get; set; }
    public Brush? BorderBrush { get; set; }
    public Thickness BorderThickness { get; set; }
    public CornerRadius CornerRadius { get; set; }
    public FlowDirection FlowDirection { get; set; }
    public DataTemplate? TitleTemplate { get; set; }
    public DataTemplate? ContentTemplate { get; set; }
    public string PrimaryButtonText { get; set; } = string.Empty;
    public string SecondaryButtonText { get; set; } = string.Empty;
    public string CloseButtonText { get; set; } = string.Empty;
    public bool IsPrimaryButtonEnabled { get; set; } = true;
    public bool IsSecondaryButtonEnabled { get; set; } = true;
    public ContentDialogButton DefaultButton { get; set; } = ContentDialogButton.Close;
    public BackdropType UnderlayBackdrop { get; set; } = BackdropType.Mica;
    public UnderlayCoverMode UnderlayBackdropCoverMode { get; set; } = UnderlayCoverMode.ClientArea;
    public bool ShowUnderlayBackdrop { get; set; }

    public bool CenterInParent { get; set; } = true;
    public Style PrimaryButtonStyle { get; set; } = DefaultButtonStyle;
    public Style SecondaryButtonStyle { get; set; } = DefaultButtonStyle;
    public Style CloseButtonStyle { get; set; } = DefaultButtonStyle;

    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? PrimaryButtonClick;

    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? SecondaryButtonClick;

    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? CloseButtonClick;
    public event TypedEventHandler<ContentDialogWindow, EventArgs>? Loaded;
    public event TypedEventHandler<ContentDialogWindow, WindowEventArgs>? Closed;

    public Window? OwnerWindow { get; set; }
    public bool HasTitleBar { get; set; } = true;
    public bool IsResizable { get; set; }

    /// <summary>
    /// Displays a dialog window and returns the user's selection when it is closed.
    /// <br/>
    /// No need to worry—once the window is closed, it is no longer part of the visual tree.
    /// Note: A FrameworkElement cannot be shared across multiple parents.
    /// If the Content is a FrameworkElement, it must not already be owned by another parent—for example, using `new MainWindow().Content`.
    /// This popup can only be shown once before the Content is changed again, because each dialog instance creates a new window, and sharing the same FrameworkElement across multiple windows is not allowed.
    /// </summary>
    /// <returns>The result of the user's choice.</returns>
    public async Task<ContentDialogResult> ShowAsync()
    {
        return await ShowAsync(true);
    }

    /// <summary>
    /// Displays a dialog window and returns the user's selection when it is closed.
    /// <br/>
    /// No need to worry—once the window is closed, it is no longer part of the visual tree.
    /// Note: A FrameworkElement cannot be shared across multiple parents.
    /// If the Content is a FrameworkElement, it must not already be owned by another parent—for example, using `new MainWindow().Content`.
    /// This popup can only be shown once before the Content is changed again, because each dialog instance creates a new window, and sharing the same FrameworkElement across multiple windows is not allowed.
    /// </summary>
    /// <param name="isModal">
    /// Whether to block the owner window. Defaults to true, but has no effect if OwnerWindow is null—will still display as a normal (non-modal) window.
    /// </param>
    /// <returns>The result of the user's choice.</returns>
    public async Task<ContentDialogResult> ShowAsync(bool isModal)
    {
        Popup popup = null;
        ContentDialogWindow window = new()
        {
            Title = Title ?? string.Empty,
            Content = Content,
            HasTitleBar = HasTitleBar,
            IsResizable = IsResizable,

            PrimaryButtonText = PrimaryButtonText,
            SecondaryButtonText = SecondaryButtonText,
            CloseButtonText = CloseButtonText,
            DefaultButton = DefaultButton,
            IsPrimaryButtonEnabled = IsPrimaryButtonEnabled,
            IsSecondaryButtonEnabled = IsSecondaryButtonEnabled,

            PrimaryButtonStyle = PrimaryButtonStyle,
            SecondaryButtonStyle = SecondaryButtonStyle,
            CloseButtonStyle = CloseButtonStyle,

            SystemBackdrop = SystemBackdrop,
            RequestedTheme = RequestedTheme
        };

        if (isModal && ShowUnderlayBackdrop && OwnerWindow != null)
        {
            int verticalOffset = 0;
            switch (OwnerWindow.AppWindow.TitleBar.PreferredHeightOption)
            {
                case TitleBarHeightOption.Standard:
                    verticalOffset = 32;
                    break;
                case TitleBarHeightOption.Tall:
                    verticalOffset = 48;
                    break;
                case TitleBarHeightOption.Collapsed:
                    verticalOffset = 0;
                    break;
            }

            popup = PopupHelper.CreatePopup(OwnerWindow.Content.XamlRoot, UnderlayBackdrop, UnderlayBackdropCoverMode, verticalOffset);
        }

        window.PrimaryButtonClick += PrimaryButtonClick;
        window.SecondaryButtonClick += SecondaryButtonClick;
        window.CloseButtonClick += CloseButtonClick;

        window.SetParent(OwnerWindow, isModal, CenterInParent);

        window.Loaded += (window, e) =>
        {
            Loaded?.Invoke(window, e);
            if (isModal && ShowUnderlayBackdrop)
            {
                popup.IsOpen = true;
            }
            window.AppWindow.Show();
            if (OwnerWindow?.Content is Control control)
            {
                var presenter = window.AppWindow.Presenter.As<OverlappedPresenter>();
                control.IsEnabled = !presenter.IsModal;
            }
        };

        TaskCompletionSource<ContentDialogResult> resultCompletionSource = new();
        window.Closed += (o, e) =>
        {
            Closed?.Invoke(window, e);
            if (isModal && popup != null)
            {
                popup.IsOpen = false;
            }
            resultCompletionSource.SetResult(window.Result);
            if (OwnerWindow?.Content is Control control)
            {
                control.IsEnabled = true;
            }
        };

        return await resultCompletionSource.Task;
    }

    private static Style DefaultButtonStyle => (Style) Application.Current.Resources["DefaultButtonStyle"];
}
