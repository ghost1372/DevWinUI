using static DevWinUI.NativeValues;

namespace DevWinUI;
public static partial class NativeMethods
{
    /// <summary>
    /// FlushMenuThemes clears the current theme settings for menus. It ensures that any changes to theme settings are
    /// applied immediately.
    /// </summary>
    [LibraryImport(ExternDll.UxTheme, EntryPoint = "#136", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial void FlushMenuThemes();

    [LibraryImport(ExternDll.UxTheme, EntryPoint = "#135", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int SetPreferredAppMode(PreferredAppMode preferredAppMode);

    [LibraryImport(ExternDll.User32)]
    internal static partial IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    [DllImport(ExternDll.User32)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static unsafe extern int FillRect(IntPtr hDC, ref Windows.Win32.Foundation.RECT lprc, Windows.Win32.Graphics.Gdi.HBRUSH hbr);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    internal static partial IntPtr SetWindowLong64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [LibraryImport("user32.dll", EntryPoint = "SetWindowLongW")]
    internal static partial IntPtr SetWindowLong32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    
    [LibraryImport(NativeValues.ExternDll.Shell32, EntryPoint = "Shell_NotifyIconW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static unsafe partial bool Shell_NotifyIcon32(uint dwMessage, NOTIFYICONDATAW32* lpData);

    [LibraryImport(NativeValues.ExternDll.Shell32, EntryPoint = "Shell_NotifyIconW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static unsafe partial bool Shell_NotifyIcon64(uint dwMessage, NOTIFYICONDATAW64* lpData);

    [DllImport(NativeValues.ExternDll.Shell32, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern int Shell_NotifyIconGetRect([In] ref NOTIFYICONIDENTIFIER identifier, [Out] out Windows.Graphics.RectInt32 iconLocation);

    /// <summary>
    /// Sets a new value for a specified window attribute of a given window handle. The method adapts based on the
    /// architecture size.
    /// </summary>
    /// <param name="hWnd">Specifies the handle to the window whose attribute is being modified.</param>
    /// <param name="nIndex">Indicates the specific attribute to be changed for the window.</param>
    /// <param name="dwNewLong">Represents the new value to be set for the specified window attribute.</param>
    /// <returns>Returns the previous value of the specified window attribute.</returns>
    public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong) => IntPtr.Size == 4
        ? SetWindowLong32(hWnd, nIndex, dwNewLong)
        : SetWindowLong64(hWnd, nIndex, dwNewLong);
    internal static HWND[]? EnumThreadWindows(Func<HWND, nint, bool> predicate, nint lParam)
    {
        var list = new List<HWND>();
        var handler = new WNDENUMPROC((_hWnd, _lParam) =>
        {
            try
            {
                if (predicate((HWND)_hWnd, _lParam)) list.Add((HWND)_hWnd);
            }
            catch { }

            return true;
        });

        EnumThreadWindows(PInvoke.GetCurrentThreadId(), handler, new LPARAM(lParam));
        return list.Count != 0 ? list.Distinct().ToArray() : Array.Empty<HWND>();
    }

    [DllImport(NativeValues.ExternDll.User32, ExactSpelling = true, PreserveSig = false)]
    internal static extern bool EnumThreadWindows([In] uint dwThreadId, [In] WNDENUMPROC lpfn, [In] nint lParam);

    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate bool WNDENUMPROC([In] nint param0, [In] nint param1);

    internal static void SetWindowStyle(IntPtr hWnd, WindowStyle newStyle)
    {
        var h = new HWND(hWnd);
        var currentStyle = Windows.Win32.PInvoke.GetWindowLong(h, Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        var r = Windows.Win32.PInvoke.SetWindowLong(h, Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)newStyle);
        if (r != currentStyle)
            Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
        // Redraw window
        Windows.Win32.PInvoke.SetWindowPos(h, new HWND(IntPtr.Zero), 0, 0, 0, 0, Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOMOVE | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOSIZE | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOZORDER | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER);
    }

    internal static unsafe bool Shell_NotifyIcon(uint dwMessage, in NOTIFYICONDATAW32 lpData)
    {
        fixed (NOTIFYICONDATAW32* lpDataLocal = &lpData)
        {
            bool __result = Shell_NotifyIcon32(dwMessage, lpDataLocal);
            return __result;
        }
    }
    internal static unsafe bool Shell_NotifyIcon(uint dwMessage, in NOTIFYICONDATAW64 lpData)
    {
        fixed (NOTIFYICONDATAW64* lpDataLocal = &lpData)
        {
            bool __result = Shell_NotifyIcon64(dwMessage, lpDataLocal);
            return __result;
        }
    }
}
