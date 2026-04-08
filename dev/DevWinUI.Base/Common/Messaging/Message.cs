namespace DevWinUI;

public struct Message
{
    internal Message(IntPtr hwnd, uint messageId, nuint wParam, IntPtr lParam)
    {
        Hwnd = hwnd;
        MessageId = messageId;
        WParam = wParam;
        LParam = lParam;
    }

    public IntPtr Hwnd { get; private set; }

    public uint MessageId { get; private set; }

    public nuint WParam { get; private set; }

    public nint LParam { get; private set; }

    internal int LowOrder => unchecked((short)LParam);

    internal int HighOrder => unchecked((short)((long)LParam >> 16));

    /// <inheritdoc />
    public override string ToString()
    {
        switch ((NativeValues.WindowMessage)MessageId)
        {
            case NativeValues.WindowMessage.WM_SIZING:
                string side = WParam switch
                {
                    1 => "Left",
                    2 => "Right",
                    3 => "Top",
                    4 => "Top-Left",
                    5 => "Top-Right",
                    6 => "Bottom",
                    7 => "Bottom-Left",
                    8 => "Bottom-Right",
                    _ => WParam.ToString(),
                };
                var rect = Marshal.PtrToStructure<RECT>((IntPtr)LParam);

                return $"WM_SIZING: Side: {side} Rect: {rect.left},{rect.top},{rect.right},{rect.bottom}";
            default:
                break;
        }
        return $"{(NativeValues.WindowMessage)MessageId}: LParam={LParam} WParam={WParam}";
    }
}
