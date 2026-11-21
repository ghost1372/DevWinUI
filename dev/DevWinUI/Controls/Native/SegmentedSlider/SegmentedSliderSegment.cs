using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public partial class SegmentedSliderSegment
{
    public Rectangle TrackRectangle { get; set; } = null!;
    public Rectangle FillSegmentRectangle { get; set; } = null!;
    public TextBlock TitleTextBlock { get; set; } = null!;
}
