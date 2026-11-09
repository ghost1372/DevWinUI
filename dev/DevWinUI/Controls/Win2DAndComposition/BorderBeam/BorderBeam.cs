namespace DevWinUI;

[TemplatePart(Name = PartHost, Type = typeof(Grid))]
public sealed partial class BorderBeam : Control
{
    private const string PartHost = "PART_Host";
    private const double SegmentLength = 3;

    private sealed record RimPath(Vector2[] Points, double Perimeter);

    private sealed record BeamStyle(double Length, double Duration, double Delay, float Thickness, Color From, Color To, int Version);

    private Grid? _host;
    private CanvasAnimatedControl? _canvas;
    private volatile RimPath? _path;
    private volatile BeamStyle? _style;
    private TimeSpan _origin;
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
            if (_canvas is not null) _canvas.Paused = !_visible;
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
        double r = Rr;
        return 2 * (Math.Max(0, RimW - 2 * r) + Math.Max(0, RimH - 2 * r)) + 2 * Math.PI * r;
    }

    private Vector2 RimPoint(double s)
    {
        double w = RimW, h = RimH, r = Rr;
        double top = Math.Max(0, w - 2 * r), side = Math.Max(0, h - 2 * r), arc = Math.PI * r / 2;
        double k = r == 0 ? 1 : r;
        double x, y;

        if (s < top) { x = r + s; y = 0; }
        else if ((s -= top) < arc) { double a = s / k; x = w - r + r * Math.Cos(-Math.PI / 2 + a); y = r + r * Math.Sin(-Math.PI / 2 + a); }
        else if ((s -= arc) < side) { x = w; y = r + s; }
        else if ((s -= side) < arc) { double a = s / k; x = w - r + r * Math.Cos(a); y = h - r + r * Math.Sin(a); }
        else if ((s -= arc) < top) { x = w - r - s; y = h; }
        else if ((s -= top) < arc) { double a = s / k; x = r + r * Math.Cos(Math.PI / 2 + a); y = h - r + r * Math.Sin(Math.PI / 2 + a); }
        else if ((s -= arc) < side) { x = 0; y = h - r - s; }
        else { s -= side; double a = s / k; x = r + r * Math.Cos(Math.PI + a); y = r + r * Math.Sin(Math.PI + a); }

        return new Vector2((float)(x + Inset), (float)(y + Inset));
    }

    private void RebuildPath()
    {
        double perimeter = Perimeter();

        if (ActualWidth <= 0 || ActualHeight <= 0 || perimeter <= 0)
        {
            _path = null;
        }
        else
        {
            int n = Math.Max(32, (int)Math.Round(perimeter / SegmentLength));
            var points = new Vector2[n + 1];
            for (int i = 0; i < n; i++) points[i] = RimPoint(i * perimeter / n);
            points[n] = points[0];

            _path = new RimPath(points, perimeter);
        }

        PublishStyle(false);
    }

    private void PublishStyle(bool restart)
    {
        int version = _style?.Version ?? 0;
        if (restart || _style is null) version++;

        _style = new BeamStyle(Math.Max(1, Size), Duration, Math.Max(0, Delay), (float)BorderWidth, ColorFrom, ColorTo, version);
    }

    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var path = _path;
        var style = _style;
        if (path is null || style is null || style.Duration <= 0) return;

        var points = path.Points;
        int n = points.Length - 1;
        double perimeter = path.Perimeter;
        double step = perimeter / n;

        if (style.Version != _seenVersion)
        {
            _seenVersion = style.Version;
            _origin = args.Timing.TotalTime;
        }

        double elapsed = (args.Timing.TotalTime - _origin).TotalSeconds - style.Delay;
        double progress = elapsed > 0 ? elapsed / style.Duration % 1 : 0;
        double head = progress * perimeter;
        int first = (int)Math.Floor(head / step - 0.5);

        var ds = args.DrawingSession;
        var from = style.From;
        var to = style.To;

        for (int k = 0; k < n; k++)
        {
            int i = ((first - k) % n + n) % n;
            double d = head - (i + 0.5) * step;
            if (d < 0) d += perimeter;
            if (d > style.Length) break;

            double t = d / style.Length;
            double mix = Math.Min(1, t * 2);
            double alpha = t < 0.5 ? 1 : 2 * (1 - t);

            var color = Color.FromArgb(
                (byte)Math.Round(alpha * (from.A + (to.A - from.A) * mix)),
                (byte)Math.Round(from.R + (to.R - from.R) * mix),
                (byte)Math.Round(from.G + (to.G - from.G) * mix),
                (byte)Math.Round(from.B + (to.B - from.B) * mix));

            ds.DrawLine(points[i], points[i + 1], color, style.Thickness);
        }
    }
}
