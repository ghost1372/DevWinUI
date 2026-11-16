namespace DevWinUI;

public sealed partial class Diagram
{
    public DiagramNode? Root
    {
        get { return (DiagramNode?)GetValue(RootProperty); }
        set { SetValue(RootProperty, value); }
    }

    public static readonly DependencyProperty RootProperty =
        DependencyProperty.Register(nameof(Root), typeof(DiagramNode), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).OnRootChanged()));

    public DiagramLayout Layout
    {
        get { return (DiagramLayout)GetValue(LayoutProperty); }
        set { SetValue(LayoutProperty, value); }
    }

    public static readonly DependencyProperty LayoutProperty =
        DependencyProperty.Register(nameof(Layout), typeof(DiagramLayout), typeof(Diagram),
            new PropertyMetadata(DiagramLayout.Manual, (d, e) => ((Diagram)d).OnLayoutModeChanged()));

    public DiagramDirection Direction
    {
        get { return (DiagramDirection)GetValue(DirectionProperty); }
        set { SetValue(DirectionProperty, value); }
    }

    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(DiagramDirection), typeof(Diagram),
            new PropertyMetadata(DiagramDirection.TopToBottom, (d, e) => ((Diagram)d).OnLayoutModeChanged()));

    public DiagramEdgeStyle EdgeStyle
    {
        get { return (DiagramEdgeStyle)GetValue(EdgeStyleProperty); }
        set { SetValue(EdgeStyleProperty, value); }
    }

    public static readonly DependencyProperty EdgeStyleProperty =
        DependencyProperty.Register(nameof(EdgeStyle), typeof(DiagramEdgeStyle), typeof(Diagram),
            new PropertyMetadata(DiagramEdgeStyle.Bezier, (d, e) => ((Diagram)d).UpdateEdges()));

    public double EdgeCornerRadius
    {
        get { return (double)GetValue(EdgeCornerRadiusProperty); }
        set { SetValue(EdgeCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty EdgeCornerRadiusProperty =
        DependencyProperty.Register(nameof(EdgeCornerRadius), typeof(double), typeof(Diagram),
            new PropertyMetadata(8d, (d, e) => ((Diagram)d).UpdateEdges()));

    public bool ShowArrows
    {
        get { return (bool)GetValue(ShowArrowsProperty); }
        set { SetValue(ShowArrowsProperty, value); }
    }

    public static readonly DependencyProperty ShowArrowsProperty =
        DependencyProperty.Register(nameof(ShowArrows), typeof(bool), typeof(Diagram),
            new PropertyMetadata(false, (d, e) => ((Diagram)d).UpdateEdges()));

    public DiagramSelectionMode SelectionMode
    {
        get { return (DiagramSelectionMode)GetValue(SelectionModeProperty); }
        set { SetValue(SelectionModeProperty, value); }
    }

    public static readonly DependencyProperty SelectionModeProperty =
        DependencyProperty.Register(nameof(SelectionMode), typeof(DiagramSelectionMode), typeof(Diagram),
            new PropertyMetadata(DiagramSelectionMode.None, (d, e) => ((Diagram)d).OnSelectionModeChanged()));

    public bool Collapsible
    {
        get { return (bool)GetValue(CollapsibleProperty); }
        set { SetValue(CollapsibleProperty, value); }
    }

    public static readonly DependencyProperty CollapsibleProperty =
        DependencyProperty.Register(nameof(Collapsible), typeof(bool), typeof(Diagram),
            new PropertyMetadata(false, (d, e) => ((Diagram)d).OnStructureOptionChanged()));

    public bool AllowDrag
    {
        get { return (bool)GetValue(AllowDragProperty); }
        set { SetValue(AllowDragProperty, value); }
    }

    public static readonly DependencyProperty AllowDragProperty =
        DependencyProperty.Register(nameof(AllowDrag), typeof(bool), typeof(Diagram), new PropertyMetadata(true));

    public bool SnapToGrid
    {
        get { return (bool)GetValue(SnapToGridProperty); }
        set { SetValue(SnapToGridProperty, value); }
    }

    public static readonly DependencyProperty SnapToGridProperty =
        DependencyProperty.Register(nameof(SnapToGrid), typeof(bool), typeof(Diagram), new PropertyMetadata(false));

    public double SnapSize
    {
        get { return (double)GetValue(SnapSizeProperty); }
        set { SetValue(SnapSizeProperty, value); }
    }

    public static readonly DependencyProperty SnapSizeProperty =
        DependencyProperty.Register(nameof(SnapSize), typeof(double), typeof(Diagram), new PropertyMetadata(8d));

    public bool AutoFit
    {
        get { return (bool)GetValue(AutoFitProperty); }
        set { SetValue(AutoFitProperty, value); }
    }

    public static readonly DependencyProperty AutoFitProperty =
        DependencyProperty.Register(nameof(AutoFit), typeof(bool), typeof(Diagram),
            new PropertyMetadata(false, (d, e) => ((Diagram)d).OnAutoFitChanged()));

    public bool ShowControls
    {
        get { return (bool)GetValue(ShowControlsProperty); }
        set { SetValue(ShowControlsProperty, value); }
    }

    public static readonly DependencyProperty ShowControlsProperty =
        DependencyProperty.Register(nameof(ShowControls), typeof(bool), typeof(Diagram),
            new PropertyMetadata(true, (d, e) => ((Diagram)d).UpdateControlsVisibility()));

    public bool ShowMinimap
    {
        get { return (bool)GetValue(ShowMinimapProperty); }
        set { SetValue(ShowMinimapProperty, value); }
    }

    public static readonly DependencyProperty ShowMinimapProperty =
        DependencyProperty.Register(nameof(ShowMinimap), typeof(bool), typeof(Diagram),
            new PropertyMetadata(true, (d, e) => ((Diagram)d).OnShowMinimapChanged()));

    public DiagramBackground BackgroundPattern
    {
        get { return (DiagramBackground)GetValue(BackgroundPatternProperty); }
        set { SetValue(BackgroundPatternProperty, value); }
    }

    public static readonly DependencyProperty BackgroundPatternProperty =
        DependencyProperty.Register(nameof(BackgroundPattern), typeof(DiagramBackground), typeof(Diagram),
            new PropertyMetadata(DiagramBackground.Dots, (d, e) => ((Diagram)d).BuildBackground()));

    public double RowSpacing
    {
        get { return (double)GetValue(RowSpacingProperty); }
        set { SetValue(RowSpacingProperty, value); }
    }

    public static readonly DependencyProperty RowSpacingProperty =
        DependencyProperty.Register(nameof(RowSpacing), typeof(double), typeof(Diagram),
            new PropertyMetadata(150d, (d, e) => ((Diagram)d).OnSpacingChanged()));

    public double LevelSpacing
    {
        get { return (double)GetValue(LevelSpacingProperty); }
        set { SetValue(LevelSpacingProperty, value); }
    }

    public static readonly DependencyProperty LevelSpacingProperty =
        DependencyProperty.Register(nameof(LevelSpacing), typeof(double), typeof(Diagram),
            new PropertyMetadata(56d, (d, e) => ((Diagram)d).OnSpacingChanged()));

    public double SiblingSpacing
    {
        get { return (double)GetValue(SiblingSpacingProperty); }
        set { SetValue(SiblingSpacingProperty, value); }
    }

    public static readonly DependencyProperty SiblingSpacingProperty =
        DependencyProperty.Register(nameof(SiblingSpacing), typeof(double), typeof(Diagram),
            new PropertyMetadata(32d, (d, e) => ((Diagram)d).OnSpacingChanged()));

    public double DefaultNodeWidth
    {
        get { return (double)GetValue(DefaultNodeWidthProperty); }
        set { SetValue(DefaultNodeWidthProperty, value); }
    }

    public static readonly DependencyProperty DefaultNodeWidthProperty =
        DependencyProperty.Register(nameof(DefaultNodeWidth), typeof(double), typeof(Diagram),
            new PropertyMetadata(220d, (d, e) => ((Diagram)d).OnStructureOptionChanged()));

    public double WorldWidth
    {
        get { return (double)GetValue(WorldWidthProperty); }
        set { SetValue(WorldWidthProperty, value); }
    }

    public static readonly DependencyProperty WorldWidthProperty =
        DependencyProperty.Register(nameof(WorldWidth), typeof(double), typeof(Diagram),
            new PropertyMetadata(0d, (d, e) => ((Diagram)d).LayoutNodes()));

    public double MinZoom
    {
        get { return (double)GetValue(MinZoomProperty); }
        set { SetValue(MinZoomProperty, value); }
    }

    public static readonly DependencyProperty MinZoomProperty =
        DependencyProperty.Register(nameof(MinZoom), typeof(double), typeof(Diagram), new PropertyMetadata(0.2));

    public double MaxZoom
    {
        get { return (double)GetValue(MaxZoomProperty); }
        set { SetValue(MaxZoomProperty, value); }
    }

    public static readonly DependencyProperty MaxZoomProperty =
        DependencyProperty.Register(nameof(MaxZoom), typeof(double), typeof(Diagram), new PropertyMetadata(2.5));

    public bool EnableCulling
    {
        get { return (bool)GetValue(EnableCullingProperty); }
        set { SetValue(EnableCullingProperty, value); }
    }

    public static readonly DependencyProperty EnableCullingProperty =
        DependencyProperty.Register(nameof(EnableCulling), typeof(bool), typeof(Diagram),
            new PropertyMetadata(true, (d, e) => ((Diagram)d).Cull()));

    public double CullMargin
    {
        get { return (double)GetValue(CullMarginProperty); }
        set { SetValue(CullMarginProperty, value); }
    }

    public static readonly DependencyProperty CullMarginProperty =
        DependencyProperty.Register(nameof(CullMargin), typeof(double), typeof(Diagram),
            new PropertyMetadata(400d, (d, e) => ((Diagram)d).Cull()));

    public string? HighlightText
    {
        get { return (string?)GetValue(HighlightTextProperty); }
        set { SetValue(HighlightTextProperty, value); }
    }

    public static readonly DependencyProperty HighlightTextProperty =
        DependencyProperty.Register(nameof(HighlightText), typeof(string), typeof(Diagram),
            new PropertyMetadata(string.Empty, (d, e) => ((Diagram)d).ApplyFilter()));

    public DataTemplate? NodeTemplate
    {
        get { return (DataTemplate?)GetValue(NodeTemplateProperty); }
        set { SetValue(NodeTemplateProperty, value); }
    }

    public static readonly DependencyProperty NodeTemplateProperty =
        DependencyProperty.Register(nameof(NodeTemplate), typeof(DataTemplate), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).OnStructureOptionChanged()));

    public DataTemplateSelector? NodeTemplateSelector
    {
        get { return (DataTemplateSelector?)GetValue(NodeTemplateSelectorProperty); }
        set { SetValue(NodeTemplateSelectorProperty, value); }
    }

    public static readonly DependencyProperty NodeTemplateSelectorProperty =
        DependencyProperty.Register(nameof(NodeTemplateSelector), typeof(DataTemplateSelector), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).OnStructureOptionChanged()));

    public Brush? PatternBrush
    {
        get { return (Brush?)GetValue(PatternBrushProperty); }
        set { SetValue(PatternBrushProperty, value); }
    }

    public static readonly DependencyProperty PatternBrushProperty =
        DependencyProperty.Register(nameof(PatternBrush), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).BuildBackground()));

    public Brush? ConnectorBrush
    {
        get { return (Brush?)GetValue(ConnectorBrushProperty); }
        set { SetValue(ConnectorBrushProperty, value); }
    }

    public static readonly DependencyProperty ConnectorBrushProperty =
        DependencyProperty.Register(nameof(ConnectorBrush), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyEdgeBrushes()));

    public Brush? NodeBackground
    {
        get { return (Brush?)GetValue(NodeBackgroundProperty); }
        set { SetValue(NodeBackgroundProperty, value); }
    }

    public static readonly DependencyProperty NodeBackgroundProperty =
        DependencyProperty.Register(nameof(NodeBackground), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyNodeBrushes()));

    public Brush? NodeBorderBrush
    {
        get { return (Brush?)GetValue(NodeBorderBrushProperty); }
        set { SetValue(NodeBorderBrushProperty, value); }
    }

    public static readonly DependencyProperty NodeBorderBrushProperty =
        DependencyProperty.Register(nameof(NodeBorderBrush), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyNodeBrushes()));

    public Brush? NodeCaptionForeground
    {
        get { return (Brush?)GetValue(NodeCaptionForegroundProperty); }
        set { SetValue(NodeCaptionForegroundProperty, value); }
    }

    public static readonly DependencyProperty NodeCaptionForegroundProperty =
        DependencyProperty.Register(nameof(NodeCaptionForeground), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyNodeBrushes()));

    public Brush? SelectedNodeBackground
    {
        get { return (Brush?)GetValue(SelectedNodeBackgroundProperty); }
        set { SetValue(SelectedNodeBackgroundProperty, value); }
    }

    public static readonly DependencyProperty SelectedNodeBackgroundProperty =
        DependencyProperty.Register(nameof(SelectedNodeBackground), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyNodeBrushes()));

    public Brush? SelectedNodeBorderBrush
    {
        get { return (Brush?)GetValue(SelectedNodeBorderBrushProperty); }
        set { SetValue(SelectedNodeBorderBrushProperty, value); }
    }

    public static readonly DependencyProperty SelectedNodeBorderBrushProperty =
        DependencyProperty.Register(nameof(SelectedNodeBorderBrush), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyNodeBrushes()));

    public Brush? TogglerBackground
    {
        get { return (Brush?)GetValue(TogglerBackgroundProperty); }
        set { SetValue(TogglerBackgroundProperty, value); }
    }

    public static readonly DependencyProperty TogglerBackgroundProperty =
        DependencyProperty.Register(nameof(TogglerBackground), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyNodeBrushes()));

    public Brush? TogglerBorderBrush
    {
        get { return (Brush?)GetValue(TogglerBorderBrushProperty); }
        set { SetValue(TogglerBorderBrushProperty, value); }
    }

    public static readonly DependencyProperty TogglerBorderBrushProperty =
        DependencyProperty.Register(nameof(TogglerBorderBrush), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyNodeBrushes()));

    public Brush? TogglerForeground
    {
        get { return (Brush?)GetValue(TogglerForegroundProperty); }
        set { SetValue(TogglerForegroundProperty, value); }
    }

    public static readonly DependencyProperty TogglerForegroundProperty =
        DependencyProperty.Register(nameof(TogglerForeground), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyNodeBrushes()));

    public Brush? ViewportStroke
    {
        get { return (Brush?)GetValue(ViewportStrokeProperty); }
        set { SetValue(ViewportStrokeProperty, value); }
    }

    public static readonly DependencyProperty ViewportStrokeProperty =
        DependencyProperty.Register(nameof(ViewportStroke), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyViewportBrushes()));

    public Brush? ViewportFill
    {
        get { return (Brush?)GetValue(ViewportFillProperty); }
        set { SetValue(ViewportFillProperty, value); }
    }

    public static readonly DependencyProperty ViewportFillProperty =
        DependencyProperty.Register(nameof(ViewportFill), typeof(Brush), typeof(Diagram),
            new PropertyMetadata(null, (d, e) => ((Diagram)d).ApplyViewportBrushes()));

    private void OnLayoutModeChanged()
    {
        _epoch++;
        _fitPending = AutoFit;
        UpdateControlsVisibility();
        QueueRebuild();
    }

    private void OnStructureOptionChanged()
    {
        _epoch++;
        QueueRebuild();
    }

    private void OnSpacingChanged()
    {
        _layoutDirty = true;
        LayoutNodes();
    }

    private void OnAutoFitChanged()
    {
        _fitPending = AutoFit;
        TryAutoFit();
    }

    private void OnSelectionModeChanged()
    {
        if (SelectionMode == DiagramSelectionMode.None) ClearSelection();
    }
}
