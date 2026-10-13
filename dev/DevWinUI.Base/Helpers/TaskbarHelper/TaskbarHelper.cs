using System.Runtime.InteropServices.Marshalling;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

public static partial class TaskbarHelper
{
    private static readonly Guid CLSID_TaskbarList = new("56fdf344-fd6d-11d0-958a-006097c9a090");
    private static readonly Guid IID_ITaskbarList3 = new("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf");

    private static readonly ITaskbarList3 taskbarInstance;
    private static readonly bool taskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);

    static TaskbarHelper()
    {
        if (taskbarSupported)
        {
            unsafe
            {
                Guid clsid = CLSID_TaskbarList;
                Guid iid = IID_ITaskbarList3;
                void* ppv;
                var hr = PInvoke.CoCreateInstance(&clsid, null, Windows.Win32.System.Com.CLSCTX.CLSCTX_INPROC_SERVER, &iid, &ppv);
                if (hr.Succeeded && ppv != null)
                {
                    taskbarInstance = (ITaskbarList3)new StrategyBasedComWrappers().GetOrCreateObjectForComInstance((IntPtr)ppv, CreateObjectFlags.None);
                    Marshal.Release((IntPtr)ppv);
                    taskbarInstance.HrInit();
                }
            }
        }
    }

    /// <summary>
    /// Sets the progress state of a specified window in the taskbar if supported.
    /// </summary>
    /// <param name="windowHandle">Identifies the window for which the progress state is being set.</param>
    /// <param name="taskbarState">Specifies the desired progress state to be applied to the window.</param>
    public static void SetProgressState(IntPtr windowHandle, TaskbarStates taskbarState)
    {
        if (taskbarSupported) taskbarInstance.SetProgressState(windowHandle, taskbarState);
    }

    /// <summary>
    /// Sets the progress value of a taskbar item for a specified window. It updates the visual representation of
    /// progress in the taskbar.
    /// </summary>
    /// <param name="windowHandle">Identifies the window for which the taskbar progress is being set.</param>
    /// <param name="progressValue">Represents the current progress value to be displayed in the taskbar.</param>
    /// <param name="progressMax">Indicates the maximum value of progress to determine the completion percentage.</param>
    public static void SetProgressValue(IntPtr windowHandle, double progressValue, double progressMax)
    {
        if (taskbarSupported) taskbarInstance.SetProgressValue(windowHandle, (ulong)progressValue, (ulong)progressMax);
    }

    /// <summary>
    /// Flashes the specified window's taskbar button to draw the user's attention.
    /// </summary>
    /// <param name="window">The window to flash.</param>
    /// <returns>
    /// <see langword="true"/> if the window was active before the call; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool FlashWindow(Window window)
    {
        return FlashWindow(WindowNative.GetWindowHandle(window));
    }

    /// <summary>
    /// Flashes the specified window's taskbar button to draw the user's attention.
    /// </summary>
    /// <param name="hwnd">The handle of the window to flash.</param>
    /// <returns>
    /// <see langword="true"/> if the window was active before the call; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool FlashWindow(IntPtr hwnd)
    {
        return FlashWindow(hwnd, TaskbarFlashFlags.FLASHW_TRAY | TaskbarFlashFlags.FLASHW_TIMERNOFG, 3, 0);
    }

    /// <summary>
    /// Flashes the specified window's taskbar button to draw the user's attention.
    /// </summary>
    /// <param name="window">The window to flash.</param>
    /// <param name="flags">Specifies which parts of the window should flash and the flashing behavior.</param>
    /// <param name="count">The number of times to flash. Use zero to flash until the window comes to the foreground.</param>
    /// <param name="timeout">The interval between flashes, in milliseconds. Use zero for the system default.</param>
    /// <returns>
    /// <see langword="true"/> if the window was active before the call; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool FlashWindow(Window window, TaskbarFlashFlags flags, uint count, uint timeout)
    {
        return FlashWindow(WindowNative.GetWindowHandle(window), flags, count, timeout);
    }

    /// <summary>
    /// Flashes the specified window's taskbar button to draw the user's attention.
    /// </summary>
    /// <param name="hwnd">The handle of the window to flash.</param>
    /// <param name="flags">Specifies which parts of the window should flash and the flashing behavior.</param>
    /// <param name="count">The number of times to flash. Use zero to flash until the window comes to the foreground.</param>
    /// <param name="timeout">The interval between flashes, in milliseconds. Use zero for the system default.</param>
    /// <returns>
    /// <see langword="true"/> if the window was active before the call; otherwise, <see langword="false"/>.
    /// </returns>
    public static unsafe bool FlashWindow(IntPtr hwnd, TaskbarFlashFlags flags, uint count, uint timeout)
    {
        FLASHWINFO flashInfo = new()
        {
            cbSize = (uint)sizeof(FLASHWINFO),
            hwnd = (Windows.Win32.Foundation.HWND)hwnd,
            dwFlags = (FLASHWINFO_FLAGS)flags,
            uCount = count,
            dwTimeout = timeout
        };

        return PInvoke.FlashWindowEx(&flashInfo);
    }
}
