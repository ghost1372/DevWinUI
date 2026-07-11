using System;
using System.Collections.Generic;
using System.Text;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

public static partial class SingleInstanceWindowActivator
{
    private static Window? _window;

    public static void Register(Window window)
    {
        _window = window;
    }

    public static void Activate()
    {
        if (_window == null)
            return;

        _window.DispatcherQueue.TryEnqueue(() =>
        {
            var hwnd = WindowNative.GetWindowHandle(_window);

            var appWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(hwnd));

            appWindow?.Show();

            if (appWindow?.Presenter is OverlappedPresenter presenter)
            {
                presenter.Restore();
            }

            PInvoke.ShowWindow(new HWND(hwnd), SHOW_WINDOW_CMD.SW_RESTORE);

            PInvoke.SetForegroundWindow(new HWND(hwnd));
        });
    }
}
