namespace DevWinUI;

public partial class HorizontalSimpleGridLayout : VirtualizingLayout
{
    public double LayoutWidth { get; private set; }
    public List<GridLength> ColumnWidths { get; } = new();

    public double Spacing
    {
        get { return (double)GetValue(SpacingProperty); }
        set { SetValue(SpacingProperty, value); }
    }

    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(HorizontalSimpleGridLayout), new PropertyMetadata(0.0, OnSpacingChanged));
    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is HorizontalSimpleGridLayout layout)
        {
            layout.InvalidateMeasure();
        }
    }

    protected override void UninitializeForContextCore(VirtualizingLayoutContext context)
    {
        base.UninitializeForContextCore(context);
        context.LayoutState = null;
    }

    protected override Size MeasureOverride(VirtualizingLayoutContext context, Size availableSize)
    {
        int itemCount = context.ItemCount;
        if (itemCount == 0)
        {
            LayoutWidth = 0;
            return new Size(0, 0);
        }

        var widths = ResolveColumnWidths(availableSize.Width - (Spacing * (itemCount - 1)), itemCount);
        double x = 0;
        double maxHeight = 0;

        for (int i = 0; i < itemCount; i++)
        {
            var element = context.GetOrCreateElementAt(i);
            double width = widths[i];
            element.Measure(new Size(width, availableSize.Height));
            maxHeight = Math.Max(maxHeight, element.DesiredSize.Height);
            x += width + Spacing;
        }

        LayoutWidth = x - Spacing;
        return new Size(LayoutWidth, maxHeight);
    }

    protected override Size ArrangeOverride(VirtualizingLayoutContext context, Size finalSize)
    {
        int itemCount = context.ItemCount;
        if (itemCount == 0)
        {
            LayoutWidth = 0;
            return finalSize;
        }

        var widths = ResolveColumnWidths(finalSize.Width - (Spacing * (itemCount - 1)), itemCount);
        double x = 0;

        for (int i = 0; i < itemCount; i++)
        {
            var element = context.GetOrCreateElementAt(i);
            double width = widths[i];
            element.Arrange(new Rect(x, 0, width, finalSize.Height));
            x += width + Spacing;
        }

        LayoutWidth = x - Spacing;
        return finalSize;
    }

    private double[] ResolveColumnWidths(double totalWidth, int itemCount)
    {
        double[] result = new double[itemCount];

        if (ColumnWidths.Count == 0)
        {
            double equal = totalWidth / itemCount;
            for (int i = 0; i < itemCount; i++)
                result[i] = equal;
            return result;
        }

        double fixedTotal = 0;
        double starTotal = 0;

        for (int i = 0; i < ColumnWidths.Count; i++)
        {
            var g = ColumnWidths[i];
            if (g.IsAbsolute)
                fixedTotal += g.Value;
            else if (g.IsStar)
                starTotal += g.Value;
        }

        double remaining = Math.Max(0, totalWidth - fixedTotal);
        double starUnit = starTotal > 0 ? remaining / starTotal : 0;

        for (int i = 0; i < itemCount; i++)
        {
            if (i < ColumnWidths.Count)
            {
                var g = ColumnWidths[i];
                if (g.IsAbsolute)
                    result[i] = g.Value;
                else if (g.IsStar)
                    result[i] = g.Value * starUnit;
                else
                    result[i] = totalWidth / itemCount;
            }
            else
            {
                result[i] = totalWidth / itemCount;
            }
        }

        return result;
    }
}
