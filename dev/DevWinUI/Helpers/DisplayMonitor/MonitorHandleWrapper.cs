using Windows.Win32.Graphics.Gdi;

namespace DevWinUI;

internal sealed class MonitorHandleWrapper
{
    public HMONITOR Handle;
    public MonitorInfo? Info;
}
