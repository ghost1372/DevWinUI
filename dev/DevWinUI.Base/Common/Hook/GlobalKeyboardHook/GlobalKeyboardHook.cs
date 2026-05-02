using Windows.System;
using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

public sealed partial class GlobalKeyboardHook : IDisposable
{
    public event EventHandler<GlobalKeyEventArgs>? KeyDown;
    public event EventHandler<GlobalKeyEventArgs>? KeyUp;
    public event EventHandler<GlobalKeyEventArgs>? KeyPressed;

    private Thread? _thread;
    private HHOOK _hook;
    private uint _threadId;
    private bool _running;
    private UnhookWindowsHookExSafeHandle? _hookId;
    private static GlobalKeyboardHook Current { get; set; }

    public GlobalKeyboardHook()
    {
        if (Current == null)
        {
            Current = this;
        }
    }

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

        if (_threadId != 0)
        {
            PInvoke.PostThreadMessage(_threadId, PInvoke.WM_QUIT, 0, 0);
        }

        _thread?.Join();
        _thread = null;
    }

    private unsafe void ThreadProc()
    {
        _threadId = PInvoke.GetCurrentThreadId();

        delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT> callback = &HookCallback;

        _hook = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_KEYBOARD_LL, callback, HINSTANCE.Null, 0);

        _hookId = new UnhookWindowsHookExSafeHandle(_hook);

        if (_hook.IsNull)
            throw new InvalidOperationException("Failed to install keyboard hook.");

        MSG msg;
        while (PInvoke.GetMessage(out msg, HWND.Null, 0, 0))
        {
            PInvoke.TranslateMessage(msg);
            PInvoke.DispatchMessage(msg);
        }

        PInvoke.UnhookWindowsHookEx(_hook);
        _hook = default;
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static LRESULT HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode >= 0)
        {
            var data = System.Runtime.InteropServices.Marshal
               .PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

            var key = (VirtualKey)data.vkCode;

            var args = new GlobalKeyEventArgs(key);

            switch ((uint)wParam.Value)
            {
                case PInvoke.WM_KEYDOWN:
                case PInvoke.WM_SYSKEYDOWN:
                    Current?.KeyDown?.Invoke(Current, args);
                    Current?.KeyPressed?.Invoke(Current, args);
                    break;

                case PInvoke.WM_KEYUP:
                case PInvoke.WM_SYSKEYUP:
                    Current?.KeyUp?.Invoke(Current, args);
                    break;
            }
        }

        return PInvoke.CallNextHookEx(Current?._hookId, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        Stop();
    }
}
