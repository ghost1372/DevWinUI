﻿using Windows.Foundation.Metadata;

namespace DevWinUI;

[Experimental]
[ContentProperty(Name = nameof(Content))]
public partial class ModernWindow : Microsoft.UI.Xaml.Window
{
    private RainbowFrame rainbowFrame;
    private ModernSystemMenu modernSystemMenu;
    private WindowMessageMonitor? mainMonitor;
    private WindowMessageMonitor? nonClientMonitor;
    public delegate void WindowMessageEventHandler(object? sender, WindowMessageEventArgs e);
    public event WindowMessageEventHandler? WindowMessageReceived;
    public event WindowMessageEventHandler? WindowInputNonClientPointerSourceMessageReceived;

    /// <summary>
    /// Brings the window to the front of the Z-order, making it the active window.
    /// </summary>
    /// <returns>Returns true if the window was successfully brought to the front, otherwise false.</returns>
    public bool BringToFront() => WindowHelper.SetForegroundWindow(Hwnd);

    /// <summary>
    /// Switches the focus to the current window.
    /// </summary>
    public void SwitchToThisWindow() => WindowHelper.SwitchToThisWindow(Hwnd);

    /// <summary>
    /// Reactivates window.
    /// </summary>
    public void ReActivateWindow() => WindowHelper.ReActivateWindow(Hwnd);

    public ModernWindow()
    {
        mainMonitor = new WindowMessageMonitor(Hwnd);
        mainMonitor.WindowMessageReceived += OnWindowMessageReceivedInternal;

        var inputNonClientPointerSourceHandle = WindowHelper.FindWindow(Hwnd, "InputNonClientPointerSource");
        if (inputNonClientPointerSourceHandle != IntPtr.Zero)
        {
            nonClientMonitor = new WindowMessageMonitor(inputNonClientPointerSourceHandle);
            nonClientMonitor.WindowMessageReceived += OnWindowInputNonClientPointerSourceMessageReceivedInternal;
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
