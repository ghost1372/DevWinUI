using System.Runtime.InteropServices.Marshalling;

namespace DevWinUI;

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
