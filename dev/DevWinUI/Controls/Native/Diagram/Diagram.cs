using System.Collections.Specialized;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = PartRoot, Type = typeof(Grid))]
[TemplatePart(Name = PartWorld, Type = typeof(Grid))]
[TemplatePart(Name = PartBack, Type = typeof(Canvas))]
[TemplatePart(Name = PartEdges, Type = typeof(Canvas))]
[TemplatePart(Name = PartNodes, Type = typeof(Canvas))]
[TemplatePart(Name = PartMinimap, Type = typeof(Border))]
[TemplatePart(Name = PartMiniCanvas, Type = typeof(Canvas))]
[TemplatePart(Name = PartControls, Type = typeof(Border))]
[TemplatePart(Name = PartZoomIn, Type = typeof(Button))]
[TemplatePart(Name = PartZoomOut, Type = typeof(Button))]
[TemplatePart(Name = PartFit, Type = typeof(Button))]
[TemplatePart(Name = PartArrange, Type = typeof(Button))]
[TemplatePart(Name = PartZoomText, Type = typeof(TextBlock))]
[ContentProperty(Name = nameof(Nodes))]
public sealed partial class Diagram : Control
{
    private const string PartRoot = "PART_Root";
    private const string PartWorld = "PART_World";
    private const string PartBack = "PART_Back";
    private const string PartEdges = "PART_Edges";
    private const string PartNodes = "PART_Nodes";
    private const string PartMinimap = "PART_Minimap";
    private const string PartMiniCanvas = "PART_MiniCanvas";
    private const string PartControls = "PART_Controls";
    private const string PartZoomIn = "PART_ZoomIn";
    private const string PartZoomOut = "PART_ZoomOut";
    private const string PartFit = "PART_Fit";
    private const string PartArrange = "PART_Arrange";
    private const string PartZoomText = "PART_ZoomText";

    private const double Pad = 32;
    private const double MiniW = 200, MiniH = 140, MiniPad = 8;

    private Grid? _root, _world;
    private Canvas? _back, _edges, _nodes;
    private Border? _mini, _controls;
    private Canvas? _miniCanvas, _miniWorld;
    private Rectangle? _miniView;
    private TextBlock? _zoomText;
    private Button? _arrangeButton;

    private readonly List<DiagramNodeView> _all = new();
    private readonly Dictionary<DiagramNode, DiagramNodeView> _views = new();
    private readonly Dictionary<string, DiagramNodeView> _byId = new();
    private readonly List<DiagramEdgeView> _edgeViews = new();
    private readonly Dictionary<DiagramNodeView, Rectangle> _miniRects = new();
    private readonly List<DiagramNode> _hooked = new();

    private DiagramNodeView? _pressed;
    private Point _grab, _pressScreen;
    private bool _dragMoved;
    private int _z;
    private int _epoch;
    private bool _rebuildQueued, _layoutDirty = true, _fitPending;

    private DiagramNode? _anchor;
    private DiagramNode? _pendingFocus;
    private Point _anchorScreen;

    private readonly CompositeTransform _xf = new();
    private readonly TranslateTransform _bgXf = new();
    private double _zoom = 1, _panX, _panY;
    private bool _panning, _panMoved, _panLeft;
    private Point _panLast, _panStart;

    private bool _boundsDirty = true, _hasBounds;
    private double _bMinX, _bMinY, _bMaxX, _bMaxY;

    private readonly CompositeTransform _miniXf = new();
    private double _miniScale = 1, _miniOx, _miniOy;
    private bool _miniDragging;

    public Diagram()
    {
        DefaultStyleKey = typeof(Diagram);

        Nodes = new ObservableCollection<DiagramNode>();
        Edges = new ObservableCollection<DiagramEdge>();
        Nodes.CollectionChanged += OnNodesChanged;
        Edges.CollectionChanged += (_, _) => QueueRebuild();
        SelectedNodes.CollectionChanged += OnSelectedNodesChanged;
        PinTypes = new ObservableCollection<DiagramPinType>(DiagramPinType.CreateDefaults());
        PinTypes.CollectionChanged += OnPinTypesChanged;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
    }

    public event EventHandler<DiagramNodeEventArgs>? NodeMoved;

    public event EventHandler<DiagramNodeEventArgs>? NodeSelect;

    public event EventHandler<DiagramNodeEventArgs>? NodeUnselect;

    public event EventHandler<DiagramNodeEventArgs>? NodeExpand;

    public event EventHandler<DiagramNodeEventArgs>? NodeCollapse;

    public event EventHandler<DiagramConditionEventArgs>? ConditionChanged;

    public ObservableCollection<DiagramNode> Nodes { get; }

    public ObservableCollection<DiagramEdge> Edges { get; }

    public ObservableCollection<DiagramNode> SelectedNodes { get; } = new();

    public void Refresh()
    {
        _epoch++;
        Rebuild();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _root = GetTemplateChild(PartRoot) as Grid;
        _world = GetTemplateChild(PartWorld) as Grid;
        _back = GetTemplateChild(PartBack) as Canvas;
        _edges = GetTemplateChild(PartEdges) as Canvas;
        _nodes = GetTemplateChild(PartNodes) as Canvas;
        _mini = GetTemplateChild(PartMinimap) as Border;
        _miniCanvas = GetTemplateChild(PartMiniCanvas) as Canvas;
        _controls = GetTemplateChild(PartControls) as Border;
        _zoomText = GetTemplateChild(PartZoomText) as TextBlock;
        _arrangeButton = GetTemplateChild(PartArrange) as Button;
        BindPanels();
        BindDock();
        UpdateDockViewport();

        Wire(GetTemplateChild(PartZoomIn) as Button, () => ZoomBy(1.25));
        Wire(GetTemplateChild(PartZoomOut) as Button, () => ZoomBy(0.8));
        Wire(GetTemplateChild(PartFit) as Button, () => FitToContent());
        Wire(_arrangeButton, ArrangeAll);

        if (_world != null) _world.RenderTransform = _xf;
        if (_back != null) _back.RenderTransform = _bgXf;

        if (_root != null)
        {
            _root.PointerPressed += Root_PointerPressed;
            _root.PointerMoved += Root_PointerMoved;
            _root.PointerReleased += Root_PointerReleased;
            _root.PointerCaptureLost += Root_PointerCaptureLost;
            _root.PointerWheelChanged += Root_PointerWheelChanged;
        }

        if (_mini != null)
        {
            _mini.PointerPressed += Mini_PointerPressed;
            _mini.PointerMoved += Mini_PointerMoved;
            _mini.PointerReleased += Mini_PointerReleased;
            _mini.PointerCaptureLost += Mini_PointerCaptureLost;
            _mini.PointerWheelChanged += Mini_PointerWheelChanged;
        }

        _fitPending = AutoFit;
        UpdateControlsVisibility();
        OnShowMinimapChanged();
        BuildLegend();
        UpdateHint();
        Rebuild();
        BuildBackground();
        ApplyView();
    }

    private static void Wire(Button? button, Action action)
    {
        if (button != null) button.Click += (_, _) => action();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Hook();
        WatchViewport();
        QueueRebuild();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Unhook();
        UnwatchViewport();
        StopAnimation();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateDockViewport();
        if (_root != null)
            _root.Clip = new RectangleGeometry { Rect = new Rect(0, 0, ActualWidth, ActualHeight) };

        BuildBackground();
        LayoutNodes();
    }

    private void OnNodesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (IsLoaded) Hook();
        QueueRebuild();
    }

    private void OnRootChanged()
    {
        if (IsLoaded) Hook();
        QueueRebuild();
    }

    private IEnumerable<DiagramNode> TopLevel()
    {
        if (Root != null) yield return Root;
        foreach (var n in Nodes) yield return n;
    }

    private IEnumerable<DiagramNode> EnumerateNodes()
    {
        var stack = new Stack<DiagramNode>();
        foreach (var t in TopLevel().Reverse()) stack.Push(t);

        var seen = new HashSet<DiagramNode>();
        while (stack.Count > 0)
        {
            var n = stack.Pop();
            if (!seen.Add(n)) continue;

            yield return n;
            foreach (var c in n.Children.Reverse()) stack.Push(c);
        }
    }

    private void Hook()
    {
        Unhook();
        foreach (var n in TopLevel())
        {
            n.Changed += OnNodeChanged;
            _hooked.Add(n);
        }
    }

    private void Unhook()
    {
        foreach (var n in _hooked) n.Changed -= OnNodeChanged;
        _hooked.Clear();
    }

    private void OnNodeChanged(object? sender, EventArgs e) => QueueRebuild();

    private void QueueRebuild()
    {
        if (_rebuildQueued) return;

        _rebuildQueued = true;
        if (!DispatcherQueue.TryEnqueue(() => { _rebuildQueued = false; Rebuild(); }))
        {
            _rebuildQueued = false;
            Rebuild();
        }
    }

    private static void Collect(DiagramNode node, DiagramNode? parent, List<DiagramNode> list,
        List<(DiagramNode, DiagramNode)> implied, HashSet<DiagramNode> seen)
    {
        if (parent != null) implied.Add((parent, node));
        if (!seen.Add(node)) return;

        list.Add(node);
        foreach (var c in node.Children) Collect(c, node, list, implied, seen);
    }

    private void Rebuild()
    {
        if (_root == null || _nodes == null || _edges == null) return;

        _pressed = null;
        _link = null;
        _nodes.Children.Clear();
        _edges.Children.Clear();
        _all.Clear();
        _views.Clear();
        _byId.Clear();
        _edgeViews.Clear();
        _boundsDirty = true;
        _layoutDirty = true;

        var list = new List<DiagramNode>();
        var implied = new List<(DiagramNode, DiagramNode)>();
        var seen = new HashSet<DiagramNode>();
        foreach (var top in TopLevel()) Collect(top, null, list, implied, seen);

        var selected = new HashSet<DiagramNode>(SelectedNodes);
        var full = new Dictionary<DiagramNode, DiagramNodeView>();
        var order = new List<DiagramNodeView>();

        foreach (var node in list)
        {
            double w = node.W > 0 ? node.W : Math.Max(40, DefaultNodeWidth);
            double ph = node.HasPins && node.H <= 0 ? PinNodeHeight(node) : 0;
            foreach (var pin in node.Inputs) pin.IsOutput = false;
            foreach (var pin in node.Outputs) pin.IsOutput = true;
            var nv = new DiagramNodeView
            {
                Node = node,
                HueBrush = new SolidColorBrush(node.Hue),
                Width = w,
                FixedHeight = ph > 0 ? ph : Math.Max(0, node.H),
                LastHeight = ph > 0 ? ph : node.H > 0 ? node.H : EstimateHeight(node, w),
                Selected = selected.Contains(node),
            };
            full[node] = nv;
            order.Add(nv);
            if (!string.IsNullOrEmpty(node.Id)) _byId[node.Id!] = nv;
        }

        var links = new List<DiagramEdgeView>();
        var pairs = new HashSet<(DiagramNodeView, DiagramNodeView)>();
        var pinKeys = new HashSet<(DiagramNodeView, DiagramNodeView, DiagramPin, DiagramPin)>();

        void Link(DiagramNodeView a, DiagramNodeView b, DiagramEdge? edge)
        {
            if (a == b) return;

            var fk = edge?.FromPin;
            var tk = edge?.ToPin;
            var fp = fk != null ? a.Node.Outputs.FirstOrDefault(p => p.Key == fk) : null;
            var tp = tk != null ? b.Node.Inputs.FirstOrDefault(p => p.Key == tk) : null;
            bool pinned = fp != null && tp != null;

            if (pinned ? !pinKeys.Add((a, b, fp!, tp!)) : !pairs.Add((a, b))) return;

            links.Add(new DiagramEdgeView
            {
                Edge = edge,
                From = a,
                To = b,
                FromPin = pinned ? fp : null,
                ToPin = pinned ? tp : null,
                Label = edge?.Label,
                Dashed = edge?.IsDashed ?? false,
                Animated = edge?.IsAnimated ?? false,
            });

            if (pinned) return;

            a.Children.Add(b);
            b.ParentCount++;
        }

        foreach (var (p, c) in implied) Link(full[p], full[c], null);
        foreach (var edge in Edges)
        {
            if (edge.From == null || edge.To == null) continue;
            if (_byId.TryGetValue(edge.From, out var a) && _byId.TryGetValue(edge.To, out var b)) Link(a, b, edge);
        }

        ResolveTypes(order, links);

        var visible = new HashSet<DiagramNodeView>();
        if (Collapsible)
        {
            var stack = new Stack<DiagramNodeView>();
            foreach (var r in order)
                if (r.ParentCount == 0) stack.Push(r);

            if (stack.Count == 0)
            {
                foreach (var r in order) visible.Add(r);
            }

            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                if (!visible.Add(cur) || !cur.Node.Expanded) continue;
                foreach (var child in cur.Children) stack.Push(child);
            }
        }
        else
        {
            foreach (var r in order) visible.Add(r);
        }

        foreach (var nv in order)
        {
            if (!visible.Contains(nv)) continue;

            nv.Match = Matches(nv.Node);
            _all.Add(nv);
            _views[nv.Node] = nv;
        }

        foreach (var ev in links)
            if (visible.Contains(ev.From) && visible.Contains(ev.To)) _edgeViews.Add(ev);

        RebuildMinimap();
        LayoutNodes();

        if (_anchor != null && _views.ContainsKey(_anchor))
        {
            _panX = _anchorScreen.X - _anchor.Left * _zoom;
            _panY = _anchorScreen.Y - _anchor.Top * _zoom;
            ApplyView();
        }

        _anchor = null;
        UpdateConflicts();
        UpdateAnimation();

        if (_pendingFocus is { } focus)
        {
            _pendingFocus = null;
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
            {
                if (_views.TryGetValue(focus, out var fv)) fv.Toggler?.Focus(FocusState.Programmatic);
            });
        }
    }

    private bool Matches(DiagramNode node)
    {
        var text = HighlightText;
        if (string.IsNullOrWhiteSpace(text)) return true;

        bool Has(string? s) => s != null && s.Contains(text, StringComparison.OrdinalIgnoreCase);
        return Has(node.Title) || Has(node.Caption) || Has(node.Data?.ToString());
    }

    private void ApplyFilter()
    {
        foreach (var v in _all)
        {
            v.Match = Matches(v.Node);
            UpdateFace(v);
            UpdateMiniRect(v);
        }
    }

    private void UpdateControlsVisibility()
    {
        if (_controls != null) _controls.Visibility = ShowControls ? Visibility.Visible : Visibility.Collapsed;
        if (_arrangeButton != null) _arrangeButton.Visibility = Layout == DiagramLayout.Layered ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateZoomText()
    {
        if (_zoomText != null) _zoomText.Text = $"{Math.Round(_zoom * 100)}%";
    }
}
