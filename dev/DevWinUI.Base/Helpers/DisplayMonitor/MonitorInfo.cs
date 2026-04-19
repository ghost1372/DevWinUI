using Windows.Win32.Graphics.Gdi;

namespace DevWinUI;

internal partial class MonitorInfo
{
    private readonly HMONITOR _monitor;

    public string Name { get; }

    public Rect RectMonitor { get; }

    public Rect RectWork { get; }

    public bool IsPrimary => _monitor == PInvoke.MonitorFromWindow(new(IntPtr.Zero), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);

    public override string ToString() => $"{Name} {RectMonitor.Width}x{RectMonitor.Height}";

    internal unsafe MonitorInfo(HMONITOR monitor, RECT rect)
    {
        RectMonitor =
            new Rect(new Point(rect.left, rect.top),
            new Point(rect.right, rect.bottom));
        _monitor = monitor;
        var info = new MONITORINFOEXW() { monitorInfo = new MONITORINFO() { cbSize = (uint)sizeof(MONITORINFOEXW) } };
        GetMonitorInfo(monitor, ref info);
        RectWork =
            new Rect(new Point(info.monitorInfo.rcWork.left, info.monitorInfo.rcWork.top),
            new Point(info.monitorInfo.rcWork.right, info.monitorInfo.rcWork.bottom));
        Name = new string(info.szDevice.AsSpan()).Replace("\0", "").Trim();
    }
    public unsafe static IList<MonitorInfo> GetDisplayMonitors()
    {
        int monitorCount = PInvoke.GetSystemMetrics(Windows.Win32.UI.WindowsAndMessaging.SYSTEM_METRICS_INDEX.SM_CMONITORS);
        List<MonitorInfo> list = new List<MonitorInfo>(monitorCount);
        var cbhandle = GCHandle.Alloc(list);
        var ptr = GCHandle.ToIntPtr(cbhandle);

        LPARAM data = new LPARAM(ptr);
        bool ok = PInvoke.EnumDisplayMonitors(new HDC(0), (RECT?)null, &GetDisplayMonitorsEnumProc, data);
        cbhandle.Free();
        if (!ok)
            Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
        return list;
    }

    [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    private unsafe static BOOL GetDisplayMonitorsEnumProc(HMONITOR hMonitor, HDC hdcMonitor, RECT* lprcMonitor, LPARAM dwData)
    {
        var handle = GCHandle.FromIntPtr(dwData.Value);
        if (!lprcMonitor->IsEmpty && handle.IsAllocated && handle.Target is List<MonitorInfo> list)
            list.Add(new MonitorInfo(hMonitor, *lprcMonitor));
        return new BOOL(1);
    }

    private static unsafe bool GetMonitorInfo(HMONITOR hMonitor, ref MONITORINFOEXW lpmi)
    {
        fixed (MONITORINFOEXW* lpmiLocal = &lpmi)

        {
            bool __result = GetMonitorInfo(hMonitor, lpmiLocal);
            return __result;
        }
    }

    [DllImport("User32", ExactSpelling = true, EntryPoint = "GetMonitorInfoW")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern unsafe bool GetMonitorInfo(HMONITOR hMonitor, MONITORINFOEXW* lpmi);

    public static unsafe MonitorInfo? GetNearestDisplayMonitor(IntPtr hwnd)
    {
        HMONITOR nearestMonitor = PInvoke.MonitorFromWindow(new HWND(hwnd), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
        if (nearestMonitor == HMONITOR.Null)
            return null;

        var info = new MONITORINFOEXW() { monitorInfo = new MONITORINFO() { cbSize = (uint)sizeof(MONITORINFOEXW) } };
        if (!GetMonitorInfo(nearestMonitor, ref info))
            return null;

        RECT rect = info.monitorInfo.rcMonitor;
        return new MonitorInfo(nearestMonitor, rect);
    }

    public unsafe static MonitorInfo? GetNearestDisplayMonitor2(IntPtr hwnd)
    {
        HMONITOR nearestMonitor = PInvoke.MonitorFromWindow(new HWND(hwnd), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
        if (nearestMonitor == HMONITOR.Null)
            return null;

        var wrapper = new MonitorHandleWrapper { Handle = nearestMonitor };
        var gch = GCHandle.Alloc(wrapper, GCHandleType.Normal);
        LPARAM data = new LPARAM(GCHandle.ToIntPtr(gch));

        try
        {
            bool ok = PInvoke.EnumDisplayMonitors(HDC.Null, (RECT?)null, &GetNearestDisplayMonitorEnumProc, data);
            if (!ok)
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());

            return wrapper.Info;
        }
        finally
        {
            gch.Free();
        }
    }


    [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    private unsafe static BOOL GetNearestDisplayMonitorEnumProc(HMONITOR monitor, HDC hdc, RECT* rect, LPARAM dwData)
    {
        var gch = GCHandle.FromIntPtr(dwData.Value);
        if (!gch.IsAllocated)
            return new BOOL(0);

        if (gch.Target is not MonitorHandleWrapper wrapper || rect is null)
            return new BOOL(1);

        // compare current enumerated monitor with the desired one
        if (monitor == wrapper.Handle)
        {
            wrapper.Info = new MonitorInfo(monitor, *rect);
            return new BOOL(0); // stop enumeration
        }

        return new BOOL(1); // continue enumeration
    }
}
