namespace DevWinUI;

public partial class MouseHookEventArgs : EventArgs
{
    public HookLevel HookLevel { get; }
    public MouseHookMessageType MessageType { get; }
    public int Delta { get; }
    public int X { get; }
    public int Y { get; }
    public uint Message { get; }

    public MouseHookEventArgs(HookLevel hookLevel, MouseHookMessageType messageType, int x, int y, int delta, uint message)
    {
        HookLevel = hookLevel;
        MessageType = messageType;
        X = x;
        Y = y;
        Delta = delta;
        Message = message;
    }
}
