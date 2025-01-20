using System.Collections.ObjectModel;
using System.Diagnostics;
using WinRT.Interop;

namespace DevWinUI;

public partial class WindowHelper
{
    internal static List<Win32Window> processWindowList = new List<Win32Window>();
    internal static Process currentProcess;
    internal static List<Win32Window> topLevelWindowList = new List<Win32Window>();
    public static ObservableCollection<Window> ActiveWindows { get;} = new();
    public static void TrackWindow(Window window)
    {
        window.Closed += (sender, args) =>
        {
            ActiveWindows.Remove(window);
        };

        ActiveWindows.AddIfNotExists(window);
    }
    public static Window GetWindowForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (Window window in ActiveWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return window;
                }
            }
        }
        return null;
    }
    public static void SwitchToThisWindow(Window window)
    {
        if (window != null)
        {
            SwitchToThisWindow(new HWND(WindowNative.GetWindowHandle(window)));
        }
    }
    public static void SwitchToThisWindow(IntPtr windowHandle)
    {
        PInvoke.SwitchToThisWindow(new HWND(windowHandle), true);
    }

    public static void ReActivateWindow(Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        ReActivateWindow(hwnd);
    }
    public static void ReActivateWindow(IntPtr windowHandle)
    {
        var activeWindow = PInvoke.GetActiveWindow();
        var hwnd = new HWND(windowHandle);
        if (hwnd == activeWindow)
        {
            PInvoke.SendMessage(hwnd, NativeValues.WM_ACTIVATE, NativeValues.WA_INACTIVE, IntPtr.Zero);
            PInvoke.SendMessage(hwnd, NativeValues.WM_ACTIVATE, NativeValues.WA_ACTIVE, IntPtr.Zero);
        }
        else
        {
            PInvoke.SendMessage(hwnd, NativeValues.WM_ACTIVATE, NativeValues.WA_ACTIVE, IntPtr.Zero);
            PInvoke.SendMessage(hwnd, NativeValues.WM_ACTIVATE, NativeValues.WA_INACTIVE, IntPtr.Zero);
        }
    }

    public static void SetWindowCornerRadius(Window window, NativeValues.DWM_WINDOW_CORNER_PREFERENCE cornerPreference)
    {
        SetWindowCornerRadius(WindowNative.GetWindowHandle(window), cornerPreference);
    }

    public static void SetWindowCornerRadius(IntPtr hwnd, NativeValues.DWM_WINDOW_CORNER_PREFERENCE cornerPreference)
    {
        if (OSVersionHelper.IsWindows11_22000_OrGreater)
        {
            unsafe
            {
                uint preference = (uint)cornerPreference;
                PInvoke.DwmSetWindowAttribute(new HWND(hwnd), Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, &preference, sizeof(uint));
            }
        }
    }
    public static IReadOnlyList<Win32Window> GetTopLevelWindows()
    {
        unsafe
        {
            topLevelWindowList?.Clear();
            delegate* unmanaged[Stdcall]<HWND, LPARAM, BOOL> callback = &EnumWindowsCallback;

            PInvoke.EnumWindows(callback, IntPtr.Zero);

            return topLevelWindowList.AsReadOnly();

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static BOOL EnumWindowsCallback(HWND hWnd, LPARAM lParam)
            {
                topLevelWindowList.Add(new Win32Window(hWnd));
                return true;
            }
        }
    }
    
    public static IReadOnlyList<Win32Window> GetProcessWindowList()
    {
        unsafe
        {
            processWindowList?.Clear();
            currentProcess = Process.GetCurrentProcess();
            delegate* unmanaged[Stdcall]<HWND, LPARAM, BOOL> callback = &EnumWindowsCallback;

            PInvoke.EnumWindows(callback, IntPtr.Zero);

            return processWindowList.AsReadOnly();

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static BOOL EnumWindowsCallback(HWND hWnd, LPARAM lParam)
            {
                var window = new Win32Window(hWnd);
                if (window.ProcessId == currentProcess.Id)
                {
                    processWindowList.Add(window);
                }
                return true;
            }
        }
    }

    public static string GetWindowText(IntPtr hwnd)
    {
        const int MAX_Length = 1024;
        Span<char> buffer = stackalloc char[(int)MAX_Length];

        unsafe
        {
            fixed (char* pBuffer = buffer)
            {
                int result = PInvoke.GetWindowText(new HWND(hwnd), pBuffer, MAX_Length);
                if (result > 0)
                {
                    string windowText = new string(pBuffer, 0, result);
                    return windowText;
                }
            }
        }
        return null;
    }

    public static string GetClassName(IntPtr hwnd)
    {
        const int MAX_Length = 256;
        Span<char> buffer = stackalloc char[(int)MAX_Length];

        unsafe
        {
            fixed (char* pBuffer = buffer)
            {
                int result = PInvoke.GetClassName(new HWND(hwnd), pBuffer, MAX_Length);
                if (result > 0)
                {
                    string className = new string(pBuffer, 0, result);
                    return className;
                }
            }
        }
        return null;
    }

    public static AppWindow GetCurrentAppWindow()
    {
        var tops = GetProcessWindowList();

        var firstWinUI3 = tops.FirstOrDefault(w => w.ClassName == "WinUIDesktopWin32WindowClass");

        var windowId = Win32Interop.GetWindowIdFromWindow(firstWinUI3.Handle);

        return AppWindow.GetFromWindowId(windowId);
    }

    /// <summary>
    /// Use XamlRoot
    /// </summary>
    /// <param name="uIElement"></param>
    /// <returns></returns>
    public static AppWindow GetAppWindow(UIElement uIElement)
    {
        if (uIElement == null)
        {
            return null;
        }

        return AppWindow.GetFromWindowId(uIElement.XamlRoot.ContentIslandEnvironment.AppWindowId);
    }

    /// <summary>
    /// Use Microsoft.UI.Composition.Visual
    /// </summary>
    /// <param name="uIElement"></param>
    /// <returns></returns>
    public static AppWindow GetAppWindow2(UIElement uIElement)
    {
        if (uIElement == null)
        {
            return null;
        }

        Microsoft.UI.Composition.Visual visual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(uIElement);
        var ci = Microsoft.UI.Content.ContentIsland.FindAllForCompositor(visual.Compositor);
        if (ci[0] != null)
        {
            return AppWindow.GetFromWindowId(ci[0].Environment.AppWindowId);
        }

        return null;
    }

    /// <summary>
    /// Use XamlRoot
    /// </summary>
    /// <param name="uIElement"></param>
    /// <returns></returns>
    public static IntPtr GetWindowHandle(UIElement uIElement)
    {
        if (uIElement == null)
        {
            return IntPtr.Zero;
        }

        return Win32Interop.GetWindowFromWindowId(uIElement.XamlRoot.ContentIslandEnvironment.AppWindowId);
    }

    /// <summary>
    /// Use Microsoft.UI.Composition.Visual
    /// </summary>
    /// <param name="uIElement"></param>
    /// <returns></returns>
    public static IntPtr GetWindowHandle2(UIElement uIElement)
    {
        if (uIElement == null)
        {
            return IntPtr.Zero;
        }

        Microsoft.UI.Composition.Visual visual = Microsoft.UI.Xaml.Hosting.ElementCompositionPreview.GetElementVisual(uIElement);
        var ci = Microsoft.UI.Content.ContentIsland.FindAllForCompositor(visual.Compositor);
        if (ci[0] != null)
        {
            return Win32Interop.GetWindowFromWindowId(ci[0].Environment.AppWindowId);

        }

        return IntPtr.Zero;
    }

    public static (int, int) GetScreenSize()
            => (PInvoke.GetSystemMetrics(Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX.SM_CXSCREEN), PInvoke.GetSystemMetrics(Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX.SM_CYSCREEN));

    public static void RemoveWindowBorderAndTitleBar(Window window) => RemoveWindowBorderAndTitleBar((nint)window.AppWindow.Id.Value);
    public static void RemoveWindowBorderAndTitleBar(IntPtr hWnd)
    {
        const int WS_BORDER = 0x00800000;
        const int WS_CAPTION = 0x00C00000;
        const int WS_THICKFRAME = 0x00040000;

        var style = PInvoke.GetWindowLong(new HWND(hWnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        // Remove border, caption, and thick frame
        style &= ~WS_BORDER & ~WS_CAPTION & ~WS_THICKFRAME;

        PInvoke.SetWindowLong(new HWND(hWnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);

        // Update the window's appearance
        PInvoke.SetWindowPos(new HWND(hWnd), new HWND(IntPtr.Zero), 0, 0, 0, 0, Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOMOVE | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
    }
}
