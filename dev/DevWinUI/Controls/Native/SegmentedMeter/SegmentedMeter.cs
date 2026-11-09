using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

/// <summary>
/// A segmented meter that shows how a whole is divided into parts, as a horizontal or vertical bar with an optional legend.
/// </summary>
[ContentProperty(Name = nameof(Segments))]
[TemplatePart(Name = PartRoot, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartStart, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartBody, Type = typeof(Grid))]
[TemplatePart(Name = PartTrack, Type = typeof(Border))]
[TemplatePart(Name = PartBarGrid, Type = typeof(Grid))]
[TemplatePart(Name = PartLegend, Type = typeof(MeterLegendPanel))]
[TemplatePart(Name = PartEnd, Type = typeof(ContentPresenter))]
public sealed partial class SegmentedMeter : Control
{
    private const string PartRoot = "PART_Root";
    private const string PartStart = "PART_Start";
    private const string PartBody = "PART_Body";
    private const string PartTrack = "PART_Track";
    private const string PartBarGrid = "PART_BarGrid";
    private const string PartLegend = "PART_Legend";
    private const string PartEnd = "PART_End";

    private const string BarThicknessKeyPrefix = "BarThickness";
    private const string SegmentStyleKeyPrefix = "Segment";
    private const string IconStyleKeySuffix = "Icon";
    private const string LegendMarkerSizeKey = "LegendMarkerSize";
    private const string VerticalMinLengthKey = "VerticalMinLength";
    private const string LegendItemStyleKey = "LegendItemStyle";
    private const string LegendLabelStyleKey = "LegendLabelStyle";
    private const string LegendValueStyleKey = "LegendValueStyle";

    private const string PercentFormat = "0.#%";
    private const double Tolerance = 1e-6;
    private const double FallbackThickness = 8;
    private const double FallbackMarkerSize = 8;
    private const double FallbackVerticalMinLength = 120;
    private const double AnimationDuration = 450;

    private static readonly int KindCount = Enum.GetValues<MeterSegmentKind>().Length;
    private static readonly int SizeCount = Enum.GetValues<MeterBarSize>().Length;

    private static readonly MeterSegmentKind[] AutoCycle =
    {
        MeterSegmentKind.Accent,
        MeterSegmentKind.Success,
        MeterSegmentKind.Attention,
        MeterSegmentKind.Danger,
        MeterSegmentKind.Neutral,
    };

    private readonly ObservableCollection<MeterSegment> _segments = new();
    private readonly List<MeterSegment> _tracked = new();
    private readonly List<double> _fractions = new();
    private readonly List<BarSlot> _bars = new();
    private readonly Dictionary<MeterSegment, double> _previous = new();
    private readonly Style?[] _segmentStyles;
    private readonly Style?[] _iconStyles;
    private readonly double[] _thicknesses;
    private readonly DispatcherQueueHandler _rebuildHandler;
    private readonly EventHandler<object> _renderingHandler;

    private FrameworkElement _root = null!;
    private ContentPresenter _start = null!;
    private Grid _body = null!;
    private Border _track = null!;
    private Grid _barGrid = null!;
    private MeterLegendPanel _legend = null!;
    private ContentPresenter _end = null!;

    private ColumnDefinition? _restColumn;
    private RowDefinition? _restRow;
    private Style? _legendItemStyle;
    private Style? _legendLabelStyle;
    private Style? _legendValueStyle;
    private double _markerSize = FallbackMarkerSize;
    private double _verticalMinLength = FallbackVerticalMinLength;

    private bool _barHorizontal;
    private double _barRadius;
    private double _barGap;
    private int _shapeFirst;
    private int _shapeLast;
    private bool _shapeFull;
    private long _animationStart;
    private bool _animating;

    private bool _loaded;
    private bool _templated;
    private bool _firstShow = true;
    private bool _rebuildPending;

    private bool Active
    {
        get { return _loaded && _templated; }
    }

    public SegmentedMeter()
    {
        DefaultStyleKey = typeof(SegmentedMeter);

        _segmentStyles = new Style?[KindCount];
        _iconStyles = new Style?[KindCount];
        _thicknesses = new double[SizeCount];
        _rebuildHandler = OnRebuildRequested;
        _renderingHandler = OnRendering;

        _segments.CollectionChanged += OnSegmentsChanged;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _templated = false;

        var root = GetTemplateChild(PartRoot) as FrameworkElement;
        var start = GetTemplateChild(PartStart) as ContentPresenter;
        var body = GetTemplateChild(PartBody) as Grid;
        var track = GetTemplateChild(PartTrack) as Border;
        var barGrid = GetTemplateChild(PartBarGrid) as Grid;
        var legend = GetTemplateChild(PartLegend) as MeterLegendPanel;
        var end = GetTemplateChild(PartEnd) as ContentPresenter;

        if (root is null || start is null || body is null || track is null || barGrid is null || legend is null || end is null) return;

        _root = root;
        _start = start;
        _body = body;
        _track = track;
        _barGrid = barGrid;
        _legend = legend;
        _end = end;
        _templated = true;

        CacheResources();
        UpdateSlots();
        Rebuild();
    }

    protected override AutomationPeer OnCreateAutomationPeer()
    {
        return new SegmentedMeterAutomationPeer(this);
    }

    internal string GetSummary()
    {
        var builder = new StringBuilder();

        for (int i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];

            if (builder.Length > 0) builder.Append(", ");
            if (!string.IsNullOrEmpty(segment.Label)) builder.Append(segment.Label).Append(' ');
            builder.Append(FormatPercent(Fraction(segment.Value)));
        }

        return builder.ToString();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        AttachSegments();
        Rebuild();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        StopAnimation();
        DetachSegments();
    }

    private void AttachSegments()
    {
        DetachSegments();

        for (int i = 0; i < _segments.Count; i++)
        {
            var segment = _segments[i];
            segment.Changed += OnSegmentChanged;
            _tracked.Add(segment);
        }
    }

    private void DetachSegments()
    {
        for (int i = 0; i < _tracked.Count; i++) _tracked[i].Changed -= OnSegmentChanged;
        _tracked.Clear();
    }

    private void OnSegmentsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (_loaded) AttachSegments();
        RequestRebuild();
    }

    private void OnSegmentChanged(object? sender, EventArgs e)
    {
        RequestRebuild();
    }

    private void UpdateSlots()
    {
        if (!_templated) return;

        _start.Visibility = StartContent is null ? Visibility.Collapsed : Visibility.Visible;
        _end.Visibility = EndContent is null ? Visibility.Collapsed : Visibility.Visible;
    }

    private void CacheResources()
    {
        var resources = _root.Resources;

        for (int i = 0; i < KindCount; i++)
        {
            string key = SegmentStyleKeyPrefix + (MeterSegmentKind)i;
            _segmentStyles[i] = resources.TryGetValue(key, out var style) ? style as Style : null;
            _iconStyles[i] = resources.TryGetValue(key + IconStyleKeySuffix, out var icon) ? icon as Style : null;
        }

        for (int i = 0; i < SizeCount; i++)
        {
            string key = BarThicknessKeyPrefix + (MeterBarSize)i;
            _thicknesses[i] = resources.TryGetValue(key, out var value) && value is double thickness ? thickness : FallbackThickness;
        }

        _markerSize = resources.TryGetValue(LegendMarkerSizeKey, out var marker) && marker is double markerSize
            ? markerSize
            : FallbackMarkerSize;

        _verticalMinLength = resources.TryGetValue(VerticalMinLengthKey, out var min) && min is double minLength
            ? minLength
            : FallbackVerticalMinLength;

        _legendItemStyle = resources.TryGetValue(LegendItemStyleKey, out var item) ? item as Style : null;
        _legendLabelStyle = resources.TryGetValue(LegendLabelStyleKey, out var label) ? label as Style : null;
        _legendValueStyle = resources.TryGetValue(LegendValueStyleKey, out var value2) ? value2 as Style : null;
    }

    private void RequestRebuild()
    {
        if (!Active || _rebuildPending) return;

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
        if (!Active) return;

        bool horizontal = Orientation == Orientation.Horizontal;
        double used = ComputeFractions();

        bool legend = BuildLegend();
        ArrangeBody(horizontal, legend);

        if (BuildBar(horizontal, used)) StartAnimation();
        else StopAnimation();
    }

    private double Fraction(double value)
    {
        double range = Maximum - Minimum;
        if (!(range > 0)) return 0;

        double fraction = (value - Minimum) / range;
        return fraction > 0 ? Math.Min(fraction, 1) : 0;
    }

    private double ComputeFractions()
    {
        _fractions.Clear();

        double used = 0;
        for (int i = 0; i < _segments.Count; i++)
        {
            double fraction = Math.Max(0, Math.Min(Fraction(_segments[i].Value), 1 - used));
            _fractions.Add(fraction);
            used += fraction;
        }

        return used;
    }

    private double ResolveThickness()
    {
        double thickness = BarThickness;
        return thickness > 0 ? thickness : _thicknesses[(int)BarSize];
    }

    private void ArrangeBody(bool horizontal, bool legend)
    {
        _body.RowDefinitions.Clear();
        _body.ColumnDefinitions.Clear();

        int trackIndex = legend && LegendPosition == MeterLegendPosition.Start ? 1 : 0;
        int legendIndex = 1 - trackIndex;

        if (legend)
        {
            for (int i = 0; i < 2; i++)
            {
                if (horizontal) _body.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
                else _body.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }
        }

        Grid.SetRow(_track, horizontal ? trackIndex : 0);
        Grid.SetColumn(_track, horizontal ? 0 : trackIndex);
        Grid.SetRow(_legend, horizontal ? legendIndex : 0);
        Grid.SetColumn(_legend, horizontal ? 0 : legendIndex);

        if (horizontal)
        {
            _track.HorizontalAlignment = HorizontalAlignment.Stretch;
            _track.VerticalAlignment = VerticalAlignment.Top;
            _legend.HorizontalAlignment = HorizontalAlignment.Stretch;
            _legend.VerticalAlignment = VerticalAlignment.Top;
        }
        else
        {
            _track.HorizontalAlignment = HorizontalAlignment.Left;
            _track.VerticalAlignment = VerticalAlignment.Stretch;
            _legend.HorizontalAlignment = HorizontalAlignment.Left;
            _legend.VerticalAlignment = VerticalAlignment.Center;
        }
    }

    private bool BuildBar(bool horizontal, double used)
    {
        double thickness = ResolveThickness();
        double radius = thickness / 2;

        _barHorizontal = horizontal;
        _barRadius = radius;
        _barGap = Math.Max(0, Gap);
        _shapeFirst = -2;
        _shapeLast = -2;

        _track.CornerRadius = new CornerRadius(radius);

        if (horizontal)
        {
            _track.Height = thickness;
            _track.Width = double.NaN;
            _track.MinHeight = 0;
        }
        else
        {
            _track.Width = thickness;
            _track.Height = double.NaN;
            _track.MinHeight = _verticalMinLength;
        }

        _previous.Clear();
        for (int i = 0; i < _bars.Count; i++) _previous[_bars[i].Segment] = _bars[i].Current;
        _bars.Clear();

        _barGrid.Children.Clear();
        _barGrid.ColumnDefinitions.Clear();
        _barGrid.RowDefinitions.Clear();

        bool reveal = false;
        if (_firstShow && used > 0)
        {
            _firstShow = false;
            reveal = Animated;
        }

        bool animate = Animated && !reveal;
        bool changed = false;

        Compositor? compositor = null;
        CubicBezierEasingFunction? easing = null;

        if (reveal)
        {
            compositor = ElementCompositionPreview.GetElementVisual(_barGrid).Compositor;
            easing = compositor.CreateCubicBezierEasingFunction(new Vector2(0.22f, 1f), new Vector2(0.36f, 1f));
        }

        int ordinal = 0;
        for (int i = 0; i < _fractions.Count; i++)
        {
            var segment = _segments[i];
            double target = _fractions[i];
            double from = target;

            if (animate) from = _previous.TryGetValue(segment, out var shown) ? shown : 0;
            if (target <= 0 && from <= 0) continue;

            var bar = new Border();
            Paint(bar, ResolveKind(segment, i), segment.Fill);
            ToolTipService.SetToolTip(bar, FormatTip(segment));

            var length = new GridLength(from, GridUnitType.Star);
            ColumnDefinition? column = null;
            RowDefinition? row = null;

            if (horizontal)
            {
                column = new ColumnDefinition { Width = length };
                _barGrid.ColumnDefinitions.Add(column);
                Grid.SetColumn(bar, ordinal);
            }
            else
            {
                row = new RowDefinition { Height = length };
                _barGrid.RowDefinitions.Add(row);
                Grid.SetRow(bar, ordinal);
            }

            _barGrid.Children.Add(bar);
            _bars.Add(new BarSlot(segment, bar, column, row, from, target));

            if (from != target) changed = true;
            if (compositor is not null && easing is not null) Reveal(bar, ordinal, compositor, easing, horizontal);

            ordinal++;
        }

        _restColumn = null;
        _restRow = null;

        if (horizontal)
        {
            _restColumn = new ColumnDefinition();
            _barGrid.ColumnDefinitions.Add(_restColumn);
        }
        else
        {
            _restRow = new RowDefinition();
            _barGrid.RowDefinitions.Add(_restRow);
        }

        ApplyFractions();
        return changed;
    }

    private void ApplyFractions()
    {
        double used = 0;
        int first = -1;
        int last = -1;

        for (int i = 0; i < _bars.Count; i++)
        {
            double current = _bars[i].Current;

            if (current > 0)
            {
                if (first < 0) first = i;
                last = i;
            }

            used += current;
        }

        bool full = used >= 1 - Tolerance;

        if (first != _shapeFirst || last != _shapeLast || full != _shapeFull)
        {
            _shapeFirst = first;
            _shapeLast = last;
            _shapeFull = full;
            ApplyShape(first, last, full);
        }

        var rest = new GridLength(Math.Max(0, 1 - used), GridUnitType.Star);
        if (_restColumn is not null) _restColumn.Width = rest;
        if (_restRow is not null) _restRow.Height = rest;
    }

    private void ApplyShape(int first, int last, bool full)
    {
        for (int i = 0; i < _bars.Count; i++)
        {
            var bar = _bars[i].Element;
            double start = i == first ? _barRadius : 0;
            double end = i == last && full ? _barRadius : 0;
            double trailingGap = i == last ? 0 : _barGap;

            if (_barHorizontal)
            {
                bar.Margin = new Thickness(0, 0, trailingGap, 0);
                bar.CornerRadius = new CornerRadius(start, end, end, start);
            }
            else
            {
                bar.Margin = new Thickness(0, 0, 0, trailingGap);
                bar.CornerRadius = new CornerRadius(start, start, end, end);
            }
        }
    }

    private void StartAnimation()
    {
        _animationStart = Stopwatch.GetTimestamp();
        if (_animating) return;

        _animating = true;
        Microsoft.UI.Xaml.Media.CompositionTarget.Rendering += _renderingHandler;
    }

    private void StopAnimation()
    {
        if (!_animating) return;

        _animating = false;
        Microsoft.UI.Xaml.Media.CompositionTarget.Rendering -= _renderingHandler;
    }

    private void OnRendering(object? sender, object e)
    {
        if (!Active)
        {
            StopAnimation();
            return;
        }

        double progress = Math.Min(1, Stopwatch.GetElapsedTime(_animationStart).TotalMilliseconds / AnimationDuration);
        double inverse = 1 - progress;
        double eased = 1 - inverse * inverse * inverse;

        for (int i = 0; i < _bars.Count; i++)
        {
            var slot = _bars[i];
            if (slot.From == slot.To) continue;

            slot.Current = progress >= 1 ? slot.To : slot.From + (slot.To - slot.From) * eased;

            var length = new GridLength(slot.Current, GridUnitType.Star);
            if (slot.Column is not null) slot.Column.Width = length;
            else if (slot.Row is not null) slot.Row.Height = length;
        }

        ApplyFractions();

        if (progress >= 1) StopAnimation();
    }

    private bool BuildLegend()
    {
        _legend.Children.Clear();
        _legend.Orientation = LegendOrientation;

        bool any = false;

        if (ShowLegend)
        {
            for (int i = 0; i < _segments.Count; i++)
            {
                var segment = _segments[i];
                if (string.IsNullOrEmpty(segment.Label)) continue;

                _legend.Children.Add(CreateLegendItem(segment, i));
                any = true;
            }
        }

        _legend.Visibility = any ? Visibility.Visible : Visibility.Collapsed;
        return any;
    }

    private StackPanel CreateLegendItem(MeterSegment segment, int index)
    {
        var kind = ResolveKind(segment, index);

        var row = new StackPanel();
        ApplyStyle(row, _legendItemStyle);

        if (!string.IsNullOrEmpty(segment.Icon))
        {
            var icon = new FontIcon { Glyph = segment.Icon };
            ApplyStyle(icon, _iconStyles[(int)kind]);
            if (segment.Fill is not null) icon.Foreground = segment.Fill;

            row.Children.Add(icon);
        }
        else
        {
            var dot = new Border
            {
                Width = _markerSize,
                Height = _markerSize,
                CornerRadius = new CornerRadius(_markerSize / 2),
                VerticalAlignment = VerticalAlignment.Center,
            };
            Paint(dot, kind, segment.Fill);

            row.Children.Add(dot);
        }

        var label = new TextBlock { Text = segment.Label };
        ApplyStyle(label, _legendLabelStyle);
        row.Children.Add(label);

        if (ShowValue)
        {
            var value = new TextBlock { Text = FormatPercent(Fraction(segment.Value)) };
            ApplyStyle(value, _legendValueStyle);
            row.Children.Add(value);
        }

        return row;
    }

    private static MeterSegmentKind ResolveKind(MeterSegment segment, int index)
    {
        var kind = segment.Kind;
        return kind != MeterSegmentKind.Auto ? kind : AutoCycle[index % AutoCycle.Length];
    }

    private void Paint(Border target, MeterSegmentKind kind, Brush? fill)
    {
        ApplyStyle(target, _segmentStyles[(int)kind]);
        if (fill is not null) target.Background = fill;
    }

    private static void ApplyStyle(FrameworkElement target, Style? style)
    {
        if (style is not null) target.Style = style;
    }

    private string FormatTip(MeterSegment segment)
    {
        string percent = FormatPercent(Fraction(segment.Value));
        return string.IsNullOrEmpty(segment.Label) ? percent : segment.Label + ": " + percent;
    }

    private static string FormatPercent(double fraction)
    {
        return fraction.ToString(PercentFormat, CultureInfo.CurrentCulture);
    }

    private static void Reveal(Border bar, int ordinal, Compositor compositor, CubicBezierEasingFunction easing, bool horizontal)
    {
        var visual = ElementCompositionPreview.GetElementVisual(bar);

        var grow = compositor.CreateScalarKeyFrameAnimation();
        grow.InsertKeyFrame(0f, 0f);
        grow.InsertKeyFrame(1f, 1f, easing);
        grow.Duration = TimeSpan.FromMilliseconds(600);
        grow.DelayTime = TimeSpan.FromMilliseconds(ordinal * 90);
        grow.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;

        visual.StartAnimation(horizontal ? "Scale.X" : "Scale.Y", grow);
    }
}
