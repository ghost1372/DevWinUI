namespace DevWinUI;

public enum MouseHookMessageType : uint
{
    Unknown = 0,
    MouseMove = PInvoke.WM_MOUSEMOVE,
    LeftButtonDown = PInvoke.WM_LBUTTONDOWN,
    LeftButtonUp = PInvoke.WM_LBUTTONUP,
    RightButtonDown = PInvoke.WM_RBUTTONDOWN,
    RightButtonUp = PInvoke.WM_RBUTTONUP,
    MiddleButtonDown = PInvoke.WM_MBUTTONDOWN,
    MiddleButtonUp = PInvoke.WM_MBUTTONUP,
    MouseWheel = PInvoke.WM_MOUSEWHEEL
}
