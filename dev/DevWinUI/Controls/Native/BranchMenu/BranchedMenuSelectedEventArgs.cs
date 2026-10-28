namespace DevWinUI;

public sealed partial class BranchedMenuSelectedEventArgs : EventArgs
{
    public BranchedMenuSelectedEventArgs(string value, object item)
    {
        Value = value;
        Item = item;
    }

    public string Value { get; }

    public object Item { get; }
}
