namespace DevWinUI;

public partial class SnapLayoutManager
{
    private const int HTMAXBUTTON = 9;

    private Window window;
    private FrameworkElement snapElement;
    private WindowMessageMonitor monitor;
    private IntPtr hwnd;
    public bool IsEnabled { get; set; } = true;

    public void Attach(Window window, FrameworkElement element)
    {
        Detach();

        this.window = window;
        this.snapElement = element;
        this.hwnd = WindowNative.GetWindowHandle(this.window);
        this.window.Closed += OnWindowClosed;

        monitor = new WindowMessageMonitor(this.window);
        monitor.WindowMessageReceived += OnWindowMessageReceived;

        window.ExtendsContentIntoTitleBar = true;
    }

    private void OnWindowClosed(object sender, WindowEventArgs e)
    {
        Detach();
    }

    
    public void Detach()
    {
        if (window != null)
        {
            window.Closed -= OnWindowClosed;
        }

        if (monitor != null)
        {
            monitor.WindowMessageReceived -= OnWindowMessageReceived;
            (monitor as IDisposable)?.Dispose();
        }

        monitor = null;
        window = null;
        snapElement = null;
    }

    private void OnWindowMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        if (!IsEnabled)
            return;

        switch (e.MessageType)
        {
            case (uint)NativeValues.WindowMessage.WM_NCHITTEST:
                HandleNcHitTest(e);
                break;

            case (uint)NativeValues.WindowMessage.WM_NCRBUTTONDOWN:
                e.Result = 0;
                e.Handled = true;
                break;
        }
    }

    private void HandleNcHitTest(WindowMessageEventArgs e)
    {
        if (snapElement == null || window == null)
            return;

        int x = (short)(e.Message.LParam.ToInt32() & 0xFFFF);
        int y = (short)(e.Message.LParam.ToInt32() >> 16);

        var mouse = new Point(x, y);

        Rect rect = GetButtonScreenRect(snapElement);
        if (rect == Rect.Empty)
            return;

        if (rect.Contains(mouse))
        {
            e.Result = (nint)HTMAXBUTTON;
            e.Handled = true;
        }
    }

    private Rect GetButtonScreenRect(FrameworkElement element)
    {
        if (element == null)
            return Rect.Empty;

        var root = window.Content as UIElement;
        if (root == null)
            return Rect.Empty;

        var point = element.TransformToVisual(root).TransformPoint(new Windows.Foundation.Point(0, 0));

        double scale = element.XamlRoot.RasterizationScale;
        double left = point.X * scale;
        double top = point.Y * scale;
        double width = element.ActualWidth * scale;
        double height = element.ActualHeight * scale;

        var pt = new System.Drawing.Point((int)Math.Round(left), (int)Math.Round(top));
        Windows.Win32.PInvoke.ClientToScreen(new(hwnd), ref pt);

        return new Rect(pt.X, pt.Y, width, height);
    }
}

