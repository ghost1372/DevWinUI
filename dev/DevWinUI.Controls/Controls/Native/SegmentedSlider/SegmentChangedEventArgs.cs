namespace DevWinUI;

public partial class SegmentChangedEventArgs : EventArgs
{
    public int Index { get; }
    public SegmentedSliderSegment Segment { get; }
    public SegmentedSliderTimeInfo TimeInfo { get; }
    public SegmentChangedEventArgs(int index, SegmentedSliderSegment segment, SegmentedSliderTimeInfo timeInfo)
    {
        Index = index;
        Segment = segment;
        TimeInfo = timeInfo;
    }
}
