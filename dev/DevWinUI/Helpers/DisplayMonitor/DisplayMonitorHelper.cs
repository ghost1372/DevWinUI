using WinRT.Interop;

namespace DevWinUI;

public static partial class DisplayMonitorHelper
{
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
    public static DisplayMonitorDetails GetMonitorInfo(Microsoft.UI.Xaml.Window? window)
    {
        if (window is not null)
        {
            return GetMonitorInfo(WindowNative.GetWindowHandle(window));
        }

        return GetPrimaryMonitorInfo();
    }

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
