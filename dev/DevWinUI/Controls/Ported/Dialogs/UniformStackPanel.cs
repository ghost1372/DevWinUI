// https://github.com/SuGar0218/SuGarToolkit.WinUI3

namespace DevWinUI;

internal partial class UniformStackPanel : Panel
{
    #region DependencyProperty

    public Orientation Orientation
    {
        get => (Orientation) GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(UniformStackPanel),
        new PropertyMetadata(default(Orientation), (d, e) => ((UniformStackPanel) d).InvalidateMeasure())
    );

    public double Spacing
    {
        get => (double) GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register(
        nameof(Spacing),
        typeof(double),
        typeof(UniformStackPanel),
        new PropertyMetadata(default(double), (d, e) => ((UniformStackPanel) d).InvalidateMeasure())
    );

    public int FirstPosition
    {
        get => (int) GetValue(FirstPositionProperty);
        set => SetValue(FirstPositionProperty, value);
    }

    public static readonly DependencyProperty FirstPositionProperty = DependencyProperty.Register(
        nameof(FirstPosition),
        typeof(int),
        typeof(UniformStackPanel),
        new PropertyMetadata(default(int))
    );

    public HorizontalAlignment ChildHorizontalAlignment
    {
        get => (HorizontalAlignment) GetValue(ChildHorizontalAlignmentProperty);
        set => SetValue(ChildHorizontalAlignmentProperty, value);
    }

    public static readonly DependencyProperty ChildHorizontalAlignmentProperty = DependencyProperty.Register(
        nameof(ChildHorizontalAlignment),
        typeof(HorizontalAlignment),
        typeof(UniformStackPanel),
        new PropertyMetadata(default(HorizontalAlignment), (d, e) => ((UniformStackPanel) d).InvalidateMeasure())
    );

    public VerticalAlignment ChildVerticalAlignment
    {
        get => (VerticalAlignment) GetValue(ChildVerticalAlignmentProperty);
        set => SetValue(ChildVerticalAlignmentProperty, value);
    }

    public static readonly DependencyProperty ChildVerticalAlignmentProperty = DependencyProperty.Register(
        nameof(ChildVerticalAlignment),
        typeof(VerticalAlignment),
        typeof(UniformStackPanel),
        new PropertyMetadata(default(VerticalAlignment), (d, e) => ((UniformStackPanel) d).InvalidateMeasure())
    );

    #endregion

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
            return ZeroSize;

        foreach (UIElement child in Children)
        {
            if (child is FrameworkElement element)
            {
                element.HorizontalAlignment = HorizontalAlignment.Stretch;
                element.VerticalAlignment = VerticalAlignment.Stretch;
            }
        }

        return Orientation switch
        {
            Orientation.Horizontal => MeasureWhenHorizontal(availableSize),
            Orientation.Vertical => MeasureWhenVertical(availableSize),
            _ => throw new InvalidOperationException(nameof(Orientation))
        };
    }

    private Size MeasureWhenHorizontal(Size availableSize)
    {
        List<UIElement> visibleChildren = [.. Children.Where(child => child.Visibility is Visibility.Visible)];
        if (visibleChildren.Count == 0)
            return ZeroSize;

        double totalSpacing = Spacing * (visibleChildren.Count - 1);
        Size childAvailableSize = new((availableSize.Width - totalSpacing) / visibleChildren.Count, availableSize.Height);
        double maxWidth = 0;
        double maxHeight = 0;
        foreach (UIElement child in visibleChildren)
        {
            child.Measure(childAvailableSize);
            maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
            maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
        }
        return new Size(maxWidth * visibleChildren.Count + totalSpacing, maxHeight);
    }

    private Size MeasureWhenVertical(Size availableSize)
    {
        List<UIElement> visibleChildren = [.. Children.Where(child => child.Visibility is Visibility.Visible)];
        if (visibleChildren.Count == 0)
            return ZeroSize;

        double totalSpacing = Spacing * (visibleChildren.Count - 1);
        Size childAvailableSize = new(availableSize.Width, (availableSize.Height - totalSpacing) / visibleChildren.Count);
        double maxWidth = 0;
        double maxHeight = 0;
        foreach (UIElement child in visibleChildren)
        {
            child.Measure(childAvailableSize);
            maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
            maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
        }
        return new Size(maxWidth, maxHeight * visibleChildren.Count + totalSpacing);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
            return ZeroSize;

        return Orientation switch
        {
            Orientation.Horizontal => ArrangeWhenHorizontal(finalSize),
            Orientation.Vertical => ArrangeWhenVertical(finalSize),
            _ => throw new InvalidOperationException(nameof(Orientation))
        };
    }

    private Size ArrangeWhenHorizontal(Size finalSize)
    {
        List<UIElement> visibleChildren = [.. Children.Where(child => child.Visibility is Visibility.Visible)];
        if (visibleChildren.Count == 0)
            return ZeroSize;

        Size childAvailableSize = new((finalSize.Width - Spacing * (visibleChildren.Count - 1)) / visibleChildren.Count, finalSize.Height);
        double x = 0;
        double height = 0;
        foreach (UIElement child in visibleChildren)
        {
            child.Arrange(new Rect(new Point(x, 0), childAvailableSize));
            x += childAvailableSize.Width + Spacing;
            if (height < child.ActualSize.Y)
            {
                height = child.ActualSize.Y;
            }
        }
        return new Size(x - Spacing, height);
    }

    private Size ArrangeWhenVertical(Size finalSize)
    {
        List<UIElement> visibleChildren = [.. Children.Where(child => child.Visibility is Visibility.Visible)];
        if (visibleChildren.Count == 0)
            return ZeroSize;

        Size childAvailableSize = new(finalSize.Width, (finalSize.Height - Spacing * (visibleChildren.Count - 1)) / visibleChildren.Count);
        double y = 0;
        double width = 0;
        foreach (UIElement child in visibleChildren)
        {
            child.Arrange(new Rect(new Point(0, y), childAvailableSize));
            y += childAvailableSize.Height + Spacing;
            if (width < child.ActualSize.X)
            {
                width = child.ActualSize.X;
            }
        }
        return new Size(width, y - Spacing);
    }

    private static readonly Size ZeroSize = new(0, 0);
}
