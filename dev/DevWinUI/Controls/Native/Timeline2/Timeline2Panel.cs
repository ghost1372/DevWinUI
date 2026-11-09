namespace DevWinUI;

public sealed partial class Timeline2Panel : Panel
{
    private bool _equalSlots;

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Timeline2Panel), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

    private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Timeline2Panel)d).InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        int count = Children.Count;
        if (count == 0)
        {
            return new Size(0, 0);
        }

        return Orientation == Orientation.Vertical
            ? MeasureVertical(availableSize, count)
            : MeasureHorizontal(availableSize, count);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int count = Children.Count;
        if (count == 0)
        {
            return finalSize;
        }

        if (Orientation == Orientation.Vertical)
        {
            double y = 0;
            for (int i = 0; i < count; i++)
            {
                var child = Children[i];
                double height = child.DesiredSize.Height;
                child.Arrange(new Rect(0, y, finalSize.Width, height));
                y += height;
            }
        }
        else
        {
            double slot = finalSize.Width / count;
            double x = 0;
            for (int i = 0; i < count; i++)
            {
                var child = Children[i];
                double width = _equalSlots ? slot : child.DesiredSize.Width;
                child.Arrange(new Rect(x, 0, width, finalSize.Height));
                x += width;
            }
        }

        return finalSize;
    }

    private Size MeasureVertical(Size availableSize, int count)
    {
        var childAvailable = new Size(availableSize.Width, double.PositiveInfinity);
        double width = 0;
        double height = 0;

        for (int i = 0; i < count; i++)
        {
            var child = Children[i];
            child.Measure(childAvailable);
            height += child.DesiredSize.Height;
            width = Math.Max(width, child.DesiredSize.Width);
        }

        return new Size(double.IsInfinity(availableSize.Width) ? width : availableSize.Width, height);
    }

    private Size MeasureHorizontal(Size availableSize, int count)
    {
        bool finite = !double.IsInfinity(availableSize.Width);
        _equalSlots = finite;

        var childAvailable = new Size(finite ? availableSize.Width / count : double.PositiveInfinity, availableSize.Height);
        double width = 0;
        double height = 0;
        double leading = 0;
        double trailing = 0;
        double bar = 0;
        bool centered = false;

        for (int i = 0; i < count; i++)
        {
            var child = Children[i];
            child.Measure(childAvailable);
            height = Math.Max(height, child.DesiredSize.Height);
            width += child.DesiredSize.Width;

            if (child is Timeline2Item { IsCentered: true } item)
            {
                centered = true;
                leading = Math.Max(leading, item.LeadingExtent);
                trailing = Math.Max(trailing, item.TrailingExtent);
                bar = Math.Max(bar, item.BarExtent);
            }
        }

        if (centered)
        {
            height = Math.Max(height, bar + (2 * Math.Max(leading, trailing)));
        }

        return new Size(finite ? availableSize.Width : width, height);
    }
}
