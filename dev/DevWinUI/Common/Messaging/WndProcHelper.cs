namespace DevWinUI;
public partial class WndProcHelper
{
    private IntPtr Handle { get; set; }
    private NativeValues.WNDPROC newMainWindowWndProc = null;
    private IntPtr oldMainWindowWndProc = IntPtr.Zero;

    private NativeValues.WNDPROC newInputNonClientPointerSourceWndProc = null;
    private IntPtr oldInputNonClientPointerSourceWndProc = IntPtr.Zero;

    public WndProcHelper(Microsoft.UI.Xaml.Window window)
    {
        Handle = WindowNative.GetWindowHandle(window);
    }

    public WndProcHelper(IntPtr hwnd)
    {
        Handle = hwnd;
    }

    public IntPtr CallWindowProc(IntPtr hWnd, NativeValues.WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        return NativeMethods.CallWindowProc(oldMainWindowWndProc, hWnd, Msg, wParam, lParam);
    }

    public IntPtr CallInputNonClientPointerSourceWindowProc(IntPtr hWnd, NativeValues.WindowMessage Msg, IntPtr wParam, IntPtr lParam)
    {
        return NativeMethods.CallWindowProc(oldInputNonClientPointerSourceWndProc, hWnd, Msg, wParam, lParam);
    }
    public void RegisterWndProc(NativeValues.WNDPROC wndProc)
    {
        newMainWindowWndProc = wndProc;
        oldMainWindowWndProc = PInvoke.SetWindowLong(new HWND(Handle), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_WNDPROC, (int)Marshal.GetFunctionPointerForDelegate(newMainWindowWndProc));
    }

    public void RegisterInputNonClientPointerSourceWndProc(NativeValues.WNDPROC wndProc)
    {
        var inputNonClientPointerSourceHandle = PInvoke.FindWindowEx(new HWND(Handle), HWND.Null, "InputNonClientPointerSource", null);

        if (inputNonClientPointerSourceHandle != HWND.Null)
        {
            var style = PInvoke.GetWindowLong(new HWND(Handle), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            PInvoke.SetWindowLong(new HWND(Handle), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE, (style & ~(int)NativeValues.WindowStyle.WS_SYSMENU));

            newInputNonClientPointerSourceWndProc = wndProc;
            oldInputNonClientPointerSourceWndProc = PInvoke.SetWindowLong(inputNonClientPointerSourceHandle, Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWLP_WNDPROC, (int)Marshal.GetFunctionPointerForDelegate(newInputNonClientPointerSourceWndProc));
        }
    }
}
