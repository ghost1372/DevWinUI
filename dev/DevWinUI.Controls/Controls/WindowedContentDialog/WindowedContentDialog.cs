using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public partial class WindowedContentDialog : StandaloneContentDialogBase
{
    private Rectangle smokeLayerCache;
    private Border backdropLayerCache;
    public string? WindowTitle { get; set; }
    public SystemBackdrop? SystemBackdrop { get; set; } = new MicaBackdrop();

    public bool CenterInParent { get; set; } = true;
    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? PrimaryButtonClick;

    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? SecondaryButtonClick;

    public event TypedEventHandler<ContentDialogWindow, ContentDialogWindowButtonClickEventArgs>? CloseButtonClick;
    public event TypedEventHandler<ContentDialogWindow, EventArgs>? Loaded;
    public event TypedEventHandler<ContentDialogWindow, WindowEventArgs>? Closed;

    public Window? OwnerWindow { get; set; }
    public bool HasTitleBar { get; set; } = true;
    public bool IsResizable { get; set; }
    public override Task<ContentDialogResult> ShowAsync() => ShowAsync(isModal: true);

    /// <summary>
    /// Displays a dialog window and returns the user's selection when it is closed.
    /// <br/>
    /// No need to worry—once the window is closed, it is no longer part of the visual tree.
    /// Note: A FrameworkElement cannot be shared across multiple parents.
    /// If the DialogContent is a FrameworkElement, it must not already be owned by another parent—for example, using `new MainWindow().DialogContent`.
    /// This popup can only be shown once before the DialogContent is changed again, because each dialog instance creates a new window, and sharing the same FrameworkElement across multiple windows is not allowed.
    /// </summary>
    /// <param name="isModal">
    /// Whether to block the owner window. Defaults to true, but has no effect if OwnerWindow is null—will still display as a normal (non-modal) window.
    /// </param>
    /// <returns>The result of the user's choice.</returns>
    public async Task<ContentDialogResult> ShowAsync(bool isModal)
    {
        ContentDialogWindow dialogWindow = new()
        {
            Title = WindowTitle,
            DialogTitle = Title,
            DialogContent = Content,
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
        if (OwnerWindow?.Content?.XamlRoot == null)
            return;

        switch (Underlay)
        {
            case UnderlayMode.SmokeLayer:
                HandleSmokeLayer(dialogWindow);
                break;

            case UnderlayMode.SystemBackdrop:
                HandleSystemBackdrop(dialogWindow);
                break;
        }
    }
    private void HandleSmokeLayer(ContentDialogWindow dialogWindow)
    {
        if (UnderlaySmokeLayer == null || UnderlaySmokeLayer.SmokeLayerKind == WindowedContentDialogSmokeLayerKind.None)
            return;

        DisableOwnerEvents(dialogWindow);

        var popup = new Popup
        {
            XamlRoot = OwnerWindow.Content.XamlRoot,
            RequestedTheme = RequestedTheme
        };

        if (UnderlaySmokeLayer.SmokeLayerKind == WindowedContentDialogSmokeLayerKind.Darken)
        {
            Rectangle darkLayer = new()
            {
                Opacity = 0.0,
                OpacityTransition = UnderlaySmokeLayer.OpacityTransition,
                Fill = new SolidColorBrush(SmokeFillColor),
            };
            SizeToXamlRoot(darkLayer, OwnerWindow);
            popup.Child = darkLayer;
            smokeLayerCache = darkLayer;
        }
        else if (UnderlaySmokeLayer.SmokeLayerKind is WindowedContentDialogSmokeLayerKind.Custom && UnderlaySmokeLayer.CustomSmokeLayer != null)
        {
            popup.Child = UnderlaySmokeLayer.CustomSmokeLayer;
        }

        AttachPopupLifecycle(dialogWindow, popup, isSmokeLayer: true);
    }
    private void HandleSystemBackdrop(ContentDialogWindow dialogWindow)
    {
        if (UnderlaySystemBackdrop == null || UnderlaySystemBackdrop.Backdrop == BackdropType.None)
            return;

        DisableOwnerEvents(dialogWindow);

        var popup = PopupHelper.CreatePopup(
            OwnerWindow.Content.XamlRoot,
            UnderlaySystemBackdrop.Backdrop,
            UnderlaySystemBackdrop.CoverMode == UnderlayCoverMode.Full,
            GetTitleBarOffset(OwnerWindow));

        backdropLayerCache = popup.Child as Border;
        backdropLayerCache.OpacityTransition = UnderlaySystemBackdrop.OpacityTransition;
        AttachPopupLifecycle(dialogWindow, popup, isSmokeLayer: false);
    }

    private void DisableOwnerEvents(ContentDialogWindow dialogWindow)
    {
        dialogWindow.Opened -= DialogWindow_Opened;
        dialogWindow.Closed -= DialogWindow_Closed;

        dialogWindow.Opened += DialogWindow_Opened;
        dialogWindow.Closed += DialogWindow_Closed;
    }

    private void DialogWindow_Opened(ContentDialogWindow sender, EventArgs e)
    {
        if (OwnerWindow.Content is Control control)
            control.IsEnabled = false;
    }

    private void DialogWindow_Closed(object sender, WindowEventArgs e)
    {
        if (OwnerWindow.Content is Control control)
            control.IsEnabled = true;
    }

    private void AttachPopupLifecycle(ContentDialogWindow dialogWindow, Popup popup, bool isSmokeLayer)
    {
        if (isSmokeLayer)
        {
            dialogWindow.Opened -= OnDialogWindowOpened;
            dialogWindow.Opened += OnDialogWindowOpened;
        }
        else
        {
            dialogWindow.Loaded -= OnDialogWindowOpened;
            dialogWindow.Loaded += OnDialogWindowOpened;
        }

        dialogWindow.Closed -= DialogWindow_ClosedPopup;
        dialogWindow.Closed += DialogWindow_ClosedPopup;

        void OnDialogWindowOpened(object sender, EventArgs e)
        {
            popup.IsOpen = true;
            popup.Child.Opacity = 1.0;
            OwnerWindow.SizeChanged += OnOwnerWindowSizeChanged;
        }

        async void DialogWindow_ClosedPopup(object sender, WindowEventArgs e)
        {
            popup.Child.Opacity = 0.0;
            await Task.Delay(popup.Child.OpacityTransition?.Duration ?? new TimeSpan(0));
            popup.IsOpen = false;
            popup.Child = null;
            OwnerWindow.SizeChanged -= OnOwnerWindowSizeChanged;
        }
    }
    private void OnOwnerWindowSizeChanged(object sender, WindowSizeChangedEventArgs args)
    {
        switch (Underlay)
        {
            case UnderlayMode.SmokeLayer:
                SizeToXamlRoot(smokeLayerCache, OwnerWindow);
                break;

            case UnderlayMode.SystemBackdrop:
                SizeToXamlRoot(backdropLayerCache, OwnerWindow);
                break;
        }
    }
}
