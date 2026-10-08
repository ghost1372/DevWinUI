// Ported from https://github.com/HO-COOH/WinUIEssentials

using Windows.Win32.UI.Input.KeyboardAndMouse;
using Windows.Win32.UI.Input.Pointer;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

/// <summary>
/// Monitors the "InputNonClientPointerSource" child window so caption button hover can be tracked
/// and the system's own caption button tooltips are suppressed.
/// </summary>
internal sealed partial class InputNonClientPointerSource
{
    private const string SinkClassName = "InputNonClientPointerSource";

    // win32k only shows
    private const uint CaptionButtonStylesToRemove = (uint)(WINDOW_STYLE.WS_MINIMIZEBOX | WINDOW_STYLE.WS_MAXIMIZEBOX);

    // win32k always shows the system tooltip for HTCLOSE, but never for HTBORDER
    private const nint CloseButtonSubstituteHitTest = (nint)PInvoke.HTBORDER;

    private readonly ModernWindowCaptionButtonToolTip _owner;
    private DevWinUI.WindowMessageMonitor? _monitor;
    private nint _hwnd;
    private uint _strippedStyle;
    private uint? _pressedPointerId;
    private nint _lastHitTest = (nint)PInvoke.HTNOWHERE;
    private bool _isSettingStyle;

    public InputNonClientPointerSource(ModernWindowCaptionButtonToolTip owner)
    {
        _owner = owner;
    }

    public nint Hwnd => _hwnd;
    public bool IsInitialized => _hwnd != 0;

    private static bool IsMouseButtonDown() =>
        (((ushort)PInvoke.GetAsyncKeyState((int)VIRTUAL_KEY.VK_LBUTTON)
        | (ushort)PInvoke.GetAsyncKeyState((int)VIRTUAL_KEY.VK_RBUTTON)
        | (ushort)PInvoke.GetAsyncKeyState((int)VIRTUAL_KEY.VK_MBUTTON)) & 0x8000) != 0;

    // The sink window can be destroyed and recreated by WinUI (e.g. when the title bar is reset)
    public bool EnsureInitialized(nint parent)
    {
        if (parent == 0)
            return false;
        if (_hwnd != 0 && _hwnd == DevWinUI.WindowHelper.FindWindow(parent, SinkClassName))
            return true;
        return Initialize(parent);
    }

    public bool Initialize(nint parent)
    {
        if (_hwnd != 0)
            Detach();
        if (parent == 0)
            return false;

        var sink = DevWinUI.WindowHelper.FindWindow(parent, SinkClassName);
        if (sink == 0)
            return false;

        _monitor = new DevWinUI.WindowMessageMonitor(sink);
        _monitor.WindowMessageReceived += OnWindowMessage;

        _hwnd = sink;
        DisableMinimizeAndMaximizeTooltipByStyle();
        return true;
    }

    public void Detach()
    {
        _lastHitTest = (nint)PInvoke.HTNOWHERE;
        var oldSink = _hwnd;
        _hwnd = 0;
        var oldStyle = _strippedStyle;
        _strippedStyle = 0;
        DisposeMonitor();
        if (oldSink == 0 || !PInvoke.IsWindow((HWND)oldSink))
            return;

        if (oldStyle != 0)
        {
            var style = (uint)PInvoke.GetWindowLong((HWND)oldSink, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            PInvoke.SetWindowLong((HWND)oldSink, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)(style | oldStyle));
        }
    }

    private void DisposeMonitor()
    {
        if (_monitor is null)
            return;

        _monitor.WindowMessageReceived -= OnWindowMessage;
        _monitor.Dispose();
        _monitor = null;
    }

    public bool IsPointerOver()
    {
        if (!PInvoke.GetCursorPos(out var cursor))
            return false;
        return (nint)PInvoke.WindowFromPoint(cursor) == _hwnd;
    }

    private void DisableMinimizeAndMaximizeTooltipByStyle()
    {
        var style = (uint)PInvoke.GetWindowLong((HWND)_hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        _strippedStyle = style & CaptionButtonStylesToRemove;
        if (_strippedStyle != 0)
        {
            _isSettingStyle = true;
            PInvoke.SetWindowLong((HWND)_hwnd, WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)(style & ~CaptionButtonStylesToRemove));
            _isSettingStyle = false;
        }
    }

    private nint RestoreHitTest(nint hitTest) =>
        hitTest == CloseButtonSubstituteHitTest && _lastHitTest == (nint)PInvoke.HTCLOSE ? (nint)PInvoke.HTCLOSE : hitTest;

    private bool IsPressed()
    {
        if (IsMouseButtonDown())
            return true;

        // for pen and touch
        if (_pressedPointerId is uint id)
        {
            if (PInvoke.GetPointerInfo(id, out var info) && (info.pointerFlags & POINTER_FLAGS.POINTER_FLAG_INCONTACT) != 0)
                return true;
            _pressedPointerId = null;
        }
        return false;
    }

    private void OnPointerPressed()
    {
        _owner.CloseToolTip();
        _owner.IsSuppressedUntilButtonChanges = true;
    }

    private void OnPointerPressed(uint pointerId)
    {
        _pressedPointerId = pointerId;
        OnPointerPressed();
    }

    private static uint LoWord(nuint value) => (uint)(value & 0xFFFF);
    private static uint HiWord(nuint value) => (uint)((value >> 16) & 0xFFFF);
    private static nuint MakeWParam(nint lo, uint hi) => (nuint)(((uint)lo & 0xFFFF) | (hi << 16));

    private void HandleHitTest(DevWinUI.WindowMessageEventArgs e, HWND hwnd, nuint wparam, nint lparam)
    {
        // The real result is set first, so a failure below can never change or lose it
        nint hitTest = PInvoke.DefSubclassProc(hwnd, PInvoke.WM_NCHITTEST, wparam, lparam);
        e.Result = hitTest;
        e.Handled = true;

        try
        {
            _lastHitTest = hitTest;
            if (hitTest == (nint)PInvoke.HTCLOSE && !IsPressed())
                e.Result = CloseButtonSubstituteHitTest;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private void OnWindowMessage(object? sender, DevWinUI.WindowMessageEventArgs e)
    {
        var hwnd = (HWND)e.Message.Hwnd;
        var msg = e.Message.MessageId;
        var wparam = e.Message.WParam;
        var lparam = e.Message.LParam;

        if (msg == PInvoke.WM_NCHITTEST)
        {
            HandleHitTest(e, hwnd, wparam, lparam);
            return;
        }

        var originalWParam = wparam;
        var originalLParam = lparam;

        // In each case, restore the hit-test value / clean up before anything that can throw,
        // so the message is still forwarded correctly if the tooltip logic fails
        try
        {
            switch (msg)
            {
                case PInvoke.WM_STYLECHANGING:
                    if ((nint)wparam == (nint)WINDOW_LONG_PTR_INDEX.GWL_STYLE && !_isSettingStyle)
                    {
                        // STYLESTRUCT { DWORD styleOld; DWORD styleNew; }
                        var styleNew = (uint)Marshal.ReadInt32(lparam, 4);
                        _strippedStyle = styleNew & CaptionButtonStylesToRemove;
                        Marshal.WriteInt32(lparam, 4, (int)(styleNew & ~CaptionButtonStylesToRemove));
                    }
                    break;

                // The system computed the hit-test values in these messages from HTBORDER for the close button
                case PInvoke.WM_NCMOUSEMOVE:
                    wparam = (nuint)RestoreHitTest((nint)wparam);
                    _owner.SetHoveredButton(ModernWindowCaptionButtonsExtensions.FromHitTest((nint)wparam));
                    break;

                case PInvoke.WM_NCMOUSEHOVER:
                case PInvoke.WM_NCLBUTTONUP:
                case PInvoke.WM_NCRBUTTONUP:
                case PInvoke.WM_NCMBUTTONUP:
                    wparam = (nuint)RestoreHitTest((nint)wparam);
                    break;

                case PInvoke.WM_NCLBUTTONDOWN:
                case PInvoke.WM_NCLBUTTONDBLCLK:
                case PInvoke.WM_NCRBUTTONDOWN:
                case PInvoke.WM_NCRBUTTONDBLCLK:
                case PInvoke.WM_NCMBUTTONDOWN:
                case PInvoke.WM_NCMBUTTONDBLCLK:
                    wparam = (nuint)RestoreHitTest((nint)wparam);
                    OnPointerPressed();
                    break;

                case PInvoke.WM_NCXBUTTONUP:
                    wparam = MakeWParam(RestoreHitTest((nint)LoWord(wparam)), HiWord(wparam));
                    break;

                case PInvoke.WM_NCXBUTTONDOWN:
                case PInvoke.WM_NCXBUTTONDBLCLK:
                    wparam = MakeWParam(RestoreHitTest((nint)LoWord(wparam)), HiWord(wparam));
                    OnPointerPressed();
                    break;

                case PInvoke.WM_SETCURSOR:
                case PInvoke.WM_MOUSEACTIVATE:
                    lparam = (nint)(MakeWParam(RestoreHitTest((nint)LoWord((nuint)lparam)), HiWord((nuint)lparam)));
                    break;

                // HIWORD(wparam) of the WM_NCPOINTER* messages holds pointer flags, not a hit-test code
                case PInvoke.WM_NCPOINTERUPDATE:
                    if ((HiWord(wparam) & (uint)POINTER_FLAGS.POINTER_FLAG_INCONTACT) != 0)
                        OnPointerPressed(LoWord(wparam));
                    else
                        _owner.SetHoveredButton(ModernWindowCaptionButtonsExtensions.FromHitTest(_lastHitTest));
                    break;

                case PInvoke.WM_NCPOINTERDOWN:
                case PInvoke.WM_POINTERDOWN:
                    OnPointerPressed(LoWord(wparam));
                    break;

                case PInvoke.WM_NCPOINTERUP:
                case PInvoke.WM_POINTERUP:
                case PInvoke.WM_POINTERCAPTURECHANGED:
                    _pressedPointerId = null;
                    break;

                case PInvoke.WM_LBUTTONDOWN:
                case PInvoke.WM_RBUTTONDOWN:
                case PInvoke.WM_MBUTTONDOWN:
                case PInvoke.WM_XBUTTONDOWN:
                    OnPointerPressed();
                    break;

                case PInvoke.WM_NCMOUSELEAVE:
                case PInvoke.WM_MOUSELEAVE:
                case PInvoke.WM_POINTERLEAVE:
                    // Leave notifications are only a hint, the cursor position decides whether the button is still hovered
                    if (!IsPointerOver())
                        _owner.SetHoveredButton(ModernWindowCaptionButtons.None);
                    break;

                case PInvoke.WM_NCDESTROY:
                    DisposeMonitor();
                    _hwnd = 0;
                    _strippedStyle = 0;
                    _lastHitTest = (nint)PInvoke.HTNOWHERE;
                    _pressedPointerId = null;
                    _owner.SetHoveredButton(ModernWindowCaptionButtons.None);
                    break;
            }
        }
        catch
        {
        }

        // Only forwarded here when the parameters were rewritten, otherwise the monitor forwards the original message
        if (wparam != originalWParam || lparam != originalLParam)
        {
            e.Result = (nint)PInvoke.DefSubclassProc(hwnd, msg, wparam, lparam);
            e.Handled = true;
        }
    }
}
