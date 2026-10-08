// Ported from https://github.com/HO-COOH/WinUIEssentials

using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

/// <summary>
/// Shows a modern (WinUI) tooltip for the Minimize / Maximize / Close caption buttons
/// of a window with <see cref="Window.ExtendsContentIntoTitleBar"/> enabled.
/// Place it anywhere in the window's content and set <see cref="Window"/>.
/// </summary>
public sealed partial class ModernWindowCaptionButtonToolTip : Control
{
    private static readonly string MinimizeString = GeneralHelper.LoadNativeString(900);
    private static readonly string MaximizeString = GeneralHelper.LoadNativeString(901);
    private static readonly string RestoreDownString = GeneralHelper.LoadNativeString(903);
    private static readonly string CloseString = GeneralHelper.LoadNativeString(905);
    private static readonly TimeSpan BetweenShowDelay = TimeSpan.FromMilliseconds(200);
    private const double MouseOffset = 20;

    private readonly InputNonClientPointerSource _nonClientInputSink;
    private WeakReference<Window>? _window;
    private nint _hwnd;
    private AppWindow? _appWindow;
    private ModernWindowCaptionButtons _hoveredButton = ModernWindowCaptionButtons.None;
    private DateTime _lastToolTipClosed = DateTime.MinValue;
    private ToolTip? _toolTip;
    private TranslateTransform? _placementTransform;
    private DispatcherQueueTimer? _openTimer;
    private DispatcherQueueTimer? _closeTimer;

    private static readonly uint HoverTimeMs = NativeMethods.MouseHoverTime();

    internal bool IsSuppressedUntilButtonChanges { get; set; }

    public ModernWindowCaptionButtonToolTip()
    {
        _nonClientInputSink = new InputNonClientPointerSource(this);
        IsTabStop = false;
        IsHitTestVisible = false;

        Loaded += (_, _) => EnsureSinkInitialized();
        Unloaded += (_, _) =>
        {
            try { SetHoveredButton(ModernWindowCaptionButtons.None); } catch { }
        };
        ActualThemeChanged += (_, _) =>
        {
            try { UpdateToolTipTheme(); } catch { }
        };
    }

    public ModernWindowCaptionButtonToolTip(Window window) : this()
    {
        Window = window;
    }

    public Window? Window
    {
        get => _window is not null && _window.TryGetTarget(out var w) ? w : null;
        set
        {
            if (value == Window)
                return;
            DetachWindow();
            SetWindow(value);
        }
    }

    private void SetWindow(Window? window)
    {
        if (window is null)
            return;

        _window = new WeakReference<Window>(window);
        _hwnd = WindowNative.GetWindowHandle(window);
        _appWindow = window.AppWindow;
        if (_appWindow is not null)
            _appWindow.Changed += AppWindow_Changed;
        window.Closed += Window_Closed;
        _nonClientInputSink.Initialize(_hwnd);
    }

    private void Window_Closed(object sender, WindowEventArgs args) => DetachWindow();

    private void EnsureSinkInitialized()
    {
        try
        {
            if (_hwnd != 0)
                _nonClientInputSink.EnsureInitialized(_hwnd);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private void DetachWindow()
    {
        // Win32 cleanup first, so a throwing WinRT call below can never leave this object registered with Windows
        _nonClientInputSink.Detach();

        _hwnd = 0;
        if (Window is { } window)
            window.Closed -= Window_Closed;
        _window = null;

        if (_appWindow is not null)
        {
            _appWindow.Changed -= AppWindow_Changed;
            _appWindow = null;
        }
        SetHoveredButton(ModernWindowCaptionButtons.None);
    }

    private void AppWindow_Changed(AppWindow sender, AppWindowChangedEventArgs args)
    {
        EnsureSinkInitialized();
        if (args.DidPositionChange || args.DidSizeChange || args.DidPresenterChange || args.DidVisibilityChange)
            CloseToolTip();
    }

    internal void SetHoveredButton(ModernWindowCaptionButtons button)
    {
        var previous = _hoveredButton;
        _hoveredButton = button;
        if (button == previous)
            return;

        IsSuppressedUntilButtonChanges = false;

        // A tooltip that is already showing moves to the next caption button without delay
        if (previous != ModernWindowCaptionButtons.None && IsButtonEnabled(button) && _toolTip is { IsOpen: true })
        {
            ShowToolTip();
            return;
        }

        CloseToolTip();

        if (button == ModernWindowCaptionButtons.None)
            return;

        EnsureTimers();
        var isReshow = DateTime.UtcNow - _lastToolTipClosed < BetweenShowDelay;
        _openTimer!.Interval = TimeSpan.FromMilliseconds(isReshow ? HoverTimeMs * 3 / 2 : HoverTimeMs * 2);
        _openTimer.Start();
    }

    private void EnsureTimers()
    {
        if (_openTimer is not null)
            return;

        var queue = DispatcherQueue;

        _openTimer = queue.CreateTimer();
        _openTimer.IsRepeating = false;
        _openTimer.Tick += (_, _) =>
        {
            try
            {
                if (_hoveredButton != ModernWindowCaptionButtons.None && !IsSuppressedUntilButtonChanges)
                    ShowToolTip();
            }
            catch { }
        };

        _closeTimer = queue.CreateTimer();
        _closeTimer.IsRepeating = false;
        _closeTimer.Tick += (_, _) =>
        {
            try
            {
                CloseToolTip();
                IsSuppressedUntilButtonChanges = true;
            }
            catch { }
        };
    }

    private bool IsButtonEnabled(ModernWindowCaptionButtons button) => _hwnd != 0 && button.IsEnabled(_hwnd);

    private string GetToolTipText(ModernWindowCaptionButtons button) => button switch
    {
        ModernWindowCaptionButtons.Minimize => MinimizeString,
        ModernWindowCaptionButtons.Maximize => PInvoke.IsZoomed((HWND)_hwnd) ? RestoreDownString : MaximizeString,
        ModernWindowCaptionButtons.Close => CloseString,
        _ => string.Empty
    };

    private void EnsureToolTip()
    {
        if (_toolTip is not null)
            return;

        _toolTip = new ToolTip { Placement = PlacementMode.Bottom, PlacementTarget = this };
        ToolTipService.SetToolTip(this, _toolTip);
    }

    private void TranslateSelf(Windows.Foundation.Point anchor)
    {
        // WinUI clamps the tooltip to the monitor under the PlacementTarget's top-left corner.
        // Move this invisible control to the correct monitor in case the window spans multiple monitors
        if (_placementTransform is null)
        {
            _placementTransform = new TranslateTransform();
            RenderTransform = _placementTransform;
        }
        var offset = TransformToVisual(null).Inverse.TransformPoint(anchor);
        _placementTransform.X += offset.X;
        _placementTransform.Y += offset.Y;
    }

    private void ShowToolTip()
    {
        var xamlRoot = XamlRoot;
        if (xamlRoot is null || _hwnd == 0 || !IsButtonEnabled(_hoveredButton))
            return;

        PInvoke.GetCursorPos(out var cursor);

        var captionRect = new RECT { top = cursor.Y };
        if (_nonClientInputSink.IsInitialized)
            PInvoke.GetWindowRect((HWND)_nonClientInputSink.Hwnd, out captionRect);
        var captionTop = new System.Drawing.Point(cursor.X, Math.Min(captionRect.top, cursor.Y));
        PInvoke.ScreenToClient((HWND)_hwnd, ref cursor);
        PInvoke.ScreenToClient((HWND)_hwnd, ref captionTop);

        double scale = xamlRoot.RasterizationScale;
        double cursorY = cursor.Y / scale;
        var anchor = new Windows.Foundation.Point(cursor.X / scale, captionTop.Y / scale);

        TranslateSelf(anchor);

        EnsureToolTip();
        var toolTip = _toolTip!;
        var isOpen = toolTip.IsOpen;
        // WinUI only places an open tooltip again when PlacementRect changes, we have to force a re-assign
        if (isOpen)
            toolTip.PlacementRect = null;

        // Below the cursor normally, but above the whole title bar when WinUI flips it for lack of space,
        // otherwise it would cover the cursor and the caption buttons
        toolTip.PlacementRect = new Windows.Foundation.Rect(0, 0, 1, cursorY + MouseOffset - anchor.Y);
        toolTip.Content = GetToolTipText(_hoveredButton);
        UpdateToolTipTheme();
        if (!isOpen)
            toolTip.IsOpen = true;

        _closeTimer!.Stop();
        _closeTimer.Interval = TimeSpan.FromSeconds(NativeMethods.MessageDurationSeconds());
        _closeTimer.Start();
    }

    internal void CloseToolTip()
    {
        _openTimer?.Stop();
        _closeTimer?.Stop();
        if (_toolTip is { IsOpen: true })
        {
            _toolTip.IsOpen = false;
            _lastToolTipClosed = DateTime.UtcNow;
        }
    }

    private void UpdateToolTipTheme()
    {
        if (_toolTip is not null)
            _toolTip.RequestedTheme = ActualTheme;
    }
}
