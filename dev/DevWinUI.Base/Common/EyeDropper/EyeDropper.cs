using Microsoft.UI.Dispatching;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

public sealed partial class EyeDropper
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    private GlobalMouseHook hook = new GlobalMouseHook();
    private EyeDropperPreviewWindow previewWindow = new EyeDropperPreviewWindow();

    public event EventHandler<Color> ColorPicked;

    private Color selectedColor;
    public void Start()
    {
        PInvoke.GetCursorPos(out System.Drawing.Point mousePosition);

        previewWindow.Activate();

        MovePreviewWindowToMouse(mousePosition.X, mousePosition.Y);

        hook.StatusChanged -= Hook_StatusChanged;
        hook.StatusChanged += Hook_StatusChanged;

        hook.Start();
    }

    private void Hook_StatusChanged(object? sender, MouseHookEventArgs e)
    {
        if (e.MessageType == MouseHookMessageType.MouseMove)
        {
            selectedColor = ColorHelper.GetColorAt(e.X, e.Y);
            dispatcherQueue.TryEnqueue(() =>
            {
                previewWindow.UpdateColor(selectedColor);
                MovePreviewWindowToMouse(e.X, e.Y);
            });
        }

        if (e.MessageType == MouseHookMessageType.LeftButtonDown)
        {
            ColorPicked?.Invoke(this, selectedColor);
            Stop();
        }
    }

    public void Stop()
    {
        dispatcherQueue.TryEnqueue(() =>
        {
            hook.Stop();

            hook.StatusChanged -= Hook_StatusChanged;

            previewWindow.Close();
        });
    }

    private void MovePreviewWindowToMouse(int mouseX, int mouseY)
    {
        var currentStyle = NativeMethods.GetWindowLongPtr(previewWindow.hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        currentStyle &= -12845057;
        PInvoke.SetWindowLong(new HWND(previewWindow.hwnd), WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)currentStyle);
        PInvoke.SetWindowPos(new HWND(previewWindow.hwnd), new HWND(IntPtr.Zero), mouseX, mouseY, 0, 0, SET_WINDOW_POS_FLAGS.SWP_DRAWFRAME | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
    }
}
