using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.System;

namespace DevWinUI;

[TemplatePart(Name = PartHost, Type = typeof(Grid))]
public sealed partial class LineSidebar : Control
{
    private const string PartHost = "PART_Host";
    private const double MarkerThickness = 2;
    private const double TickThickness = 1;

    private sealed class Node
    {
        public bool IsTick;
        public int ItemIndex;
        public ContentControl Row = null!;
        public Rectangle? Marker;
        public ScaleTransform? Scale;
        public TranslateTransform? Shift;
        public SolidColorBrush? LabelBrush;
        public TextBlock? IndexText;
        public double Cur, Target;
    }

    private readonly List<Node> _nodes = new();

    private Grid? _host;
    private Grid? _root;
    private StackPanel? _list;
    private TimeSpan _last;
    private bool _running;
    private bool _buildQueued;
    private bool _inside;
    private double _py;
    private int _focusIndex = -1;

    public event TypedEventHandler<LineSidebar, LineSidebarItemClickEventArgs>? ItemClick;

    public LineSidebar()
    {
        DefaultStyleKey = typeof(LineSidebar);

        RegisterPropertyChangedCallback(FontSizeProperty, (_, _) => Rebuild());
        Unloaded += (_, _) => StopLoop();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _host = GetTemplateChild(PartHost) as Grid;
        Rebuild();
    }

    private static void OnRebuildChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((LineSidebar)d).Rebuild();
    }

    private static void OnActiveIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((LineSidebar)d).OnActiveChanged();
    }

    private void Rebuild()
    {
        if (_buildQueued) return;
        _buildQueued = true;
        DispatcherQueue.TryEnqueue(Build);
    }

    private void Build()
    {
        _buildQueued = false;
        if (_host is null) return;

        StopLoop();
        _nodes.Clear();
        _focusIndex = -1;

        var items = Items ?? Array.Empty<string>();
        double itemGap = ItemGap, markerLength = MarkerLength;
        var markerColor = MarkerColor;
        var textColor = TextColor;
        bool showMarker = ShowMarker;

        _list = new StackPanel { Orientation = Orientation.Vertical, Spacing = itemGap };

        double markerCol = showMarker ? markerLength + MarkerGap : 0;
        double tickLen = markerLength * TickScale;

        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            string text = items[i];

            var grid = new Grid
            {
                Background = new SolidColorBrush(Colors.Transparent),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(markerCol) },
                    new ColumnDefinition { Width = GridLength.Auto },
                },
            };

            var row = new ContentControl
            {
                Content = grid,
                IsTabStop = true,
                UseSystemFocusVisuals = true,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
            };
            AutomationProperties.SetName(row, text);

            var node = new Node { ItemIndex = i, Row = row };

            if (showMarker)
            {
                node.Scale = new ScaleTransform { ScaleX = 1 };
                node.Marker = new Rectangle
                {
                    Width = markerLength,
                    Height = MarkerThickness,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                    Fill = new SolidColorBrush(markerColor),
                    RenderTransformOrigin = new Point(0, 0.5),
                    RenderTransform = node.Scale,
                    IsHitTestVisible = false,
                };
                grid.Children.Add(node.Marker);
            }

            node.Shift = new TranslateTransform();
            var group = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                RenderTransform = node.Shift,
                VerticalAlignment = VerticalAlignment.Center,
            };

            if (ShowIndex)
            {
                node.IndexText = new TextBlock
                {
                    Text = (i + 1).ToString("D2"),
                    FontSize = FontSize * 0.7,
                    Foreground = new SolidColorBrush(markerColor),
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(0, 0, 12, 0),
                };
                group.Children.Add(node.IndexText);
            }

            node.LabelBrush = new SolidColorBrush(textColor);
            group.Children.Add(new TextBlock
            {
                Text = text,
                FontSize = FontSize,
                Foreground = node.LabelBrush,
                VerticalAlignment = VerticalAlignment.Center,
            });
            Grid.SetColumn(group, 1);
            grid.Children.Add(group);

            row.Tapped += (_, _) => Activate(index);
            row.KeyDown += (_, e) =>
            {
                if (e.Key is VirtualKey.Enter or VirtualKey.Space)
                {
                    Activate(index);
                    e.Handled = true;
                }
            };
            row.GotFocus += (_, _) => { _focusIndex = index; StartLoop(); };
            row.LostFocus += (_, _) =>
            {
                if (_focusIndex == index) _focusIndex = -1;
                StartLoop();
            };

            _list.Children.Add(row);
            _nodes.Add(node);

            if (showMarker && i < items.Count - 1)
            {
                var tickScale = new ScaleTransform { ScaleX = 1 };
                var tick = new Rectangle
                {
                    Width = tickLen,
                    Height = TickThickness,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, -(itemGap / 2 + TickThickness / 2)),
                    Fill = new SolidColorBrush(markerColor),
                    RenderTransformOrigin = new Point(0, 0.5),
                    RenderTransform = tickScale,
                    IsHitTestVisible = false,
                };
                grid.Children.Add(tick);
                _nodes.Add(new Node { IsTick = true, ItemIndex = i, Row = row, Marker = tick, Scale = tickScale });
            }
        }

        _root = new Grid
        {
            Background = new SolidColorBrush(Colors.Transparent),
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        _root.Children.Add(_list);
        _root.PointerMoved += OnPointerMoved;
        _root.PointerEntered += OnPointerMoved;
        _root.PointerExited += (_, _) => { _inside = false; StartLoop(); };
        _root.PointerCanceled += (_, _) => { _inside = false; StartLoop(); };

        _host.Children.Clear();
        _host.Children.Add(_root);

        foreach (var n in _nodes)
        {
            n.Target = ComputeTarget(n);
            n.Cur = n.Target;
            Apply(n);
        }

        UpdateAutomation();
    }

    private void Activate(int index)
    {
        var items = Items;
        if (items is null || index < 0 || index >= items.Count) return;

        ActiveIndex = index;
        ItemClick?.Invoke(this, new LineSidebarItemClickEventArgs(index, items[index]));
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (_root is null) return;
        _py = e.GetCurrentPoint(_root).Position.Y;
        _inside = true;
        StartLoop();
    }

    private void OnActiveChanged()
    {
        UpdateAutomation();
        StartLoop();
    }

    private void UpdateAutomation()
    {
        int active = ActiveIndex;
        foreach (var n in _nodes)
        {
            if (!n.IsTick)
                AutomationProperties.SetItemStatus(n.Row, n.ItemIndex == active ? "Selected" : "");
        }
    }

    private void StartLoop()
    {
        if (_running || _nodes.Count == 0) return;
        _running = true;
        _last = TimeSpan.Zero;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopLoop()
    {
        if (!_running) return;
        _running = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        var now = ((RenderingEventArgs)e).RenderingTime;
        if (now == _last) return;

        double dt = _last == TimeSpan.Zero
            ? 1000.0 / 60
            : Math.Min(100, (now - _last).TotalMilliseconds);
        _last = now;

        double smoothing = Smoothing;
        double alpha = smoothing <= 0 ? 1 : 1 - Math.Exp(-dt / smoothing);
        bool moving = false;

        foreach (var n in _nodes)
        {
            n.Target = ComputeTarget(n);
            n.Cur += (n.Target - n.Cur) * alpha;

            if (Math.Abs(n.Target - n.Cur) < 0.002) n.Cur = n.Target;
            else moving = true;

            Apply(n);
        }

        if (!moving) StopLoop();
    }

    private double ComputeTarget(Node n)
    {
        double pinned = 0;
        if (!n.IsTick && (n.ItemIndex == ActiveIndex || n.ItemIndex == _focusIndex)) pinned = 1;

        double prox = 0;
        double radius = ProximityRadius;
        if (_inside && _root is not null && radius > 0)
        {
            double cy = CenterY(n);
            if (!double.IsNaN(cy))
            {
                double t = Math.Clamp(1 - Math.Abs(_py - cy) / radius, 0, 1);
                prox = Falloff switch
                {
                    LineSidebarFalloff.Linear => t,
                    LineSidebarFalloff.Sharp => t * t * t,
                    _ => t * t * (3 - 2 * t),
                };
            }
        }

        return Math.Max(pinned, prox);
    }

    private double CenterY(Node n)
    {
        try
        {
            double top = n.Row.TransformToVisual(_root).TransformPoint(new Point(0, 0)).Y;
            return n.IsTick
                ? top + n.Row.ActualHeight + ItemGap / 2
                : top + n.Row.ActualHeight / 2;
        }
        catch
        {
            return double.NaN;
        }
    }

    private void Apply(Node n)
    {
        double e = n.Cur;
        double maxShift = MaxShift;
        double stretch = 1 + e * maxShift / Math.Max(1, MarkerLength);

        if (n.IsTick)
        {
            if (n.Scale is not null) n.Scale.ScaleX = ScaleTick ? stretch : 1;
            return;
        }

        if (n.Shift is not null) n.Shift.X = e * maxShift;
        if (n.LabelBrush is not null) n.LabelBrush.Color = Mix(TextColor, AccentColor, e);
        if (n.IndexText is not null) n.IndexText.Opacity = 1 - e;
        if (n.Scale is not null) n.Scale.ScaleX = stretch;
    }

    private static Color Mix(Color a, Color b, double t) => Color.FromArgb(
        255,
        (byte)Math.Round(a.R + (b.R - a.R) * t),
        (byte)Math.Round(a.G + (b.G - a.G) * t),
        (byte)Math.Round(a.B + (b.B - a.B) * t));
}
