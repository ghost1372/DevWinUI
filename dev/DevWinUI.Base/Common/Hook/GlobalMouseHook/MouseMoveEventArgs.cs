namespace DevWinUI;

public partial class MouseMoveEventArgs : EventArgs
{
    public HookLevel HookLevel { get; }
    public int X { get; }
    public int Y { get; }

    public MouseMoveEventArgs(HookLevel hookLevel, int x, int y)
    {
        HookLevel = hookLevel;
        X = x;
        Y = y;
    }
}
