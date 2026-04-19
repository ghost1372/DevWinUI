namespace DevWinUI;
public partial class PinBoxPasswordChangedEventArgs : EventArgs
{
    public string OldValue { get; }
    public string NewValue { get; }

    public PinBoxPasswordChangedEventArgs(string oldValue, string newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
