﻿using Microsoft.UI.Windowing;

namespace DevWinUI;
public partial class ModalWindow : Window
{
    private IntPtr _parentHwnd;

    public ModalWindow(IntPtr parentHwnd)
    {
        _parentHwnd = parentHwnd;
        var presenter = OverlappedPresenter.CreateForDialog();

        SetOwner(_parentHwnd);
        presenter.IsModal = true;
        AppWindow.SetPresenter(presenter);
        AppWindow.Show();
        Closed += ModalWindow_Closed;
    }

    private void ModalWindow_Closed(object sender, WindowEventArgs args)
    {
        ShowParentWindow();
    }

    private void SetOwner(IntPtr parentHwnd)
    {
        IntPtr childHwnd = Win32Interop.GetWindowFromWindowId(AppWindow.Id);
        NativeMethods.SetWindowLong(childHwnd, -8, parentHwnd);
    }

    public void ShowParentWindow()
    {
        var windowId = Win32Interop.GetWindowIdFromWindow(_parentHwnd);
        var parentAppWindow = AppWindow.GetFromWindowId(windowId);
        parentAppWindow.Show();
        WindowHelper.SwitchToThisWindow(_parentHwnd);
    }
}
