namespace DevWinUI;

public static partial class DisplayMonitorHelper
{
    /// <summary>
    /// Retrieves information about all available display monitors, including their device names,
    /// bounding rectangles, working areas, and primary status.
    /// </summary>
    /// <returns>A list of <see cref="DisplayMonitorDetails"/> objects representing all detected monitors.</returns>
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
    /// Retrieves detailed information about the display monitor associated with a specified window handle.
    /// If no monitor is associated with the handle, information for the primary monitor is returned.
    /// This method directly queries the system for the nearest monitor without performing enumeration.
    /// </summary>
    /// <param name="hwnd">A handle to the window used to determine the associated monitor.</param>
    /// <returns>
    /// A <see cref="DisplayMonitorDetails"/> object containing monitor details such as device name,
    /// working area, monitor bounds, and primary status.
    /// </returns>
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
    /// Retrieves detailed information about the display monitor associated with a specified window handle.
    /// If no monitor is associated with the handle, information for the primary monitor is returned.
    /// Unlike <see cref="GetMonitorInfo(IntPtr)"/>, this version determines the target monitor by
    /// enumerating all available monitors using <c>EnumDisplayMonitors</c>, which may provide more
    /// reliable results on certain multi-display configurations.
    /// </summary>
    /// <param name="hwnd">A handle to the window used to determine the associated monitor.</param>
    /// <returns>
    /// A <see cref="DisplayMonitorDetails"/> object containing monitor details such as device name,
    /// working area, monitor bounds, and primary status.
    /// </returns>
    public static DisplayMonitorDetails GetMonitorInfo2(IntPtr hwnd)
    {
        var monitorInfo = MonitorInfo.GetNearestDisplayMonitor2(hwnd);
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
    /// Retrieves information about the display monitor associated with the specified <see cref="Microsoft.UI.Xaml.Window"/>.
    /// If the window is <c>null</c>, the primary monitor information is returned.
    /// </summary>
    /// <param name="window">The XAML window for which monitor information should be retrieved.</param>
    /// <returns>A <see cref="DisplayMonitorDetails"/> object describing the associated or primary monitor.</returns>
    public static DisplayMonitorDetails GetMonitorInfo(Microsoft.UI.Xaml.Window? window)
    {
        if (window is not null)
        {
            return GetMonitorInfo(WindowNative.GetWindowHandle(window));
        }

        return GetPrimaryMonitorInfo();
    }

    /// <summary>
    /// Retrieves information about the display monitor associated with the specified <see cref="Microsoft.UI.Xaml.Window"/>.
    /// If the window is <c>null</c>, the primary monitor information is returned.
    /// Unlike <see cref="GetMonitorInfo(Microsoft.UI.Xaml.Window)"/>, this version uses
    /// enumeration to match the monitor handle, improving detection consistency in complex display setups.
    /// </summary>
    /// <param name="window">The XAML window for which monitor information should be retrieved.</param>
    /// <returns>A <see cref="DisplayMonitorDetails"/> object describing the associated or primary monitor.</returns>
    public static DisplayMonitorDetails GetMonitorInfo2(Microsoft.UI.Xaml.Window? window)
    {
        if (window is not null)
        {
            return GetMonitorInfo2(WindowNative.GetWindowHandle(window));
        }

        return GetPrimaryMonitorInfo();
    }

    /// <summary>
    /// Retrieves information about the system's primary display monitor, including its name,
    /// bounding rectangle, and working area.
    /// </summary>
    /// <returns>A <see cref="DisplayMonitorDetails"/> object representing the primary monitor.</returns>
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
