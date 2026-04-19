using static DevWinUI.NativeValues;

namespace DevWinUI;

public partial class WindowManager : IDisposable
{
    private readonly Window _window;
    private readonly WindowMessageMonitor _monitor;
    public double Width
    {
        get => _window.AppWindow.Size.Width / (WindowHelper.GetDpiForWindow(_window) / 96d);
        set => WindowHelper.SetWindowSize(_window, value, Height);
    }

    public double Height
    {
        get => _window.AppWindow.Size.Height / (WindowHelper.GetDpiForWindow(_window) / 96d);
        set => WindowHelper.SetWindowSize(_window, Width, value);
    }

    private double _minWidth = 136;
    public double MinWidth
    {
        get => _minWidth;
        set
        {
            _minWidth = value;
            if (Width < value)
                Width = value;
        }
    }

    private double _minHeight = 39;
    public double MinHeight
    {
        get => _minHeight;
        set
        {
            _minHeight = value;
            if (Height < value)
                Height = value;
        }
    }

    private double _maxWidth = 0;

    public double MaxWidth
    {
        get => _maxWidth;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            _maxWidth = value;
            if (value > 0 && Width > value)
                Width = value;
        }
    }

    private double _maxHeight = 0;

    public double MaxHeight
    {
        get => _maxHeight;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            _maxHeight = value;
            if (value > 0 && Height > value)
                Height = value;
        }
    }
    public WindowManager(Window window)
    {
        _window = window;

        _monitor = new WindowMessageMonitor(_window);
        _monitor.WindowMessageReceived -= OnWindowMessageReceived;
        _monitor.WindowMessageReceived += OnWindowMessageReceived;
    }

    private unsafe void OnWindowMessageReceived(object sender, WindowMessageEventArgs e)
    {
        if (e.MessageType == (uint)NativeValues.WindowMessage.WM_GETMINMAXINFO)
        {
            MINMAXINFO* rect2 = (MINMAXINFO*)e.Message.LParam;
            var currentDpi = WindowHelper.GetDpiForWindow(_window);

            // Restrict min-size
            rect2->ptMinTrackSize.X = (int)(Math.Max(MinWidth * (currentDpi / 96f), rect2->ptMinTrackSize.X));
            rect2->ptMinTrackSize.Y = (int)(Math.Max(MinHeight * (currentDpi / 96f), rect2->ptMinTrackSize.Y));
            // Restrict max-size
            if (!double.IsNaN(MaxWidth) && MaxWidth > 0)
                rect2->ptMaxTrackSize.X = (int)(Math.Min(Math.Max(MaxWidth, MinWidth) * (currentDpi / 96f), rect2->ptMaxTrackSize.X)); 
            if (!double.IsNaN(MaxHeight) && MaxHeight > 0)
                rect2->ptMaxTrackSize.Y = (int)(Math.Min(Math.Max(MaxHeight, MinHeight) * (currentDpi / 96f), rect2->ptMaxTrackSize.Y)); 
        }
    }

    public void Dispose()
    {
        _monitor.WindowMessageReceived -= OnWindowMessageReceived;
        _monitor.Dispose();
    }
}
