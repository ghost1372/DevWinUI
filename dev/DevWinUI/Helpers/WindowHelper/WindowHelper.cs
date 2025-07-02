using System.Diagnostics;

namespace DevWinUI;

public partial class WindowHelper
{
    internal static List<Win32Window> processWindowList = new List<Win32Window>();
    internal static Process currentProcess;
    internal static List<Win32Window> topLevelWindowList = new List<Win32Window>();

    /// <summary>
    /// Holds a collection of currently active windows in the application.
    /// </summary>
    public static ObservableCollection<Microsoft.UI.Xaml.Window> ActiveWindows { get; } = new();

    /// <summary>
    /// Tracks a specified window and removes it from the active list when closed.
    /// </summary>
    /// <param name="window">The window to be tracked for its closed event.</param>
    public static void TrackWindow(Microsoft.UI.Xaml.Window window)
    {
        window.Closed += (sender, args) =>
        {
            ActiveWindows.Remove(window);
        };

        ActiveWindows.AddIfNotExists(window);
    }

    /// <summary>
    /// Removes a specified window from the active windows list.
    /// </summary>
    /// <param name="window">The window to be removed from the active windows collection.</param>
    public static void RemoveWindowFromTrack(Microsoft.UI.Xaml.Window window)
    {
        ActiveWindows.DeleteIfExists(window);
    }

    public static Microsoft.UI.Xaml.Window GetWindowForElement(UIElement element)
    {
        if (element.XamlRoot != null)
        {
            foreach (Microsoft.UI.Xaml.Window window in ActiveWindows)
            {
                if (element.XamlRoot == window.Content.XamlRoot)
                {
                    return window;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Switches the focus to a specified window if it is not null.
    /// </summary>
    /// <param name="window">The parameter represents a window that will be focused if it exists.</param>
    public static void SwitchToThisWindow(Microsoft.UI.Xaml.Window window)
    {
        if (window != null)
        {
            SwitchToThisWindow(new HWND(WindowNative.GetWindowHandle(window)));
        }
    }

    /// <summary>
    /// Switches the focus to a specified window identified by its handle.
    /// </summary>
    /// <param name="hwnd">Identifies the window to which the focus will be switched.</param>
    public static void SwitchToThisWindow(IntPtr hwnd)
    {
        PInvoke.SwitchToThisWindow(new HWND(hwnd), true);
    }

    /// <summary>
    /// Reactivates a specified window.
    /// </summary>
    /// <param name="window">The window to be reactivated.</param>
    public static void ReActivateWindow(Microsoft.UI.Xaml.Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        ReActivateWindow(hwnd);
    }

    /// <summary>
    /// Reactivates a specified window.
    /// before sending the messages.
    /// </summary>
    /// <param name="hwnd"></param>
    public static void ReActivateWindow(IntPtr hwnd)
    {
        var activeWindow = PInvoke.GetActiveWindow();
        var handle = new HWND(hwnd);
        if (handle == activeWindow)
        {
            PInvoke.SendMessage(handle, (int)NativeValues.WindowMessage.WM_ACTIVATE, (int)NativeValues.WindowMessage.WA_INACTIVE, IntPtr.Zero);
            PInvoke.SendMessage(handle, (int)NativeValues.WindowMessage.WM_ACTIVATE, (int)NativeValues.WindowMessage.WA_ACTIVE, IntPtr.Zero);
        }
        else
        {
            PInvoke.SendMessage(handle, (int)NativeValues.WindowMessage.WM_ACTIVATE, (int)NativeValues.WindowMessage.WA_ACTIVE, IntPtr.Zero);
            PInvoke.SendMessage(handle, (int)NativeValues.WindowMessage.WM_ACTIVATE, (int)NativeValues.WindowMessage.WA_INACTIVE, IntPtr.Zero);
        }
    }

    /// <summary>
    /// Sets the corner radius of a specified window.
    /// </summary>
    /// <param name="window">Specifies the window whose corner radius will be modified.</param>
    /// <param name="cornerPreference">Indicates the preferred style for the window corners.</param>
    public static void SetWindowCornerRadius(Microsoft.UI.Xaml.Window window, NativeValues.DWM_WINDOW_CORNER_PREFERENCE cornerPreference)
    {
        SetWindowCornerRadius(WindowNative.GetWindowHandle(window), cornerPreference);
    }

    /// <summary>
    /// Sets the corner radius of a specified window.
    /// </summary>
    /// <param name="hwnd">Identifies the window for which the corner radius will be set.</param>
    /// <param name="cornerPreference">Specifies the desired corner style for the window.</param>
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

    /// <summary>
    /// Retrieves the corner radius preference for a specified window.
    /// </summary>
    /// <param name="window">The window from which the corner radius preference is obtained.</param>
    /// <returns>Returns the corner radius preference as a NativeValues.DWM_WINDOW_CORNER_PREFERENCE value.</returns>
    public static NativeValues.DWM_WINDOW_CORNER_PREFERENCE GetWindowCornerRadius(Microsoft.UI.Xaml.Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        return GetWindowCornerRadius(hwnd);
    }

    /// <summary>
    /// Retrieves the corner radius preference of a specified window.
    /// </summary>
    /// <param name="hwnd">The handle of the window for which the corner radius preference is being retrieved.</param>
    /// <returns>Returns the corner radius preference or a default value if the retrieval fails.</returns>
    public static NativeValues.DWM_WINDOW_CORNER_PREFERENCE GetWindowCornerRadius(IntPtr hwnd)
    {
        uint cornerPreference = 0;
        unsafe
        {
            HRESULT result = PInvoke.DwmGetWindowAttribute(new HWND(hwnd), Windows.Win32.Graphics.Dwm.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, &cornerPreference, (uint)sizeof(uint));
            if (result.Succeeded)
            {
                return (NativeValues.DWM_WINDOW_CORNER_PREFERENCE)cornerPreference;
            }
        }

        return NativeValues.DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT;
    }

    /// <summary>
    /// Retrieves a read-only list of all top-level windows currently open in the system.
    /// </summary>
    /// <returns>Returns an IReadOnlyList of Win32Window objects representing the top-level windows.</returns>
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

    /// <summary>
    /// Retrieves a read-only list of windows associated with the current process. It enumerates all top-level windows
    /// and filters them by the current process ID.
    /// </summary>
    /// <returns>Returns an IReadOnlyList of Win32Window objects representing the windows of the current process.</returns>
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

    /// <summary>
    /// Retrieves the title text of a specified window.
    /// </summary>
    /// <param name="hwnd">Identifies the window from which to retrieve the title text.</param>
    /// <returns>Returns the title text of the window as a string, or null if the operation fails.</returns>
    public static string GetWindowText(IntPtr hwnd)
    {
        const int MAX_Length = 1024;
        Span<char> buffer = stackalloc char[MAX_Length];
        int result = PInvoke.GetWindowText(new HWND(hwnd), buffer);
        return result > 0 ? buffer.Slice(0, (int)result).ToString() : string.Empty;
    }

    /// <summary>
    /// Retrieves the class name of the window associated with the specified handle.
    /// </summary>
    /// <param name="hwnd">The handle of the window for which the class name is being retrieved.</param>
    /// <returns>Returns the class name as a string or null if the operation fails.</returns>
    public static string GetClassName(IntPtr hwnd)
    {
        const int MAX_Length = 256;
        Span<char> buffer = stackalloc char[MAX_Length];
        int result = PInvoke.GetClassName(new HWND(hwnd), buffer);
        return result > 0 ? buffer.Slice(0, (int)result).ToString() : string.Empty;
    }

    /// <summary>
    /// Retrieves the current AppWindow using finding MainWindow.
    /// </summary>
    /// <returns>An AppWindow instance representing the current window.</returns>
    public static AppWindow GetCurrentAppWindow()
    {
        var tops = GetProcessWindowList();

        var firstWinUI3 = tops.FirstOrDefault(w => w.ClassName == "WinUIDesktopWin32WindowClass" || w.ClassName == "Microsoft.UI.Windowing.Window");
        var windowId = Win32Interop.GetWindowIdFromWindow(firstWinUI3.Handle);

        return AppWindow.GetFromWindowId(windowId);
    }


    /// <summary>
    /// Retrieves the current AppWindow using XamlRoot method.
    /// </summary>
    /// <returns>An AppWindow instance representing the current window.</returns>
    public static AppWindow GetAppWindow(UIElement uIElement)
    {
        if (uIElement == null)
        {
            return null;
        }

        return AppWindow.GetFromWindowId(uIElement.XamlRoot.ContentIslandEnvironment.AppWindowId);
    }

    /// <summary>
    /// Retrieves the current AppWindow using Microsoft.UI.Composition.Visual method.
    /// </summary>
    /// <returns>An AppWindow instance representing the current window.</returns>
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
    /// Retrieves window handle from UIElement using XamlRoot method
    /// </summary>
    /// <returns>Return an IntPtr</returns>
    public static IntPtr GetWindowHandle(UIElement uIElement)
    {
        if (uIElement == null)
        {
            return IntPtr.Zero;
        }

        return Win32Interop.GetWindowFromWindowId(uIElement.XamlRoot.ContentIslandEnvironment.AppWindowId);
    }

    /// <summary>
    /// Retrieves window handle from UIElement using Microsoft.UI.Composition.Visual method
    /// </summary>
    /// <returns>Return an IntPtr</returns>
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

    /// <summary>
    /// Retrieves the current screen size.
    /// </summary>
    /// <returns>A tuple containing the width and height of the screen.</returns>
    public static (int, int) GetScreenSize()
            => (PInvoke.GetSystemMetrics(Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX.SM_CXSCREEN), PInvoke.GetSystemMetrics(Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX.SM_CYSCREEN));

    /// <summary>
    /// Removes the border and title bar from a specified window.
    /// </summary>
    /// <param name="window">The parameter represents the window from which the border and title bar will be removed.</param>
    public static void RemoveWindowBorderAndTitleBar(Microsoft.UI.Xaml.Window window) => RemoveWindowBorderAndTitleBar((nint)window.AppWindow.Id.Value);

    /// <summary>
    /// Removes the border and title bar from a specified window.
    /// </summary>
    /// <param name="hwnd"></param>
    public static void RemoveWindowBorderAndTitleBar(IntPtr hwnd)
    {
        var style = PInvoke.GetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        // Remove border, caption, and thick frame
        style &= ~(int)NativeValues.WindowStyle.WS_BORDER & ~(int)NativeValues.WindowStyle.WS_CAPTION & ~(int)NativeValues.WindowStyle.WS_THICKFRAME;

        PInvoke.SetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);

        // Update the window's appearance
        PInvoke.SetWindowPos(new HWND(hwnd), new HWND(IntPtr.Zero), 0, 0, 0, 0, Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOMOVE | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
    }

    /// <summary>
    /// Allows mouse clicks to pass through a transparent window.
    /// </summary>
    /// <param name="window">The window to be made click-through.</param>
    public static void MakeTransparentWindowClickThrough(Microsoft.UI.Xaml.Window window) => MakeTransparentWindowClickThrough((nint)window.AppWindow.Id.Value);

    /// <summary>
    /// Allows mouse clicks to pass through a transparent window.
    /// </summary>
    /// <param name="hwnd"></param>
    public static void MakeTransparentWindowClickThrough(IntPtr hwnd)
    {
        var currentStyle = PInvoke.GetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, currentStyle | (int)NativeValues.WindowStyle.WS_EX_LAYERED | (int)NativeValues.WindowStyle.WS_EX_TRANSPARENT);
    }

    /// <summary>
    /// Resizes and centers a window based on a specified percentage of the available work area.
    /// </summary>
    /// <param name="window">The window to be resized and centered within the available work area.</param>
    /// <param name="percentage">Indicates the size of the window as a percentage of the work area dimensions.</param>
    /// <exception cref="ArgumentException">Thrown when the percentage is less than or equal to 0 or greater than 100.</exception>
    public static void ResizeAndCenterWindowToPercentageOfWorkArea(Microsoft.UI.Xaml.Window window, double percentage)
    {
        // Validate the percentage
        if (percentage <= 0 || percentage > 100)
        {
            throw new ArgumentException("Percentage must be between 1 and 100.", nameof(percentage));
        }

        Rect maxRect = DisplayMonitorHelper.GetMonitorInfo(window).RectWork;

        // Calculate new dimensions based on the percentage.
        double scaleFactor = percentage / 100.0;
        int newWidth = (int)(maxRect.Width * scaleFactor);
        int newHeight = (int)(maxRect.Height * scaleFactor);

        // Calculate top-left coordinates to center the window inside maxRect.
        int newX = (int)(maxRect.X + (maxRect.Width - newWidth) / 2.0);
        int newY = (int)(maxRect.Y + (maxRect.Height - newHeight) / 2.0);

        window.AppWindow.MoveAndResize(new RectInt32(newX, newY, newWidth, newHeight));
    }

    /// <summary>
    /// Sets the owner of a child window to a specified parent window.
    /// </summary>
    /// <param name="parentWindow">The main window that will own the child window.</param>
    /// <param name="childWindow">The window that will be owned by the specified parent.</param>
    public static void SetWindowOwner(Microsoft.UI.Xaml.Window parentWindow, Microsoft.UI.Xaml.Window childWindow) => SetWindowOwner(WindowNative.GetWindowHandle(parentWindow), WindowNative.GetWindowHandle(childWindow));

    /// <summary>
    /// Sets the owner of a child window to a specified parent window.
    /// </summary>
    /// <param name="parentHwnd">The main window that will own the child window.</param>
    /// <param name="childHwnd">The window that will be owned by the specified parent.</param>
    public static void SetWindowOwner(IntPtr parentHwnd, IntPtr childHwnd)
    {
        NativeMethods.SetWindowLong(childHwnd, -8, parentHwnd);
    }

    /// <summary>
    /// Locates a window by its class name.
    /// </summary>
    /// <param name="hwnd">Specifies the handle to the parent window to search within.</param>
    /// <param name="lpszClass">Defines the class name of the window to be found.</param>
    /// <returns>Returns the handle to the found window or zero if not found.</returns>
    public static IntPtr FindWindow(IntPtr hwnd, string lpszClass)
    {
        return PInvoke.FindWindowEx(new HWND(hwnd), HWND.Null, lpszClass, null);
    }

    /// <summary>
    /// Brings the specified window to the front and makes it the active window.
    /// </summary>
    /// <param name="window">The window to be activated and brought to the foreground.</param>
    /// <returns>Returns true if the operation was successful, otherwise false.</returns>
    public static bool SetForegroundWindow(Microsoft.UI.Xaml.Window window) => SetForegroundWindow(WindowNative.GetWindowHandle(window));

    /// <summary>
    /// Sets the specified window to the foreground, making it the active window. This can bring a window to the front
    /// of other windows.
    /// </summary>
    /// <param name="hwnd"></param>
    /// <returns>Returns true if the operation was successful, otherwise false.</returns>
    public static bool SetForegroundWindow(IntPtr hwnd)
    {
        return PInvoke.SetForegroundWindow(new HWND(hwnd));
    }

    /// <summary>
    /// Centers the specified window on the screen.
    /// </summary>
    /// <param name="window">The window to be centered on the screen.</param>
    /// <returns>Returns true if the operation was successful, otherwise false.</returns>
    public static bool CenterOnScreen(Microsoft.UI.Xaml.Window window) => CenterOnScreen(WindowNative.GetWindowHandle(window));

    /// <summary>
    /// Centers the specified window on the screen.
    /// </summary>
    /// <param name="window">The window to be centered on the screen.</param>
    /// <param name="width">Defines the desired width of the window for centering purposes.</param>
    /// <param name="height">Defines the desired height of the window for centering purposes.</param>
    /// <returns>Returns a boolean indicating whether the window was successfully centered.</returns>
    public static bool CenterOnScreen(Microsoft.UI.Xaml.Window window, double? width, double? height) => CenterOnScreen(WindowNative.GetWindowHandle(window), width, height);

    /// <summary>
    /// Centers the specified window on the screen.
    /// </summary>
    /// <param name="hwnd">The handle of the window to be centered on the screen.</param>
    /// <returns>Returns true if the window was successfully centered.</returns>
    public static bool CenterOnScreen(IntPtr hwnd) => CenterOnScreen(hwnd, null, null);

    /// <summary>
    /// Centers the specified window on the screen.
    /// </summary>
    /// <param name="hwnd">The handle of the window to be centered on the screen.</param>
    /// <param name="width">Specifies the desired width of the window; if not provided, the current width is used.</param>
    /// <param name="height">Specifies the desired height of the window; if not provided, the current height is used.</param>
    /// <returns>Indicates whether the window was successfully repositioned.</returns>
    public static bool CenterOnScreen(IntPtr hwnd, double? width, double? height)
    {
        var monitor = DisplayMonitorHelper.GetMonitorInfo(hwnd);
        if (monitor is null)
            return false;

        var dpi = PInvoke.GetDpiForWindow(new HWND(hwnd));
        if (!PInvoke.GetWindowRect(new HWND(hwnd), out RECT windowRect))
            return false;

        var scalingFactor = dpi / 96.0;
        var w = width.HasValue ? (int)(width.Value * scalingFactor) : (windowRect.right - windowRect.left);
        var h = height.HasValue ? (int)(height.Value * scalingFactor) : (windowRect.bottom - windowRect.top);

        var cx = (monitor.RectMonitor.Left + monitor.RectMonitor.Right) / 2;
        var cy = (monitor.RectMonitor.Bottom + monitor.RectMonitor.Top) / 2;
        var left = (int)cx - (w / 2);
        var top = (int)cy - (h / 2);

        return PInvoke.SetWindowPos(new HWND(hwnd), new HWND(), left, top, w, h,
            Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOZORDER |
            Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
    }
}
