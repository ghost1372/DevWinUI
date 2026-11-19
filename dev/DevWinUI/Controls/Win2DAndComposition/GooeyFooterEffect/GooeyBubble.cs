// https://github.com/DinoChan

namespace DevWinUI;

public partial class GooeyBubble
{
    public required DoubleTimeline OffsetTimeline { get; set; }
    public required DoubleTimeline SizeTimeline { get; set; }
    public double X { get; set; }
}
