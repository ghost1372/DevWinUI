// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

internal class WindowStyleHelper
{
    public WindowStyleHelper(IntPtr hwnd)
    {
        _hwnd = hwnd;
    }

    private readonly IntPtr _hwnd;

    private bool _canMinimize;
    private bool _canMaximize;

    public bool CanMinimize
    {
        get => _canMinimize;
        set
        {
            if (_canMinimize == value)
                return;

            if (value)
            {
                EnableMinimize();
            }
            else
            {
                DisableMinimize();
            }
        }
    }

    public bool CanMaximize
    {
        get => _canMaximize;
        set
        {
            if (_canMaximize == value)
                return;

            if (value)
            {
                EnableMaximize();
            }
            else
            {
                DisableMaximize();
            }
        }
    }

    public nint EnableMinimize()
    {
        _canMinimize = true;
        nint style = NativeMethods.GetWindowLongPtr(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        style |= WindowStyles.WS_MINIMIZEBOX;
        return NativeMethods.SetWindowLong(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
    }

    public nint DisableMinimize()
    {
        _canMinimize = false;
        nint style = NativeMethods.GetWindowLongPtr(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        style &= ~WindowStyles.WS_MINIMIZEBOX;
        return NativeMethods.SetWindowLong(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
    }

    public nint EnableMaximize()
    {
        _canMaximize = true;
        nint style = NativeMethods.GetWindowLongPtr(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        style |= WindowStyles.WS_MAXIMIZEBOX;
        return NativeMethods.SetWindowLong(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
    }

    public nint DisableMaximize()
    {
        _canMaximize = false;
        nint style = NativeMethods.GetWindowLongPtr(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE);
        style &= ~WindowStyles.WS_MAXIMIZEBOX;
        return NativeMethods.SetWindowLong(_hwnd, (int)WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
    }
}
