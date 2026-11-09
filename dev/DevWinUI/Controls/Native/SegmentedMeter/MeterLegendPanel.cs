namespace DevWinUI;

public sealed partial class MeterLegendPanel : Panel
{
    private const double Epsilon = 0.01;

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(MeterLegendPanel), new PropertyMetadata(Orientation.Horizontal, OnLayoutChanged));

    public double HorizontalSpacing
    {
        get { return (double)GetValue(HorizontalSpacingProperty); }
        set { SetValue(HorizontalSpacingProperty, value); }
    }

    public static readonly DependencyProperty HorizontalSpacingProperty =
        DependencyProperty.Register(nameof(HorizontalSpacing), typeof(double), typeof(MeterLegendPanel), new PropertyMetadata(16.0, OnLayoutChanged));

    public double VerticalSpacing
    {
        get { return (double)GetValue(VerticalSpacingProperty); }
        set { SetValue(VerticalSpacingProperty, value); }
    }

    public static readonly DependencyProperty VerticalSpacingProperty =
        DependencyProperty.Register(nameof(VerticalSpacing), typeof(double), typeof(MeterLegendPanel), new PropertyMetadata(6.0, OnLayoutChanged));

    private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MeterLegendPanel)d).InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double horizontal = HorizontalSpacing;
        double vertical = VerticalSpacing;
        double available = availableSize.Width;
        bool wrap = Orientation == Orientation.Horizontal;
        var probe = new Size(available, double.PositiveInfinity);

        double x = 0, y = 0, rowHeight = 0, maxWidth = 0;
        bool any = false;

        foreach (UIElement child in Children)
        {
            if (child.Visibility == Visibility.Collapsed) continue;

            child.Measure(probe);
            var size = child.DesiredSize;

            if (wrap)
            {
                if (any && x > 0 && x + size.Width > available + Epsilon)
                {
                    y += rowHeight + vertical;
                    x = 0;
                    rowHeight = 0;
                }

                x += size.Width;
                maxWidth = Math.Max(maxWidth, x);
                x += horizontal;
                rowHeight = Math.Max(rowHeight, size.Height);
            }
            else
            {
                if (any) y += vertical;
                y += size.Height;
                maxWidth = Math.Max(maxWidth, size.Width);
            }

            any = true;
        }

        if (!any) return new Size(0, 0);

        double width = Math.Min(maxWidth, available);
        return new Size(width, wrap ? y + rowHeight : y);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double horizontal = HorizontalSpacing;
        double vertical = VerticalSpacing;
        bool wrap = Orientation == Orientation.Horizontal;

        double x = 0, y = 0, rowHeight = 0;

        foreach (UIElement child in Children)
        {
            if (child.Visibility == Visibility.Collapsed) continue;

            var size = child.DesiredSize;

            if (wrap)
            {
                if (x > 0 && x + size.Width > finalSize.Width + Epsilon)
                {
                    y += rowHeight + vertical;
                    x = 0;
                    rowHeight = 0;
                }

                child.Arrange(new Rect(x, y, size.Width, size.Height));
                x += size.Width + horizontal;
                rowHeight = Math.Max(rowHeight, size.Height);
            }
            else
            {
                child.Arrange(new Rect(0, y, Math.Max(size.Width, finalSize.Width), size.Height));
                y += size.Height + vertical;
            }
        }

        return finalSize;
    }
}
