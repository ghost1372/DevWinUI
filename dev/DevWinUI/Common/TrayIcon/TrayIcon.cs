using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Win32.UI.WindowsAndMessaging;
using WinRT;
using static DevWinUI.NativeValues;

namespace DevWinUI;

public partial class TrayIcon : IDisposable
{
    private const uint TrayIconCallbackId = 0x8765;
    private readonly Window _window;
    private readonly nint _windowHandle;
    private readonly WindowMessageMonitor _monitor;
    private IconId currentIcon;
    private string _tooltip;
    private FrameworkElement _root;
    private FlyoutBase? _currentFlyout;
    private bool _disposed;

    public TrayIcon(uint trayiconId, IconId iconId, string tooltip) : this(trayiconId, tooltip)
    {
        currentIcon = iconId;
    }

    public TrayIcon(uint trayiconId, string iconPath, string tooltip) : this(trayiconId, tooltip)
    {
        SetIcon(iconPath);
    }

    private TrayIcon(uint trayiconId, string tooltip)
    {
        TrayIconId = trayiconId;
        _tooltip = tooltip;
        _window = new Window();
        _window.Content = _root = new Microsoft.UI.Xaml.Controls.Grid();
        _windowHandle = WindowNative.GetWindowHandle(_window);
        _window.AppWindow.IsShownInSwitchers = false;
        NativeMethods.SetWindowStyle(_windowHandle, WindowStyle.Popup);

        _window.AppWindow.Presenter.As<OverlappedPresenter>().IsAlwaysOnTop = true;
        _monitor = new WindowMessageMonitor(_windowHandle);
        _monitor.WindowMessageReceived -= WindowMessageReceived;
        _monitor.WindowMessageReceived += WindowMessageReceived;
    }

    ~TrayIcon()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        _disposed = true;
        if (disposing)
        {
            _window.Close();
            _monitor.Dispose();
        }
        RemoveFromTray(TrayIconId);
    }

    private void CheckDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(TrayIcon));
    }

    public uint TrayIconId { get; }

    private bool _isVisible;

    public bool IsVisible
    {
        get { return _isVisible; }
        set
        {
            CheckDisposed();
            _isVisible = value;
            if (_isVisible)
                AddToTray(TrayIconId);
            else
                RemoveFromTray(TrayIconId);
        }
    }

    private unsafe void WindowMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        switch (e.MessageType)
        {
            case (uint)WindowMessage.WM_GETMINMAXINFO:
                {
                    MINMAXINFO* rect2 = (MINMAXINFO*)e.Message.LParam;
                    // Restrict size to 0x0 to prevent the window from being physically shown
                    rect2->ptMinTrackSize.X = 0;
                    rect2->ptMinTrackSize.Y = 0;
                }
                break;
            case (uint)(WindowMessage)TrayIconCallbackId: // Callback from tray icon defined in AddToTray()
                {
                    ProcessTrayIconEvents(e.Message);
                    break;
                }
            case (uint)WindowMessage.WM_ACTIVATE:
                {
                    if (e.Message.WParam == 0) // Window lost focus
                    {
                        _window.DispatcherQueue.TryEnqueue(() => _currentFlyout?.Hide());
                    }
                    break;
                }
        }
    }

    private void ShowFlyout(FlyoutBase flyout)
    {
        CheckDisposed();
        CloseFlyout();
        var icon = new NOTIFYICONIDENTIFIER()
        {
            uID = TrayIconId,
            hWnd = _windowHandle,
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONIDENTIFIER>(),
        };
        var hresult = NativeMethods.Shell_NotifyIconGetRect(ref icon, out var location);
        if (hresult == 0)
        {
            if (_currentFlyout != null && _currentFlyout != flyout)
                _currentFlyout.Hide();
            flyout.ShouldConstrainToRootBounds = false;
            _currentFlyout = flyout;
            ((Grid)_window.Content).ContextFlyout = flyout;
            _window.Activate();
            WindowHelper.ShowWindow(_windowHandle);
            _window.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(location.X, location.Y, 0, 0), Microsoft.UI.Windowing.DisplayArea.GetFromPoint(new Windows.Graphics.PointInt32(0, 0), Microsoft.UI.Windowing.DisplayAreaFallback.Primary));
            WindowHelper.SetForegroundWindow(_windowHandle);
            _currentFlyout.ShowAt(_root, new FlyoutShowOptions() { Position = new Windows.Foundation.Point(0, 0) });
            _currentFlyout.Closing += CurrentFlyout_Closing;
        }
    }

    private void CurrentFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
    {
        sender.Closing -= CurrentFlyout_Closing;
        WindowHelper.HideWindow(_windowHandle);
    }

    public void CloseFlyout()
    {
        CheckDisposed();
        if (_currentFlyout is not null)
        {
            _currentFlyout.Closing -= CurrentFlyout_Closing;
            if (_currentFlyout.IsOpen)
                _currentFlyout.Hide();
            _currentFlyout = null;
        }
        WindowHelper.HideWindow(_windowHandle);
    }

    public string Tooltip
    {
        get => _tooltip;
        set
        {
            CheckDisposed();
            if (_tooltip != value)
            {
                _tooltip = value;
                UpdateTooltip();
            }
        }
    }

    private void UpdateTooltip()
    {
        if (!IsVisible)
            return;

        if (Environment.Is64BitProcess)
        {
            var notifyIconData = CreateIconData64(TrayIconId, HICON.Null, Tooltip, 0);
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_MODIFY, notifyIconData);
        }
        else
        {
            var notifyIconData = CreateIconData32(TrayIconId, HICON.Null, Tooltip, 0);
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_MODIFY, notifyIconData);
        }
    }

    private void UpdateIcon()
    {
        if (!IsVisible || currentIcon.Value == 0)
            return;
        var hicon = new HICON((nint)currentIcon.Value);

        if (Environment.Is64BitProcess)
        {
            var notifyIconData = CreateIconData64(TrayIconId, hicon, null, 0);
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_MODIFY, notifyIconData);
        }
        else
        {
            var notifyIconData = CreateIconData32(TrayIconId, hicon, null, 0);
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_MODIFY, notifyIconData);
        }
    }

    public unsafe void SetIcon(string iconPath)
    {
        fixed (char* nameLocal = iconPath)
        {
            var size = (int)(WindowHelper.GetDpiForWindow(_windowHandle) / 6d);
            var id = Windows.Win32.PInvoke.LoadImage(HINSTANCE.Null, nameLocal, GDI_IMAGE_TYPE.IMAGE_ICON, size, size, IMAGE_FLAGS.LR_LOADFROMFILE);
            if (id.IsNull)
                throw new ArgumentException($"Failed to load icon from {iconPath}");
            SetIcon(new IconId((ulong)id.Value));
        }
    }

    public void SetIcon(IconId iconId)
    {
        currentIcon = iconId;
        if (IsVisible)
            UpdateIcon();
    }

    private void AddToTray(uint iconId)
    {
        if (currentIcon.Value == 0) // Fallback to default icon
        {
            var lresult = Windows.Win32.PInvoke.SendMessage(new Windows.Win32.Foundation.HWND(_windowHandle), (uint)DevWinUI.NativeValues.WindowMessage.WM_GETICON, 1, (nint)0);
            if (lresult > 0)
                currentIcon = new IconId((ulong)lresult.Value);
            else
            {
                lresult = Windows.Win32.PInvoke.SendMessage(new Windows.Win32.Foundation.HWND(_windowHandle), (uint)DevWinUI.NativeValues.WindowMessage.WM_GETICON, 0, (nint)0);
                if (lresult > 0)
                    currentIcon = new IconId((ulong)lresult.Value);
            }
        }

        HICON hicon;
        if (currentIcon.Value > 0)
        {
            hicon = new HICON((nint)currentIcon.Value);
        }
        else // Fall back to default application icon
        {
            var icon = Windows.Win32.PInvoke.LoadIcon(Windows.Win32.Foundation.HINSTANCE.Null, lpIconName: Windows.Win32.PInvoke.IDI_APPLICATION);
            hicon = new HICON((nint)icon);
        }

        if (Environment.Is64BitProcess)
        {
            var notifyIconData = CreateIconData64(iconId, hicon, Tooltip, TrayIconCallbackId);
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_ADD, notifyIconData);
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_SETVERSION, notifyIconData);
        }
        else
        {
            var notifyIconData = CreateIconData32(iconId, hicon, Tooltip, TrayIconCallbackId);
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_ADD, notifyIconData);
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_SETVERSION, notifyIconData);
        }
    }

    private static uint CreateIconData(HICON hicon, string? tooltip, uint callbackId, out __ushort_128 tip)
    {
        const uint NIF_MESSAGE = 0x00000001;
        const uint NIF_ICON = 0x00000002;
        const uint NIF_TIP = 0x00000004;
        const uint NIF_SHOWTIP = 0x80;
        uint flags = 0;
        if (!hicon.IsNull)
            flags = flags | NIF_ICON;
        tip = new __ushort_128();
        if (!string.IsNullOrEmpty(tooltip))
        {
            for (int i = 0; i < 128 && i < tooltip.Length; i++)
            {
                tip[i] = (ushort)tooltip[i];
            }
            flags = flags | NIF_TIP | NIF_SHOWTIP;
        }
        if (callbackId > 0)
            flags = flags | NIF_MESSAGE;
        return flags;
    }

    private NOTIFYICONDATAW64 CreateIconData64(uint iconId, HICON hicon, string? tooltip = null, uint callbackId = 0)
    {
        System.Diagnostics.Debug.Assert(Environment.Is64BitProcess);
        uint flags = CreateIconData(hicon, tooltip, callbackId, out var tip);

        return new NOTIFYICONDATAW64
        {
            hWnd = new Windows.Win32.Foundation.HWND(_windowHandle),
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW64>(),
            uID = iconId,
            uFlags = flags,
            hIcon = hicon,
            uCallbackMessage = TrayIconCallbackId,
            szTip = tip,
            VersionOrTimeout = 4,
        };
    }

    private NOTIFYICONDATAW32 CreateIconData32(uint iconId, HICON hicon, string? tooltip = null, uint callbackId = 0)
    {
        System.Diagnostics.Debug.Assert(!Environment.Is64BitProcess);
        uint flags = CreateIconData(hicon, tooltip, callbackId, out var tip);

        return new NOTIFYICONDATAW32
        {
            hWnd = new Windows.Win32.Foundation.HWND(_windowHandle),
            cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW32>(),
            uID = iconId,
            uFlags = flags,
            hIcon = hicon,
            uCallbackMessage = TrayIconCallbackId,
            szTip = tip,
            VersionOrTimeout = 4
        };
    }

    private void RemoveFromTray(uint iconId)
    {
        if (Environment.Is64BitProcess)
        {
            var notifyIconData = new NOTIFYICONDATAW64
            {
                hWnd = new Windows.Win32.Foundation.HWND(_windowHandle),
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW64>(),
                uID = iconId,
            };
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_DELETE, notifyIconData);
        }
        else
        {
            var notifyIconData = new NOTIFYICONDATAW32
            {
                hWnd = new Windows.Win32.Foundation.HWND(_windowHandle),
                cbSize = (uint)Marshal.SizeOf<NOTIFYICONDATAW32>(),
                uID = iconId,
            };
            NativeMethods.Shell_NotifyIcon((uint)NOTIFYICON.NIM_DELETE, notifyIconData);
        }
    }

    private void ProcessTrayIconEvents(Message message)
    {
        var iconid = message.HighOrder;
        if (iconid != TrayIconId)
            return;
        var type = (WindowMessage)(message.LParam & 0xffff);
        var lparam = message.LParam & 0xffff0000;

        TrayIconEventArgs? args = null;
        switch (type)
        {
            case WindowMessage.WM_LBUTTONDBLCLK:
                LeftDoubleClick?.Invoke(this, args = new TrayIconEventArgs());
                break;
            case WindowMessage.WM_RBUTTONDBLCLK:
                RightDoubleClick?.Invoke(this, args = new TrayIconEventArgs());
                break;
            case WindowMessage.NIN_SELECT:
                LeftClick?.Invoke(this, args = new TrayIconEventArgs());
                break;
            case WindowMessage.WM_CONTEXTMENU:
                RightClick?.Invoke(this, args = new TrayIconEventArgs());
                break;
        }
        if (args?.Flyout != null)
            ShowFlyout(args.Flyout);
    }

    public event Windows.Foundation.TypedEventHandler<TrayIcon, TrayIconEventArgs>? LeftClick;

    public event Windows.Foundation.TypedEventHandler<TrayIcon, TrayIconEventArgs>? RightClick;

    public event Windows.Foundation.TypedEventHandler<TrayIcon, TrayIconEventArgs>? LeftDoubleClick;

    public event Windows.Foundation.TypedEventHandler<TrayIcon, TrayIconEventArgs>? RightDoubleClick;
}
