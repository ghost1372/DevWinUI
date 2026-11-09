namespace DevWinUI;

public partial class RoadmapLane : ItemsControl
{
    public RoadmapLane()
    {
        DefaultStyleKey = typeof(RoadmapLane);
    }

    protected override bool IsItemItsOwnContainerOverride(object item)
    {
        return item is RoadmapSegment;
    }

    protected override DependencyObject GetContainerForItemOverride()
    {
        return new RoadmapSegment();
    }

    public object Header
    {
        get { return GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(RoadmapLane), new PropertyMetadata(null));

    public double HeaderWidth
    {
        get { return (double)GetValue(HeaderWidthProperty); }
        set { SetValue(HeaderWidthProperty, value); }
    }

    public static readonly DependencyProperty HeaderWidthProperty =
        DependencyProperty.Register(nameof(HeaderWidth), typeof(double), typeof(RoadmapLane), new PropertyMetadata(120d));
}
