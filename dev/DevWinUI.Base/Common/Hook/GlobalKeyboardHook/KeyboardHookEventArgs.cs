using Windows.System;

namespace DevWinUI;

public sealed partial class KeyboardHookEventArgs : EventArgs
{
    const int KEY_DOWN_MASK = 0x8000;
    public bool IsKeyDown(VirtualKey key)
    {
        return (PInvoke.GetKeyState((int)key) & KEY_DOWN_MASK) != 0;
    }

    public bool IsShift =>
        IsKeyDown(VirtualKey.Shift) ||
        IsKeyDown(VirtualKey.LeftShift) ||
        IsKeyDown(VirtualKey.RightShift);

    public bool IsCtrl =>
        IsKeyDown(VirtualKey.Control) ||
        IsKeyDown(VirtualKey.LeftControl) ||
        IsKeyDown(VirtualKey.RightControl);

    public bool IsAlt =>
        IsKeyDown(VirtualKey.Menu) ||
        IsKeyDown(VirtualKey.LeftMenu) ||
        IsKeyDown(VirtualKey.RightMenu);

    public bool IsWindows =>
        IsKeyDown(VirtualKey.LeftWindows) ||
        IsKeyDown(VirtualKey.RightWindows);
    public VirtualKey Key { get; }

    public KeyboardHookEventArgs(VirtualKey key)
    {
        Key = key;
    }
}
