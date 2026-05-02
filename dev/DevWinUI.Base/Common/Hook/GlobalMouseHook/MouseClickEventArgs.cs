namespace DevWinUI;

public partial class MouseClickEventArgs : EventArgs
{
    public HookLevel HookLevel { get; }
    public MouseButton Button { get; }
    public int X { get; }
    public int Y { get; }
    public int Delta { get; }

    public MouseClickEventArgs(HookLevel hookLevel, MouseButton button, int x, int y, int delta = 0)
    {
        HookLevel = hookLevel;
        Button = button;
        X = x;
        Y = y;
        Delta = delta;
    }
}
