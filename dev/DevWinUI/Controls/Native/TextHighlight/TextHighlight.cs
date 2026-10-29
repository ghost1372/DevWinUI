using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = PartRoot, Type = typeof(Grid))]
[TemplatePart(Name = PartLayer, Type = typeof(Canvas))]
[TemplatePart(Name = PartText, Type = typeof(TextBlock))]
public sealed partial class TextHighlight : Control
{

    private const string PartRoot = "PART_Root";
    private const string PartLayer = "PART_Layer";
    private const string PartText = "PART_Text";

    private readonly Run _run = new();
    private readonly List<TextHighlightBand> _bands = new();
    private readonly CubicEase _ease = new() { EasingMode = EasingMode.EaseOut };

    private Grid? _root;
    private Canvas? _layer;
    private TextBlock? _textBlock;
    private Storyboard? _storyboard;
    private int _version;
    private bool _target;
    private bool _pendingAnimate;
    private bool _rebuildQueued;
    private bool _loaded, _viewSeen, _hovering;

    public TextHighlight()
    {
        DefaultStyleKey = typeof(TextHighlight);

        RegisterPropertyChangedCallback(FontSizeProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontFamilyProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontWeightProperty, (_, _) => QueueRebuild());
        RegisterPropertyChangedCallback(FontStyleProperty, (_, _) => QueueRebuild());

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override AutomationPeer OnCreateAutomationPeer() => new TextHighlightAutomationPeer(this);

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

    private void OnHighlightBrushChanged()
    {
        Brush brush = HighlightBrush;
        foreach (TextHighlightBand band in _bands)
        {
            band.Rect.Fill = brush;
        }
    }

    private void OnBandOpacityChanged()
    {
        double opacity = BandOpacity;
        foreach (TextHighlightBand band in _bands)
        {
            band.Rect.Opacity = opacity;
        }
    }

    private void OnBandCornerRadiusChanged()
    {
        double radius = BandCornerRadius;
        foreach (TextHighlightBand band in _bands)
        {
            band.Rect.RadiusX = radius;
            band.Rect.RadiusY = radius;
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
            TextHighlightTrigger.Mount => _loaded,
            TextHighlightTrigger.View => _viewSeen,
            TextHighlightTrigger.Hover => _hovering,
            _ => false
        });
        SetTarget(desired);
    }

    private void SetTarget(bool drawn)
    {
        if (drawn == _target) return;
        _target = drawn;

        if (_bands.Count == 0)
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
        string target = HighlightText;

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
        double pad = BandPadding;

        void Flush()
        {
            if (!open) return;
            lines.Add(new Rect(left - pad, top - pad, (right - left) + pad * 2, (bottom - top) + pad * 2));
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

    private void Rebuild()
    {
        _rebuildQueued = false;
        if (!_loaded || _layer is null) return;

        _version++;
        _storyboard?.Stop();
        _storyboard = null;
        _layer.Children.Clear();
        _bands.Clear();

        bool snapDrawn = _target && !_pendingAnimate;
        Brush brush = HighlightBrush;
        double opacity = BandOpacity;
        double radius = BandCornerRadius;

        foreach (Rect r in MeasureLines())
        {
            var scale = new ScaleTransform { ScaleX = snapDrawn ? 1 : 0, ScaleY = 1 };
            var rect = new Rectangle
            {
                Width = r.Width,
                Height = r.Height,
                RadiusX = radius,
                RadiusY = radius,
                Fill = brush,
                Opacity = opacity,
                RenderTransform = scale,
                RenderTransformOrigin = new Point(0, 0.5),
                IsHitTestVisible = false
            };
            Canvas.SetLeft(rect, r.X);
            Canvas.SetTop(rect, r.Y);
            _layer.Children.Add(rect);
            _bands.Add(new TextHighlightBand(rect, scale, r.Width));
        }

        if (_pendingAnimate && _target && _bands.Count > 0)
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
        foreach (TextHighlightBand band in _bands)
        {
            band.Scale.ScaleX = drawn ? 1 : 0;
        }
    }

    private void Animate(bool toDrawn)
    {
        double duration = Math.Max(0.01, Duration);
        double delay = Math.Max(0, Delay);
        double total = 0;
        foreach (TextHighlightBand band in _bands)
        {
            total += band.Width;
        }
        if (total <= 0) total = 1;

        int token = ++_version;
        _storyboard?.Stop();

        var sb = new Storyboard();
        double acc = 0;

        foreach (TextHighlightBand band in _bands)
        {
            double begin, dur;
            if (toDrawn)
            {
                begin = delay + duration * acc / total;
                dur = Math.Max(0.05, duration * band.Width / total);
                acc += band.Width;
            }
            else
            {
                begin = 0;
                dur = Math.Max(0.05, duration * 0.5);
            }

            var anim = new DoubleAnimation
            {
                To = toDrawn ? 1 : 0,
                Duration = TimeSpan.FromSeconds(dur),
                BeginTime = TimeSpan.FromSeconds(begin),
                EasingFunction = _ease,
                FillBehavior = FillBehavior.HoldEnd
            };
            Storyboard.SetTarget(anim, band.Scale);
            Storyboard.SetTargetProperty(anim, "ScaleX");
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
