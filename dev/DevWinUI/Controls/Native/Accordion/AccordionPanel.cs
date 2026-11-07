namespace DevWinUI;

public partial class AccordionPanel : Panel
{
    public AccordionPanel()
    {
        this.PointerWheelChanged += OnPointerWheelChanged;
        this.ManipulationMode = Microsoft.UI.Xaml.Input.ManipulationModes.TranslateY;
        this.ManipulationStarted += OnManipulationStarted;
        this.ManipulationDelta += OnManipulationDelta;
        this.ManipulationCompleted += OnManipulationCompleted;
    }

    private double TabHeight { get; set; }

    protected override Size MeasureOverride(Size availableSize)
    {
        var sample = base.Children.LastOrDefault() as AccordionItem;
        if (sample != null)
        {
            this.TabHeight = sample.HeaderHeight;
        }

        bool isHeightUnbounded = double.IsInfinity(availableSize.Height) || double.IsNaN(availableSize.Height);
        double height = isHeightUnbounded ? GetFallbackHeight() : availableSize.Height;

        double maxWidth = MeasureItems(availableSize.Width, height);

        // The header height is only known after the item template has been applied and
        // the item measured, so the first pass uses a TabHeight of 0. Re-measure once with the real value.
        if (sample != null && sample.HeaderHeight != this.TabHeight)
        {
            this.TabHeight = sample.HeaderHeight;
            if (isHeightUnbounded)
            {
                height = GetFallbackHeight();
            }
            maxWidth = MeasureItems(availableSize.Width, height);
        }

        // Returning an infinite size from MeasureOverride throws.
        double width = double.IsInfinity(availableSize.Width) ? maxWidth : availableSize.Width;
        return new Size(width, height);
    }

    // Fallback height: based on TabHeight x visible tabs + extra padding
    private double GetFallbackHeight()
    {
        return MaxTabs * TabHeight + 100;
    }

    private double MeasureItems(double width, double height)
    {
        double contentHeight = GetContentHeight(height);
        double maxWidth = 0;
        foreach (var item in base.Children)
        {
            item.Measure(new Size(width, contentHeight));
            maxWidth = Math.Max(maxWidth, item.DesiredSize.Width);
        }
        return maxWidth;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double contentHeight = GetContentHeight(finalSize.Height);
        double y = 0;

        foreach (var item in base.Children.Reverse())
        {
            var rect = new Rect(0, y, finalSize.Width, contentHeight);
            item.Arrange(rect);
            y += TabHeight;
        }

        int n = 0;
        double ty = -(finalSize.Height - MaxTabs * TabHeight);
        foreach (AccordionItem item in base.Children.Reverse())
        {
            if (n < this.SelectedIndex)
            {
                item.IsUp = true;
                item.TranslateY(ty);
            }
            else
            {
                item.IsUp = false;
                item.TranslateY(0);
            }
            n++;
        }

        int index = Math.Max(0, this.SelectedIndex - 2);
        index = Math.Max(0, Math.Min(index, base.Children.Count - 4));
        double cy = index * this.TabHeight;
        this.AnimateY(-cy);

        return finalSize;
    }

    private double GetContentHeight(double availableHeight)
    {
        return Math.Max(0, availableHeight - MaxTabs * TabHeight + TabHeight);
    }

    private double GetTopBound()
    {
        return -(this.ActualHeight - MaxTabs * TabHeight);
    }
}
