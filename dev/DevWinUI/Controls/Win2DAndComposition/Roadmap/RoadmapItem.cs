namespace DevWinUI;

[TemplatePart(Name = HeaderPartName, Type = typeof(ContentPresenter))]
[TemplatePart(Name = ContentPartName, Type = typeof(ContentPresenter))]
public partial class RoadmapItem : ContentControl
{
    private const string HeaderPartName = "PART_Header";
    private const string ContentPartName = "PART_Content";

    private ContentPresenter? _header;
    private ContentPresenter? _content;
    private RoadmapMode _mode = RoadmapMode.Simple;

    internal double LayoutCenterX { get; set; }
    internal int LayoutSide { get; set; } = -1;
    internal Rect LayoutBounds { get; set; }

    public RoadmapItem()
    {
        DefaultStyleKey = typeof(RoadmapItem);
        RegisterPropertyChangedCallback(ForegroundProperty, (s, dp) => UpdateVisuals());
    }

    internal RoadmapMode Mode
    {
        get { return _mode; }
        set
        {
            if (_mode != value)
            {
                _mode = value;
                UpdateVisuals();
            }
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _header = GetTemplateChild(HeaderPartName) as ContentPresenter;
        _content = GetTemplateChild(ContentPartName) as ContentPresenter;
        UpdateVisuals();
    }

    private Roadmap? GetOwner()
    {
        return ItemsControl.ItemsControlFromItemContainer(this) as Roadmap;
    }

    private void UpdateVisuals()
    {
        string state = _mode switch
        {
            RoadmapMode.Boxed => "Boxed",
            RoadmapMode.Icon => "Icon",
            RoadmapMode.Neon => "Neon",
            _ => "Simple"
        };

        VisualStateManager.GoToState(this, state, false);

        bool useAccent = _mode == RoadmapMode.Icon;
        HorizontalAlignment alignment = useAccent ? HorizontalAlignment.Center : HorizontalAlignment.Left;
        double opacity = Status == RoadmapItemStatus.Planned ? 0.55 : 1;

        if (_header != null)
        {
            _header.Foreground = useAccent ? Accent : HeaderForeground;
            _header.HorizontalContentAlignment = alignment;
            _header.Opacity = opacity;
        }

        if (_content != null)
        {
            _content.Foreground = useAccent ? Accent : Foreground;
            _content.HorizontalContentAlignment = alignment;
            _content.Opacity = opacity;
        }
    }

    private static void OnBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RoadmapItem item)
        {
            item.UpdateVisuals();
            item.GetOwner()?.InvalidateCanvas();
        }
    }

    private static void OnStatusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RoadmapItem item)
        {
            item.UpdateVisuals();

            Roadmap? owner = item.GetOwner();
            owner?.InvalidateCanvas();
            owner?.UpdateAnimation();
        }
    }

    private static void OnCanvasPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RoadmapItem item)
        {
            item.GetOwner()?.InvalidateCanvas();
        }
    }

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RoadmapItem item)
        {
            item.GetOwner()?.InvalidateLayoutAndCanvas();
        }
    }
}
