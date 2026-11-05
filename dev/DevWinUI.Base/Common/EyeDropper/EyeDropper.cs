using Microsoft.UI.Dispatching;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

public sealed partial class EyeDropper
{
    private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

    private GlobalMouseHook hook = new GlobalMouseHook();
    private EyeDropperPreviewWindow? previewWindow;

    public event EventHandler<Color> ColorPicked;

    private Color selectedColor;
    private int mouseX;
    private int mouseY;
    private int isUpdateQueued;
    private volatile bool isRunning;

    private bool isPreviewFrameRemoved;

    public void Start()
    {
        if (isRunning)
        {
            return;
        }

        isRunning = true;

        PInvoke.GetCursorPos(out System.Drawing.Point mousePosition);

        previewWindow = new EyeDropperPreviewWindow();
        RemovePreviewWindowFrame(previewWindow.hwnd);

        UpdatePreview(mousePosition.X, mousePosition.Y);

        previewWindow.Activate();

        hook.StatusChanged -= Hook_StatusChanged;
        hook.StatusChanged += Hook_StatusChanged;

        hook.Start();
    }
    private void Hook_StatusChanged(object? sender, MouseHookEventArgs e)
    {
        if (e.MessageType == MouseHookMessageType.MouseMove)
        {
            mouseX = e.X;
            mouseY = e.Y;

            if (Interlocked.Exchange(ref isUpdateQueued, 1) == 0 && !dispatcherQueue.TryEnqueue(ProcessMouseMove))
            {
                Volatile.Write(ref isUpdateQueued, 0);
            }
        }
        else if (e.MessageType == MouseHookMessageType.LeftButtonDown)
        {
            dispatcherQueue.TryEnqueue(() =>
            {
                if (!isRunning)
                {
                    return;
                }

                selectedColor = ColorHelper.GetColorAt(e.X, e.Y);
                StopCore();
                ColorPicked?.Invoke(this, selectedColor);
            });
        }
    }

    public void Stop()
    {
        dispatcherQueue.TryEnqueue(StopCore);
    }

    private void StopCore()
    {
        if (!isRunning)
        {
            return;
        }

        isRunning = false;

        hook.Stop();

        hook.StatusChanged -= Hook_StatusChanged;

        previewWindow?.Close();
        previewWindow = null;
    }

    private void ProcessMouseMove()
    {
        Volatile.Write(ref isUpdateQueued, 0);

        if (isRunning)
        {
            UpdatePreview(mouseX, mouseY);
        }
    }

    private void UpdatePreview(int mouseX, int mouseY)
    {
        if (previewWindow is null)
        {
            return;
        }

        selectedColor = ColorHelper.GetColorAt(mouseX, mouseY);
        previewWindow.UpdateColor(selectedColor);
        MovePreviewWindowToMouse(previewWindow.hwnd, mouseX, mouseY);
    }

    private static void RemovePreviewWindowFrame(IntPtr hwnd)
    {
        var currentStyle = NativeMethods.GetWindowLongPtr(hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        currentStyle &= -12845057;
        PInvoke.SetWindowLong(new HWND(hwnd), WINDOW_LONG_PTR_INDEX.GWL_STYLE, (int)currentStyle);
        PInvoke.SetWindowPos(new HWND(hwnd), new HWND(IntPtr.Zero), 0, 0, 0, 0, SET_WINDOW_POS_FLAGS.SWP_DRAWFRAME | SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
    }

    private static void MovePreviewWindowToMouse(IntPtr hwnd, int mouseX, int mouseY)
    {
        PInvoke.SetWindowPos(new HWND(hwnd), new HWND(IntPtr.Zero), mouseX, mouseY, 0, 0, SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
    }
}
