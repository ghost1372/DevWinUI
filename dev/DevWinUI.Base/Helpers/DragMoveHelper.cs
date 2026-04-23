using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class DragMoveHelper
{
    private IntPtr hwnd = IntPtr.Zero;
    private Microsoft.UI.Windowing.AppWindow appWindow;
    private bool bMoving = false;
    private int nX = 0;
    private int nY = 0;
    private int nXWindow = 0;
    private int nYWindow = 0;
    public DragMoveHelper(Window window) : this(WindowNative.GetWindowHandle(window)) { }
    public DragMoveHelper(IntPtr hwnd)
    {
        this.hwnd = hwnd;
        Microsoft.UI.WindowId myWndId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(this.hwnd);
        appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(myWndId);
    }

    public void SetDragMove(FrameworkElement element)
    {

        element.PointerMoved -= Root_PointerMoved;
        element.PointerMoved += Root_PointerMoved;

        element.PointerPressed -= Root_PointerPressed;
        element.PointerPressed += Root_PointerPressed;

        element.PointerReleased -= Root_PointerReleased;
        element.PointerReleased += Root_PointerReleased;
    }
    public void UnSetDragMove(FrameworkElement element)
    {
        element.PointerMoved -= Root_PointerMoved;
        element.PointerPressed -= Root_PointerPressed;
        element.PointerReleased -= Root_PointerReleased;
    }
    private void Root_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        ((UIElement)sender).ReleasePointerCaptures();
        bMoving = false;
    }

    private void Root_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint((UIElement)sender).Properties;
        if (properties.IsLeftButtonPressed)
        {
            ((UIElement)sender).CapturePointer(e.Pointer);
            nXWindow = appWindow.Position.X;
            nYWindow = appWindow.Position.Y;
            System.Drawing.Point pt;
            PInvoke.GetCursorPos(out pt);
            nX = pt.X;
            nY = pt.Y;
            bMoving = true;
        }
    }

    private void Root_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        var properties = e.GetCurrentPoint((UIElement)sender).Properties;
        if (properties.IsLeftButtonPressed)
        {
            System.Drawing.Point pt;
            PInvoke.GetCursorPos(out pt);

            if (bMoving)
                appWindow.Move(new Windows.Graphics.PointInt32(nXWindow + (pt.X - nX), nYWindow + (pt.Y - nY)));

            e.Handled = true;
        }
    }
}

