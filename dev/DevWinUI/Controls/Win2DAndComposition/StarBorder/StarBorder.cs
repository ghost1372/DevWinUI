using Microsoft.UI.Xaml.Input;
using WinRT;

namespace DevWinUI;

public sealed partial class StarBorder : ContentControl
{
    private const float GlowPad = 12f;
    private const int TrailSegments = 48;
    private const int MaxStars = 6;
    private const int MaxParticles = 160;

    private CanvasControl? _canvas;
    private FrameworkElement? _root;
    private Border? _surface;
    private ContentPresenter? _content;

    private readonly Random _rng = new();
    private readonly CanvasStrokeStyle _flat = new() { StartCap = CanvasCapStyle.Flat, EndCap = CanvasCapStyle.Flat };
    private readonly Vector2[] _trailPts = new Vector2[TrailSegments + 1];
    private readonly Vector2[] _starPts = new Vector2[8];
    private readonly List<Pulse> _pulses = new();
    private readonly List<Particle> _particles = new();

    private StarBorderPerimeter? _path;
    private bool _loaded, _offscreen, _rendering;
    private bool _pointerOver, _keyboardFocus, _hovered;
    private TimeSpan _lastRenderingTime;
    private double _time, _phase, _speedFactor = 1, _lapRemaining, _sparkleAcc;
    private double _brighten, _brightenTarget, _reveal = 1, _revealTarget = 1;

    public StarBorder()
    {
        DefaultStyleKey = typeof(StarBorder);

        Loaded += (_, _) =>
        {
            _loaded = true;
            if (XamlRoot != null) XamlRoot.Changed += OnXamlRootChanged;
            ApplyVisuals(); RebuildPath(); ApplyHoverTargets();
            RequestFrame();
        };
        Unloaded += (_, _) =>
        {
            _loaded = false;
            if (XamlRoot != null) XamlRoot.Changed -= OnXamlRootChanged;
            StopLoop();
        };
        SizeChanged += (_, _) => { ApplyVisuals(); RebuildPath(); RequestFrame(); };
        IsEnabledChanged += (_, _) => ApplyVisuals();
        EffectiveViewportChanged += (_, e) =>
        {
            bool off = e.BringIntoViewDistanceX > 0 || e.BringIntoViewDistanceY > 0;
            if (off == _offscreen) return;
            _offscreen = off;
            if (!off) RequestFrame();
        };

        PointerEntered += (_, _) => { _pointerOver = true; UpdateHover(); };
        PointerExited += (_, _) => { _pointerOver = false; UpdateHover(); };
        GotFocus += (_, e) =>
        {
            _keyboardFocus = (e.OriginalSource as Control)?.FocusState == FocusState.Keyboard;
            UpdateHover();
        };
        LostFocus += (_, _) => { _keyboardFocus = false; UpdateHover(); };

        AddHandler(PointerPressedEvent, new PointerEventHandler(OnPointerPressedAny), true);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (_canvas != null) _canvas.Draw -= OnDraw;

        _root = GetTemplateChild("PART_Root") as FrameworkElement;
        _surface = GetTemplateChild("PART_Surface") as Border;
        _content = GetTemplateChild("PART_Content") as ContentPresenter;
        _canvas = GetTemplateChild("PART_Canvas") as CanvasControl;
        if (_canvas != null) _canvas.Draw += OnDraw;

        ApplyVisuals();
        RebuildPath();
    }

    private void OnPropsChanged()
    {
        if (_surface == null) return;
        ApplyVisuals(); RebuildPath(); ApplyHoverTargets(); RequestFrame();
    }

    private void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs args) => RequestFrame();

    private int ClampedStars() => Math.Clamp(Stars, 1, MaxStars);

    private readonly record struct Palette(Color Surface, Color Text, Color Border, Color Star, Color Trail);

    private Palette ResolvePalette()
    {
        bool dark = Theme == StarBorderTheme.Dark;
        var ink = Color.FromArgb(255, 17, 17, 17);

        var surface = BackgroundColor.A != 0 ? BackgroundColor
            : dark ? Color.FromArgb(255, 11, 11, 15) : Color.FromArgb(255, 255, 255, 255);
        var text = TextColor.A != 0 ? TextColor : dark ? Color.FromArgb(255, 255, 255, 255) : ink;
        var border = BorderColor.A != 0 ? BorderColor
            : dark ? Color.FromArgb(38, 255, 255, 255) : Color.FromArgb(38, 0, 0, 0);
        var star = StarColor.A != 0 ? StarColor : dark ? Color.FromArgb(255, 255, 255, 255) : ink;
        var trail = TrailColor.A != 0 ? TrailColor : star;
        return new Palette(surface, text, border, star, trail);
    }

    private void ApplyVisuals()
    {
        if (_surface == null) return;
        var pal = ResolvePalette();
        _surface.Background = new SolidColorBrush(pal.Surface);
        _surface.BorderBrush = new SolidColorBrush(pal.Border);
        _surface.BorderThickness = new Thickness(LineThickness);
        double max = (ActualWidth > 0 && ActualHeight > 0) ? Math.Min(ActualWidth, ActualHeight) / 2 : 1e6;
        _surface.CornerRadius = new CornerRadius(Math.Min(Radius, max));
        if (_content != null) _content.Foreground = new SolidColorBrush(pal.Text);
        if (_root != null) _root.Opacity = IsEnabled ? 1 : 0.5;
    }

    private void RebuildPath()
    {
        double w = ActualWidth, h = ActualHeight, t = LineThickness;
        if (w <= t + 1 || h <= t + 1) { _path = null; return; }
        double r = Math.Min(Radius, Math.Min(w, h) / 2) - t / 2;
        _path = new StarBorderPerimeter((float)(w - t), (float)(h - t), (float)Math.Max(0, r));
    }

    private void UpdateHover()
    {
        bool h = _pointerOver || _keyboardFocus;
        if (h == _hovered) return;
        _hovered = h;
        if (h && HoverMode == StarHoverMode.Lap) _lapRemaining = 1;
        ApplyHoverTargets();
        RequestFrame();
    }

    private void ApplyHoverTargets()
    {
        _brightenTarget = (_hovered && HoverMode == StarHoverMode.Brighten) ? 1 : 0;
        _revealTarget = HoverMode == StarHoverMode.Reveal ? (_hovered ? 1 : 0) : 1;
    }

    private void OnPointerPressedAny(object sender, PointerRoutedEventArgs e)
    {
        if (!ClickPulse || _path == null) return;
        var p = e.GetCurrentPoint(this).Position;
        float inset = (float)LineThickness / 2f;
        SpawnPulse(_path.Nearest(new Vector2((float)p.X - inset, (float)p.Y - inset)));
    }

    private void SpawnPulse(float start)
    {
        if (!ClickPulse || _path == null) return;
        _pulses.Add(new Pulse { Start = start });
        RequestFrame();
    }

    private bool CanRender =>
        _loaded && !_offscreen && _path != null && Visibility == Visibility.Visible &&
        XamlRoot?.IsHostVisible != false;

    private bool NeedsFrames() =>
        _reveal > 0.005 || _revealTarget > 0.5 ||
        _pulses.Count > 0 || _particles.Count > 0 ||
        Math.Abs(_brighten - _brightenTarget) > 0.005 ||
        Math.Abs(_reveal - _revealTarget) > 0.005;

    private void RequestFrame()
    {
        if (!CanRender) return;
        _canvas?.Invalidate();
        if (!_rendering && NeedsFrames())
        {
            _lastRenderingTime = TimeSpan.Zero;
            CompositionTarget.Rendering += OnRendering;
            _rendering = true;
        }
    }

    private void StopLoop()
    {
        if (!_rendering) return;
        CompositionTarget.Rendering -= OnRendering;
        _rendering = false;
    }

    private void OnRendering(object? sender, object e)
    {
        TimeSpan now = e.As<RenderingEventArgs>().RenderingTime;
        if (now == _lastRenderingTime) return;

        double dt = _lastRenderingTime == TimeSpan.Zero
            ? 1.0 / 60
            : Math.Min((now - _lastRenderingTime).TotalSeconds, 0.05);
        _lastRenderingTime = now;
        if (!CanRender) { StopLoop(); return; }

        _time = now.TotalSeconds;
        Update(dt);
        _canvas?.Invalidate();
        if (!NeedsFrames()) StopLoop();
    }

    private float HeadDistance(int i, int n)
    {
        double ph = (_phase + (double)i / n) % 1.0;
        double f = Direction == StarDirection.Clockwise ? ph : 1 - ph;
        return (float)(f * (_path?.Total ?? 0));
    }

    private int Sgn => Direction == StarDirection.Clockwise ? 1 : -1;

    private void Update(double dt)
    {
        if (_path == null) return;

        double dur = Math.Max(0.2, Duration);
        double targetSpeed = (HoverMode == StarHoverMode.Lap && _lapRemaining > 0) ? 4.5 : 1;
        _speedFactor += (targetSpeed - _speedFactor) * (1 - Math.Exp(-dt * 10));

        double dPhase = dt / dur * _speedFactor;
        _phase = (_phase + dPhase) % 1.0;
        if (_lapRemaining > 0) _lapRemaining = Math.Max(0, _lapRemaining - dPhase);

        _brighten += (_brightenTarget - _brighten) * (1 - Math.Exp(-dt * 8));
        _reveal += (_revealTarget - _reveal) * (1 - Math.Exp(-dt * 8));
        if (Math.Abs(_brighten - _brightenTarget) < 0.005) _brighten = _brightenTarget;
        if (Math.Abs(_reveal - _revealTarget) < 0.005) _reveal = _revealTarget;

        float pulseSpeed = _path.Total / 1.6f;
        for (int i = _pulses.Count - 1; i >= 0; i--)
        {
            _pulses[i].Traveled += pulseSpeed * (float)dt;
            if (_pulses[i].Traveled >= _path.Total / 2) _pulses.RemoveAt(i);
        }

        int n = ClampedStars();
        if (Sparkle && _reveal > 0.2)
        {
            _sparkleAcc += dt * 60;
            while (_sparkleAcc >= 1)
            {
                _sparkleAcc -= 1;
                if (_particles.Count >= MaxParticles) break;
                var head = _path.PointAt(HeadDistance(_rng.Next(n), n));
                double a = _rng.NextDouble() * Math.PI * 2;
                float sp = 8f + (float)_rng.NextDouble() * 22f;
                float life = 0.5f + (float)_rng.NextDouble() * 0.5f;
                _particles.Add(new Particle
                {
                    Pos = head + new Vector2((float)(_rng.NextDouble() - 0.5) * 4, (float)(_rng.NextDouble() - 0.5) * 4),
                    Vel = new Vector2((float)Math.Cos(a), (float)Math.Sin(a)) * sp,
                    Life = life,
                    MaxLife = life,
                    Size = 0.8f + (float)_rng.NextDouble() * 1.0f
                });
            }
        }
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var p = _particles[i];
            p.Life -= (float)dt;
            if (p.Life <= 0) { _particles.RemoveAt(i); continue; }
            p.Pos += p.Vel * (float)dt;
        }
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (_path == null || _path.Total <= 1) return;
        var ds = args.DrawingSession;
        float inset = (float)LineThickness / 2f;
        ds.Transform = Matrix3x2.CreateTranslation(GlowPad + inset, GlowPad + inset);

        var pal = ResolvePalette();
        float glow = (float)Math.Clamp(Glow, 0, 1);
        float vis = (float)_reveal;
        float bright = (float)_brighten;
        float glowEff = Math.Min(1f, glow * (1f + 0.8f * bright));
        float scale = 1f + 0.35f * bright;
        float width = (float)LineThickness + 1f;
        int n = ClampedStars();
        int sgn = Sgn;
        float stretch = 1f + 0.5f * (float)Math.Clamp((_speedFactor - 1) / 3.5, 0, 1);
        float trailLen = (float)Math.Clamp(TrailLength, 0, 1) * stretch * _path.Total;

        foreach (var p in _particles)
        {
            float k = p.Life / p.MaxLife;
            ds.FillCircle(p.Pos, p.Size * (0.4f + 0.6f * k), WithAlpha(pal.Star, 0.9f * k * vis));
        }

        if (vis > 0.002f)
        {
            for (int i = 0; i < n; i++)
            {
                float head = HeadDistance(i, n);
                float tw = 0.5f + 0.5f * MathF.Sin((float)_time * 7f + i * 1.7f);
                DrawTrail(ds, head, sgn, trailLen, pal.Star, pal.Trail, vis, width, glowEff);
                DrawHead(ds, _path.PointAt(head), pal.Star, vis, glowEff, scale, tw,
                         (float)_time * 1.2f + i, Sparkle);
            }
        }

        foreach (var pulse in _pulses)
        {
            float life = 1f - pulse.Traveled / (_path.Total / 2);
            float len = Math.Min(pulse.Traveled, 0.14f * _path.Total);
            float h1 = pulse.Start + pulse.Traveled, h2 = pulse.Start - pulse.Traveled;
            DrawTrail(ds, h1, 1, len, pal.Star, pal.Trail, life, width + 0.5f, Math.Max(glowEff, 0.5f));
            DrawTrail(ds, h2, -1, len, pal.Star, pal.Trail, life, width + 0.5f, Math.Max(glowEff, 0.5f));
            ds.FillCircle(_path.PointAt(h1), 2.4f, WithAlpha(pal.Star, life));
            ds.FillCircle(_path.PointAt(h2), 2.4f, WithAlpha(pal.Star, life));
        }
    }

    private void DrawTrail(CanvasDrawingSession ds, float head, int sgn, float len,
                           Color c0, Color c1, float alpha, float width, float glow)
    {
        if (_path == null || alpha <= 0.002f || len <= 0.5f) return;

        for (int k = 0; k <= TrailSegments; k++)
            _trailPts[k] = _path.PointAt(head - sgn * (k / (float)TrailSegments) * len);

        if (glow > 0.01f)
        {
            for (int k = 0; k < TrailSegments; k++)
            {
                float fade = 1f - (k + 0.5f) / TrailSegments; fade *= fade;
                var col = Lerp(c0, c1, k / (float)TrailSegments);
                ds.DrawLine(_trailPts[k], _trailPts[k + 1], WithAlpha(col, alpha * fade * glow * 0.30f), width * 5f, _flat);
            }
        }
        for (int k = 0; k < TrailSegments; k++)
        {
            float fade = 1f - (k + 0.5f) / TrailSegments; fade *= fade;
            var col = Lerp(c0, c1, k / (float)TrailSegments);
            ds.DrawLine(_trailPts[k], _trailPts[k + 1], WithAlpha(col, alpha * fade), width, _flat);
        }
    }

    private void DrawHead(CanvasDrawingSession ds, Vector2 pos, Color c, float alpha, float glow,
                          float scale, float twinkle, float rot, bool glint)
    {
        if (glow > 0.01f)
        {
            ds.FillCircle(pos, 16f * scale, WithAlpha(c, alpha * glow * 0.10f));
            ds.FillCircle(pos, 10f * scale, WithAlpha(c, alpha * glow * 0.18f));
            ds.FillCircle(pos, 5.5f * scale, WithAlpha(c, alpha * glow * 0.35f));
        }

        float outer = (4.5f + 2f * twinkle) * scale;
        float inner = outer * 0.28f;
        for (int j = 0; j < 8; j++)
        {
            float ang = rot + j * MathF.PI / 4f - MathF.PI / 2f;
            float r = (j % 2 == 0) ? outer : inner;
            _starPts[j] = pos + new Vector2(MathF.Cos(ang), MathF.Sin(ang)) * r;
        }
        using var star = CanvasGeometry.CreatePolygon(ds, _starPts);
        ds.FillGeometry(star, WithAlpha(c, alpha));

        if (glint)
        {
            float L = (9f + 7f * twinkle) * scale;
            float a = rot + MathF.PI / 4f;
            var d1 = new Vector2(MathF.Cos(a), MathF.Sin(a)) * L;
            var d2 = new Vector2(-d1.Y, d1.X);
            var col = WithAlpha(c, alpha * (0.35f + 0.55f * twinkle));
            ds.DrawLine(pos - d1, pos + d1, col, 1f);
            ds.DrawLine(pos - d2, pos + d2, col, 1f);
        }
    }

    private static Color WithAlpha(Color c, float a) =>
        Color.FromArgb((byte)Math.Clamp(a * 255f, 0f, 255f), c.R, c.G, c.B);

    private static Color Lerp(Color a, Color b, float t) => Color.FromArgb(255,
        (byte)(a.R + (b.R - a.R) * t), (byte)(a.G + (b.G - a.G) * t), (byte)(a.B + (b.B - a.B) * t));

    private sealed class Pulse { public float Start; public float Traveled; }
    private sealed class Particle { public Vector2 Pos, Vel; public float Life, MaxLife, Size; }
}
