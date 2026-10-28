using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

/// <summary>
/// Task status glyph: dashed ring, spinning or progress arc, then a closed ring with a check or cross.
/// </summary>
public sealed partial class StatusMark : ContentControl
{
    private const float Tau = 2f * MathF.PI;

    private CanvasControl? _canvas;
    private FrameworkElement? _labelHost;
    private TextBlock? _labelText;
    private ContentPresenter? _contentHost;
    private Rectangle? _strikeRect;
    private ScaleTransform? _strikeScale;

    private readonly CanvasStrokeStyle _round = new()
    {
        StartCap = CanvasCapStyle.Round,
        EndCap = CanvasCapStyle.Round,
        LineJoin = CanvasLineJoin.Round
    };

    private static readonly Vector2[] CheckPts = { new(7.2f, 12.4f), new(10.6f, 15.8f), new(16.8f, 8.8f) };
    private static readonly Vector2[] CrossA = { new(8.5f, 8.5f), new(15.5f, 15.5f) };
    private static readonly Vector2[] CrossB = { new(15.5f, 8.5f), new(8.5f, 15.5f) };

    private bool _loaded, _offscreen, _rendering, _snapped;
    private TimeSpan _lastRenderingTime;
    private double _time;
    private string? _lastAutoName;

    private double _join, _arc, _cDone, _cErr, _fill, _check, _cross, _strike, _strikeWait;
    private double _sweep = 0.68 * Tau, _spin;

    public StatusMark()
    {
        DefaultStyleKey = typeof(StatusMark);

        Loaded += (_, _) =>
        {
            _loaded = true;
            if (XamlRoot != null) XamlRoot.Changed += OnXamlRootChanged;
            ApplyVisuals();
            SnapToTargets();
            RequestFrame();
        };
        Unloaded += (_, _) =>
        {
            _loaded = false;
            if (XamlRoot != null) XamlRoot.Changed -= OnXamlRootChanged;
            StopLoop();
        };
        EffectiveViewportChanged += (_, e) =>
        {
            bool off = e.BringIntoViewDistanceX > 0 || e.BringIntoViewDistanceY > 0;
            if (off == _offscreen) return;
            _offscreen = off;
            if (!off) RequestFrame();
        };
        ActualThemeChanged += (_, _) => { ApplyVisuals(); RequestFrame(); };
        RegisterPropertyChangedCallback(ForegroundProperty, (_, _) => { ApplyVisuals(); RequestFrame(); });
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (_canvas != null) _canvas.Draw -= OnDraw;

        _canvas = GetTemplateChild("PART_Canvas") as CanvasControl;
        _labelHost = GetTemplateChild("PART_LabelHost") as FrameworkElement;
        _labelText = GetTemplateChild("PART_Label") as TextBlock;
        _contentHost = GetTemplateChild("PART_Content") as ContentPresenter;
        _strikeRect = GetTemplateChild("PART_Strike") as Rectangle;
        _strikeScale = _strikeRect?.RenderTransform as ScaleTransform;
        if (_canvas != null) _canvas.Draw += OnDraw;

        ApplyVisuals();
    }

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        ApplyVisuals();
    }

    private void OnPropsChanged()
    {
        ApplyVisuals();
        if (!_snapped) SnapToTargets();
        RequestFrame();
    }

    private bool IsIndeterminate => Status == StatusMarkStatus.Running && double.IsNaN(Progress);

    private double NormalizedProgress =>
        double.IsNaN(Progress) ? double.NaN : Math.Clamp(Progress / (Maximum > 0 ? Maximum : 1), 0, 1);

    private readonly record struct Targets(double Join, double Arc, double Done, double Err, double Fill,
                                           double Check, double Cross);

    private Targets GetTargets() => Status switch
    {
        StatusMarkStatus.Running => new(1, 1, 0, 0, 0, 0, 0),
        StatusMarkStatus.Done => new(1, 0, 1, 0, 1, 1, 0),
        StatusMarkStatus.Failed => new(1, 0, 0, 1, 1, 0, 1),
        StatusMarkStatus.Cancelled => new(0, 0, 0, 0, 0, 0, 1),
        _ => new(0, 0, 0, 0, 0, 0, 0),
    };

    private double SweepTarget() => IsIndeterminate
        ? Math.Clamp(ArcLength, 0.05, 1) * Tau
        : Math.Clamp(double.IsNaN(Progress) ? ArcLength : NormalizedProgress, 0.02, 1) * Tau;

    private void SnapToTargets()
    {
        var t = GetTargets();
        _join = t.Join; _arc = t.Arc; _cDone = t.Done; _cErr = t.Err; _fill = t.Fill;
        _check = t.Check; _cross = t.Cross;
        _sweep = SweepTarget();
        _strike = (Status == StatusMarkStatus.Done && Strike) ? 1 : 0;
        _strikeWait = 0;
        ApplyStrike();
        _snapped = true;
    }

    private Color Ink()
    {
        if (Foreground is SolidColorBrush b) return b.Color;
        return ActualTheme == ElementTheme.Light
            ? Color.FromArgb(255, 17, 17, 17) : Color.FromArgb(255, 255, 255, 255);
    }

    private string StateText() => Status switch
    {
        StatusMarkStatus.Running => double.IsNaN(Progress)
            ? "In progress"
            : $"In progress, {Math.Round(NormalizedProgress * 100)}%",
        StatusMarkStatus.Done => "Done",
        StatusMarkStatus.Failed => "Failed",
        StatusMarkStatus.Cancelled => "Cancelled",
        _ => "Pending",
    };

    private void ApplyVisuals()
    {
        if (_canvas != null)
        {
            _canvas.Width = _canvas.Height = Math.Max(1, GlyphSize);
        }

        bool hasContent = Content != null;
        bool hasLabel = !hasContent && !string.IsNullOrEmpty(Label);
        if (_labelText != null) _labelText.Visibility = hasLabel ? Visibility.Visible : Visibility.Collapsed;
        if (_contentHost != null) _contentHost.Visibility = hasContent ? Visibility.Visible : Visibility.Collapsed;
        if (_labelHost != null)
            _labelHost.Visibility = (hasContent || hasLabel) ? Visibility.Visible : Visibility.Collapsed;
        if (_strikeRect != null) _strikeRect.Fill = Foreground;

        string? text = Content as string ?? (string.IsNullOrEmpty(Label) ? null : Label);
        string name = text == null ? StateText() : $"{StateText()}: {text}";
        var current = AutomationProperties.GetName(this);
        if (string.IsNullOrEmpty(current) || current == _lastAutoName)
        {
            AutomationProperties.SetName(this, name);
            _lastAutoName = name;
        }
        if (_labelHost != null)
            AutomationProperties.SetAccessibilityView(_labelHost,
                text != null ? AccessibilityView.Raw : AccessibilityView.Content);

        ApplyStrike();
    }

    private void ApplyStrike()
    {
        if (_strikeRect == null || _strikeScale == null) return;
        if (ReduceMotion)
        {
            _strikeScale.ScaleX = 1;
            _strikeRect.Opacity = _strike;
        }
        else
        {
            _strikeScale.ScaleX = _strike;
            _strikeRect.Opacity = _strike > 0.001 ? 1 : 0;
        }
    }

    private bool CanRender =>
        _loaded && !_offscreen && Visibility == Visibility.Visible && XamlRoot?.IsHostVisible != false;

    private void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs args) => RequestFrame();

    private bool NeedsFrames()
    {
        if (IsIndeterminate) return true;
        var t = GetTargets();
        const double e = 0.003;
        return Math.Abs(_join - t.Join) > e || Math.Abs(_arc - t.Arc) > e ||
               Math.Abs(_cDone - t.Done) > e || Math.Abs(_cErr - t.Err) > e ||
               Math.Abs(_fill - t.Fill) > e || Math.Abs(_check - t.Check) > e ||
               Math.Abs(_cross - t.Cross) > e ||
               Math.Abs(_strike - ((Status == StatusMarkStatus.Done && Strike) ? 1 : 0)) > e ||
               (Status == StatusMarkStatus.Running && Math.Abs(_sweep - SweepTarget()) > 0.003) ||
               (_spin > 0.001 && !IsIndeterminate);
    }

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
        var now = ((RenderingEventArgs)e).RenderingTime;
        if (now == _lastRenderingTime) return;

        double dt = _lastRenderingTime == TimeSpan.Zero
            ? 1.0 / 60
            : Math.Min((now - _lastRenderingTime).TotalSeconds, 0.05);
        _lastRenderingTime = now;
        _time = now.TotalSeconds;

        if (!CanRender) { StopLoop(); return; }

        Update(dt);
        _canvas?.Invalidate();
        if (!NeedsFrames()) StopLoop();
    }

    private static void Ease(ref double v, double target, double rate, double dt)
    {
        v += (target - v) * (1 - Math.Exp(-rate * dt));
        if (Math.Abs(v - target) < 0.003) v = target;
    }

    private void Update(double dt)
    {
        var t = GetTargets();
        Ease(ref _join, t.Join, 9, dt);
        Ease(ref _arc, t.Arc, 8, dt);
        Ease(ref _cDone, t.Done, 10, dt);
        Ease(ref _cErr, t.Err, 10, dt);
        Ease(ref _fill, t.Fill, 8, dt);
        if (Status == StatusMarkStatus.Running) Ease(ref _sweep, SweepTarget(), 8, dt);

        if (IsIndeterminate && !ReduceMotion)
        {
            _spin = (_spin + Tau * dt / Math.Max(0.1, SpinDuration / 1000.0)) % Tau;
        }
        else if (_spin > 0)
        {
            double target = Tau;
            _spin += (target - _spin) * (1 - Math.Exp(-8 * dt));
            if (target - _spin < 0.002) _spin = 0;
        }

        double drawSec = Math.Max(0.01, DrawDuration / 1000.0);
        bool gate = Status == StatusMarkStatus.Cancelled || _join > 0.85;
        _check = Step(_check, t.Check, gate, dt, drawSec);
        _cross = Step(_cross, t.Cross, gate, dt, drawSec);

        bool wantStrike = Status == StatusMarkStatus.Done && Strike;
        if (wantStrike)
        {
            if (_check >= 1) _strikeWait += dt;
            if (_strikeWait * 1000 >= StrikeDelay) _strike = Math.Min(1, _strike + dt / 0.28);
        }
        else
        {
            _strikeWait = 0;
            _strike = Math.Max(0, _strike - dt / 0.15);
        }
        ApplyStrike();
    }

    private static double Step(double v, double target, bool gate, double dt, double drawSec)
    {
        if (target >= 1) return gate ? Math.Min(1, v + dt / drawSec) : v;
        return Math.Max(0, v - dt / (drawSec * 0.5));
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var ds = args.DrawingSession;
        ds.Transform = Matrix3x2.CreateScale((float)(Math.Max(1, GlyphSize) / 24.0));

        float sw = (float)Math.Max(0.5, StrokeWidth);
        float r = 11f - sw / 2f;
        var c = new Vector2(12, 12);

        var ink = Ink();
        var col = Lerp(Lerp(ink, DoneColor, (float)_cDone), ErrorColor, (float)_cErr);

        float alphaMul = 1f;
        if (ReduceMotion && IsIndeterminate)
        {
            float breath = 0.4f + 0.6f * (0.5f + 0.5f * MathF.Sin((float)_time * Tau / 2.4f));
            alphaMul = Mix(1f, breath, (float)_arc);
        }

        if (_fill > 0.002)
            ds.FillCircle(c, r, WithAlpha(col, (float)(FillOpacity * _fill)));

        float arc = (float)_arc;
        float join = (float)_join;
        float rot = -MathF.PI / 2f + (float)_spin;
        float sweep = Mix(Tau, (float)_sweep, arc);
        float trackAlpha = Mix(1f, 0.25f, arc) * alphaMul;
        int n = Math.Max(1, Dashes);

        if (join >= 0.995f)
        {
            ds.DrawCircle(c, r, WithAlpha(col, trackAlpha), sw);
            if (arc > 0.002f && sweep < Tau - 0.01f)
                DrawArc(ds, c, r, sw, rot, rot + sweep, WithAlpha(col, alphaMul));
        }
        else
        {
            const float dashFrac = 0.55f;
            float frac = Mix(dashFrac, 1f, join);
            float shrink = (sw / 2f / r) * (1f - join);
            DrawDashes(ds, c, r, sw, n, rot, frac, shrink, rot - 1f, rot + Tau + 1f, WithAlpha(col, trackAlpha));
            if (arc > 0.002f)
                DrawDashes(ds, c, r, sw, n, rot, frac, shrink, rot, rot + sweep, WithAlpha(col, alphaMul));
        }

        if (_check > 0.002) DrawPolyline(ds, CheckPts, (float)_check, col, sw);
        if (_cross > 0.002)
        {
            if (ReduceMotion)
            {
                DrawPolyline(ds, CrossA, 1f, col, sw, (float)_cross);
                DrawPolyline(ds, CrossB, 1f, col, sw, (float)_cross);
            }
            else
            {
                DrawPolyline(ds, CrossA, Math.Min(1f, (float)_cross * 2f), col, sw);
                DrawPolyline(ds, CrossB, Math.Clamp((float)_cross * 2f - 1f, 0f, 1f), col, sw);
            }
        }
    }

    private static Vector2 Pt(Vector2 c, float r, float a) => c + new Vector2(MathF.Cos(a), MathF.Sin(a)) * r;

    private static void AddArcFigure(CanvasPathBuilder pb, Vector2 c, float r, float a0, float a1)
    {
        pb.BeginFigure(Pt(c, r, a0));
        pb.AddArc(Pt(c, r, a1), r, r, 0f, CanvasSweepDirection.Clockwise,
                  (a1 - a0) > MathF.PI ? CanvasArcSize.Large : CanvasArcSize.Small);
        pb.EndFigure(CanvasFigureLoop.Open);
    }

    private void DrawArc(CanvasDrawingSession ds, Vector2 c, float r, float sw, float a0, float a1, Color color)
    {
        using var pb = new CanvasPathBuilder(ds);
        AddArcFigure(pb, c, r, a0, a1);
        using var g = CanvasGeometry.CreatePath(pb);
        ds.DrawGeometry(g, color, sw, _round);
    }

    private void DrawDashes(CanvasDrawingSession ds, Vector2 c, float r, float sw, int n, float rot,
                            float frac, float shrink, float winLo, float winHi, Color color)
    {
        float period = Tau / n;
        using var pb = new CanvasPathBuilder(ds);
        bool any = false;
        for (int i = 0; i < n; i++)
        {
            float s = rot + i * period + shrink;
            float e = rot + i * period + period * frac - shrink;
            float lo = Math.Max(s, winLo), hi = Math.Min(e, winHi);
            if (hi - lo < 0.004f) continue;
            AddArcFigure(pb, c, r, lo, hi);
            any = true;
        }
        if (!any) return;
        using var g = CanvasGeometry.CreatePath(pb);
        ds.DrawGeometry(g, color, sw, _round);
    }

    private void DrawPolyline(CanvasDrawingSession ds, Vector2[] pts, float frac, Color color, float sw,
                              float alpha = 1f)
    {
        if (frac <= 0.002f) return;
        float total = 0;
        for (int i = 1; i < pts.Length; i++) total += Vector2.Distance(pts[i - 1], pts[i]);
        float remaining = total * Math.Clamp(frac, 0f, 1f);

        using var pb = new CanvasPathBuilder(ds);
        pb.BeginFigure(pts[0]);
        for (int i = 1; i < pts.Length && remaining > 0; i++)
        {
            float len = Vector2.Distance(pts[i - 1], pts[i]);
            if (remaining >= len) { pb.AddLine(pts[i]); remaining -= len; }
            else { pb.AddLine(Vector2.Lerp(pts[i - 1], pts[i], remaining / len)); remaining = 0; }
        }
        pb.EndFigure(CanvasFigureLoop.Open);
        using var g = CanvasGeometry.CreatePath(pb);
        ds.DrawGeometry(g, WithAlpha(color, alpha), sw, _round);
    }

    private static float Mix(float a, float b, float t) => a + (b - a) * t;

    private static Color WithAlpha(Color c, float a) =>
        Color.FromArgb((byte)Math.Clamp(c.A * a, 0f, 255f), c.R, c.G, c.B);

    private static Color Lerp(Color a, Color b, float t) => Color.FromArgb(
        (byte)(a.A + (b.A - a.A) * t), (byte)(a.R + (b.R - a.R) * t),
        (byte)(a.G + (b.G - a.G) * t), (byte)(a.B + (b.B - a.B) * t));
}
