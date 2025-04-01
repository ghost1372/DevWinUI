namespace DevWinUI;

public static partial class DisplayMonitorHelper
{
    /// <summary>
    /// Retrieves a list of display monitor details including name, monitor rectangle, work rectangle, and primary
    /// status.
    /// </summary>
    /// <returns>Returns a list of DisplayMonitorDetails objects.</returns>
    public static List<DisplayMonitorDetails> GetMonitorInfo()
    {
        var monitorInfos = MonitorInfo.GetDisplayMonitors();
        return monitorInfos.Select(x => new DisplayMonitorDetails
        {
            Name = x.Name,
            RectMonitor = x.RectMonitor,
            RectWork = x.RectWork,
            IsPrimary = x.IsPrimary
        }).ToList();
    }

    /// <summary>
    /// Retrieves details about the display monitor associated with a given window handle. If no monitor is found, it
    /// returns information about the primary monitor.
    /// </summary>
    /// <param name="hwnd">Represents a handle to a window, used to identify the associated display monitor.</param>
    /// <returns>Returns an object containing details such as the monitor's name, working area, and whether it is the primary
    /// monitor.</returns>
    public static DisplayMonitorDetails GetMonitorInfo(IntPtr hwnd)
    {
        var monitorInfo = MonitorInfo.GetNearestDisplayMonitor(hwnd);
        if (monitorInfo is not null)
        {
            return new()
            {
                Name = monitorInfo.Name,
                RectMonitor = monitorInfo.RectMonitor,
                RectWork = monitorInfo.RectWork,
                IsPrimary = monitorInfo.IsPrimary
            };
        }

        return GetPrimaryMonitorInfo();
    }

    /// <summary>
    /// Retrieves information about the display monitor associated with a specified window or the primary monitor if no
    /// window is provided.
    /// </summary>
    /// <param name="window">The window context used to obtain the monitor details.</param>
    /// <returns>Returns details about the monitor, either for the specified window or the primary monitor.</returns>
    public static DisplayMonitorDetails GetMonitorInfo(Microsoft.UI.Xaml.Window? window)
    {
        if (window is not null)
        {
            return GetMonitorInfo(WindowNative.GetWindowHandle(window));
        }

        return GetPrimaryMonitorInfo();
    }

    /// <summary>
    /// Retrieves information about the primary display monitor, including its name and dimensions.
    /// </summary>
    /// <returns>Returns a DisplayMonitorDetails object containing details of the primary monitor.</returns>
    public static DisplayMonitorDetails GetPrimaryMonitorInfo()
    {
        var primaryMonitorInfo = MonitorInfo.GetDisplayMonitors().FirstOrDefault(x => x.IsPrimary);
        return new()
        {
            Name = primaryMonitorInfo!.Name,
            RectMonitor = primaryMonitorInfo.RectMonitor,
            RectWork = primaryMonitorInfo.RectWork,
            IsPrimary = primaryMonitorInfo.IsPrimary
        };
    }
}
