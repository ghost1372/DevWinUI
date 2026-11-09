using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using WinRT;
using Color = Windows.UI.Color;

namespace DevWinUI;

[TemplatePart(Name = PartRoot, Type = typeof(Grid))]
[TemplatePart(Name = PartHost, Type = typeof(Grid))]
[TemplatePart(Name = PartFill, Type = typeof(Rectangle))]
[TemplatePart(Name = PartParticles, Type = typeof(Canvas))]
[TemplatePart(Name = PartContent, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartInvHost, Type = typeof(Grid))]
[TemplatePart(Name = PartInvContent, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PartOutline, Type = typeof(Rectangle))]
public sealed partial class FizzyCover : ContentControl
{
    private const string PartRoot = "PART_Root";
    private const string PartHost = "PART_Host";
    private const string PartFill = "PART_Fill";
    private const string PartParticles = "PART_Particles";
    private const string PartContent = "PART_Content";
    private const string PartInvHost = "PART_InvHost";
    private const string PartInvContent = "PART_InvContent";
    private const string PartOutline = "PART_Outline";

    private const double OutlineWidth = 1.5;
    private const double VisTauMs = 150;

    private Grid? _root, _host, _invHost;
    private Rectangle? _fill, _outline;
    private TranslateTransform? _fillShift;
    private Canvas? _canvasParticles;
    private ContentPresenter? _content, _invContent;
    private RectangleGeometry? _invClip;
    private CompositionRoundedRectangleGeometry? _geo;

    private readonly List<FizzyCoverParticle> _particles = new();
    private readonly SolidColorBrush _fillBrush = new(Colors.White);
    private readonly SolidColorBrush _contentBrush = new(Colors.White);
    private readonly SolidColorBrush _invBrush = new(Colors.Black);
    private double _w, _h;
    private bool _mirror;

    private double _progress;
    private double _vis;
    private bool _kbFocus;
    private bool _pointerOver;
    private bool _pointerDown;
    private bool _hasPointer;
    private Point _cursor;

    private TimeSpan _last;
    private bool _running;
    private bool _visible = true;
    private bool _particlesQueued;
    private UIElement? _hooked;

    public FizzyCover()
    {
        DefaultStyleKey = typeof(FizzyCover);

        PointerEntered += (_, _) => { _pointerOver = true; Kick(); };
        PointerExited += (_, _) => { _pointerOver = false; Kick(); };
        AddHandler(PointerPressedEvent, new PointerEventHandler((_, _) => { _pointerDown = true; Kick(); }), true);
        AddHandler(PointerReleasedEvent, new PointerEventHandler((_, _) => { _pointerDown = false; Kick(); }), true);
        AddHandler(PointerCanceledEvent, new PointerEventHandler((_, _) => { _pointerDown = false; Kick(); }), true);
        AddHandler(PointerCaptureLostEvent, new PointerEventHandler((_, _) => { _pointerDown = false; Kick(); }), true);
        AddHandler(TappedEvent, new TappedEventHandler((_, _) =>
        {
            if (Trigger == FizzyTrigger.Toggle) Pressed = !Pressed;
        }), true);

        GotFocus += (_, e) =>
        {
            _kbFocus = (e.OriginalSource as Control)?.FocusState == FocusState.Keyboard;
            Kick();
        };
        LostFocus += (_, _) => { _kbFocus = false; Kick(); };
        IsEnabledChanged += (_, _) =>
        {
            Opacity = IsEnabled ? 1 : 0.5;
            if (!IsEnabled) { _hasPointer = false; _pointerOver = false; _pointerDown = false; }
            Kick();
        };
        ActualThemeChanged += (_, _) => ApplyColors();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        EffectiveViewportChanged += (_, e) =>
        {
            _visible = e.EffectiveViewport.Width > 0 && e.EffectiveViewport.Height > 0;
            if (_visible) Kick(); else StopLoop();
        };
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_root != null) _root.SizeChanged -= OnRootSizeChanged;

        _root = GetTemplateChild(PartRoot) as Grid;
        _host = GetTemplateChild(PartHost) as Grid;
        _fill = GetTemplateChild(PartFill) as Rectangle;
        _canvasParticles = GetTemplateChild(PartParticles) as Canvas;
        _content = GetTemplateChild(PartContent) as ContentPresenter;
        _invHost = GetTemplateChild(PartInvHost) as Grid;
        _invContent = GetTemplateChild(PartInvContent) as ContentPresenter;
        _outline = GetTemplateChild(PartOutline) as Rectangle;

        _fillShift = new TranslateTransform();
        if (_fill != null) _fill.RenderTransform = _fillShift;
        _invClip = new RectangleGeometry();
        if (_invHost != null) _invHost.Clip = _invClip;
        if (_content != null) _content.Foreground = _contentBrush;
        if (_invContent != null) _invContent.Foreground = _invBrush;

        _geo = null;
        if (_root != null) _root.SizeChanged += OnRootSizeChanged;

        UpdateMirror();
        ApplyColors();
        QueueParticles();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        UpdateMirror();
    }

    private void OnRootSizeChanged(object sender, SizeChangedEventArgs e) => OnSized(e.NewSize.Width, e.NewSize.Height);

    private bool IsDark => ActualTheme == ElementTheme.Dark;

    private Color Ink => InkColor.A > 0 ? InkColor : IsDark ? Colors.White : Colors.Black;
    private Color Invert => InvertColor.A > 0 ? InvertColor : IsDark ? Color.FromArgb(255, 16, 16, 16) : Colors.White;

    private double CornerR(double h) => Math.Min(Math.Max(0, Radius), h / 2);

    private void UpdateMirror()
    {
        object? twin = InvertedContent ?? (Content is UIElement ? null : Content);
        _mirror = twin != null;
        if (_invContent != null) _invContent.Content = twin;
        if (_invHost != null) _invHost.Visibility = _mirror ? Visibility.Visible : Visibility.Collapsed;
        Paint(0, EaseInOutCubic(_progress), move: false);
    }

    private void ApplyColors()
    {
        var ink = Ink;
        _fillBrush.Color = ink;
        _contentBrush.Color = ink;
        _invBrush.Color = Invert;
        if (_fill != null) _fill.Fill = _fillBrush;
        if (_outline != null) _outline.Stroke = new SolidColorBrush(ink);
        FocusVisualPrimaryBrush = new SolidColorBrush(ink);

        foreach (var p in _particles) p.Argb = uint.MaxValue;
        Paint(0, EaseInOutCubic(_progress), move: false);
        Kick();
    }

    private void ApplyShape() => OnSized(_w, _h);

    private void OnSized(double w, double h)
    {
        _w = w;
        _h = h;
        double r = CornerR(h);

        if (_host != null)
        {
            var visual = ElementCompositionPreview.GetElementVisual(_host);
            if (_geo == null)
            {
                var comp = visual.Compositor;
                _geo = comp.CreateRoundedRectangleGeometry();
                visual.Clip = comp.CreateGeometricClip(_geo);
            }
            _geo.Size = new Vector2((float)w, (float)h);
            _geo.CornerRadius = new Vector2((float)r, (float)r);
        }

        if (_outline != null)
        {
            _outline.StrokeThickness = OutlineWidth;
            _outline.Margin = new Thickness(OutlineWidth / 2);
            _outline.RadiusX = _outline.RadiusY = Math.Max(0, r - OutlineWidth / 2);
        }
        Paint(0, EaseInOutCubic(_progress), move: false);
        Kick();
    }

    private void QueueParticles()
    {
        if (_particlesQueued) return;
        _particlesQueued = true;
        DispatcherQueue.TryEnqueue(() =>
        {
            _particlesQueued = false;
            BuildParticles();
        });
    }

    private void BuildParticles()
    {
        if (_canvasParticles == null) return;
        _canvasParticles.Children.Clear();
        _particles.Clear();

        int count = Math.Max(0, Count);
        double baseSize = Size, variation = Variation;
        var shape = ParticleShape;

        var rnd = new Random(7);
        for (int i = 0; i < count; i++)
        {
            double size = Math.Max(1.5, baseSize * 4 * (1 - variation * rnd.NextDouble()));
            var kind = shape == FizzyShape.Mixed ? (FizzyShape)(1 + rnd.Next(4)) : shape;

            var brush = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            var t = new CompositeTransform { CenterX = size / 2, CenterY = size / 2 };
            Shape el = MakeShape(kind, size);
            el.Fill = brush;
            el.RenderTransform = t;
            el.IsHitTestVisible = false;
            _canvasParticles.Children.Add(el);

            _particles.Add(new FizzyCoverParticle
            {
                El = el,
                Brush = brush,
                T = t,
                X = rnd.NextDouble(),
                Y = rnd.NextDouble(),
                Speed = 0.6 + 0.8 * rnd.NextDouble(),
                Phase = rnd.NextDouble() * Math.PI * 2,
                Freq = 0.6 + 1.4 * rnd.NextDouble(),
                Rot = rnd.NextDouble() * 360,
                SpinRate = (rnd.NextDouble() < 0.5 ? -1 : 1) * (30 + 90 * rnd.NextDouble()),
                Size = size,
                Argb = uint.MaxValue,
            });
        }

        Paint(0, EaseInOutCubic(_progress), move: false);
        Kick();
    }

    private static Shape MakeShape(FizzyShape kind, double s)
    {
        switch (kind)
        {
            case FizzyShape.Square:
                return new Rectangle { Width = s, Height = s };

            case FizzyShape.Triangle:
                return new Polygon
                {
                    Points = new PointCollection
                    {
                        new Point(s / 2, 0), new Point(s, s * 0.87), new Point(0, s * 0.87),
                    },
                };

            case FizzyShape.Cross:
                double a = s / 3;
                return new Polygon
                {
                    Points = new PointCollection
                    {
                        new Point(a, 0), new Point(2 * a, 0), new Point(2 * a, a), new Point(s, a),
                        new Point(s, 2 * a), new Point(2 * a, 2 * a), new Point(2 * a, s), new Point(a, s),
                        new Point(a, 2 * a), new Point(0, 2 * a), new Point(0, a), new Point(a, a),
                    },
                };

            default:
                return new Ellipse { Width = s, Height = s };
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _hooked = XamlRoot?.Content;
        _hooked?.AddHandler(PointerMovedEvent, new PointerEventHandler(OnRootPointerMoved), true);
        _hooked?.AddHandler(PointerExitedEvent, new PointerEventHandler(OnRootPointerExited), true);
        Kick();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _hooked?.RemoveHandler(PointerMovedEvent, new PointerEventHandler(OnRootPointerMoved));
        _hooked?.RemoveHandler(PointerExitedEvent, new PointerEventHandler(OnRootPointerExited));
        _hooked = null;
        StopLoop();
    }

    private void OnRootPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!IsEnabled) return;
        try
        {
            _cursor = e.GetCurrentPoint(this).Position;
            _hasPointer = true;
            Kick();
        }
        catch
        {
            _hasPointer = false;
        }
    }

    private void OnRootPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _hasPointer = false;
        Kick();
    }

    private double PointerDistance()
    {
        if (!_hasPointer || !IsEnabled) return double.MaxValue;
        double dx = Math.Max(Math.Max(-_cursor.X, _cursor.X - _w), 0);
        double dy = Math.Max(Math.Max(-_cursor.Y, _cursor.Y - _h), 0);
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private void Kick()
    {
        if (_running || !_visible || !IsLoaded || _root == null) return;
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

    private double TargetFill()
    {
        bool engaged = _pointerOver || _pointerDown || _kbFocus;
        if (!IsEnabled) engaged = false;
        bool locked = Trigger == FizzyTrigger.Toggle && Pressed;
        return engaged || locked ? 1 : 0;
    }

    private void OnRendering(object? sender, object e)
    {
        TimeSpan now = e.As<RenderingEventArgs>().RenderingTime;
        if (now == _last) return;

        double dt = _last == TimeSpan.Zero
            ? 1000.0 / 60
            : Math.Min(100, (now - _last).TotalMilliseconds);
        _last = now;

        double target = TargetFill();
        double duration = FillDuration;
        if (duration <= 0) _progress = target;
        else
        {
            double step = dt / 1000 / duration;
            _progress += Math.Clamp(target - _progress, -step, step);
        }
        double fill = EaseInOutCubic(_progress);

        double proximity = Proximity;
        double d = PointerDistance();
        double near = d <= 0 ? 1 : proximity <= 0 ? 0 : Math.Clamp(1 - d / proximity, 0, 1);
        double visTarget = Math.Max(near * near * (3 - 2 * near), _progress > 0 ? 1 : 0);
        _vis += (visTarget - _vis) * (1 - Math.Exp(-dt / VisTauMs));
        if (Math.Abs(visTarget - _vis) < 0.002) _vis = visTarget;

        bool awake = d < proximity + WakeMargin || _progress > 0 || target > 0;
        Paint(dt, fill, move: awake);

        bool moving = awake || Math.Abs(target - _progress) > 1e-4 || _vis > 0.002;
        if (!moving) StopLoop();
    }

    private void Paint(double dt, double fill, bool move)
    {
        double w = _w, h = _h;
        if (w <= 0 || h <= 0) return;

        if (_fillShift != null) _fillShift.Y = (1 - fill) * h;
        if (_invClip != null) _invClip.Rect = new Rect(0, h * (1 - fill), w, h * fill + 1);

        var ink = Ink;
        var inv = Invert;

        if (_mirror)
        {
            _contentBrush.Color = ink;
        }
        else
        {
            double t = Math.Clamp((fill - 0.35) / 0.3, 0, 1);
            _contentBrush.Color = Lerp(ink, inv, t);
        }

        if (_canvasParticles == null) return;
        _canvasParticles.Visibility = _vis > 0.002 ? Visibility.Visible : Visibility.Collapsed;
        if (_vis <= 0.002) return;

        double drift = Drift, spin = Spin, crest = Crest;
        double fillTop = h * (1 - fill);
        double edge = 4 * fill * (1 - fill);
        double rise = IdleSpeed + (ActiveSpeed - IdleSpeed) * fill;
        double alphaBase = RestAlpha + (ActiveAlpha - RestAlpha) * fill;

        var rnd = Random.Shared;

        foreach (var p in _particles)
        {
            if (move)
            {
                p.Y -= rise * p.Speed * 30 * dt / 1000 / h;
                p.Phase += p.Freq * dt / 1000;
                p.Rot += spin * p.SpinRate * dt / 1000;
                if (p.Y < -0.15)
                {
                    p.Y = 1.15;
                    p.X = rnd.NextDouble();
                }
            }

            double px = p.X * w + Math.Sin(p.Phase) * drift * 6;
            double py = p.Y * h;

            double bump = Math.Exp(-Math.Pow((py - fillTop) / (h * 0.2), 2));
            double scale = 1 + crest * 0.9 * edge * bump;
            double alpha = Math.Clamp(alphaBase * _vis * (1 + crest * 0.8 * edge * bump), 0, 1);

            p.T.TranslateX = px - p.Size / 2;
            p.T.TranslateY = py - p.Size / 2;
            p.T.Rotation = p.Rot;
            p.T.ScaleX = p.T.ScaleY = scale;

            var c = fill > 0 && py > fillTop ? inv : ink;
            uint argb = (uint)((byte)Math.Round(alpha * 255) << 24 | c.R << 16 | c.G << 8 | c.B);
            if (argb != p.Argb)
            {
                p.Argb = argb;
                p.Brush.Color = Color.FromArgb((byte)(argb >> 24), c.R, c.G, c.B);
            }
        }
    }

    private static Color Lerp(Color a, Color b, double t) => Color.FromArgb(255,
        (byte)(a.R + (b.R - a.R) * t), (byte)(a.G + (b.G - a.G) * t), (byte)(a.B + (b.B - a.B) * t));

    private static double EaseInOutCubic(double t) =>
        t < 0.5 ? 4 * t * t * t : 1 - Math.Pow(-2 * t + 2, 3) / 2;
}
