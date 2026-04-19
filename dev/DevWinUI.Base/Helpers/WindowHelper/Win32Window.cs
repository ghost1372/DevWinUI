namespace DevWinUI;
public partial class Win32Window
{
    public Win32Window(IntPtr handle)
    {
        Handle = handle;
        unsafe
        {
            uint _processId;

            ThreadId = PInvoke.GetWindowThreadProcessId(new HWND(handle), &_processId);
            ProcessId = _processId;
        }
    }

    public IntPtr Handle { get; }
    public uint ThreadId { get; }
    public uint ProcessId { get; }
    public string ClassName => WindowHelper.GetClassName(Handle);
    public string Text => WindowHelper.GetWindowText(Handle);
    public bool IsEnabled => PInvoke.IsWindowEnabled(new HWND(Handle));

    public override string ToString()
    {
        var s = ClassName;
        var text = Text;
        if (text != null)
        {
            s += " '" + text + "'";
        }
        return s;
    }
}
