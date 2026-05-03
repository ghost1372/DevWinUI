using Windows.Win32.UI.WindowsAndMessaging;
using static DevWinUI.NativeValues;

namespace DevWinUI;

public sealed partial class GlobalMouseHook : IDisposable
{
    public event EventHandler<MouseMoveEventArgs>? MouseMove;
    public event EventHandler<MouseClickEventArgs>? MouseClick;

    private Thread? _hookThread;
    private bool _running;
    private IntPtr _hookId = IntPtr.Zero;
    public HookLevel Level { get; set; } = HookLevel.Global;
    public IntPtr TargetWindowHandle { get; set; } = IntPtr.Zero;
    private uint _hookThreadId;
    private HookProc _HookProcedure;
    public GlobalMouseHook() => Init(IntPtr.Zero, HookLevel.Global);

    public GlobalMouseHook(IntPtr hwnd) => Init(hwnd, HookLevel.Application);

    private void Init(IntPtr hwnd, HookLevel level)
    {
        TargetWindowHandle = hwnd;
        this.Level = level;
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
    private void HookLoop()
    {
        _hookThreadId = PInvoke.GetCurrentThreadId();
        _HookProcedure = new HookProc(HookProcCallBack);

        using var process = System.Diagnostics.Process.GetCurrentProcess();
        using var module = process.MainModule!;

        var _hModule = module != null ? NativeMethods.GetModuleHandle(module.ModuleName) : IntPtr.Zero;
        _hookId = NativeMethods.SetWindowsHookEx((int)WINDOWS_HOOK_ID.WH_MOUSE_LL, _HookProcedure, _hModule, 0);

        MSG msg;
        while (PInvoke.GetMessage(out msg, HWND.Null, 0, 0))
        {
            // loop until WM_QUIT
        }
    }
    public int HookProcCallBack(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode < 0)
            return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);

        var data = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam)!;
        int x = data.pt.X;
        int y = data.pt.Y;

        if (Level == HookLevel.Application)
        {
            if (TargetWindowHandle == IntPtr.Zero)
                throw new NullReferenceException($"{nameof(TargetWindowHandle)} is null. Please ensure that the window handle (HWND) is properly set before proceeding.");

            var hwnd = PInvoke.WindowFromPoint(new System.Drawing.Point(x, y));
            hwnd = PInvoke.GetAncestor(hwnd, GET_ANCESTOR_FLAGS.GA_ROOT);
            if (hwnd != TargetWindowHandle)
                return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        switch ((uint)wParam)
        {
            case PInvoke.WM_MOUSEMOVE:
                MouseMove?.Invoke(this, new MouseMoveEventArgs(Level, x, y));
                break;
            case PInvoke.WM_LBUTTONDOWN:
                MouseClick?.Invoke(this, new MouseClickEventArgs(Level, MouseButton.Left, x, y));
                break;
            case PInvoke.WM_RBUTTONDOWN:
                MouseClick?.Invoke(this, new MouseClickEventArgs(Level, MouseButton.Right, x, y));
                break;
            case PInvoke.WM_MBUTTONDOWN:
                MouseClick?.Invoke(this, new MouseClickEventArgs(Level, MouseButton.Middle, x, y));
                break;
            case PInvoke.WM_MOUSEWHEEL:
                int delta = (short)((data.mouseData >> 16) & 0xffff);
                MouseClick?.Invoke(this, new MouseClickEventArgs(Level, MouseButton.Wheel, x, y, delta));
                break;
        }

        return NativeMethods.CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    public void Stop()
    {
        if (!_running) return;

        _running = false;

        if (_hookId != IntPtr.Zero)
        {
            PInvoke.UnhookWindowsHookEx(new HHOOK(_hookId));
            _hookId = IntPtr.Zero;
        }

        if (_hookThreadId != 0)
        {
            PInvoke.PostThreadMessage(_hookThreadId, PInvoke.WM_QUIT, UIntPtr.Zero, IntPtr.Zero);
            _hookThread?.Join();
            _hookThread = null;
            _hookThreadId = 0;
        }
    }

    public void Dispose() => Stop();
}
