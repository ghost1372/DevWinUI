using System.Runtime.InteropServices.Marshalling;

namespace DevWinUI;

public enum TaskbarStates
{
    NoProgress = 0,
    Indeterminate = 0x1,
    Normal = 0x2,
    Error = 0x4,
    Paused = 0x8
}

public static partial class TaskbarHelper
{
    private static readonly Guid CLSID_TaskbarList = new("56fdf344-fd6d-11d0-958a-006097c9a090");
    private static readonly Guid IID_ITaskbarList3 = new("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf");

    [Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [GeneratedComInterface]
    internal partial interface ITaskbarList3
    {
        // ITaskbarList
        [PreserveSig]
        void HrInit();
        [PreserveSig]
        void AddTab(IntPtr hwnd);
        [PreserveSig]
        void DeleteTab(IntPtr hwnd);
        [PreserveSig]
        void ActivateTab(IntPtr hwnd);
        [PreserveSig]
        void SetActiveAlt(IntPtr hwnd);

        // ITaskbarList2
        [PreserveSig]
        void MarkFullscreenWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.Bool)] bool fFullscreen);

        // ITaskbarList3
        [PreserveSig]
        void SetProgressValue(IntPtr hwnd, ulong ullCompleted, ulong ullTotal);
        [PreserveSig]
        void SetProgressState(IntPtr hwnd, TaskbarStates state);
    }

    private static readonly ITaskbarList3 taskbarInstance;
    private static readonly bool taskbarSupported = Environment.OSVersion.Version >= new Version(6, 1);

    static TaskbarHelper()
    {
        if (taskbarSupported)
        {
            unsafe
            {
                void* ppv;
                HRESULT hr = PInvoke.CoCreateInstance(
                    in CLSID_TaskbarList,
                    null,
                    Windows.Win32.System.Com.CLSCTX.CLSCTX_INPROC_SERVER,
                    in IID_ITaskbarList3,
                    out ppv);

                if (hr.Succeeded && ppv != null)
                {
                    taskbarInstance = (ITaskbarList3)new StrategyBasedComWrappers().GetOrCreateObjectForComInstance((IntPtr)ppv, CreateObjectFlags.None);
                    Marshal.Release((IntPtr)ppv);
                    taskbarInstance.HrInit();
                }
            }
        }
    }

    /// <summary>
    /// Sets the progress state of a specified window in the taskbar if supported.
    /// </summary>
    /// <param name="windowHandle">Identifies the window for which the progress state is being set.</param>
    /// <param name="taskbarState">Specifies the desired progress state to be applied to the window.</param>
    public static void SetProgressState(IntPtr windowHandle, TaskbarStates taskbarState)
    {
        if (taskbarSupported) taskbarInstance.SetProgressState(windowHandle, taskbarState);
    }

    /// <summary>
    /// Sets the progress value of a taskbar item for a specified window. It updates the visual representation of
    /// progress in the taskbar.
    /// </summary>
    /// <param name="windowHandle">Identifies the window for which the taskbar progress is being set.</param>
    /// <param name="progressValue">Represents the current progress value to be displayed in the taskbar.</param>
    /// <param name="progressMax">Indicates the maximum value of progress to determine the completion percentage.</param>
    public static void SetProgressValue(IntPtr windowHandle, double progressValue, double progressMax)
    {
        if (taskbarSupported) taskbarInstance.SetProgressValue(windowHandle, (ulong)progressValue, (ulong)progressMax);
    }
}
