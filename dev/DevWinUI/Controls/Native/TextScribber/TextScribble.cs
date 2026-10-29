using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = PartRoot, Type = typeof(Grid))]
[TemplatePart(Name = PartLayer, Type = typeof(Canvas))]
[TemplatePart(Name = PartText, Type = typeof(TextBlock))]
public sealed partial class TextScribble : Control
{

    private const string PartRoot = "PART_Root";
    private const string PartLayer = "PART_Layer";
    private const string PartText = "PART_Text";

    private readonly Run _run = new();
    private readonly List<TextScribbleSegment> _segments = new();
    private readonly CubicEase _ease = new() { EasingMode = EasingMode.EaseInOut };

    private Grid? _root;
    private Canvas? _layer;
    private TextBlock? _textBlock;
    private Storyboard? _storyboard;
    private int _version;
    private bool _target;
    private bool _pendingAnimate;
    private bool _rebuildQueued;
    private bool _loaded, _viewSeen, _hovering;

    public TextScribble()
    {
        DefaultStyleKey = typeof(TextScribble);

        RegisterPropertyChangedCallback(FontSizeProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontFamilyProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontWeightProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontStyleProperty, (_, _) => QueueRebuild());

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new TextScribbleAutomationPeer(this);

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_root is not null)
        {
            _root.PointerEntered -= OnPointerEntered;
            _root.PointerExited -= OnPointerExited;
        }
        if (_textBlock is not null)
        {
            _textBlock.SizeChanged -= OnTextSizeChanged;
            _textBlock.Inlines.Clear();
        }

        _root = GetTemplateChild(PartRoot) as Grid;
        _layer = GetTemplateChild(PartLayer) as Canvas;
        _textBlock = GetTemplateChild(PartText) as TextBlock;

        if (_root is not null)
        {
            _root.PointerEntered += OnPointerEntered;
            _root.PointerExited += OnPointerExited;
        }
        if (_textBlock is not null)
        {
            _run.Text = Text ?? "";
            _textBlock.Inlines.Add(_run);
            _textBlock.SizeChanged += OnTextSizeChanged;
        }

        QueueRebuild();
    }

    private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
    {
        _hovering = true;
        Evaluate();
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _hovering = false;
        Evaluate();
    }

    private void OnTextSizeChanged(object sender, SizeChangedEventArgs e) => QueueRebuild();

    private void OnTextChanged()
    {
        _run.Text = Text ?? "";
        QueueRebuild();
    }

    private void OnMarkBrushChanged()
    {
        Brush brush = MarkBrush;
        foreach (TextScribbleSegment segment in _segments)
        {
            segment.Path.Stroke = brush;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        EffectiveViewportChanged -= OnEffectiveViewportChanged;
        EffectiveViewportChanged += OnEffectiveViewportChanged;
        QueueRebuild();
        Evaluate();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        EffectiveViewportChanged -= OnEffectiveViewportChanged;
        _version++;
        _storyboard?.Stop();
        _storyboard = null;
    }

    private void OnEffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs args)
    {
        if (_viewSeen || ActualWidth <= 0 || ActualHeight <= 0) return;

        var bounds = new Rect(0, 0, ActualWidth, ActualHeight);
        bounds.Intersect(args.EffectiveViewport);
        if (!bounds.IsEmpty && bounds.Width > 0 && bounds.Height > 0)
        {
            _viewSeen = true;
            Evaluate();
        }
    }

    private void Evaluate()
    {
        bool desired = Drawn ?? (Trigger switch
        {
            TextScribbleTrigger.Mount => _loaded,
            TextScribbleTrigger.View => _viewSeen,
            TextScribbleTrigger.Hover => _hovering,
            _ => false
        });
        SetTarget(desired);
    }

    private void SetTarget(bool drawn)
    {
        if (drawn == _target) return;
        _target = drawn;

        if (_segments.Count == 0)
        {
            _pendingAnimate = drawn;
            return;
        }
        Animate(drawn);
    }

    private void QueueRebuild()
    {
        if (_rebuildQueued) return;
        _rebuildQueued = true;
        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, Rebuild);
    }

    private bool TryGetRange(out int from, out int length)
    {
        string s = Text ?? "";
        string target = MarkText;

        from = 0;
        length = s.Length;
        if (string.IsNullOrEmpty(target)) return length > 0;

        from = s.IndexOf(target, StringComparison.Ordinal);
        length = target.Length;
        return from >= 0;
    }

    private static double RightEdge(TextPointer start, int index, Rect r)
    {
        double right = r.Right;
        TextPointer? next = start.GetPositionAtOffset(index + 1, LogicalDirection.Forward);
        if (next is null) return right;

        double tolerance = r.Height * 0.5;

        Rect forward = next.GetCharacterRect(LogicalDirection.Forward);
        if (forward.Height > 0 && Math.Abs(forward.Top - r.Top) < tolerance && forward.Left > right)
        {
            right = forward.Left;
        }

        Rect backward = next.GetCharacterRect(LogicalDirection.Backward);
        if (backward.Height > 0 && Math.Abs(backward.Top - r.Top) < tolerance)
        {
            right = Math.Max(right, backward.Right);
        }

        return right;
    }

    private List<Rect> MeasureLines()
    {
        var lines = new List<Rect>();
        string s = Text ?? "";
        if (s.Length == 0 || _textBlock is null || _textBlock.ActualWidth <= 0) return lines;
        if (!TryGetRange(out int from, out int length)) return lines;

        TextPointer start = _run.ContentStart;
        double curTop = double.NaN, left = 0, right = 0, top = 0, bottom = 0;
        bool open = false;

        void Flush()
        {
            if (!open) return;
            lines.Add(new Rect(left, top, right - left, bottom - top));
            open = false;
        }

        for (int i = from; i < from + length; i++)
        {
            if (char.IsWhiteSpace(s[i]) || char.IsLowSurrogate(s[i])) continue;

            TextPointer? p = start.GetPositionAtOffset(i, LogicalDirection.Forward);
            if (p is null) break;

            Rect r = p.GetCharacterRect(LogicalDirection.Forward);
            if (r.Height <= 0) continue;

            double charRight = RightEdge(start, i, r);

            if (open && Math.Abs(r.Top - curTop) > r.Height * 0.5) Flush();

            if (!open)
            {
                open = true;
                curTop = r.Top;
                left = r.Left;
                right = charRight;
                top = r.Top;
                bottom = r.Bottom;
            }
            else
            {
                left = Math.Min(left, r.Left);
                right = Math.Max(right, charRight);
                top = Math.Min(top, r.Top);
                bottom = Math.Max(bottom, r.Bottom);
            }
        }
        Flush();
        return lines;
    }

    private static double Jit(Random rng, double amp) => (rng.NextDouble() * 2 - 1) * amp;

    private static double Dist(Point a, Point b)
    {
        double dx = b.X - a.X, dy = b.Y - a.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private static int SeedOf(string s)
    {
        unchecked
        {
            int h = 17;
            foreach (char c in s)
            {
                h = h * 31 + c;
            }
            return h;
        }
    }

    private List<List<Point>> BuildStrokes(List<Rect> lines)
    {
        var strokes = new List<List<Point>>();
        if (lines.Count == 0) return strokes;

        var rng = new Random(SeedOf(Text ?? ""));
        int passes = Math.Clamp(Iterations ?? (Type == TextScribbleType.Scribble ? 2 : 1), 1, 4);
        double pad = MarkPadding;

        if (Type == TextScribbleType.Circle || Type == TextScribbleType.Box)
        {
            double l = double.MaxValue, t = double.MaxValue, r = double.MinValue, b = double.MinValue;
            foreach (Rect ln in lines)
            {
                l = Math.Min(l, ln.Left);
                t = Math.Min(t, ln.Top);
                r = Math.Max(r, ln.Right);
                b = Math.Max(b, ln.Bottom);
            }

            var union = new Rect(l, t, r - l, b - t);
            for (int p = 0; p < passes; p++)
            {
                strokes.Add(Type == TextScribbleType.Circle
                    ? CircleStroke(union, p, rng, pad)
                    : BoxStroke(union, p, rng, pad));
            }
        }
        else
        {
            foreach (Rect ln in lines)
            {
                for (int p = 0; p < passes; p++)
                {
                    strokes.Add(LineStroke(ln, p, rng, pad));
                }
            }
        }
        return strokes;
    }

    private List<Point> LineStroke(Rect r, int pass, Random rng, double pad)
    {
        double x0 = r.Left - pad + Jit(rng, 2), x1 = r.Right + pad + Jit(rng, 2);
        double len = Math.Max(1, x1 - x0), h = r.Height;

        double y = Type switch
        {
            TextScribbleType.StrikeThrough => r.Top + h * 0.55,
            TextScribbleType.Scribble => r.Bottom - h * 0.04 + pad,
            _ => r.Bottom - h * 0.10 + pad
        };
        if (pass > 0) y += Jit(rng, 1.2);

        double a = Jit(rng, 1.5), b = Jit(rng, 1.5);
        double phase = rng.NextDouble() * Math.PI * 2;
        bool squiggle = Type == TextScribbleType.Scribble;
        double amp = Math.Max(1.5, h * 0.07);
        double wavelength = Math.Max(8, h * 0.35);
        double step = squiggle ? 2.0 : 5.0;
        int n = Math.Max(2, (int)(len / step));

        var pts = new List<Point>(n + 1);
        for (int i = 0; i <= n; i++)
        {
            double t = i / (double)n;
            double yy = y + a + (b - a) * t;
            yy += squiggle
                ? Math.Sin(t * len / wavelength * Math.PI * 2 + phase) * amp
                : Math.Sin(t * Math.PI * 2 + phase) * 0.6;
            pts.Add(new Point(x0 + t * len, yy));
        }
        return pts;
    }

    private List<Point> CircleStroke(Rect u, int pass, Random rng, double pad)
    {
        double cx = u.X + u.Width / 2, cy = u.Y + u.Height / 2;
        double rx = u.Width / 2 * 1.18 + pad + 8 + pass * 1.5;
        double ry = u.Height / 2 * 1.40 + pad + 4 + pass * 1.2;
        double tilt = Jit(rng, 0.03) - 0.03;
        double th0 = Math.PI * 1.1 + Jit(rng, 0.25);
        double total = Math.PI * 2 + 0.55 + Jit(rng, 0.15);
        double phase = rng.NextDouble() * Math.PI * 2;
        int n = Math.Max(60, (int)(total * (rx + ry) / 6));

        double cos = Math.Cos(tilt), sin = Math.Sin(tilt);
        var pts = new List<Point>(n + 1);
        for (int i = 0; i <= n; i++)
        {
            double s = i / (double)n;
            double th = th0 + s * total;
            double f = 1 + 0.025 * Math.Sin(th * 2 + phase) + (s - 0.5) * 0.05;
            double dx = rx * f * Math.Cos(th), dy = ry * f * Math.Sin(th);
            pts.Add(new Point(cx + dx * cos - dy * sin, cy + dx * sin + dy * cos));
        }
        return pts;
    }

    private List<Point> BoxStroke(Rect u, int pass, Random rng, double pad)
    {
        double ex = pad + 4 + pass * 1.2, ey = pad + 2 + pass * 1.0;
        double l = u.Left - ex, r = u.Right + ex, t = u.Top - ey, b = u.Bottom + ey;

        Point Jitter(double x, double y, double a = 1.8) => new(x + Jit(rng, a), y + Jit(rng, a));

        Point topLeft = Jitter(l, t);
        Point topRight = Jitter(r, t);
        Point bottomRight = Jitter(r, b);
        Point bottomLeft = Jitter(l, b);
        Point overshoot = Jitter(l, t, 2.5);
        Point tail = new(
            overshoot.X + (topRight.X - topLeft.X) * 0.14,
            overshoot.Y + (topRight.Y - topLeft.Y) * 0.14);

        var pts = new List<Point>();

        void Edge(Point p, Point q)
        {
            double len = Math.Max(1, Dist(p, q));
            int n = Math.Max(2, (int)(len / 4));
            double bow = Jit(rng, 1.5);
            double nx = -(q.Y - p.Y) / len, ny = (q.X - p.X) / len;
            for (int i = pts.Count == 0 ? 0 : 1; i <= n; i++)
            {
                double s = i / (double)n;
                double off = Math.Sin(s * Math.PI) * bow;
                pts.Add(new Point(p.X + (q.X - p.X) * s + nx * off, p.Y + (q.Y - p.Y) * s + ny * off));
            }
        }

        Edge(topLeft, topRight);
        Edge(topRight, bottomRight);
        Edge(bottomRight, bottomLeft);
        Edge(bottomLeft, overshoot);
        Edge(overshoot, tail);
        return pts;
    }

    private void Rebuild()
    {
        _rebuildQueued = false;
        if (!_loaded || _layer is null) return;

        _version++;
        _storyboard?.Stop();
        _storyboard = null;
        _layer.Children.Clear();
        _segments.Clear();

        bool snapDrawn = _target && !_pendingAnimate;
        double sw = Math.Max(0.5, StrokeWidth);
        Brush brush = MarkBrush;

        foreach (List<Point> pts in BuildStrokes(MeasureLines()))
        {
            if (pts.Count < 2) continue;

            double length = 0;
            for (int i = 1; i < pts.Count; i++)
            {
                length += Dist(pts[i - 1], pts[i]);
            }

            double dash = (length + 4) / sw;

            var poly = new PolyLineSegment();
            for (int i = 1; i < pts.Count; i++)
            {
                poly.Points.Add(pts[i]);
            }

            var figure = new PathFigure { StartPoint = pts[0], IsClosed = false, IsFilled = false };
            figure.Segments.Add(poly);

            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);

            var path = new Path
            {
                Data = geometry,
                Stroke = brush,
                StrokeThickness = sw,
                StrokeLineJoin = PenLineJoin.Round,
                StrokeStartLineCap = PenLineCap.Round,
                StrokeEndLineCap = PenLineCap.Round,
                StrokeDashCap = PenLineCap.Round,
                StrokeDashArray = new DoubleCollection { dash, dash },
                StrokeDashOffset = snapDrawn ? 0 : dash,
                IsHitTestVisible = false
            };
            _layer.Children.Add(path);
            _segments.Add(new TextScribbleSegment(path, dash, length));
        }

        if (_pendingAnimate && _target && _segments.Count > 0)
        {
            _pendingAnimate = false;
            Animate(true);
        }
    }

    private void SnapTo(bool drawn)
    {
        _version++;
        _storyboard?.Stop();
        _storyboard = null;
        foreach (TextScribbleSegment segment in _segments)
        {
            segment.Path.StrokeDashOffset = drawn ? 0 : segment.Dash;
        }
    }

    private void Animate(bool toDrawn)
    {
        double duration = Math.Max(0.01, Duration);
        double delay = Math.Max(0, Delay);
        double total = 0;
        foreach (TextScribbleSegment segment in _segments)
        {
            total += segment.Length;
        }
        if (total <= 0) total = 1;

        int token = ++_version;
        _storyboard?.Stop();

        var sb = new Storyboard();
        double acc = 0;

        foreach (TextScribbleSegment seg in _segments)
        {
            double begin, dur;
            if (toDrawn)
            {
                begin = delay + duration * acc / total;
                dur = Math.Max(0.05, duration * seg.Length / total);
                acc += seg.Length;
            }
            else
            {
                begin = 0;
                dur = Math.Max(0.05, duration * 0.5);
            }

            var anim = new DoubleAnimation
            {
                To = toDrawn ? 0 : seg.Dash,
                Duration = TimeSpan.FromSeconds(dur),
                BeginTime = TimeSpan.FromSeconds(begin),
                EasingFunction = _ease,
                FillBehavior = FillBehavior.HoldEnd,
                EnableDependentAnimation = true
            };
            if (toDrawn) anim.From = seg.Dash;

            Storyboard.SetTarget(anim, seg.Path);
            Storyboard.SetTargetProperty(anim, "StrokeDashOffset");
            sb.Children.Add(anim);
        }

        sb.Completed += (_, _) =>
        {
            if (token != _version) return;
            SnapTo(toDrawn);
        };

        _storyboard = sb;
        sb.Begin();
    }
}
