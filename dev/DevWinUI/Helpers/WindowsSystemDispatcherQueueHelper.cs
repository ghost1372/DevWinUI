namespace DevWinUI;

public class WindowsSystemDispatcherQueueHelper
{
    private Windows.System.DispatcherQueueController? m_dispatcherQueueController;

    public void EnsureWindowsSystemDispatcherQueueController()
    {
        if (Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread() != null)
        {
            return; // A DispatcherQueue already exists.
        }

        if (m_dispatcherQueueController == null)
        {
            Windows.Win32.System.WinRT.DispatcherQueueOptions options;
            options.dwSize = (uint)Marshal.SizeOf<Windows.Win32.System.WinRT.DispatcherQueueOptions>();
            options.threadType = Windows.Win32.System.WinRT.DISPATCHERQUEUE_THREAD_TYPE.DQTYPE_THREAD_CURRENT;    // DQTYPE_THREAD_CURRENT
            options.apartmentType = Windows.Win32.System.WinRT.DISPATCHERQUEUE_THREAD_APARTMENTTYPE.DQTAT_COM_STA; // DQTAT_COM_STA

            PInvoke.CreateDispatcherQueueController(options, out m_dispatcherQueueController);
        }
    }
}
