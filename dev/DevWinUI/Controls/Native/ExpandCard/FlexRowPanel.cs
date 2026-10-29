namespace DevWinUI;

public sealed partial class FlexRowPanel : Panel
{
    private const double FallbackWidth = 800;
    private const double FallbackHeight = 420;
    private const double MinFlex = 0.0001;

    public static double GetFlex(DependencyObject d) => (double)d.GetValue(FlexProperty);

    public static void SetFlex(DependencyObject d, double value) => d.SetValue(FlexProperty, value);

    public static readonly DependencyProperty FlexProperty =
        DependencyProperty.RegisterAttached("Flex", typeof(double), typeof(FlexRowPanel), new PropertyMetadata(1.0, OnFlexChanged));

    public double Gap
    {
        get { return (double)GetValue(GapProperty); }
        set { SetValue(GapProperty, value); }
    }

    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(nameof(Gap), typeof(double), typeof(FlexRowPanel),
            new PropertyMetadata(8.0, (d, _) => ((FlexRowPanel)d).InvalidateMeasure()));

    private static void OnFlexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement element && VisualTreeHelper.GetParent(element) is FlexRowPanel panel)
        {
            panel.InvalidateArrange();
        }
    }

    private double[] ComputeEdges(double width)
    {
        int n = Children.Count;
        var edges = new double[n * 2];
        if (n == 0)
        {
            return edges;
        }

        double totalFlex = 0;
        foreach (var child in Children)
        {
            totalFlex += Math.Max(MinFlex, GetFlex(child));
        }

        double usable = Math.Max(0, width - Gap * (n - 1));
        double x = 0, acc = 0;
        for (int i = 0; i < n; i++)
        {
            acc += Math.Max(MinFlex, GetFlex(Children[i])) / totalFlex;
            double right = i == n - 1 ? width : Math.Round(acc * usable + Gap * i);
            edges[i * 2] = x;
            edges[i * 2 + 1] = right;
            x = right + Gap;
        }
        return edges;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double w = double.IsInfinity(availableSize.Width) ? FallbackWidth : availableSize.Width;
        double h = double.IsInfinity(availableSize.Height) ? FallbackHeight : availableSize.Height;
        var edges = ComputeEdges(w);
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].Measure(new Size(Math.Max(0, edges[i * 2 + 1] - edges[i * 2]), h));
        }
        return new Size(w, h);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var edges = ComputeEdges(finalSize.Width);
        for (int i = 0; i < Children.Count; i++)
        {
            Children[i].Arrange(new Rect(edges[i * 2], 0, Math.Max(0, edges[i * 2 + 1] - edges[i * 2]), finalSize.Height));
        }
        return finalSize;
    }
}
