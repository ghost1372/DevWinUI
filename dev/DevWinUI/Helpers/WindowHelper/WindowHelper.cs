﻿using System.Diagnostics;

namespace DevWinUI;

public partial class WindowHelper
{
    internal static List<Win32Window> processWindowList = new List<Win32Window>();
    internal static Process currentProcess;
    internal static List<Win32Window> topLevelWindowList = new List<Win32Window>();
    public static ObservableCollection<Microsoft.UI.Xaml.Window> ActiveWindows { get; } = new();
    public static void TrackWindow(Microsoft.UI.Xaml.Window window)
    {
        window.Closed += (sender, args) =>
        {
            ActiveWindows.Remove(window);
        };

        ActiveWindows.AddIfNotExists(window);
    }
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
    public static void SwitchToThisWindow(Microsoft.UI.Xaml.Window window)
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

    public static void ReActivateWindow(Microsoft.UI.Xaml.Window window)
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
            PInvoke.SendMessage(hwnd, (int)NativeValues.WindowMessage.WM_ACTIVATE, (int)NativeValues.WindowMessage.WA_INACTIVE, IntPtr.Zero);
            PInvoke.SendMessage(hwnd, (int)NativeValues.WindowMessage.WM_ACTIVATE, (int)NativeValues.WindowMessage.WA_ACTIVE, IntPtr.Zero);
        }
        else
        {
            PInvoke.SendMessage(hwnd, (int)NativeValues.WindowMessage.WM_ACTIVATE, (int)NativeValues.WindowMessage.WA_ACTIVE, IntPtr.Zero);
            PInvoke.SendMessage(hwnd, (int)NativeValues.WindowMessage.WM_ACTIVATE, (int)NativeValues.WindowMessage.WA_INACTIVE, IntPtr.Zero);
        }
    }

    public static void SetWindowCornerRadius(Microsoft.UI.Xaml.Window window, NativeValues.DWM_WINDOW_CORNER_PREFERENCE cornerPreference)
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
    public static NativeValues.DWM_WINDOW_CORNER_PREFERENCE GetWindowCornerRadius(Microsoft.UI.Xaml.Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        return GetWindowCornerRadius(hwnd);
    }
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

    public static void RemoveWindowBorderAndTitleBar(Microsoft.UI.Xaml.Window window) => RemoveWindowBorderAndTitleBar((nint)window.AppWindow.Id.Value);
    public static void RemoveWindowBorderAndTitleBar(IntPtr hwnd)
    {
        var style = PInvoke.GetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE);

        // Remove border, caption, and thick frame
        style &= ~(int)NativeValues.WindowStyle.WS_BORDER & ~(int)NativeValues.WindowStyle.WS_CAPTION & ~(int)NativeValues.WindowStyle.WS_THICKFRAME;

        PInvoke.SetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);

        // Update the window's appearance
        PInvoke.SetWindowPos(new HWND(hwnd), new HWND(IntPtr.Zero), 0, 0, 0, 0, Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOMOVE | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_FRAMECHANGED | Windows.Win32.UI.WindowsAndMessaging.SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
    }
    public static void MakeTransparentWindowClickThrough(Microsoft.UI.Xaml.Window window) => MakeTransparentWindowClickThrough((nint)window.AppWindow.Id.Value);
    public static void MakeTransparentWindowClickThrough(IntPtr hwnd)
    {
        var currentStyle = PInvoke.GetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        PInvoke.SetWindowLong(new HWND(hwnd), Windows.Win32.UI.WindowsAndMessaging.WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, currentStyle | (int)NativeValues.WindowStyle.WS_EX_LAYERED | (int)NativeValues.WindowStyle.WS_EX_TRANSPARENT);
    }

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

    public static void SetWindowOwner(Microsoft.UI.Xaml.Window parentWindow, Microsoft.UI.Xaml.Window childWindow) => SetWindowOwner(WindowNative.GetWindowHandle(parentWindow), WindowNative.GetWindowHandle(childWindow));
    public static void SetWindowOwner(IntPtr parentHwnd, IntPtr childHwnd)
    {
        NativeMethods.SetWindowLong(childHwnd, -8, parentHwnd);
    }

    public static IntPtr FindWindow(IntPtr hwnd, string lpszClass)
    {
        return PInvoke.FindWindowEx(new HWND(hwnd), HWND.Null, lpszClass, null);
    }
    public static bool SetForegroundWindow(Microsoft.UI.Xaml.Window window) => SetForegroundWindow(WindowNative.GetWindowHandle(window));

    public static bool SetForegroundWindow(IntPtr hwnd)
    {
        return PInvoke.SetForegroundWindow(new HWND(hwnd));
    }

    public static bool CenterOnScreen(Microsoft.UI.Xaml.Window window) => CenterOnScreen(WindowNative.GetWindowHandle(window));
    public static bool CenterOnScreen(Microsoft.UI.Xaml.Window window, double? width, double? height) => CenterOnScreen(WindowNative.GetWindowHandle(window), width, height);
    public static bool CenterOnScreen(IntPtr hwnd) => CenterOnScreen(hwnd, null, null);
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
