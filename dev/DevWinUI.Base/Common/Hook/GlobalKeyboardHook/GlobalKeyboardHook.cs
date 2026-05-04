using Windows.System;
using Windows.Win32.UI.WindowsAndMessaging;
using static DevWinUI.NativeValues;

namespace DevWinUI;

public sealed partial class GlobalKeyboardHook : IDisposable
{
    public event EventHandler<GlobalKeyEventArgs>? KeyDown;
    public event EventHandler<GlobalKeyEventArgs>? KeyUp;
    public event EventHandler<GlobalKeyEventArgs>? KeyPressed;

    private Thread? _thread;
    private uint _threadId;
    private bool _running;
    private IntPtr _hookId = IntPtr.Zero;
    private HookProc _HookProcedure;
    public void Start()
    {
        if (_running)
            return;

        _running = true;

        _thread = new Thread(ThreadProc)
        {
            IsBackground = true,
        };

        _thread.Start();
    }

    public void Stop()
    {
        if (!_running)
            return;

        _running = false;

        if (_hookId != IntPtr.Zero)
        {
            PInvoke.UnhookWindowsHookEx(new HHOOK(_hookId));
            _hookId = IntPtr.Zero;
        }

        if (_threadId != 0)
        {
            PInvoke.PostThreadMessage(_threadId, PInvoke.WM_QUIT, UIntPtr.Zero, IntPtr.Zero);
            _thread?.Join();
            _thread = null;
            _threadId = 0;
        }
    }

    private void ThreadProc()
    {
        _threadId = PInvoke.GetCurrentThreadId();
        _HookProcedure = new HookProc(HookProcCallBack);

        _hookId = NativeMethods.SetWindowsHookEx((int)WINDOWS_HOOK_ID.WH_KEYBOARD_LL, _HookProcedure, IntPtr.Zero, 0);

        if (_hookId == IntPtr.Zero)
            throw new InvalidOperationException("Failed to install keyboard hook.");

        MSG msg;
        while (PInvoke.GetMessage(out msg, HWND.Null, 0, 0))
        {
            PInvoke.TranslateMessage(msg);
            PInvoke.DispatchMessage(msg);
        }

        PInvoke.UnhookWindowsHookEx(new HHOOK(_hookId));
        _hookId = IntPtr.Zero;
    }

    private int HookProcCallBack(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);

        var data = System.Runtime.InteropServices.Marshal
               .PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

        var key = (VirtualKey)data.vkCode;

        var args = new GlobalKeyEventArgs(key);

        switch ((uint)wParam)
        {
            case PInvoke.WM_KEYDOWN:
            case PInvoke.WM_SYSKEYDOWN:
                KeyDown?.Invoke(this, args);
                KeyPressed?.Invoke(this, args);
                break;

            case PInvoke.WM_KEYUP:
            case PInvoke.WM_SYSKEYUP:
                KeyUp?.Invoke(this, args);
                break;
        }

        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        Stop();
    }
}
