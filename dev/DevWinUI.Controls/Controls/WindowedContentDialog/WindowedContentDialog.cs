using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls.Primitives;

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

    public UnderlayMode Underlay { get; set; } = UnderlayMode.SmokeLayer;
    public UnderlaySystemBackdropOptions UnderlaySystemBackdrop { get; set; } = new UnderlaySystemBackdropOptions();
    public UnderlaySmokeLayerOptions UnderlaySmokeLayer { get; set; } = new UnderlaySmokeLayerOptions();

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
        ContentDialogWindow dialogWindow = new()
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

        dialogWindow.PrimaryButtonClick += PrimaryButtonClick;
        dialogWindow.SecondaryButtonClick += SecondaryButtonClick;
        dialogWindow.CloseButtonClick += CloseButtonClick;

        dialogWindow.SetParent(OwnerWindow, isModal, CenterInParent);

        SetUnderlay(dialogWindow);
       
        TaskCompletionSource<ContentDialogResult> resultCompletionSource = new();

        dialogWindow.Loaded += (window, e) =>
        {
            Loaded?.Invoke(window, e);
            window.Open();
        };
        dialogWindow.Closed += (o, e) =>
        {
            Closed?.Invoke(dialogWindow, e);
            resultCompletionSource.SetResult(dialogWindow.Result);
        };
        return await resultCompletionSource.Task;
    }

    private void SetUnderlay(ContentDialogWindow dialogWindow)
    {
        if (OwnerWindow == null || OwnerWindow.Content == null || OwnerWindow.Content.XamlRoot == null)
            return;

        switch (Underlay)
        {
            case UnderlayMode.SmokeLayer:
                if (UnderlaySmokeLayer != null && UnderlaySmokeLayer.SmokeLayerKind != WindowedContentDialogSmokeLayerKind.None)
                {
                    dialogWindow.Opened += (window, e) =>
                    {
                        if (OwnerWindow.Content is Control control)
                        {
                            control.IsEnabled = false;
                        }
                    };
                    dialogWindow.Closed += (o, e) =>
                    {
                        if (OwnerWindow.Content is Control control)
                        {
                            control.IsEnabled = true;
                        }
                    };

                    Popup behindOverlayPopup = new()
                    {
                        XamlRoot = OwnerWindow.Content.XamlRoot,
                        RequestedTheme = RequestedTheme
                    };
                    if (UnderlaySmokeLayer.SmokeLayerKind is WindowedContentDialogSmokeLayerKind.Darken)
                    {
                        behindOverlayPopup.Child = new Border
                        {
                            HorizontalAlignment = HorizontalAlignment.Stretch,
                            VerticalAlignment = VerticalAlignment.Stretch,
                            Width = OwnerWindow.Content.XamlRoot.Size.Width,
                            Height = OwnerWindow.Content.XamlRoot.Size.Height,
                            Opacity = 0.0,
                            OpacityTransition = new ScalarTransition { Duration = TimeSpan.FromSeconds(0.25) },
                            Background = new SolidColorBrush(SmokeFillColor),
                        };

                        dialogWindow.Loaded += (o, e) =>
                        {
                            behindOverlayPopup.IsOpen = true;
                            behindOverlayPopup.Child.Opacity = 1.0;
                        };
                        dialogWindow.Closed += async (o, e) =>
                        {
                            behindOverlayPopup.Child.Opacity = 0.0;
                            await Task.Delay(behindOverlayPopup.Child.OpacityTransition.Duration);
                            behindOverlayPopup.IsOpen = false;
                        };
                    }
                    else if (UnderlaySmokeLayer.CustomSmokeLayer is not null)
                    {
                        behindOverlayPopup.Child = UnderlaySmokeLayer.CustomSmokeLayer;

                        dialogWindow.Loaded += (o, e) =>
                        {
                            behindOverlayPopup.IsOpen = true;
                            behindOverlayPopup.Child.Opacity = 1.0;
                        };
                        dialogWindow.Closed += async (o, e) =>
                        {
                            behindOverlayPopup.Child.Opacity = 0.0;
                            await Task.Delay(behindOverlayPopup.Child.OpacityTransition.Duration);
                            behindOverlayPopup.IsOpen = false;
                        };
                    }
                }
                break;
            case UnderlayMode.SystemBackdrop:

                Popup popup = null;

                if (UnderlaySystemBackdrop != null && UnderlaySystemBackdrop.Backdrop != BackdropType.None)
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

                    popup = PopupHelper.CreatePopup(OwnerWindow.Content.XamlRoot, UnderlaySystemBackdrop.Backdrop, UnderlaySystemBackdrop.CoverMode == UnderlayCoverMode.Full, verticalOffset);
                    dialogWindow.Opened += (window, e) =>
                    {
                        if (OwnerWindow.Content is Control control)
                        {
                            control.IsEnabled = false;
                        }
                    };
                    dialogWindow.Closed += (o, e) =>
                    {
                        if (OwnerWindow.Content is Control control)
                        {
                            control.IsEnabled = true;
                        }
                    };
                    dialogWindow.Loaded += (o, e) =>
                    {
                        popup.IsOpen = true;
                        popup.Child.Opacity = 1.0;
                    };
                    dialogWindow.Closed += async (o, e) =>
                    {
                        popup.Child.Opacity = 0.0;
                        await Task.Delay(popup.Child.OpacityTransition.Duration);
                        popup.IsOpen = false;
                    };
                }
                break;
        }
    }

    private static Style DefaultButtonStyle => (Style) Application.Current.Resources["DefaultButtonStyle"];
    private static Color SmokeFillColor => (Color)Application.Current.Resources["SmokeFillColorDefault"];
}
