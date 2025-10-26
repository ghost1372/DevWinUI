namespace DevWinUI;

public partial class SegmentChangedEventArgs : EventArgs
{
    public int Index { get; }
    public SegmentedSliderSegment Segment { get; }

    public SegmentChangedEventArgs(int index, SegmentedSliderSegment segment)
    {
        Index = index;
        Segment = segment;
    }
}
