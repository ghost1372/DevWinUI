namespace DevWinUI;

public sealed partial class WindowMessageEventArgs : EventArgs
{
    internal WindowMessageEventArgs(IntPtr hwnd, uint messageId, nuint wParam, nint lParam)
    {
        Message = new Message(hwnd, messageId, wParam, lParam);
    }

    public nint Result { get; set; }

    public bool Handled { get; set; }

    public Message Message { get; }

    public uint MessageType => Message.MessageId;
}
