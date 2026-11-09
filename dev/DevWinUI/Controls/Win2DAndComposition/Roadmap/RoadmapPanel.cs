namespace DevWinUI;

public sealed partial class RoadmapPanel : Panel
{
    private double _topExtent;
    private double _bottomExtent;

    internal double AxisY { get; private set; }
    internal double AxisStartX { get; private set; }
    internal double AxisEndX { get; private set; }
    internal double ZoneAbove { get; private set; }
    internal double ZoneBelow { get; private set; }

    protected override Size MeasureOverride(Size availableSize)
    {
        Roadmap? owner = ItemsControl.GetItemsOwner(this) as Roadmap;
        if (owner is null || Children.Count == 0)
        {
            return new Size(0, 0);
        }

        owner.HostPanel = this;

        RoadmapMetrics m = owner.GetMetrics();
        RoadmapMode mode = owner.Mode;
        double itemWidth = m.ItemWidth;
        double r = m.MarkerSize / 2;
        int count = Children.Count;

        ZoneAbove = 0;
        ZoneBelow = 0;

        for (int i = 0; i < count; i++)
        {
            if (Children[i] is not RoadmapItem item)
            {
                continue;
            }

            item.Measure(new Size(itemWidth, double.PositiveInfinity));
            item.LayoutSide = ResolveSide(owner, item, i);

            if (item.LayoutSide < 0)
            {
                ZoneAbove = Math.Max(ZoneAbove, item.DesiredSize.Height);
            }
            else
            {
                ZoneBelow = Math.Max(ZoneBelow, item.DesiredSize.Height);
            }
        }

        double up = r;
        double down = r;

        for (int i = 0; i < count; i++)
        {
            if (Children[i] is not RoadmapItem item)
            {
                continue;
            }

            double h = item.DesiredSize.Height;
            double near;
            double opposite;

            switch (mode)
            {
                case RoadmapMode.Simple:
                    near = m.StemLength + Roadmap.EndDotRadius + Roadmap.ContentGap + h;
                    opposite = m.LabelOffset + m.LabelHeight / 2;
                    break;

                case RoadmapMode.Boxed:
                case RoadmapMode.Neon:
                    near = m.StemLength + h;
                    opposite = m.LabelOffset + m.LabelHeight / 2;
                    break;

                default:
                    double zone = item.LayoutSide < 0 ? ZoneAbove : ZoneBelow;
                    near = r + m.StemLength + zone;
                    if (!string.IsNullOrEmpty(item.Label))
                    {
                        near += m.SecondStemLength + m.BadgeHeight;
                    }
                    opposite = r;
                    break;
            }

            if (item.LayoutSide < 0)
            {
                up = Math.Max(up, near);
                down = Math.Max(down, opposite);
            }
            else
            {
                down = Math.Max(down, near);
                up = Math.Max(up, opposite);
            }
        }

        _topExtent = up + Roadmap.VerticalMargin;
        _bottomExtent = down + Roadmap.VerticalMargin;

        double pad = Roadmap.AxisInset + itemWidth / 2;
        double minPitch = (itemWidth + Roadmap.ItemGap) / (owner.AlternatePlacement ? 2 : 1);
        if (mode == RoadmapMode.Icon)
        {
            minPitch = Math.Max(minPitch, m.MarkerSize + 12);
        }

        double usableMin = Math.Max(0, count - 1) * minPitch;
        if (owner.Distribution == RoadmapDistribution.Proportional)
        {
            usableMin = Math.Max(usableMin, GetProportionalUsableWidth(itemWidth, m.MarkerSize));
        }

        double minWidth = 2 * pad + usableMin;
        double width = double.IsInfinity(availableSize.Width) ? minWidth : Math.Max(availableSize.Width, minWidth);

        return new Size(width, _topExtent + _bottomExtent);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        Roadmap? owner = ItemsControl.GetItemsOwner(this) as Roadmap;
        if (owner is null || Children.Count == 0)
        {
            return finalSize;
        }

        RoadmapMetrics m = owner.GetMetrics();
        RoadmapMode mode = owner.Mode;
        double itemWidth = m.ItemWidth;
        double r = m.MarkerSize / 2;
        int count = Children.Count;

        double offsetY = Math.Max(0, (finalSize.Height - (_topExtent + _bottomExtent)) / 2);
        AxisY = offsetY + _topExtent;
        AxisStartX = Roadmap.AxisInset;
        AxisEndX = finalSize.Width - Roadmap.AxisInset;

        double pad = Roadmap.AxisInset + itemWidth / 2;
        double usable = Math.Max(0, finalSize.Width - 2 * pad);

        bool proportional = owner.Distribution == RoadmapDistribution.Proportional;
        double vMin = double.MaxValue;
        double vMax = double.MinValue;

        if (proportional)
        {
            for (int i = 0; i < count; i++)
            {
                if (Children[i] is RoadmapItem item && !double.IsNaN(item.Value))
                {
                    vMin = Math.Min(vMin, item.Value);
                    vMax = Math.Max(vMax, item.Value);
                }
            }

            proportional = vMin < vMax;
        }

        for (int i = 0; i < count; i++)
        {
            if (Children[i] is not RoadmapItem item)
            {
                continue;
            }

            double x;
            if (proportional && !double.IsNaN(item.Value))
            {
                x = pad + (item.Value - vMin) / (vMax - vMin) * usable;
            }
            else
            {
                x = count > 1 ? pad + usable * i / (count - 1) : pad + usable / 2;
            }

            double h = item.DesiredSize.Height;
            int side = item.LayoutSide;
            double nearest;

            switch (mode)
            {
                case RoadmapMode.Simple:
                    nearest = m.StemLength + Roadmap.EndDotRadius + Roadmap.ContentGap;
                    break;

                case RoadmapMode.Boxed:
                case RoadmapMode.Neon:
                    nearest = m.StemLength;
                    break;

                default:
                    double zone = side < 0 ? ZoneAbove : ZoneBelow;
                    nearest = r + m.StemLength + (zone - h) / 2;
                    break;
            }

            double top = side < 0 ? AxisY - nearest - h : AxisY + nearest;
            Rect bounds = new Rect(x - itemWidth / 2, top, itemWidth, h);

            item.Arrange(bounds);
            item.LayoutCenterX = x;
            item.LayoutBounds = bounds;
        }

        owner.InvalidateCanvas();
        return finalSize;
    }

    private double GetProportionalUsableWidth(double itemWidth, double markerSize)
    {
        List<(double Value, int Side)> entries = new();

        foreach (UIElement child in Children)
        {
            if (child is RoadmapItem item && !double.IsNaN(item.Value))
            {
                entries.Add((item.Value, item.LayoutSide));
            }
        }

        if (entries.Count < 2)
        {
            return 0;
        }

        entries.Sort((a, b) => a.Value.CompareTo(b.Value));

        double range = entries[^1].Value - entries[0].Value;
        if (range <= 0)
        {
            return 0;
        }

        double sameSideGap = itemWidth + Roadmap.ItemGap;
        double markerGap = markerSize + 4;
        double[] last = { double.NaN, double.NaN };
        double scale = 0;

        for (int i = 0; i < entries.Count; i++)
        {
            if (i > 0)
            {
                double delta = entries[i].Value - entries[i - 1].Value;
                if (delta > 0)
                {
                    scale = Math.Max(scale, markerGap / delta);
                }
            }

            int slot = entries[i].Side < 0 ? 0 : 1;
            if (!double.IsNaN(last[slot]))
            {
                double delta = entries[i].Value - last[slot];
                if (delta > 0)
                {
                    scale = Math.Max(scale, sameSideGap / delta);
                }
            }

            last[slot] = entries[i].Value;
        }

        return Math.Min(scale * range, 20000);
    }

    private static int ResolveSide(Roadmap owner, RoadmapItem item, int index)
    {
        switch (item.Placement)
        {
            case RoadmapPlacement.Above:
                return -1;
            case RoadmapPlacement.Below:
                return 1;
        }

        bool firstAbove = owner.FirstItemPlacement != RoadmapPlacement.Below;
        bool above = owner.AlternatePlacement ? (index % 2 == 0) == firstAbove : firstAbove;
        return above ? -1 : 1;
    }
}
