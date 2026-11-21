namespace DevWinUI;

[TemplatePart(Name = PartHost, Type = typeof(Grid))]
public sealed partial class BorderBeam : Control
{
    private const string PartHost = "PART_Host";
    private const double SegmentLength = 3;

    private enum Lead
    {
        None,
        Forward,
        Backward
    }

    private sealed record RimPath(Vector2[] Points, double Perimeter, double Origin, double ChaseStart);

    private sealed record BeamStyle(BorderBeamMode Mode, Lead Lead, double Lag, double Length, int Count, double Duration, double Speed, double Delay, float Thickness, Color From, Color To, BorderBeamDirection Direction, int Version);

    private Grid? _host;
    private CanvasAnimatedControl? _canvas;
    private volatile RimPath? _path;
    private volatile BeamStyle? _style;
    private TimeSpan _origin;
    private double _referencePerimeter;
    private double _distance;
    private int _seenVersion;
    private bool _visible = true;

    public BorderBeam()
    {
        DefaultStyleKey = typeof(BorderBeam);

        SizeChanged += (_, _) => RebuildPath();
        RegisterPropertyChangedCallback(CornerRadiusProperty, (_, _) => RebuildPath());

        Loaded += (_, _) => Attach();
        Unloaded += (_, _) => Detach();
        EffectiveViewportChanged += (_, e) =>
        {
            _visible = e.EffectiveViewport.Width > 0 && e.EffectiveViewport.Height > 0;
            _canvas?.Paused = !_visible;
        };
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Detach();
        _host = GetTemplateChild(PartHost) as Grid;
        Attach();
    }

    private void Attach()
    {
        if (_canvas is not null || _host is null || !IsLoaded) return;

        _canvas = new CanvasAnimatedControl
        {
            ClearColor = Colors.Transparent,
            IsHitTestVisible = false,
            Paused = !_visible,
        };
        _canvas.Draw += OnDraw;
        _host.Children.Add(_canvas);

        RebuildPath();
        PublishStyle(true);
    }

    private void Detach()
    {
        if (_canvas is null) return;

        _canvas.Draw -= OnDraw;
        _host?.Children.Remove(_canvas);
        _canvas.RemoveFromVisualTree();
        _canvas = null;
    }

    private double Inset => BorderWidth / 2;

    private double RimW => Math.Max(0, ActualWidth - 2 * Inset);
    private double RimH => Math.Max(0, ActualHeight - 2 * Inset);

    private double Rr => Math.Max(0, Math.Min(CornerRadius.TopLeft - Inset, Math.Min(RimW, RimH) / 2));

    private double Perimeter()
    {
        var r = Rr;
        return 2 * (Math.Max(0, RimW - 2 * r) + Math.Max(0, RimH - 2 * r)) + 2 * Math.PI * r;
    }

    private Vector2 RimPoint(double s)
    {
        var w = RimW;
        var h = RimH;
        var r = Rr;
        var top = Math.Max(0, w - 2 * r);
        var side = Math.Max(0, h - 2 * r);
        var arc = Math.PI * r / 2;
        var k = r == 0 ? 1 : r;
        double x, y;

        if (s < top) { x = r + s; y = 0; }
        else if ((s -= top) < arc) { var a = s / k; x = w - r + r * Math.Cos(-Math.PI / 2 + a); y = r + r * Math.Sin(-Math.PI / 2 + a); }
        else if ((s -= arc) < side) { x = w; y = r + s; }
        else if ((s -= side) < arc) { var a = s / k; x = w - r + r * Math.Cos(a); y = h - r + r * Math.Sin(a); }
        else if ((s -= arc) < top) { x = w - r - s; y = h; }
        else if ((s -= top) < arc) { var a = s / k; x = r + r * Math.Cos(Math.PI / 2 + a); y = h - r + r * Math.Sin(Math.PI / 2 + a); }
        else if ((s -= arc) < side) { x = 0; y = h - r - s; }
        else { s -= side; var a = s / k; x = r + r * Math.Cos(Math.PI + a); y = r + r * Math.Sin(Math.PI + a); }

        return new Vector2((float)(x + Inset), (float)(y + Inset));
    }

    private static double EdgeFraction(BorderBeamOrigin origin, BorderBeamEdge edge)
    {
        return origin switch
        {
            BorderBeamOrigin.Top => edge switch
            {
                BorderBeamEdge.LeftEdge => 0,
                BorderBeamEdge.RightEdge => 1,
                _ => 0.5,
            },
            BorderBeamOrigin.Right => edge switch
            {
                BorderBeamEdge.TopEdge => 0,
                BorderBeamEdge.BottomEdge => 1,
                _ => 0.5,
            },
            BorderBeamOrigin.Bottom => edge switch
            {
                BorderBeamEdge.RightEdge => 0,
                BorderBeamEdge.LeftEdge => 1,
                _ => 0.5,
            },
            _ => edge switch
            {
                BorderBeamEdge.BottomEdge => 0,
                BorderBeamEdge.TopEdge => 1,
                _ => 0.5,
            },
        };
    }

    private Lead ResolveLead()
    {
        var forwardSide = MirrorOrigin switch
        {
            BorderBeamOrigin.Top => BorderBeamStart.Right,
            BorderBeamOrigin.Right => BorderBeamStart.Bottom,
            BorderBeamOrigin.Bottom => BorderBeamStart.Left,
            _ => BorderBeamStart.Top,
        };

        var backwardSide = MirrorOrigin switch
        {
            BorderBeamOrigin.Top => BorderBeamStart.Left,
            BorderBeamOrigin.Right => BorderBeamStart.Top,
            BorderBeamOrigin.Bottom => BorderBeamStart.Right,
            _ => BorderBeamStart.Bottom,
        };

        if (MirrorStart == forwardSide) return Lead.Forward;
        if (MirrorStart == backwardSide) return Lead.Backward;
        return Lead.None;
    }

    private void RebuildPath()
    {
        var perimeter = Perimeter();

        if (ActualWidth <= 0 || ActualHeight <= 0 || perimeter <= 0)
        {
            _path = null;
        }
        else
        {
            var n = Math.Max(32, (int)Math.Round(perimeter / SegmentLength));
            var points = new Vector2[n + 1];
            for (var i = 0; i < n; i++) points[i] = RimPoint(i * perimeter / n);
            points[n] = points[0];

            if (_referencePerimeter <= 0) _referencePerimeter = perimeter;

            var top = Math.Max(0, RimW - 2 * Rr);
            var side = Math.Max(0, RimH - 2 * Rr);
            var arc = Math.PI * Rr / 2;

            double Offset(BorderBeamOrigin origin, BorderBeamEdge edge)
            {
                var edgeStart = origin switch
                {
                    BorderBeamOrigin.Right => top + arc,
                    BorderBeamOrigin.Bottom => top + 2 * arc + side,
                    BorderBeamOrigin.Left => 2 * top + 3 * arc + side,
                    _ => 0d,
                };

                var edgeLength = origin is BorderBeamOrigin.Left or BorderBeamOrigin.Right ? side : top;
                return edgeStart + edgeLength * EdgeFraction(origin, edge);
            }

            _path = new RimPath(points, perimeter, Offset(MirrorOrigin, MirrorEdge), Offset(ChaseOrigin, ChaseEdge));
        }

        PublishStyle(false);
        UpdateClip();
    }

    private void PublishStyle(bool restart)
    {
        var version = _style?.Version ?? 0;
        if (restart || _style is null) version++;

        _style = new BeamStyle(Mode, ResolveLead(), Math.Max(0, MirrorLag), Math.Max(1, Size), Math.Max(1, BeamCount), Duration, Math.Max(0, Speed), Math.Max(0, Delay), (float)BorderWidth, ColorFrom, ColorTo, ChaseDirection, version);
    }

    private static Color BeamColor(Color from, Color to, double d, double length)
    {
        var t = Math.Clamp(d / length, 0, 1);
        var mix = Math.Min(1, t * 2);
        var alpha = t < 0.5 ? 1 : 2 * (1 - t);

        return Color.FromArgb(
            (byte)Math.Round(alpha * (from.A + (to.A - from.A) * mix)),
            (byte)Math.Round(from.R + (to.R - from.R) * mix),
            (byte)Math.Round(from.G + (to.G - from.G) * mix),
            (byte)Math.Round(from.B + (to.B - from.B) * mix));
    }

    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var path = _path;
        var style = _style;
        if (path is null || style is null || (style.Speed <= 0 && style.Duration <= 0)) return;

        var points = path.Points;
        var n = points.Length - 1;
        var perimeter = path.Perimeter;
        var step = perimeter / n;
        var half = step / 2;

        if (style.Version != _seenVersion)
        {
            _seenVersion = style.Version;
            _origin = args.Timing.TotalTime;
            _distance = 0;
        }

        var speed = style.Speed > 0 ? style.Speed : _referencePerimeter / style.Duration;
        var elapsed = (args.Timing.TotalTime - _origin).TotalSeconds - style.Delay;
        if (elapsed <= 0) return;

        _distance += speed * args.Timing.ElapsedTime.TotalSeconds;

        var ds = args.DrawingSession;

        if (style.Mode == BorderBeamMode.Mirror)
            DrawMirror(ds, style, path, step, half, speed);
        else
            DrawChase(ds, style, path, step, half);
    }

    private static void Trim(ref Vector2 a, ref Vector2 b, float fraction, bool reverse)
    {
        if (reverse) a = Vector2.Lerp(b, a, fraction);
        else b = Vector2.Lerp(a, b, fraction);
    }

    private void DrawChase(CanvasDrawingSession ds, BeamStyle style, RimPath path, double step, double half)
    {
        var points = path.Points;
        var n = points.Length - 1;
        var perimeter = path.Perimeter;
        var spacing = perimeter / style.Count;
        var head = _distance % perimeter;
        var reverse = style.Direction == BorderBeamDirection.CounterClockwise;

        for (var i = 0; i < n; i++)
        {
            var r = reverse ? path.ChaseStart - (i + 0.5) * step : (i + 0.5) * step - path.ChaseStart;
            r %= perimeter;
            if (r < 0) r += perimeter;

            var d = head - r;
            if (d < 0) d += perimeter;
            d %= spacing;

            var a = points[i];
            var b = points[i + 1];

            if (d < half)
            {
                Trim(ref a, ref b, (float)((half + d) / step), reverse);
            }
            else if (d > spacing - half)
            {
                Trim(ref a, ref b, (float)((half - (spacing - d)) / step), reverse);
                d = 0;
            }
            else if (d > style.Length)
            {
                continue;
            }

            ds.DrawLine(a, b, BeamColor(style.From, style.To, d, style.Length), style.Thickness);
        }
    }

    private void DrawMirror(CanvasDrawingSession ds, BeamStyle style, RimPath path, double step, double half, double speed)
    {
        var points = path.Points;
        var n = points.Length - 1;
        var perimeter = path.Perimeter;

        var lagDistance = style.Lead == Lead.None ? 0 : style.Lag * speed;
        var lagForward = style.Lead == Lead.Backward ? lagDistance : 0;
        var lagBackward = style.Lead == Lead.Forward ? lagDistance : 0;

        var meet = Math.Clamp((perimeter + lagBackward - lagForward) / 2, 0, perimeter);
        var cycle = Math.Max(meet + style.Length + lagForward, perimeter - meet + style.Length + lagBackward);

        var x = _distance % cycle;
        var xForward = x - lagForward;
        var xBackward = x - lagBackward;

        for (var i = 0; i < n; i++)
        {
            var f = ((i + 0.5) * step - path.Origin) % perimeter;
            if (f < 0) f += perimeter;

            var forward = f <= meet;
            var pos = forward ? f : perimeter - f;

            var d = (forward ? xForward : xBackward) - pos;
            if (d < -half || d > style.Length) continue;

            var a = points[i];
            var b = points[i + 1];

            if (d < half)
            {
                var fraction = (float)((half + d) / step);
                if (forward) b = Vector2.Lerp(a, b, fraction);
                else a = Vector2.Lerp(b, a, fraction);
                d = Math.Max(0, d);
            }

            ds.DrawLine(a, b, BeamColor(style.From, style.To, d, style.Length), style.Thickness);
        }
    }

    private void UpdateClip()
    {
        if (_canvas is null || ActualWidth <= 0 || ActualHeight <= 0) return;

        var visual = ElementCompositionPreview.GetElementVisual(_canvas);
        var geometry = visual.Compositor.CreateRoundedRectangleGeometry();
        geometry.Size = new Vector2((float)ActualWidth, (float)ActualHeight);
        geometry.CornerRadius = new Vector2((float)CornerRadius.TopLeft);
        visual.Clip = visual.Compositor.CreateGeometricClip(geometry);
    }
}
