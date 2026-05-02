using Windows.Win32.UI.WindowsAndMessaging;

namespace DevWinUI;

public sealed partial class GlobalMouseHook : IDisposable
{
    public event EventHandler<MouseMoveEventArgs>? MouseMove;
    public event EventHandler<MouseClickEventArgs>? MouseClick;

    private Thread? _hookThread;
    private bool _running;
    private FreeLibrarySafeHandle hModule;
    private UnhookWindowsHookExSafeHandle? _hookId;
    private static GlobalMouseHook Current { get; set; }
    public bool IsAppLevelHook { get; set; } = false;
    public IntPtr TargetWindowHwnd { get; set; } = IntPtr.Zero;

    public GlobalMouseHook()
    {
        TargetWindowHwnd = IntPtr.Zero;
        IsAppLevelHook = false;
        Init();
    }

    public GlobalMouseHook(IntPtr hwnd)
    {
        TargetWindowHwnd = hwnd;
        IsAppLevelHook = true;
        Init();
    }

    private void Init()
    {
        if (Current == null)
        {
            Current = this;
        }
    }

    public void Start()
    {
        if (_running) return;

        _running = true;

        _hookThread = new Thread(HookLoop)
        {
            IsBackground = true
        };

        _hookThread.Start();
    }


    private unsafe void HookLoop()
    {
        delegate* unmanaged[Stdcall]<int, WPARAM, LPARAM, LRESULT> callback = &HookCallback;

        using var process = System.Diagnostics.Process.GetCurrentProcess();
        using var module = process.MainModule!;

        hModule = PInvoke.GetModuleHandle(module.ModuleName);

        _hookId = PInvoke.SetWindowsHookEx(WINDOWS_HOOK_ID.WH_MOUSE_LL, callback, hModule, 0);

        // Message loop
        while (PInvoke.GetMessage(out MSG msg, HWND.Null, 0, 0))
        {
            // no dispatch needed for hook-only loop
        }

    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
    private static LRESULT HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
    {
        if (nCode >= 0)
        {
            var data = System.Runtime.InteropServices.Marshal
                .PtrToStructure<MSLLHOOKSTRUCT>(lParam);

            int x = data.pt.X;
            int y = data.pt.Y;

            if (Current.IsAppLevelHook)
            {
                var hwnd = PInvoke.WindowFromPoint(new System.Drawing.Point(x, y));

                hwnd = PInvoke.GetAncestor(hwnd, GET_ANCESTOR_FLAGS.GA_ROOT);

                if (hwnd != Current?.TargetWindowHwnd)
                {
                    return PInvoke.CallNextHookEx(Current?._hookId, nCode, wParam, lParam);
                }
            }

            var hookLevel = Current.IsAppLevelHook ? HookLevel.Application : HookLevel.Global;
            switch ((uint)wParam)
            {
                case PInvoke.WM_MOUSEMOVE:
                    Current?.MouseMove?.Invoke(Current, new MouseMoveEventArgs(hookLevel, x, y));
                    break;

                case PInvoke.WM_LBUTTONDOWN:
                    Current?.MouseClick?.Invoke(Current, new MouseClickEventArgs(hookLevel, MouseButton.Left, x, y));
                    break;

                case PInvoke.WM_RBUTTONDOWN:
                    Current?.MouseClick?.Invoke(Current, new MouseClickEventArgs(hookLevel, MouseButton.Right, x, y));
                    break;

                case PInvoke.WM_MBUTTONDOWN:
                    Current?.MouseClick?.Invoke(Current, new MouseClickEventArgs(hookLevel, MouseButton.Middle, x, y));
                    break;

                case PInvoke.WM_MOUSEWHEEL:
                    int delta = (short)((data.mouseData >> 16) & 0xffff);
                    Current?.MouseClick?.Invoke(Current, new MouseClickEventArgs(hookLevel, MouseButton.Wheel, x, y, delta));
                    break;
            }

        }

        return PInvoke.CallNextHookEx(Current?._hookId, nCode, wParam, lParam);
    }
    public void Stop()
    {
        if (!_running) return;

        _running = false;

        if (_hookId is not null && !_hookId.IsInvalid)
        {
            _hookId.Dispose();
            _hookId = null;
        }
    }

    public void Dispose()
    {
        Stop();
    }
}
