namespace DevWinUI;

public class WindowsSystemDispatcherQueueHelper
{
    public IntPtr m_dispatcherQueueController = IntPtr.Zero;

    /// <summary>
    /// Ensures a Windows system dispatcher queue controller is created if one does not already exist for the current
    /// thread. Initializes the controller with specific options if necessary.
    /// </summary>
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
