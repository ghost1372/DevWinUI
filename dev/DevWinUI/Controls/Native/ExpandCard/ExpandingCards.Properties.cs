namespace DevWinUI;

public sealed partial class ExpandingCards
{
    public ObservableCollection<ExpandingCardItem> Items
    {
        get { return (ObservableCollection<ExpandingCardItem>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(ObservableCollection<ExpandingCardItem>), typeof(ExpandingCards), new PropertyMetadata(null, OnItemsChanged));

    public int DefaultActiveIndex
    {
        get { return (int)GetValue(DefaultActiveIndexProperty); }
        set { SetValue(DefaultActiveIndexProperty, value); }
    }

    public static readonly DependencyProperty DefaultActiveIndexProperty =
        DependencyProperty.Register(nameof(DefaultActiveIndex), typeof(int), typeof(ExpandingCards), new PropertyMetadata(0, OnDefaultActiveIndexChanged));

    public double ExpandedFlex
    {
        get { return (double)GetValue(ExpandedFlexProperty); }
        set { SetValue(ExpandedFlexProperty, value); }
    }

    public static readonly DependencyProperty ExpandedFlexProperty =
        DependencyProperty.Register(nameof(ExpandedFlex), typeof(double), typeof(ExpandingCards), new PropertyMetadata(5.4, OnFlexChanged));

    public double CollapsedFlex
    {
        get { return (double)GetValue(CollapsedFlexProperty); }
        set { SetValue(CollapsedFlexProperty, value); }
    }

    public static readonly DependencyProperty CollapsedFlexProperty =
        DependencyProperty.Register(nameof(CollapsedFlex), typeof(double), typeof(ExpandingCards), new PropertyMetadata(1.0, OnFlexChanged));

    public double Gap
    {
        get { return (double)GetValue(GapProperty); }
        set { SetValue(GapProperty, value); }
    }

    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(nameof(Gap), typeof(double), typeof(ExpandingCards), new PropertyMetadata(8.0));

    public bool ReduceMotion
    {
        get { return (bool)GetValue(ReduceMotionProperty); }
        set { SetValue(ReduceMotionProperty, value); }
    }

    public static readonly DependencyProperty ReduceMotionProperty =
        DependencyProperty.Register(nameof(ReduceMotion), typeof(bool), typeof(ExpandingCards), new PropertyMetadata(false));

    public CornerRadius CardCornerRadius
    {
        get { return (CornerRadius)GetValue(CardCornerRadiusProperty); }
        set { SetValue(CardCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty CardCornerRadiusProperty =
        DependencyProperty.Register(nameof(CardCornerRadius), typeof(CornerRadius), typeof(ExpandingCards),
            new PropertyMetadata(new CornerRadius(24), OnAppearanceChanged));

    public Brush? TitleForeground
    {
        get { return (Brush?)GetValue(TitleForegroundProperty); }
        set { SetValue(TitleForegroundProperty, value); }
    }

    public static readonly DependencyProperty TitleForegroundProperty =
        DependencyProperty.Register(nameof(TitleForeground), typeof(Brush), typeof(ExpandingCards), new PropertyMetadata(null, OnAppearanceChanged));

    public Brush? ScrimBrush
    {
        get { return (Brush?)GetValue(ScrimBrushProperty); }
        set { SetValue(ScrimBrushProperty, value); }
    }

    public static readonly DependencyProperty ScrimBrushProperty =
        DependencyProperty.Register(nameof(ScrimBrush), typeof(Brush), typeof(ExpandingCards), new PropertyMetadata(null, OnAppearanceChanged));

    public Brush? FocusBrush
    {
        get { return (Brush?)GetValue(FocusBrushProperty); }
        set { SetValue(FocusBrushProperty, value); }
    }

    public static readonly DependencyProperty FocusBrushProperty =
        DependencyProperty.Register(nameof(FocusBrush), typeof(Brush), typeof(ExpandingCards), new PropertyMetadata(null, OnAppearanceChanged));
}
