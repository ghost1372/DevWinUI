namespace DevWinUI;

public partial class SegmentEventArgs : EventArgs
{
    public SegmentEventArgs(string pattern)
    {
        Pattern = pattern;
    }

    public string Pattern { get; }
}
