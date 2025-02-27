using Windows.Foundation.Metadata;

namespace DevWinUI;

[Experimental]
[ContentProperty(Name = nameof(Content))]
public partial class Window : Microsoft.UI.Xaml.Window
{
    private RainbowFrame rainbowFrame;
    private ModernSystemMenu modernSystemMenu;

    public delegate void WindowMessageEventHandler(object? sender, WindowMessageEventArgs e);
    public event WindowMessageEventHandler? WindowMessageReceived;
    public event WindowMessageEventHandler? WindowInputNonClientPointerSourceMessageReceived;

    public bool BringToFront() => WindowHelper.SetForegroundWindow(Hwnd);
    public void SwitchToThisWindow() => WindowHelper.SwitchToThisWindow(Hwnd);
    public void ReActivateWindow() => WindowHelper.ReActivateWindow(Hwnd);

    public Window()
    {
        var windowMessageMonitor = new WindowMessageMonitor(Hwnd);
        windowMessageMonitor.WindowMessageReceived += OnWindowMessageReceivedInternal;

        var inputNonClientPointerSourceHandle = WindowHelper.FindWindow(Hwnd, "InputNonClientPointerSource");
        if (inputNonClientPointerSourceHandle != IntPtr.Zero)
        {
            var monitorNonClient = new WindowMessageMonitor(inputNonClientPointerSourceHandle);
            monitorNonClient.WindowMessageReceived += OnWindowInputNonClientPointerSourceMessageReceivedInternal;
        }

        rainbowFrame = new RainbowFrame(Hwnd);
        modernSystemMenu = new ModernSystemMenu(this) { IsModernSystemMenuEnabled = false };

        AppWindow.SetIcon(Icon);

        Activated += Window_Activated;
    }

    private void OnWindowInputNonClientPointerSourceMessageReceivedInternal(object? sender, WindowMessageEventArgs e)
    {
        OnWindowInputNonClientPointerSourceMessageReceived(sender, e);
    }

    private void OnWindowMessageReceivedInternal(object? sender, WindowMessageEventArgs e)
    {
        OnWindowMessageReceived(sender, e);
    }

    protected virtual void OnWindowMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        WindowMessageReceived?.Invoke(sender, e);
    }
    protected virtual void OnWindowInputNonClientPointerSourceMessageReceived(object? sender, WindowMessageEventArgs e)
    {
        WindowInputNonClientPointerSourceMessageReceived?.Invoke(sender, e);
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        Activated -= Window_Activated;
        var grid = new Grid();
        if (TitleBar != null)
        {
            SetTitleBar(TitleBar);
            ExtendsContentIntoTitleBar = true;

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.Children.Add(TitleBar);

            Grid.SetRow(TitleBar, 0);

            if (Content is FrameworkElement element)
            {
                grid.Children.Add(element);
                Grid.SetRow(element, 1);
            }
            Content = grid;
        }
    }
}
