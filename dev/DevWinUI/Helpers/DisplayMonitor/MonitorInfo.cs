using Windows.Foundation;
using Windows.Win32.Graphics.Gdi;

namespace DevWinUI;

internal partial class MonitorInfo
{
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
            list.Add(new MonitorInfo(hMonitor, lprcMonitor));
        return new BOOL(1);
    }

    public unsafe static MonitorInfo? GetNearestDisplayMonitor(IntPtr hwnd)
    {
        var nearestMonitor = PInvoke.MonitorFromWindow(new HWND(hwnd), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST);
        MonitorInfo? nearestMonitorInfo = null;

        var handle = GCHandle.Alloc(nearestMonitorInfo, GCHandleType.Pinned);
        var ptr = GCHandle.ToIntPtr(handle);
        LPARAM data = new LPARAM(ptr);

        try
        {
            bool ok = PInvoke.EnumDisplayMonitors(HDC.Null, (RECT?)null, &GetNearestDisplayMonitorEnumProc, data);
            if (!ok)
            {
                Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
            }

            if (handle.Target is MonitorInfo result)
            {
                nearestMonitorInfo = result;
            }
        }
        finally
        {
            handle.Free();
        }

        return nearestMonitorInfo;
    }

    [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    private unsafe static BOOL GetNearestDisplayMonitorEnumProc(HMONITOR monitor, HDC deviceContext, RECT* rect, LPARAM dwData)
    {
        var handle = GCHandle.FromIntPtr(dwData.Value);
        if (handle.IsAllocated && handle.Target is MonitorInfo result && rect is not null)
        {
            if (monitor == PInvoke.MonitorFromWindow(HWND.Null, MONITOR_FROM_FLAGS.MONITOR_DEFAULTTONEAREST))
            {
                handle.Target = new MonitorInfo(monitor, rect);
                return new BOOL(0);
            }
        }
        return new BOOL(1);
    }

    private readonly HMONITOR _monitor;

    internal unsafe MonitorInfo(HMONITOR monitor, RECT* rect)
    {
        RectMonitor =
            new Rect(new Point(rect->left, rect->top),
            new Point(rect->right, rect->bottom));
        _monitor = monitor;
        var info = new MONITORINFOEXW() { monitorInfo = new MONITORINFO() { cbSize = (uint)sizeof(MONITORINFOEXW) } };
        GetMonitorInfo(monitor, ref info);
        RectWork =
            new Rect(new Point(info.monitorInfo.rcWork.left, info.monitorInfo.rcWork.top),
            new Point(info.monitorInfo.rcWork.right, info.monitorInfo.rcWork.bottom));
        Name = new string(info.szDevice.AsSpan()).Replace("\0", "").Trim();
    }
    public string Name { get; }

    public Rect RectMonitor { get; }

    public Rect RectWork { get; }

    public bool IsPrimary => _monitor == PInvoke.MonitorFromWindow(new(IntPtr.Zero), MONITOR_FROM_FLAGS.MONITOR_DEFAULTTOPRIMARY);

    public override string ToString() => $"{Name} {RectMonitor.Width}x{RectMonitor.Height}";

    private static unsafe bool GetMonitorInfo(HMONITOR hMonitor, ref MONITORINFOEXW lpmi)
    {
        fixed (MONITORINFOEXW* lpmiLocal = &lpmi)
        {
            var lpmiBase = (MONITORINFO*)lpmiLocal;
            var __result = PInvoke.GetMonitorInfo(hMonitor, lpmiBase);
            return __result;
        }
    }
}
