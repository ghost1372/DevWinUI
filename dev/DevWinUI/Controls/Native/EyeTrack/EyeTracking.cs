using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;
using Windows.Win32;
using WinRT;
using Path = Microsoft.UI.Xaml.Shapes.Path;

namespace DevWinUI;

[TemplatePart(Name = RootPartName, Type = typeof(Canvas))]
public sealed partial class EyeTracking : Control
{
    private const string RootPartName = "PART_Root";
    private readonly List<EyeTrackingEye> _eyes = new();
    private readonly Random _rng = new();
    private readonly PointerEventHandler _pointerHandler;

    private Canvas? _root;
    private UIElement? _hooked;
    private IntPtr _hwnd;
    private System.Drawing.Point _lastCursor;
    private bool _hasCursor;
    private bool _loaded;
    private bool _buildQueued;
    private bool _hasPointer;
    private Point _pointer;
    private double _lastPointerTime = double.NegativeInfinity;
    private double _lastFrame;
    private double _blinkStart = -1;
    private double _nextBlink;
    private double _nextIdle;
    private double _idleX, _idleY;

    public EyeTracking()
    {
        DefaultStyleKey = typeof(EyeTracking);
        _pointerHandler = OnRootPointerMoved;
        AutomationProperties.SetName(this, "Animated eyes");

        Loaded += (_, _) =>
        {
            _loaded = true;
            AttachTracking();

            _lastFrame = -1;
            CompositionTarget.Rendering += OnRendering;
        };
        Unloaded += (_, _) =>
        {
            _loaded = false;
            CompositionTarget.Rendering -= OnRendering;
            DetachTracking();
        };
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _root = GetTemplateChild(RootPartName) as Canvas;
        Build();
    }

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((EyeTracking)d).InvalidateBuild();

    private void InvalidateBuild()
    {
        if (_root is null || _buildQueued)
        {
            return;
        }

        _buildQueued = true;
        DispatcherQueue.TryEnqueue(() =>
        {
            _buildQueued = false;
            Build();
        });
    }

    private static void OnGlobalMouseTrackingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((EyeTracking)d).AttachTracking();

    private void AttachTracking()
    {
        DetachTracking();

        if (!_loaded || XamlRoot is not { } xamlRoot)
        {
            return;
        }

        if (GlobalMouseTracking)
        {
            if (xamlRoot.ContentIslandEnvironment is { } environment)
            {
                _hwnd = Win32Interop.GetWindowFromWindowId(environment.AppWindowId);
            }

            _hasCursor = false;
        }
        else if (xamlRoot.Content is UIElement ui)
        {
            _hooked = ui;
            ui.AddHandler(PointerMovedEvent, _pointerHandler, true);
        }
    }

    private void DetachTracking()
    {
        _hooked?.RemoveHandler(PointerMovedEvent, _pointerHandler);
        _hooked = null;
        _hwnd = IntPtr.Zero;
    }

    private void OnRootPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        _pointer = e.GetCurrentPoint(null).Position;
        _hasPointer = true;
        _lastPointerTime = _lastFrame;
    }

    private void PollCursor(double now)
    {
        if (!PInvoke.GetCursorPos(out var screen))
        {
            return;
        }

        if (_hasCursor && screen.X == _lastCursor.X && screen.Y == _lastCursor.Y)
        {
            return;
        }

        _lastCursor = screen;
        _hasCursor = true;

        var client = screen;
        if (!PInvoke.ScreenToClient(new Windows.Win32.Foundation.HWND(_hwnd), ref client))
        {
            return;
        }

        double scale = XamlRoot?.RasterizationScale ?? 1;
        _pointer = new Point(client.X / scale, client.Y / scale);
        _hasPointer = true;
        _lastPointerTime = now;
    }


    private double NextBlinkDelay()
        => Math.Max(0.1, BlinkInterval / 1000.0 * (0.6 + _rng.NextDouble() * 0.8));

    private static Brush? WithOpacity(Brush? source, double opacity)
    {
        switch (source)
        {
            case SolidColorBrush solid:
                return new SolidColorBrush(solid.Color) { Opacity = solid.Opacity * opacity };

            case LinearGradientBrush linear:
                var copy = new LinearGradientBrush
                {
                    StartPoint = linear.StartPoint,
                    EndPoint = linear.EndPoint,
                    MappingMode = linear.MappingMode,
                    SpreadMethod = linear.SpreadMethod,
                    ColorInterpolationMode = linear.ColorInterpolationMode,
                    Opacity = linear.Opacity * opacity,
                };

                foreach (var stop in linear.GradientStops)
                {
                    copy.GradientStops.Add(new GradientStop { Color = stop.Color, Offset = stop.Offset });
                }

                return copy;

            default:
                return source;
        }
    }

    private EyeTrackingPalette CreatePalette()
    {
        var cyber = Kind == EyeKind.Cyber;

        return new EyeTrackingPalette
        {
            Sclera = cyber ? CyberScleraBrush : ScleraBrush,
            Iris = IrisBrush,
            IrisFill = WithOpacity(IrisBrush, 70 / 255d),
            IrisGlow = WithOpacity(IrisAccentBrush, 140 / 255d),
            IrisEdge = WithOpacity(OutlineBrush, 200 / 255d),
            Pupil = PupilBrush,
            Outline = OutlineBrush,
            Rim = RimBrush,
            Vein = VeinBrush,
            FiberDark = WithOpacity(ShadeBrush, 55 / 255d),
            FiberLight = WithOpacity(HighlightBrush, 50 / 255d),
            ReflectionLarge = WithOpacity(HighlightBrush, 235 / 255d),
            ReflectionSmall = WithOpacity(HighlightBrush, 150 / 255d),
            CyberHalo = WithOpacity(IrisBrush, 70 / 255d),
            CyberRingOuter = WithOpacity(IrisBrush, 160 / 255d),
            CyberRingInner = WithOpacity(IrisBrush, 110 / 255d),
            CyberScan = WithOpacity(IrisBrush, 130 / 255d),
            UpperShade = UpperEyelidShadeBrush,
            LowerShade = LowerEyelidShadeBrush,
        };
    }

    private static Ellipse Oval(Canvas parent, double x, double y, double w, double h,
        Brush? fill = null, Brush? stroke = null, double thickness = 0)
    {
        var e = new Ellipse();
        if (stroke is not null && thickness > 0)
        {
            e.Stroke = stroke;
            e.StrokeThickness = thickness;
            e.Width = Math.Max(0, w - thickness);
            e.Height = Math.Max(0, h - thickness);
            Canvas.SetLeft(e, x + thickness / 2);
            Canvas.SetTop(e, y + thickness / 2);
        }
        else
        {
            e.Width = w;
            e.Height = h;
            Canvas.SetLeft(e, x);
            Canvas.SetTop(e, y);
        }

        if (fill is not null)
        {
            e.Fill = fill;
        }

        parent.Children.Add(e);
        return e;
    }

    private static Line AddLine(Canvas parent, double x1, double y1, double x2, double y2, Brush? brush, double thickness)
    {
        var l = new Line
        {
            X1 = x1,
            Y1 = y1,
            X2 = x2,
            Y2 = y2,
            Stroke = brush,
            StrokeThickness = thickness,
        };
        parent.Children.Add(l);
        return l;
    }

    private static Point Pt(double cx, double cy, double r, double deg)
    {
        double a = deg * Math.PI / 180;
        return new Point(cx + r * Math.Sin(a), cy - r * Math.Cos(a));
    }

    private static Rectangle Shade(double w, double h, double y, Brush? fill, double opacity)
    {
        var r = new Rectangle { Width = w, Height = h, Fill = fill, Opacity = opacity, IsHitTestVisible = false };
        Canvas.SetTop(r, y);
        return r;
    }

    private static void ApplyCircleClip(UIElement element, double radius)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);
        var compositor = visual.Compositor;
        var geometry = compositor.CreateEllipseGeometry();
        geometry.Center = new Vector2((float)radius, (float)radius);
        geometry.Radius = new Vector2((float)radius, (float)radius);
        visual.Clip = compositor.CreateGeometricClip(geometry);
    }

    private void Build()
    {
        _eyes.Clear();

        if (_root is null)
        {
            return;
        }

        _root.Children.Clear();

        var palette = CreatePalette();
        double d = Math.Max(8, EyeSize);
        int n = Math.Max(1, EyeCount);
        _root.Width = n * d + (n - 1) * Math.Max(0, Gap);
        _root.Height = d;

        for (int i = 0; i < n; i++)
        {
            _eyes.Add(BuildEye(_root, palette, i, i * (d + Math.Max(0, Gap)), d));
        }
    }

    private EyeTrackingEye BuildEye(Canvas host, EyeTrackingPalette p, int index, double x0, double d)
    {
        double r = d / 2;
        var v = Kind;
        bool real = v == EyeKind.Realistic;
        bool cartoon = v == EyeKind.Cartoon;
        bool minimal = v == EyeKind.Minimal;
        bool cyber = v == EyeKind.Cyber;

        var eye = new EyeTrackingEye { Cx = x0 + r, Cy = r };

        var root = new Canvas
        {
            Width = d,
            Height = d,
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = eye.Blink,
        };
        Canvas.SetLeft(root, x0);
        host.Children.Add(root);

        var content = new Canvas { Width = d, Height = d };
        root.Children.Add(content);
        ApplyCircleClip(content, r);

        Oval(content, 0, 0, d, d, p.Sclera);

        if (real)
        {
            var rng = new Random(11 + index * 7);
            for (int k = 0; k < 9; k++)
            {
                double ang = rng.NextDouble() * 360;
                double endR = r * (0.64 + rng.NextDouble() * 0.14);
                var start = Pt(r, r, r * 1.02, ang);
                var end = Pt(r, r, endR, ang + (rng.NextDouble() - 0.5) * 10);
                var ctrl = Pt(r, r, (r + endR) / 2, ang + (rng.NextDouble() - 0.5) * 16);
                var fig = new PathFigure { StartPoint = start };
                fig.Segments.Add(new QuadraticBezierSegment { Point1 = ctrl, Point2 = end });
                var g = new PathGeometry();
                g.Figures.Add(fig);
                content.Children.Add(new Path
                {
                    Data = g,
                    Stroke = p.Vein,
                    StrokeThickness = Math.Max(0.6, d * 0.007),
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                });
            }
        }

        double ir = d * 0.25;
        double irisD = ir * 2;
        var iris = new Canvas
        {
            Width = irisD,
            Height = irisD,
            RenderTransformOrigin = new Point(0.5, 0.5),
            RenderTransform = eye.Iris,
        };
        Canvas.SetLeft(iris, r - ir);
        Canvas.SetTop(iris, r - ir);
        content.Children.Add(iris);

        if (real)
        {
            Oval(iris, 0, 0, irisD, irisD, p.Iris, p.IrisEdge, irisD * 0.05);
            Oval(iris, irisD * 0.15, irisD * 0.15, irisD * 0.7, irisD * 0.7, p.IrisGlow);
        }
        else if (cartoon)
        {
            Oval(iris, 0, 0, irisD, irisD, p.Iris, p.Outline, irisD * 0.06);
        }
        else if (minimal)
        {
            Oval(iris, 0, 0, irisD, irisD, p.Iris);
        }
        else
        {
            Oval(iris, 0, 0, irisD, irisD, p.IrisFill, p.Iris, 2);
        }

        double pupilD = irisD * 0.44;

        if (ShowIrisDetail && real)
        {
            var rng = new Random(101 + index * 13);
            for (int k = 0; k < 44; k++)
            {
                double ang = k * 360.0 / 44 + rng.NextDouble() * 4;
                var p1 = Pt(ir, ir, pupilD / 2 * 1.15, ang);
                var p2 = Pt(ir, ir, ir * (0.78 + rng.NextDouble() * 0.18), ang);
                AddLine(iris, p1.X, p1.Y, p2.X, p2.Y, k % 2 == 0 ? p.FiberDark : p.FiberLight, 1);
            }
        }
        else if (ShowIrisDetail && cyber)
        {
            Oval(iris, irisD * 0.14, irisD * 0.14, irisD * 0.72, irisD * 0.72, null, p.CyberRingOuter, 1);
            Oval(iris, irisD * 0.25, irisD * 0.25, irisD * 0.5, irisD * 0.5, null, p.CyberRingInner, 1);

            var ticks = new Canvas
            {
                Width = irisD,
                Height = irisD,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = eye.Ticks = new CompositeTransform(),
            };
            for (int k = 0; k < 24; k++)
            {
                var p1 = Pt(ir, ir, ir * 0.8, k * 15);
                var p2 = Pt(ir, ir, ir * 0.93, k * 15);
                AddLine(ticks, p1.X, p1.Y, p2.X, p2.Y, p.Iris, 1);
            }

            iris.Children.Add(ticks);

            var arcs = new Canvas
            {
                Width = irisD,
                Height = irisD,
                RenderTransformOrigin = new Point(0.5, 0.5),
                RenderTransform = eye.Arcs = new CompositeTransform(),
            };
            for (int k = 0; k < 3; k++)
            {
                var a = Pt(ir, ir, ir * 0.62, k * 120);
                var b = Pt(ir, ir, ir * 0.62, k * 120 + 70);
                var fig = new PathFigure { StartPoint = a };
                fig.Segments.Add(new ArcSegment
                {
                    Point = b,
                    Size = new Windows.Foundation.Size(ir * 0.62, ir * 0.62),
                    SweepDirection = SweepDirection.Clockwise,
                });
                var g = new PathGeometry();
                g.Figures.Add(fig);
                arcs.Children.Add(new Path { Data = g, Stroke = p.Iris, StrokeThickness = 2 });
            }

            iris.Children.Add(arcs);
        }

        var pupil = Oval(iris, ir - pupilD / 2, ir - pupilD / 2, pupilD, pupilD, p.Pupil);
        pupil.RenderTransformOrigin = new Point(0.5, 0.5);
        pupil.RenderTransform = eye.Pupil;

        if (ShowReflection)
        {
            double big = irisD * (cartoon ? 0.24 : 0.16);
            Oval(iris, ir - irisD * 0.17 - big / 2, ir - irisD * 0.17 - big / 2, big, big, p.ReflectionLarge);
            double small = irisD * 0.07;
            Oval(iris, ir + irisD * 0.13, ir + irisD * 0.13, small, small, p.ReflectionSmall);
        }

        if (cyber)
        {
            eye.Scan = new Rectangle { Width = d, Height = 2, Fill = p.CyberScan, IsHitTestVisible = false };
            content.Children.Add(eye.Scan);
        }

        if (ShowEyelids)
        {
            content.Children.Add(Shade(d, d * 0.42, 0, p.UpperShade, real ? 1 : 75 / 120d));
            content.Children.Add(Shade(d, d * 0.18, d * 0.82, p.LowerShade, 1));
        }

        if (cartoon)
        {
            Oval(root, 0, 0, d, d, null, p.Outline, d * 0.06);
        }
        else if (minimal)
        {
            Oval(root, 0, 0, d, d, null, p.Outline, 2);
        }
        else if (cyber)
        {
            Oval(root, 0, 0, d, d, null, p.CyberHalo, 8);
            Oval(root, 0, 0, d, d, null, p.Iris, 2);
        }
        else
        {
            Oval(root, 0, 0, d, d, null, p.Rim, 1.5);
        }

        return eye;
    }

    private void OnRendering(object? sender, object e)
    {
        if (!_loaded || _eyes.Count == 0)
        {
            return;
        }

        double now = e.As<RenderingEventArgs>().RenderingTime.TotalSeconds;
        if (_lastFrame < 0)
        {
            _lastFrame = now;
            _nextBlink = now + NextBlinkDelay();
            return;
        }

        double dt = Math.Clamp(now - _lastFrame, 0.001, 0.05);
        _lastFrame = now;

        double d = Math.Max(8, EyeSize);
        double r = d / 2;
        double maxOff = (r - d * 0.25) * Math.Clamp(PupilRange, 0, 1);
        double k = 1 - Math.Exp(-dt * 14);

        if (_hwnd != IntPtr.Zero)
        {
            PollCursor(now);
        }

        bool pointerActive = _hasPointer && _root is not null && (!IdleAnimation || now - _lastPointerTime < 2.5);
        Point origin = default;
        if (pointerActive)
        {
            origin = _root!.TransformToVisual(null).TransformPoint(default);
        }

        if (!pointerActive && IdleAnimation && now >= _nextIdle)
        {
            _idleX = (_rng.NextDouble() * 2 - 1) * 0.7;
            _idleY = (_rng.NextDouble() * 2 - 1) * 0.5;
            _nextIdle = now + 0.6 + _rng.NextDouble() * 1.8;
        }

        double blink = BlinkValue(now);

        foreach (var eye in _eyes)
        {
            double tx = 0, ty = 0, proximity = 0.25;
            if (pointerActive)
            {
                double dx = _pointer.X - origin.X - eye.Cx, dy = _pointer.Y - origin.Y - eye.Cy;
                double dist = Math.Sqrt(dx * dx + dy * dy);
                if (dist > 0.001)
                {
                    double mag = maxOff * Math.Min(1, dist / (d * 1.2));
                    tx = dx / dist * mag;
                    ty = dy / dist * mag;
                }

                proximity = Math.Clamp(1 - dist / (d * 4), 0, 1);
            }
            else if (IdleAnimation)
            {
                tx = _idleX * maxOff;
                ty = _idleY * maxOff;
            }

            eye.CurX += (tx - eye.CurX) * k;
            eye.CurY += (ty - eye.CurY) * k;
            eye.Iris.TranslateX = eye.CurX;
            eye.Iris.TranslateY = eye.CurY;

            double targetDilate = ReactivePupil ? 0.82 + 0.55 * proximity : 1;
            eye.Dilate += (targetDilate - eye.Dilate) * k;
            eye.Pupil.ScaleX = eye.Pupil.ScaleY = eye.Dilate;

            eye.Blink.ScaleY = 1 - 0.94 * blink;

            if (eye.Scan is not null)
            {
                Canvas.SetTop(eye.Scan, now / 2.5 % 1 * d);
            }

            if (eye.Ticks is not null) eye.Ticks.Rotation = now * 20 % 360;
            if (eye.Arcs is not null) eye.Arcs.Rotation = -now * 45 % 360;
        }
    }

    private double BlinkValue(double now)
    {
        if (BlinkInterval <= 0)
        {
            return 0;
        }

        if (_blinkStart < 0)
        {
            if (now < _nextBlink)
            {
                return 0;
            }

            _blinkStart = now;
        }

        double u = (now - _blinkStart) / 0.16;
        if (u >= 1)
        {
            _blinkStart = -1;
            _nextBlink = now + (_rng.NextDouble() < 0.2 ? 0.14 : NextBlinkDelay());
            return 0;
        }

        return Math.Sin(u * Math.PI);
    }
}
