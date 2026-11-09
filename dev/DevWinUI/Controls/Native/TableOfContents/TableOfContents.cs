using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using CompositionTarget = Microsoft.UI.Xaml.Media.CompositionTarget;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

public sealed record TableOfContentsItem(string Id, string Label, int Depth = 2);

[TemplatePart(Name = PartRows, Type = typeof(StackPanel))]
[TemplatePart(Name = PartTrack, Type = typeof(Path))]
[TemplatePart(Name = PartAccentHost, Type = typeof(Canvas))]
[TemplatePart(Name = PartAccent, Type = typeof(Path))]
[TemplatePart(Name = PartDot, Type = typeof(Ellipse))]
public sealed partial class TableOfContents : Control
{
    const string PartRows = "PART_Rows";
    const string PartTrack = "PART_Track";
    const string PartAccentHost = "PART_AccentHost";
    const string PartAccent = "PART_Accent";
    const string PartDot = "PART_Dot";

    const double X2 = 2;
    const double X3 = 10;
    const double Bend = 8;
    const double DotSize = 7;
    const int CurveSamples = 12;
    const int DotKeyFrames = 16;

    static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(200);

    readonly List<Button> _buttons = new();
    readonly List<TextBlock> _labels = new();
    readonly List<int> _depths = new();
    readonly List<(double Top, double Bottom)> _bounds = new();
    readonly List<Point> _samples = new();
    readonly List<string> _scratch = new();
    readonly HashSet<string> _activeSet = new();

    StackPanel? _rows;
    Path? _track;
    Canvas? _accentHost;
    Path? _accent;
    Ellipse? _dot;
    Dictionary<string, FrameworkElement>? _targets;
    ScrollViewer? _hooked;
    InsetClip? _clip;
    bool _loaded, _pending, _dotPlaced, _scrollingUp, _appliedUp;
    double _dotY, _lastOffset;

    public TableOfContents()
    {
        DefaultStyleKey = typeof(TableOfContents);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public event TypedEventHandler<TableOfContents, IReadOnlyList<string>>? ActiveChanged;
    public event TypedEventHandler<TableOfContents, string>? ItemInvoked;

    public Func<string, FrameworkElement?>? TargetResolver { get; set; }

    public void Refresh()
    {
        _targets = null;
        ScheduleUpdate();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_rows is not null) _rows.SizeChanged -= OnRowsSizeChanged;

        _rows = GetTemplateChild(PartRows) as StackPanel;
        _track = GetTemplateChild(PartTrack) as Path;
        _accentHost = GetTemplateChild(PartAccentHost) as Canvas;
        _accent = GetTemplateChild(PartAccent) as Path;
        _dot = GetTemplateChild(PartDot) as Ellipse;

        _clip = null;
        _dotPlaced = false;

        if (_dot is not null) ElementCompositionPreview.SetIsTranslationEnabled(_dot, true);
        if (_rows is not null) _rows.SizeChanged += OnRowsSizeChanged;

        RebuildRows();
    }

    static Point Cubic(Point p0, Point p1, Point p2, Point p3, double t)
    {
        double u = 1 - t;
        double a = u * u * u, b = 3 * u * u * t, c = 3 * u * t * t, d = t * t * t;
        return new Point(a * p0.X + b * p1.X + c * p2.X + d * p3.X,
                         a * p0.Y + b * p1.Y + c * p2.Y + d * p3.Y);
    }

    void OnRowsSizeChanged(object sender, SizeChangedEventArgs e) => RebuildRail();

    void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        Hook();
        _rows?.UpdateLayout();
        RebuildRail();
        ScheduleUpdate();
    }

    void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        Unhook();
        CompositionTarget.Rendering -= OnRendering;
        _pending = false;
    }

    void Hook()
    {
        Unhook();
        if (!_loaded || Root is not { } root) return;

        root.ViewChanged += OnViewChanged;
        root.SizeChanged += OnRootSizeChanged;
        _hooked = root;
        _lastOffset = root.VerticalOffset;
        _targets = null;
        ScheduleUpdate();
    }

    void Unhook()
    {
        if (_hooked is null) return;

        _hooked.ViewChanged -= OnViewChanged;
        _hooked.SizeChanged -= OnRootSizeChanged;
        _hooked = null;
    }

    void OnViewChanged(object? sender, ScrollViewerViewChangedEventArgs e) => ScheduleUpdate();

    void OnRootSizeChanged(object sender, SizeChangedEventArgs e) => ScheduleUpdate();

    void RebuildRows()
    {
        if (_rows is null) return;

        foreach (var old in _buttons)
        {
            old.Click -= OnLinkClick;
            old.PointerEntered -= OnLinkPointerEntered;
            old.PointerExited -= OnLinkPointerExited;
        }

        _rows.Children.Clear();
        _buttons.Clear();
        _labels.Clear();
        _depths.Clear();
        _targets = null;
        _dotPlaced = false;

        var items = Items ?? Array.Empty<TableOfContentsItem>();
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            int depth = item.Depth >= 3 ? 3 : 2;

            var label = new TextBlock
            {
                Text = item.Label,
                FontSize = LinkFontSize,
                TextWrapping = TextWrapping.Wrap,
                Foreground = LinkForeground,
            };
            var button = new Button
            {
                Content = label,
                Tag = item.Id,
                Padding = new Thickness(depth == 3 ? 12 : 0, 6, 0, 6),
            };
            if (LinkStyle is { } style) button.Style = style;

            button.Click += OnLinkClick;
            button.PointerEntered += OnLinkPointerEntered;
            button.PointerExited += OnLinkPointerExited;

            _rows.Children.Add(button);
            _buttons.Add(button);
            _labels.Add(label);
            _depths.Add(depth);
        }

        if (IsLoaded)
        {
            _rows.UpdateLayout();
            RebuildRail();
        }

        ApplyActive(false);
        ScheduleUpdate();
    }

    void OnLinkClick(object sender, RoutedEventArgs e) => OnItemClick((string)((Button)sender).Tag);

    void OnLinkPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        int i = _buttons.IndexOf((Button)sender);
        if (i >= 0) _labels[i].Foreground = ActiveLinkForeground;
    }

    void OnLinkPointerExited(object sender, PointerRoutedEventArgs e) => PaintRow(_buttons.IndexOf((Button)sender));

    void PaintRow(int i)
    {
        if (i < 0 || i >= _labels.Count) return;
        _labels[i].Foreground = _activeSet.Contains((string)_buttons[i].Tag) ? ActiveLinkForeground : LinkForeground;
    }

    void OnItemClick(string id)
    {
        ItemInvoked?.Invoke(this, id);

        if (Root is not { } root || root.Content is not UIElement content || Resolve(id) is not { } target) return;

        try
        {
            double y = target.TransformToVisual(content).TransformPoint(default).Y;
            root.ChangeView(null, Math.Max(0, y - ScrollOffset), null, disableAnimation: !IsAnimationEnabled);
        }
        catch (Exception)
        {
        }
    }

    double RailX(int i) => _depths[i] >= 3 ? X3 : X2;

    void RebuildRail()
    {
        if (_rows is null || _track is null || _accent is null || _accentHost is null) return;

        _bounds.Clear();
        _samples.Clear();
        double height = _rows.ActualHeight;
        _accentHost.Height = height;

        int n = _buttons.Count;
        if (n == 0 || height <= 0)
        {
            _track.Data = null;
            _accent.Data = null;
            return;
        }

        for (int i = 0; i < n; i++)
        {
            double y = _buttons[i].TransformToVisual(_rows).TransformPoint(default).Y;
            _bounds.Add((y, y + _buttons[i].ActualHeight));
        }

        _track.Data = BuildGeometry(collect: true);
        _accent.Data = BuildGeometry(collect: false);
        _dotPlaced = false;
        ApplyActive(false);
    }

    PathGeometry BuildGeometry(bool collect)
    {
        int n = _bounds.Count;
        bool curve = Kind == TableOfContentsKind.Curve;

        double Start(int i)
        {
            var (t, b) = _bounds[i];
            return t + (i > 0 && RailX(i - 1) != RailX(i) ? Math.Min(Bend, (b - t) / 2) : 0);
        }

        double End(int i)
        {
            var (t, b) = _bounds[i];
            return b - (i < n - 1 && RailX(i + 1) != RailX(i) ? Math.Min(Bend, (b - t) / 2) : 0);
        }

        void Sample(Point p)
        {
            if (collect) _samples.Add(p);
        }

        var figure = new PathFigure { StartPoint = new Point(RailX(0), Start(0)), IsClosed = false, IsFilled = false };
        Sample(figure.StartPoint);

        for (int i = 0; i < n; i++)
        {
            var end = new Point(RailX(i), End(i));
            figure.Segments.Add(new LineSegment { Point = end });
            Sample(end);
            if (i == n - 1) break;

            var next = new Point(RailX(i + 1), Start(i + 1));
            if (RailX(i) == RailX(i + 1))
            {
                figure.Segments.Add(new LineSegment { Point = next });
                Sample(next);
                continue;
            }

            double mid = (end.Y + next.Y) / 2;
            var c1 = new Point(end.X, mid);
            var c2 = new Point(next.X, mid);

            if (curve)
            {
                figure.Segments.Add(new BezierSegment { Point1 = c1, Point2 = c2, Point3 = next });
                if (collect)
                    for (int s = 1; s <= CurveSamples; s++) _samples.Add(Cubic(end, c1, c2, next, s / (double)CurveSamples));
            }
            else
            {
                figure.Segments.Add(new LineSegment { Point = c1 });
                Sample(c1);
                figure.Segments.Add(new LineSegment { Point = c2 });
                Sample(c2);
                figure.Segments.Add(new LineSegment { Point = next });
                Sample(next);
            }
        }

        var geometry = new PathGeometry();
        geometry.Figures.Add(figure);
        return geometry;
    }

    Point PointAtY(double y)
    {
        if (_samples.Count == 0) return new Point(X2, y);
        if (y <= _samples[0].Y) return _samples[0];

        for (int i = 1; i < _samples.Count; i++)
        {
            var a = _samples[i - 1];
            var b = _samples[i];
            if (y > b.Y) continue;

            double dy = b.Y - a.Y;
            if (dy <= 1e-6) return b;

            double t = (y - a.Y) / dy;
            return new Point(a.X + (b.X - a.X) * t, y);
        }

        return _samples[^1];
    }

    void ScheduleUpdate()
    {
        if (_pending || !_loaded) return;

        _pending = true;
        CompositionTarget.Rendering += OnRendering;
    }

    void OnRendering(object? sender, object e)
    {
        CompositionTarget.Rendering -= OnRendering;
        _pending = false;
        UpdateActiveFromScroll();
    }

    FrameworkElement? Resolve(string id)
    {
        if (TargetResolver is { } resolver) return resolver(id);
        if (_targets is null) BuildTargets();
        return _targets!.TryGetValue(id, out var element) ? element : null;
    }

    void BuildTargets()
    {
        var map = new Dictionary<string, FrameworkElement>();
        _targets = map;
        if (Root?.Content is not DependencyObject start) return;

        void Walk(DependencyObject node)
        {
            if (node is FrameworkElement element && GetHeadingId(element) is { Length: > 0 } id) map[id] = element;

            int count = VisualTreeHelper.GetChildrenCount(node);
            for (int i = 0; i < count; i++) Walk(VisualTreeHelper.GetChild(node, i));
        }

        Walk(start);
    }

    void UpdateActiveFromScroll()
    {
        if (!TrackScroll || Root is not { } root || Items is not { Count: > 0 } items) return;

        double offset = root.VerticalOffset;
        if (offset != _lastOffset)
        {
            _scrollingUp = offset < _lastOffset;
            _lastOffset = offset;
        }

        double viewTop = ScrollOffset;
        double viewBottom = root.ViewportHeight;
        string? lastPassed = null;
        _scratch.Clear();

        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            if (Resolve(item.Id) is not { } element) continue;

            double y;
            try
            {
                y = element.TransformToVisual(root).TransformPoint(default).Y;
            }
            catch (Exception)
            {
                continue;
            }

            if (y + element.ActualHeight <= viewTop) lastPassed = item.Id;
            else if (y < viewBottom) _scratch.Add(item.Id);
        }

        if (_scratch.Count == 0 && lastPassed is not null) _scratch.Add(lastPassed);

        if (!MatchesActive(_scratch)) ActiveIds = _scratch.ToArray();
        else if (_appliedUp != _scrollingUp) ApplyActive(true);
    }

    bool MatchesActive(List<string> ids)
    {
        var active = ActiveIds;
        if (active is null) return ids.Count == 0;
        if (active.Count != ids.Count) return false;

        for (int i = 0; i < ids.Count; i++)
            if (active[i] != ids[i]) return false;
        return true;
    }

    void OnActiveIdsChanged()
    {
        ApplyActive(true);
        ActiveChanged?.Invoke(this, ActiveIds ?? Array.Empty<string>());
    }

    void ApplyActive(bool animate)
    {
        _activeSet.Clear();
        if (ActiveIds is { } ids)
            for (int i = 0; i < ids.Count; i++) _activeSet.Add(ids[i]);

        _appliedUp = _scrollingUp;

        double top = double.MaxValue, bottom = double.MinValue;
        for (int i = 0; i < _buttons.Count; i++)
        {
            bool on = _activeSet.Contains((string)_buttons[i].Tag);
            _labels[i].Foreground = on ? ActiveLinkForeground : LinkForeground;
            AutomationProperties.SetItemStatus(_buttons[i], on ? "current" : string.Empty);

            if (on && i < _bounds.Count)
            {
                top = Math.Min(top, _bounds[i].Top);
                bottom = Math.Max(bottom, _bounds[i].Bottom);
            }
        }

        if (_accentHost is null || _dot is null) return;

        bool any = top <= bottom;
        animate &= _loaded && IsAnimationEnabled;
        UpdateClip(_accentHost, any, top, bottom, animate);
        UpdateDot(_dot, any, any ? (_scrollingUp ? top : bottom) : 0, animate);
    }

    void UpdateClip(Canvas host, bool any, double top, double bottom, bool animate)
    {
        var visual = ElementCompositionPreview.GetElementVisual(host);
        var compositor = visual.Compositor;
        _clip ??= compositor.CreateInsetClip();
        visual.Clip = _clip;

        double height = double.IsNaN(host.Height) ? 0 : host.Height;
        float topInset = any ? (float)top : 0f;
        float bottomInset = any ? (float)(height - bottom) : (float)height;

        _clip.StopAnimation("TopInset");
        _clip.StopAnimation("BottomInset");

        if (!animate)
        {
            _clip.TopInset = topInset;
            _clip.BottomInset = bottomInset;
            return;
        }

        var ease = compositor.CreateCubicBezierEasingFunction(new Vector2(0.2f, 0f), new Vector2(0f, 1f));

        ScalarKeyFrameAnimation Create(float to)
        {
            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(1f, to, ease);
            animation.Duration = AnimationDuration;
            return animation;
        }

        _clip.StartAnimation("TopInset", Create(topInset));
        _clip.StartAnimation("BottomInset", Create(bottomInset));
    }

    void UpdateDot(Ellipse dot, bool any, double toY, bool animate)
    {
        dot.Opacity = Indicator && any ? 1 : 0;
        if (!any || _samples.Count == 0) return;

        var visual = ElementCompositionPreview.GetElementVisual(dot);

        Vector3 At(double y)
        {
            var p = PointAtY(y);
            return new Vector3((float)(p.X - DotSize / 2), (float)(p.Y - DotSize / 2), 0);
        }

        visual.StopAnimation("Translation");

        if (!animate || !_dotPlaced)
        {
            visual.Properties.InsertVector3("Translation", At(toY));
            _dotPlaced = true;
            _dotY = toY;
            return;
        }

        var animation = visual.Compositor.CreateVector3KeyFrameAnimation();
        double from = _dotY;
        for (int s = 1; s <= DotKeyFrames; s++)
        {
            float t = s / (float)DotKeyFrames;
            double eased = 1 - Math.Pow(1 - t, 3);
            animation.InsertKeyFrame(t, At(from + (toY - from) * eased));
        }

        animation.Duration = AnimationDuration;
        visual.StartAnimation("Translation", animation);
        _dotY = toY;
    }
}
