using Microsoft.UI.Dispatching;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Windows.System;
using DispatcherQueuePriority = Microsoft.UI.Dispatching.DispatcherQueuePriority;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = PartLayer, Type = typeof(Canvas))]
[TemplatePart(Name = PartTextBlock, Type = typeof(TextBlock))]
[TemplatePart(Name = PartRun, Type = typeof(Run))]
public sealed partial class UnderlineAnimation : Control
{
    private const string PartLayer = "PART_Layer";
    private const string PartTextBlock = "PART_TextBlock";
    private const string PartRun = "PART_Run";

    private const double FillTint = 0.22;
    private const double RestOpacity = 0.28;

    private readonly List<UnderlineAnimationBand> _bands = new();
    private readonly CubicEase _ease = new() { EasingMode = EasingMode.EaseOut };

    private Canvas? _layer;
    private TextBlock? _textBlock;
    private Run? _run;
    private Storyboard? _storyboard;
    private int _version;
    private bool _shown;
    private bool _rebuildQueued;
    private bool _loaded, _hovering, _focusVisible;

    private EventHandler? _click;
    public event EventHandler? Click
    {
        add { _click += value; UpdateInteractive(); }
        remove { _click -= value; UpdateInteractive(); }
    }

    public UnderlineAnimation()
    {
        DefaultStyleKey = typeof(UnderlineAnimation);

        RegisterPropertyChangedCallback(FontSizeProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontFamilyProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontWeightProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontStyleProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(ForegroundProperty, (_, _) => QueueRebuild());

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;

        UpdateInteractive();
    }

    private bool IsInteractive => NavigateUri is not null || _click is not null;

    protected override AutomationPeer OnCreateAutomationPeer() => new UnderlineAnimationAutomationPeer(this);

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_textBlock is not null) _textBlock.SizeChanged -= OnTextBlockSizeChanged;

        _layer = GetTemplateChild(PartLayer) as Canvas;
        _textBlock = GetTemplateChild(PartTextBlock) as TextBlock;
        _run = GetTemplateChild(PartRun) as Run;

        if (_textBlock is not null) _textBlock.SizeChanged += OnTextBlockSizeChanged;
        if (_run is not null) _run.Text = Text ?? string.Empty;

        QueueRebuild();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        QueueRebuild();
        Evaluate();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        _hovering = false;
        _focusVisible = false;
        _version++;
        _storyboard?.Stop();
        _storyboard = null;
    }

    private void OnTextBlockSizeChanged(object sender, SizeChangedEventArgs e) => QueueRebuild();

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        _hovering = true;
        Evaluate();
    }

    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        _hovering = false;
        Evaluate();
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        _focusVisible = FocusState == FocusState.Keyboard;
        Evaluate();
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        base.OnLostFocus(e);
        _focusVisible = false;
        Evaluate();
    }

    protected override void OnTapped(TappedRoutedEventArgs e)
    {
        base.OnTapped(e);
        Activate();
    }

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Key == VirtualKey.Enter || e.Key == VirtualKey.Space)
        {
            e.Handled = true;
            Activate();
        }
    }

    private void OnTextChanged()
    {
        if (_run is not null) _run.Text = Text ?? string.Empty;
        QueueRebuild();
    }

    private void OnActiveChanged()
    {
        AutomationProperties.SetItemStatus(this, Active ? "current" : string.Empty);
        Evaluate();
    }

    private void UpdateInteractive()
    {
        bool interactive = IsInteractive;
        IsTabStop = interactive;
        ProtectedCursor = interactive ? InputSystemCursor.Create(InputSystemCursorShape.Hand) : null;
    }

    private void Activate()
    {
        if (!IsEnabled) return;

        _click?.Invoke(this, EventArgs.Empty);
        if (NavigateUri is not null) _ = Launcher.LaunchUriAsync(NavigateUri);
    }

    private void Evaluate()
    {
        bool shown = Active || _hovering || _focusVisible;
        if (shown == _shown) return;
        _shown = shown;
        if (_bands.Count > 0) Animate(shown);
    }

    private void QueueRebuild()
    {
        if (_rebuildQueued) return;
        _rebuildQueued = true;
        DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, Rebuild);
    }

    private bool TryGetRange(out int from, out int length)
    {
        string s = Text ?? string.Empty;
        string target = UnderlineText;

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
        string s = Text ?? string.Empty;
        if (_run is null || _textBlock is null || _textBlock.ActualWidth <= 0) return lines;
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

    private Brush ResolveBrush()
    {
        Brush? brush = Kind == UnderlineKind.Gradient ? (GradientBrush ?? LineBrush) : LineBrush;
        return brush ?? Foreground;
    }

    private Point OriginFor(bool shown) => Kind switch
    {
        UnderlineKind.Center => new Point(0.5, 0.5),
        UnderlineKind.Fill => new Point(0.5, 1),
        _ => shown ? new Point(0, 0.5) : new Point(1, 0.5)
    };

    private (double sx, double sy, double op) RectState(bool shown) =>
        Kind == UnderlineKind.Fill
            ? (1, shown ? 1 : 0, shown ? FillTint : 1)
            : (shown ? 1 : 0, 1, 1);

    private static List<Point> WavePoints(double x0, double x1, double yMid, double amp, double wavelength)
    {
        double len = Math.Max(1, x1 - x0);
        int n = Math.Max(4, (int)(len / 2));
        var pts = new List<Point>(n + 1);
        for (int i = 0; i <= n; i++)
        {
            double t = i / (double)n;
            pts.Add(new Point(x0 + t * len, yMid + Math.Sin(t * len / wavelength * Math.PI * 2) * amp));
        }
        return pts;
    }

    private static Path MakeWavePath(List<Point> pts, double thickness, Brush brush)
    {
        var poly = new PolyLineSegment();
        for (int i = 1; i < pts.Count; i++)
        {
            poly.Points.Add(pts[i]);
        }

        var figure = new PathFigure { StartPoint = pts[0], IsClosed = false, IsFilled = false };
        figure.Segments.Add(poly);

        var geometry = new PathGeometry();
        geometry.Figures.Add(figure);

        return new Path
        {
            Data = geometry,
            Stroke = brush,
            StrokeThickness = thickness,
            StrokeLineJoin = PenLineJoin.Round,
            StrokeStartLineCap = PenLineCap.Round,
            StrokeEndLineCap = PenLineCap.Round,
            IsHitTestVisible = false
        };
    }

    private void Rebuild()
    {
        _rebuildQueued = false;
        if (!_loaded || _layer is null) return;

        _version++;
        _storyboard?.Stop();
        _storyboard = null;
        _layer.Children.Clear();
        _bands.Clear();

        double t = Math.Max(0.5, LineThickness);
        Brush brush = ResolveBrush();

        foreach (Rect r in MeasureLines())
        {
            double yTop = r.Bottom - r.Height * 0.12 + LineOffset;
            var band = new UnderlineAnimationBand(r.Width);

            if (Kind == UnderlineKind.Wavy)
            {
                double amp = Math.Max(1.5, t);
                double wavelength = t * 4 + 4;
                double yMid = yTop + t / 2 + amp;
                List<Point> pts = WavePoints(r.Left, r.Right, yMid, amp, wavelength);

                if (Rest)
                {
                    Path faint = MakeWavePath(pts, t, brush);
                    faint.Opacity = RestOpacity;
                    _layer.Children.Add(faint);
                }

                double length = 0;
                for (int i = 1; i < pts.Count; i++)
                {
                    double dx = pts[i].X - pts[i - 1].X, dy = pts[i].Y - pts[i - 1].Y;
                    length += Math.Sqrt(dx * dx + dy * dy);
                }
                double dash = (length + 4) / t;

                Path wave = MakeWavePath(pts, t, brush);
                wave.StrokeDashCap = PenLineCap.Round;
                wave.StrokeDashArray = new DoubleCollection { dash, dash };
                wave.StrokeDashOffset = _shown ? 0 : dash;
                _layer.Children.Add(wave);

                band.Wave = wave;
                band.Dash = dash;
            }
            else
            {
                bool fill = Kind == UnderlineKind.Fill;
                double height = fill ? Math.Max(t, yTop + t - r.Top) : t;
                double top = fill ? r.Top : yTop;

                if (Rest)
                {
                    var faint = new Rectangle
                    {
                        Width = r.Width,
                        Height = t,
                        Fill = brush,
                        Opacity = RestOpacity,
                        IsHitTestVisible = false
                    };
                    Canvas.SetLeft(faint, r.Left);
                    Canvas.SetTop(faint, yTop);
                    _layer.Children.Add(faint);
                }

                var st = RectState(_shown);
                var scale = new ScaleTransform { ScaleX = st.sx, ScaleY = st.sy };
                var rect = new Rectangle
                {
                    Width = r.Width,
                    Height = height,
                    Fill = brush,
                    Opacity = st.op,
                    RenderTransform = scale,
                    RenderTransformOrigin = OriginFor(_shown),
                    IsHitTestVisible = false
                };
                Canvas.SetLeft(rect, r.Left);
                Canvas.SetTop(rect, top);
                _layer.Children.Add(rect);

                band.Rect = rect;
                band.Scale = scale;
            }
            _bands.Add(band);
        }
    }

    private void SnapTo(bool shown)
    {
        _version++;
        _storyboard?.Stop();
        _storyboard = null;

        var st = RectState(shown);
        foreach (UnderlineAnimationBand b in _bands)
        {
            if (b.Wave is not null) b.Wave.StrokeDashOffset = shown ? 0 : b.Dash;
            if (b.Scale is not null && b.Rect is not null)
            {
                b.Scale.ScaleX = st.sx;
                b.Scale.ScaleY = st.sy;
                b.Rect.Opacity = st.op;
                b.Rect.RenderTransformOrigin = OriginFor(shown);
            }
        }
    }

    private void AddAnim(Storyboard sb, DependencyObject target, string property,
        double to, double? from, double begin, double dur, bool dependent = false)
    {
        var anim = new DoubleAnimation
        {
            To = to,
            Duration = TimeSpan.FromSeconds(dur),
            BeginTime = TimeSpan.FromSeconds(begin),
            EasingFunction = _ease,
            FillBehavior = FillBehavior.HoldEnd,
            EnableDependentAnimation = dependent
        };
        if (from.HasValue) anim.From = from.Value;
        Storyboard.SetTarget(anim, target);
        Storyboard.SetTargetProperty(anim, property);
        sb.Children.Add(anim);
    }

    private void Animate(bool toShown)
    {
        double duration = Math.Max(0.01, Duration / 1000.0);
        double total = 0;
        foreach (UnderlineAnimationBand b in _bands)
        {
            total += b.Width;
        }
        if (total <= 0) total = 1;

        int token = ++_version;
        _storyboard?.Stop();

        var sb = new Storyboard();

        var order = new List<UnderlineAnimationBand>(_bands);
        if (!toShown && (Kind == UnderlineKind.Slide || Kind == UnderlineKind.Gradient))
        {
            order.Reverse();
        }

        var st = RectState(toShown);
        var hidden = RectState(false);
        double acc = 0;

        foreach (UnderlineAnimationBand band in order)
        {
            double begin = duration * acc / total;
            double dur = Math.Max(0.05, duration * band.Width / total);
            acc += band.Width;

            if (band.Wave is not null)
            {
                AddAnim(sb, band.Wave, "StrokeDashOffset", toShown ? 0 : band.Dash,
                    toShown ? band.Dash : null, begin, dur, dependent: true);
            }
            else if (band.Scale is not null && band.Rect is not null)
            {
                band.Rect.RenderTransformOrigin = OriginFor(toShown);
                AddAnim(sb, band.Scale, "ScaleX", st.sx, toShown ? hidden.sx : null, begin, dur);
                AddAnim(sb, band.Scale, "ScaleY", st.sy, toShown ? hidden.sy : null, begin, dur);
                if (Kind == UnderlineKind.Fill)
                {
                    AddAnim(sb, band.Rect, "Opacity", st.op, toShown ? hidden.op : null, begin, dur);
                }
            }
        }

        sb.Completed += (_, _) =>
        {
            if (token != _version) return;
            SnapTo(toShown);
        };

        _storyboard = sb;
        sb.Begin();
    }
}
