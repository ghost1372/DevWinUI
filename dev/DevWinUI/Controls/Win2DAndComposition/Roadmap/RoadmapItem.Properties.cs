namespace DevWinUI;

public partial class RoadmapItem
{
    public object Header
    {
        get { return GetValue(HeaderProperty); }
        set { SetValue(HeaderProperty, value); }
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(RoadmapItem), new PropertyMetadata(null));

    public DataTemplate HeaderTemplate
    {
        get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
        set { SetValue(HeaderTemplateProperty, value); }
    }

    public static readonly DependencyProperty HeaderTemplateProperty =
        DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(RoadmapItem), new PropertyMetadata(null));

    public double HeaderFontSize
    {
        get { return (double)GetValue(HeaderFontSizeProperty); }
        set { SetValue(HeaderFontSizeProperty, value); }
    }

    public static readonly DependencyProperty HeaderFontSizeProperty =
        DependencyProperty.Register(nameof(HeaderFontSize), typeof(double), typeof(RoadmapItem), new PropertyMetadata(16d));

    public Brush HeaderForeground
    {
        get { return (Brush)GetValue(HeaderForegroundProperty); }
        set { SetValue(HeaderForegroundProperty, value); }
    }

    public static readonly DependencyProperty HeaderForegroundProperty =
        DependencyProperty.Register(nameof(HeaderForeground), typeof(Brush), typeof(RoadmapItem), new PropertyMetadata(null, OnBrushPropertyChanged));

    public string Label
    {
        get { return (string)GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static readonly DependencyProperty LabelProperty =
        DependencyProperty.Register(nameof(Label), typeof(string), typeof(RoadmapItem), new PropertyMetadata(string.Empty, OnLayoutPropertyChanged));

    public string Glyph
    {
        get { return (string)GetValue(GlyphProperty); }
        set { SetValue(GlyphProperty, value); }
    }

    public static readonly DependencyProperty GlyphProperty =
        DependencyProperty.Register(nameof(Glyph), typeof(string), typeof(RoadmapItem), new PropertyMetadata(string.Empty, OnCanvasPropertyChanged));

    public Brush Accent
    {
        get { return (Brush)GetValue(AccentProperty); }
        set { SetValue(AccentProperty, value); }
    }

    public static readonly DependencyProperty AccentProperty =
        DependencyProperty.Register(nameof(Accent), typeof(Brush), typeof(RoadmapItem), new PropertyMetadata(null, OnBrushPropertyChanged));

    public RoadmapPlacement Placement
    {
        get { return (RoadmapPlacement)GetValue(PlacementProperty); }
        set { SetValue(PlacementProperty, value); }
    }

    public static readonly DependencyProperty PlacementProperty =
        DependencyProperty.Register(nameof(Placement), typeof(RoadmapPlacement), typeof(RoadmapItem), new PropertyMetadata(RoadmapPlacement.Auto, OnLayoutPropertyChanged));

    public RoadmapItemStatus Status
    {
        get { return (RoadmapItemStatus)GetValue(StatusProperty); }
        set { SetValue(StatusProperty, value); }
    }

    public static readonly DependencyProperty StatusProperty =
        DependencyProperty.Register(nameof(Status), typeof(RoadmapItemStatus), typeof(RoadmapItem), new PropertyMetadata(RoadmapItemStatus.Completed, OnStatusChanged));

    public double Value
    {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(double), typeof(RoadmapItem), new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));
}
