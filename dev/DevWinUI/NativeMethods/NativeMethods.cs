using static DevWinUI.NativeValues;

namespace DevWinUI;
public static partial class NativeMethods
{
    [LibraryImport(ExternDll.UxTheme, EntryPoint = "#136", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial void FlushMenuThemes();

    [LibraryImport(ExternDll.UxTheme, EntryPoint = "#135", SetLastError = true, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int SetPreferredAppMode(PreferredAppMode preferredAppMode);

    [LibraryImport(ExternDll.User32)]
    internal static partial IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, WindowMessage Msg, IntPtr wParam, IntPtr lParam);

    [DllImport(ExternDll.User32)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static unsafe extern int FillRect(IntPtr hDC, ref Windows.Win32.Foundation.RECT lprc, Windows.Win32.Graphics.Gdi.HBRUSH hbr);
}
