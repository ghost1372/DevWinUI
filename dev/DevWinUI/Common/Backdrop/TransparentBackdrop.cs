using Microsoft.UI.Composition;
using Windows.Win32.Graphics.Dwm;
namespace DevWinUI;
public partial class TransparentBackdrop : CompositionBrushBackdrop
{
    private Windows.Win32.Graphics.Gdi.HBRUSH backgroundBrush = Windows.Win32.Graphics.Gdi.HBRUSH.Null;

    private WindowMessageMonitor? monitor;
    private Windows.UI.Composition.CompositionColorBrush? brush;

    public TransparentBackdrop() : this(Colors.Transparent)
    {
    }

    public TransparentBackdrop(Color tintColor)
    {
        _color = tintColor;
    }

    private Color _color;

    public Color TintColor
    {
        get { return _color; }
        set
        {
            _color = value;
            if (brush != null)
            {
                brush.Color = value;
            }
        }
    }

    protected override Windows.UI.Composition.CompositionBrush CreateBrush(Windows.UI.Composition.Compositor compositor)
    {
        return Compositor.CreateColorBrush(TintColor);
    }

    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        ulong hWnd = xamlRoot.ContentIslandEnvironment.AppWindowId.Value;

        monitor = new WindowMessageMonitor((IntPtr)hWnd);
        monitor.WindowMessageReceived += Monitor_WindowMessageReceived;

        ConfigureDwm(hWnd);

        base.OnTargetConnected(connectedTarget, xamlRoot);

        var hdc = PInvoke.GetDC(new HWND((nint)hWnd));
        ClearBackground((nint)hWnd, hdc);
    }
    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        monitor?.Dispose();
        monitor = null;
        var backdrop = disconnectedTarget.SystemBackdrop;
        disconnectedTarget.SystemBackdrop = null;
        backdrop?.Dispose();
        brush?.Dispose();
        brush = null;
        if (!backgroundBrush.IsNull)
            PInvoke.DeleteObject(backgroundBrush);
        backgroundBrush = Windows.Win32.Graphics.Gdi.HBRUSH.Null;
        base.OnTargetDisconnected(disconnectedTarget);
    }

    private static void ConfigureDwm(ulong hWnd)
    {
        IntPtr handle = new IntPtr((nint)hWnd);
        var margins = new Windows.Win32.UI.Controls.MARGINS(); // You may need to set appropriate values for margins

        PInvoke.DwmExtendFrameIntoClientArea(new HWND(handle), in margins);

        var dwm = new DWM_BLURBEHIND()
        {
            dwFlags = (uint)(NativeValues.DWM_BLURBEHIND_Mask.Enable | NativeValues.DWM_BLURBEHIND_Mask.BlurRegion),
            fEnable = true,
            hRgnBlur = PInvoke.CreateRectRgn(-2, -2, -1, -1),
        };
        PInvoke.DwmEnableBlurBehindWindow(new HWND(handle), in dwm);
    }
    private bool ClearBackground(nint hwnd, nint hdc)
    {
        if (PInvoke.GetClientRect(new HWND(hwnd), out var rect))
        {
            if (backgroundBrush.IsNull)
                backgroundBrush = PInvoke.CreateSolidBrush(new Windows.Win32.Foundation.COLORREF(0));

            NativeMethods.FillRect(hdc, ref rect, backgroundBrush);
            return true;
        }
        return false;
    }

    private void Monitor_WindowMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        if (e.MessageType == (uint)NativeValues.WindowMessage.WM_ERASEBKGND)
        {
            if (ClearBackground(e.Message.Hwnd, (nint)e.Message.WParam))
            {
                e.Result = 1;
                e.Handled = true;
            }
        }
        else if ((int)e.MessageType == 798 /*WM_DWMCOMPOSITIONCHANGED*/)
        {
            ConfigureDwm((ulong)e.Message.Hwnd);
            e.Handled = true;
            e.Result = 0;
        }
    }
}
