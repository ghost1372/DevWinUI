using System.Collections.Specialized;
using System.ComponentModel;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[ContentProperty(Name = nameof(Items))]
[TemplatePart(Name = PartRoot, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartRows, Type = typeof(StackPanel))]
[TemplatePart(Name = PartTitle, Type = typeof(TextBlock))]
[TemplatePart(Name = PartRail, Type = typeof(Canvas))]
[TemplatePart(Name = PartRailBase, Type = typeof(Path))]
[TemplatePart(Name = PartRailProgress, Type = typeof(Path))]
[TemplatePart(Name = PartDot, Type = typeof(Ellipse))]
public sealed partial class AnimatedToc : Control
{
    private const string PartRoot = "PART_Root";
    private const string PartRows = "PART_Rows";
    private const string PartTitle = "PART_Title";
    private const string PartRail = "PART_Rail";
    private const string PartRailBase = "PART_RailBase";
    private const string PartRailProgress = "PART_RailProgress";
    private const string PartDot = "PART_Dot";

    private const string RailPaddingKey = "TocRailPadding";
    private const string LabelGapKey = "TocLabelGap";
    private const string RowEndPaddingKey = "TocRowEndPadding";
    private const string RowStyleKey = "TocRowStyle";
    private const string LabelStyleKey = "TocLabelStyle";
    private const string SelectedLabelStyleKey = "TocSelectedLabelStyle";

    private const double FallbackDotSize = 16;
    private const double FallbackRailPadding = 12;
    private const double FallbackLabelGap = 16;
    private const double FallbackRowEndPadding = 12;

    private const int BendSamples = 20;
    private const double BendInset = 0.25;
    private const double Stiffness = 170;
    private const double Damping = 26;
    private const double MaxFrameTime = 0.05;
    private const double SubStep = 0.004;
    private const double SettleDistance = 0.05;
    private const double SettleVelocity = 0.5;
    private const double MinSegment = 1e-3;

    private readonly List<AnimatedTocItem> _tracked = new();
    private readonly List<Button> _buttons = new();
    private readonly List<TextBlock> _labels = new();
    private readonly List<Point> _points = new();
    private readonly RoutedEventHandler _clickHandler;
    private readonly DispatcherQueueHandler _rebuildHandler;

    private FrameworkElement _root = null!;
    private StackPanel _rows = null!;
    private TextBlock _title = null!;
    private Canvas _rail = null!;
    private Path _railBase = null!;
    private Path _progress = null!;
    private Ellipse _dot = null!;
    private AnimatedTocRailGeometry _baseRail = null!;
    private AnimatedTocRailGeometry _progressRail = null!;

    private Style? _rowStyle;
    private Style? _labelStyle;
    private Style? _selectedLabelStyle;
    private double _dotSize = FallbackDotSize;
    private double _railPadding = FallbackRailPadding;
    private double _labelGap = FallbackLabelGap;
    private double _rowEndPadding = FallbackRowEndPadding;

    private double[] _cumulative = Array.Empty<double>();
    private double[] _rowLength = Array.Empty<double>();
    private double _total;

    private double _position;
    private double _velocity;
    private double _goal;
    private TimeSpan _lastFrame;
    private int _appliedSelection = -1;

    private bool _templated;
    private bool _loaded;
    private bool _wasUnloaded;
    private bool _rendering;
    private bool _hasLastFrame;
    private bool _progressVisible;
    private bool _rebuildPending;

    public event EventHandler<int>? ItemInvoked;

    public AnimatedToc()
    {
        DefaultStyleKey = typeof(AnimatedToc);

        _clickHandler = OnRowClick;
        _rebuildHandler = OnRebuildRequested;

        Items.CollectionChanged += OnItemsChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _templated = false;

        var root = GetTemplateChild(PartRoot) as FrameworkElement;
        var rows = GetTemplateChild(PartRows) as StackPanel;
        var title = GetTemplateChild(PartTitle) as TextBlock;
        var rail = GetTemplateChild(PartRail) as Canvas;
        var railBase = GetTemplateChild(PartRailBase) as Path;
        var progress = GetTemplateChild(PartRailProgress) as Path;
        var dot = GetTemplateChild(PartDot) as Ellipse;

        if (root is null || rows is null || title is null || rail is null ||
            railBase is null || progress is null || dot is null) return;

        _root = root;
        _rows = rows;
        _title = title;
        _rail = rail;
        _railBase = railBase;
        _progress = progress;
        _dot = dot;
        _baseRail = new AnimatedTocRailGeometry();
        _progressRail = new AnimatedTocRailGeometry();
        _progressVisible = false;
        _templated = true;

        CacheResources();
        Rebuild();
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new AnimatedTocAutomationPeer(this);

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        AttachItems();

        if (_wasUnloaded)
        {
            _wasUnloaded = false;
            RequestRebuild();
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        _wasUnloaded = true;
        DetachItems();
        StopRendering();

        if (_templated)
        {
            _position = _goal;
            _velocity = 0;
            Render();
        }
    }

    private void AttachItems()
    {
        DetachItems();

        foreach (var item in Items)
        {
            item.PropertyChanged += OnItemChanged;
            _tracked.Add(item);
        }
    }

    private void DetachItems()
    {
        foreach (var item in _tracked) item.PropertyChanged -= OnItemChanged;
        _tracked.Clear();
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_loaded) AttachItems();
        RequestRebuild();
    }

    private void OnItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(AnimatedTocItem.Label) or nameof(AnimatedTocItem.Depth)) RequestRebuild();
    }

    private void CacheResources()
    {
        var resources = _root.Resources;

        _railPadding = ReadDouble(resources, RailPaddingKey, FallbackRailPadding);
        _labelGap = ReadDouble(resources, LabelGapKey, FallbackLabelGap);
        _rowEndPadding = ReadDouble(resources, RowEndPaddingKey, FallbackRowEndPadding);
        _rowStyle = ReadStyle(resources, RowStyleKey);
        _labelStyle = ReadStyle(resources, LabelStyleKey);
        _selectedLabelStyle = ReadStyle(resources, SelectedLabelStyleKey);
        _dotSize = double.IsNaN(_dot.Width) ? FallbackDotSize : _dot.Width;
    }

    private static double ReadDouble(ResourceDictionary resources, string key, double fallback) =>
        resources.TryGetValue(key, out var value) && value is double number ? number : fallback;

    private static Style? ReadStyle(ResourceDictionary resources, string key) =>
        resources.TryGetValue(key, out var value) ? value as Style : null;

    private static void ApplyStyle(FrameworkElement target, Style? style)
    {
        if (style is not null) target.Style = style;
    }

    private double RailX(int depth) => _railPadding + Math.Max(0, depth) * Indent;

    private void RequestRebuild()
    {
        if (!_templated || _rebuildPending) return;

        _rebuildPending = true;
        if (!DispatcherQueue.TryEnqueue(_rebuildHandler)) _rebuildPending = false;
    }

    private void OnRebuildRequested()
    {
        if (_rebuildPending) Rebuild();
    }

    private void Rebuild()
    {
        _rebuildPending = false;
        if (!_templated) return;

        string title = Title ?? string.Empty;
        _title.Text = title;
        _title.Visibility = title.Length == 0 ? Visibility.Collapsed : Visibility.Visible;

        foreach (var button in _buttons) button.Click -= _clickHandler;
        _rows.Children.Clear();
        _buttons.Clear();
        _labels.Clear();

        for (int i = 0; i < Items.Count; i++)
        {
            var item = Items[i];

            var label = new TextBlock { Text = item.Label };
            ApplyStyle(label, _labelStyle);

            var button = new Button
            {
                Height = RowHeight,
                Padding = new Thickness(RailX(item.Depth) + _labelGap, 0, _rowEndPadding, 0),
                Content = label,
            };
            ApplyStyle(button, _rowStyle);
            button.Click += _clickHandler;

            _labels.Add(label);
            _buttons.Add(button);
            _rows.Children.Add(button);
        }

        _appliedSelection = -1;

        RebuildGeometry();
        ApplySelection(animate: false);
    }

    private void OnRowClick(object sender, RoutedEventArgs e)
    {
        int index = _rows.Children.IndexOf((UIElement)sender);
        if (index < 0) return;

        SelectedIndex = index;
        ItemInvoked?.Invoke(this, index);
    }

    private void RebuildGeometry()
    {
        _points.Clear();

        int count = Items.Count;
        double height = RowHeight;
        var rowPoint = new int[count];

        if (count > 0)
        {
            _points.Add(new Point(RailX(Items[0].Depth), 0.5 * height));

            for (int i = 0; i < count - 1; i++)
            {
                double xFrom = RailX(Items[i].Depth);
                double xTo = RailX(Items[i + 1].Depth);
                double yFrom = (i + 0.5) * height;
                double yTo = (i + 1.5) * height;

                if (xFrom != xTo)
                {
                    double y0 = yFrom + BendInset * height;
                    double y1 = yTo - BendInset * height;
                    double ym = (y0 + y1) / 2;

                    _points.Add(new Point(xFrom, y0));

                    for (int s = 1; s <= BendSamples; s++)
                    {
                        double t = (double)s / BendSamples;
                        double u = 1 - t;
                        double x = u * u * u * xFrom + 3 * u * u * t * xFrom + 3 * u * t * t * xTo + t * t * t * xTo;
                        double y = u * u * u * y0 + 3 * u * u * t * ym + 3 * u * t * t * ym + t * t * t * y1;
                        _points.Add(new Point(x, y));
                    }
                }

                _points.Add(new Point(xTo, yTo));
                rowPoint[i + 1] = _points.Count - 1;
            }
        }

        _cumulative = new double[_points.Count];
        for (int i = 1; i < _points.Count; i++)
        {
            double dx = _points[i].X - _points[i - 1].X;
            double dy = _points[i].Y - _points[i - 1].Y;
            _cumulative[i] = _cumulative[i - 1] + Math.Sqrt(dx * dx + dy * dy);
        }

        _total = _points.Count > 0 ? _cumulative[^1] : 0;

        _rowLength = new double[count];
        double maxX = 0;
        for (int i = 0; i < count; i++)
        {
            _rowLength[i] = _cumulative[rowPoint[i]];
            maxX = Math.Max(maxX, RailX(Items[i].Depth));
        }

        _rail.Width = maxX + _dotSize;
        _rail.Height = count * height;

        var basePoints = _baseRail.Segment.Points;
        basePoints.Clear();
        for (int i = 1; i < _points.Count; i++) basePoints.Add(_points[i]);

        if (_points.Count >= 2)
        {
            _baseRail.Figure.StartPoint = _points[0];
            _progressRail.Figure.StartPoint = _points[0];
            _railBase.Data = _baseRail.Geometry;
        }
        else
        {
            _railBase.Data = null;
        }

        SetProgressVisible(false);
    }

    private int IndexAt(double length)
    {
        int low = 0;
        int high = _cumulative.Length - 1;

        while (low < high)
        {
            int mid = (low + high + 1) >> 1;
            if (_cumulative[mid] <= length) low = mid;
            else high = mid - 1;
        }

        return low;
    }

    private Point PointAt(double length, int index)
    {
        if (index >= _points.Count - 1) return _points[^1];

        double span = _cumulative[index + 1] - _cumulative[index];
        double t = span > 1e-9 ? (length - _cumulative[index]) / span : 0;

        return new Point(
            _points[index].X + (_points[index + 1].X - _points[index].X) * t,
            _points[index].Y + (_points[index + 1].Y - _points[index].Y) * t);
    }

    private void ApplySelection(bool animate)
    {
        if (!_templated || _rebuildPending) return;

        int count = Items.Count;
        int selected = count == 0 ? -1 : Math.Clamp(SelectedIndex, 0, count - 1);

        if (_appliedSelection != selected)
        {
            if (_appliedSelection >= 0 && _appliedSelection < count) SetSelected(_appliedSelection, false);
            if (selected >= 0) SetSelected(selected, true);
            _appliedSelection = selected;
        }

        _goal = selected < 0 ? 0 : _rowLength[selected];

        if (animate)
        {
            StartRendering();
            return;
        }

        _position = _goal;
        _velocity = 0;
        StopRendering();
        Render();
    }

    private void SetSelected(int index, bool selected)
    {
        ApplyStyle(_labels[index], selected ? _selectedLabelStyle : _labelStyle);
        AutomationProperties.SetItemStatus(_buttons[index], selected ? "Current" : string.Empty);
    }

    private void StartRendering()
    {
        if (_rendering) return;

        _rendering = true;
        _hasLastFrame = false;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopRendering()
    {
        if (!_rendering) return;

        _rendering = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        var frameTime = ((RenderingEventArgs)e).RenderingTime;
        double dt = _hasLastFrame ? Math.Clamp((frameTime - _lastFrame).TotalSeconds, 0, MaxFrameTime) : 0;
        _lastFrame = frameTime;
        _hasLastFrame = true;

        int steps = Math.Max(1, (int)Math.Ceiling(dt / SubStep));
        double step = dt / steps;

        for (int i = 0; i < steps; i++)
        {
            double acceleration = -Stiffness * (_position - _goal) - Damping * _velocity;
            _velocity += acceleration * step;
            _position += _velocity * step;
        }

        bool settled = Math.Abs(_position - _goal) < SettleDistance && Math.Abs(_velocity) < SettleVelocity;
        if (settled)
        {
            _position = _goal;
            _velocity = 0;
        }

        Render();
        if (settled) StopRendering();
    }

    private void Render()
    {
        if (_points.Count == 0)
        {
            _dot.Visibility = Visibility.Collapsed;
            SetProgressVisible(false);
            return;
        }

        _dot.Visibility = Visibility.Visible;

        double length = Math.Clamp(_position, 0, _total);
        int index = IndexAt(length);
        var point = PointAt(length, index);

        Canvas.SetLeft(_dot, point.X - _dotSize / 2);
        Canvas.SetTop(_dot, point.Y - _dotSize / 2);

        UpdateProgress(length, index, point);
    }

    private void UpdateProgress(double length, int index, Point point)
    {
        if (length <= 0.01 || _points.Count < 2)
        {
            SetProgressVisible(false);
            return;
        }

        var points = _progressRail.Segment.Points;
        points.Clear();

        for (int i = 1; i <= index; i++) points.Add(_points[i]);
        if (index + 1 < _points.Count && length > _cumulative[index] + MinSegment) points.Add(point);

        SetProgressVisible(points.Count > 0);
    }

    private void SetProgressVisible(bool visible)
    {
        if (_progressVisible == visible) return;

        _progressVisible = visible;
        _progress.Data = visible ? _progressRail.Geometry : null;
    }
}
