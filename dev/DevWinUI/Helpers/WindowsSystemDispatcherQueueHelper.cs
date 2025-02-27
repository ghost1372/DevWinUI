namespace DevWinUI;

public class WindowsSystemDispatcherQueueHelper
{
    public IntPtr m_dispatcherQueueController = IntPtr.Zero;

    public void EnsureWindowsSystemDispatcherQueueController()
    {
        if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
        {
            return; // A DispatcherQueue already exists.
        }

        if (m_dispatcherQueueController == IntPtr.Zero)
        {
            NativeValues.DispatcherQueueOptions options;
            options.dwSize = Unsafe.SizeOf<NativeValues.DispatcherQueueOptions>();
            options.threadType = 2;    // DQTYPE_THREAD_CURRENT
            options.apartmentType = 2; // DQTAT_COM_STA

            unsafe
            {
                IntPtr dispatcherQueueController;
                NativeMethods.CreateDispatcherQueueController(options, &dispatcherQueueController);
                m_dispatcherQueueController = dispatcherQueueController;
            }
        }
    }
}
