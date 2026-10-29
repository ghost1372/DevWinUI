namespace DevWinUI;

public sealed partial class TableOfContents
{
    public static string? GetHeadingId(DependencyObject o)
    {
        return (string?)o.GetValue(HeadingIdProperty);
    }

    public static void SetHeadingId(DependencyObject o, string? value)
    {
        o.SetValue(HeadingIdProperty, value);
    }

    public static readonly DependencyProperty HeadingIdProperty =
        DependencyProperty.RegisterAttached("HeadingId", typeof(string), typeof(TableOfContents), new PropertyMetadata(null));

    public IReadOnlyList<TableOfContentsItem> Items
    {
        get { return (IReadOnlyList<TableOfContentsItem>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }

    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register(nameof(Items), typeof(IReadOnlyList<TableOfContentsItem>), typeof(TableOfContents), new PropertyMetadata(Array.Empty<TableOfContentsItem>(), (d, _) => ((TableOfContents)d).RebuildRows()));

    public TableOfContentsKind Kind
    {
        get { return (TableOfContentsKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(TableOfContentsKind), typeof(TableOfContents), new PropertyMetadata(TableOfContentsKind.Curve, (d, _) => ((TableOfContents)d).RebuildRail()));

    public bool Indicator
    {
        get { return (bool)GetValue(IndicatorProperty); }
        set { SetValue(IndicatorProperty, value); }
    }

    public static readonly DependencyProperty IndicatorProperty =
        DependencyProperty.Register(nameof(Indicator), typeof(bool), typeof(TableOfContents), new PropertyMetadata(true, (d, _) => ((TableOfContents)d).ApplyActive(false)));

    public double ScrollOffset
    {
        get { return (double)GetValue(ScrollOffsetProperty); }
        set { SetValue(ScrollOffsetProperty, value); }
    }

    public static readonly DependencyProperty ScrollOffsetProperty =
        DependencyProperty.Register(nameof(ScrollOffset), typeof(double), typeof(TableOfContents), new PropertyMetadata(0.0, (d, _) => ((TableOfContents)d).ScheduleUpdate()));

    public ScrollViewer? Root
    {
        get { return (ScrollViewer?)GetValue(RootProperty); }
        set { SetValue(RootProperty, value); }
    }

    public static readonly DependencyProperty RootProperty =
        DependencyProperty.Register(nameof(Root), typeof(ScrollViewer), typeof(TableOfContents), new PropertyMetadata(null, (d, _) => ((TableOfContents)d).Hook()));

    public IReadOnlyList<string> ActiveIds
    {
        get { return (IReadOnlyList<string>)GetValue(ActiveIdsProperty); }
        set { SetValue(ActiveIdsProperty, value); }
    }

    public static readonly DependencyProperty ActiveIdsProperty =
        DependencyProperty.Register(nameof(ActiveIds), typeof(IReadOnlyList<string>), typeof(TableOfContents), new PropertyMetadata(Array.Empty<string>(), (d, _) => ((TableOfContents)d).OnActiveIdsChanged()));

    public bool TrackScroll
    {
        get { return (bool)GetValue(TrackScrollProperty); }
        set { SetValue(TrackScrollProperty, value); }
    }

    public static readonly DependencyProperty TrackScrollProperty =
        DependencyProperty.Register(nameof(TrackScroll), typeof(bool), typeof(TableOfContents), new PropertyMetadata(true, (d, _) => ((TableOfContents)d).ScheduleUpdate()));

    public bool IsAnimationEnabled
    {
        get { return (bool)GetValue(IsAnimationEnabledProperty); }
        set { SetValue(IsAnimationEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsAnimationEnabledProperty =
        DependencyProperty.Register(nameof(IsAnimationEnabled), typeof(bool), typeof(TableOfContents), new PropertyMetadata(true));

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(TableOfContents), new PropertyMetadata("On this page"));

    public Style? LinkStyle
    {
        get { return (Style?)GetValue(LinkStyleProperty); }
        set { SetValue(LinkStyleProperty, value); }
    }

    public static readonly DependencyProperty LinkStyleProperty =
        DependencyProperty.Register(nameof(LinkStyle), typeof(Style), typeof(TableOfContents), new PropertyMetadata(null, (d, _) => ((TableOfContents)d).RebuildRows()));

    public double LinkFontSize
    {
        get { return (double)GetValue(LinkFontSizeProperty); }
        set { SetValue(LinkFontSizeProperty, value); }
    }

    public static readonly DependencyProperty LinkFontSizeProperty =
        DependencyProperty.Register(nameof(LinkFontSize), typeof(double), typeof(TableOfContents), new PropertyMetadata(13.0, (d, _) => ((TableOfContents)d).RebuildRows()));

    public Brush? LinkForeground
    {
        get { return (Brush?)GetValue(LinkForegroundProperty); }
        set { SetValue(LinkForegroundProperty, value); }
    }

    public static readonly DependencyProperty LinkForegroundProperty =
        DependencyProperty.Register(nameof(LinkForeground), typeof(Brush), typeof(TableOfContents), new PropertyMetadata(null, (d, _) => ((TableOfContents)d).ApplyActive(false)));

    public Brush? ActiveLinkForeground
    {
        get { return (Brush?)GetValue(ActiveLinkForegroundProperty); }
        set { SetValue(ActiveLinkForegroundProperty, value); }
    }

    public static readonly DependencyProperty ActiveLinkForegroundProperty =
        DependencyProperty.Register(nameof(ActiveLinkForeground), typeof(Brush), typeof(TableOfContents), new PropertyMetadata(null, (d, _) => ((TableOfContents)d).ApplyActive(false)));

    public Brush? AccentBrush
    {
        get { return (Brush?)GetValue(AccentBrushProperty); }
        set { SetValue(AccentBrushProperty, value); }
    }

    public static readonly DependencyProperty AccentBrushProperty =
        DependencyProperty.Register(nameof(AccentBrush), typeof(Brush), typeof(TableOfContents), new PropertyMetadata(null));

    public Brush? TrackBrush
    {
        get { return (Brush?)GetValue(TrackBrushProperty); }
        set { SetValue(TrackBrushProperty, value); }
    }

    public static readonly DependencyProperty TrackBrushProperty =
        DependencyProperty.Register(nameof(TrackBrush), typeof(Brush), typeof(TableOfContents), new PropertyMetadata(null));
}
