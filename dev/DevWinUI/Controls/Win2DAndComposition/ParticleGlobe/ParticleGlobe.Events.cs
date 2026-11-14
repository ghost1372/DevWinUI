using System.Globalization;

namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    #region Properties

    public bool ShowEvents
    {
        get { return (bool)GetValue(ShowEventsProperty); }
        set { SetValue(ShowEventsProperty, value); }
    }

    public static readonly DependencyProperty ShowEventsProperty =
        DependencyProperty.Register(nameof(ShowEvents), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool IsEventRippleEnabled
    {
        get { return (bool)GetValue(IsEventRippleEnabledProperty); }
        set { SetValue(IsEventRippleEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsEventRippleEnabledProperty =
        DependencyProperty.Register(nameof(IsEventRippleEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowEventLabels
    {
        get { return (bool)GetValue(ShowEventLabelsProperty); }
        set { SetValue(ShowEventLabelsProperty, value); }
    }

    public static readonly DependencyProperty ShowEventLabelsProperty =
        DependencyProperty.Register(nameof(ShowEventLabels), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public double EventDuration
    {
        get { return (double)GetValue(EventDurationProperty); }
        set { SetValue(EventDurationProperty, value); }
    }

    public static readonly DependencyProperty EventDurationProperty =
        DependencyProperty.Register(nameof(EventDuration), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(4.5, OnSettingsPropertyChanged));

    #endregion

    #region API

    public void AddEvent(double latitude, double longitude, double magnitude = 5.0, Color? color = null, string label = null)
    {
        var marker = new ParticleGlobeMarker(latitude, longitude);
        var ev = new ParticleGlobeEvent
        {
            Center = marker.ToVector(),
            Magnitude = (float)Math.Clamp(magnitude, 0.0, 10.0),
            Color = color.HasValue ? new Vector4(color.Value.R / 255f, color.Value.G / 255f, color.Value.B / 255f, 1f) : (Vector4?)null,
            Label = label,
        };

        lock (_evLock) { _evPending.Add(ev); }
    }

    public void ClearEvents()
    {
        lock (_evLock) { _evPending.Clear(); _evClear = true; }
    }

    #endregion

    private const int MaxEvents = 64;

    private readonly object _evLock = new();
    private readonly List<ParticleGlobeEvent> _evPending = new();
    private readonly List<ParticleGlobeEvent> _events = new();
    private bool _evClear, _showEvents = true, _evRipple = true, _evLabels = true;
    private float _evDuration = 4.5f;
    private CanvasTextFormat _eventFormat;

    private void SyncEventSettings()
    {
        _showEvents = ShowEvents;
        _evRipple = IsEventRippleEnabled;
        _evLabels = ShowEventLabels;
        _evDuration = (float)Math.Clamp(EventDuration, 0.5, 30.0);
    }

    private void AdvanceEvents(float dt)
    {
        lock (_evLock)
        {
            if (_evClear) { _events.Clear(); _evClear = false; }
            if (_evPending.Count > 0)
            {
                foreach (var ev in _evPending)
                {
                    ev.Life = _evDuration * (0.55f + 0.45f * ev.Mag01);
                    if (_events.Count >= MaxEvents) _events.RemoveAt(0);
                    _events.Add(ev);
                    if (_evRipple) SpawnRipple(ev.Center);
                }
                _evPending.Clear();
            }
        }

        for (int i = _events.Count - 1; i >= 0; i--)
        {
            _events[i].Age += dt;
            if (_events[i].Age >= _events[i].Life) _events.RemoveAt(i);
        }
    }

    private Vector4 EventColor(ParticleGlobeEvent ev) => ev.Color ?? _arc;

    private static float Smooth01(float x)
    {
        x = Math.Clamp(x, 0f, 1f);
        return x * x * (3f - 2f * x);
    }

    // Spike height (in globe radii) over the event's life: shoots up fast, then sinks as it fades.
    private static float SpikeHeight(ParticleGlobeEvent ev)
    {
        float rise = 1f - MathF.Pow(1f - Math.Clamp(ev.Age / 0.25f, 0f, 1f), 3f);
        float sink = 1f - 0.6f * Smooth01((ev.Age - ev.Life * 0.4f) / (ev.Life * 0.6f));
        return (0.07f + 0.42f * ev.Mag01) * rise * sink;
    }

    private static float EventFade(ParticleGlobeEvent ev) => 1f - Smooth01((ev.Age - ev.Life * 0.55f) / (ev.Life * 0.45f));

    private void DrawEventRings(CanvasDrawingSession ds)
    {
        if (!_showEvents || _events.Count == 0) return;

        const int Seg = 56;
        foreach (var ev in _events)
        {
            var col = EventColor(ev);
            float maxAng = 0.16f + 0.34f * ev.Mag01;
            Vector3 n = ev.Center;
            Vector3 up = MathF.Abs(n.Y) > 0.95f ? Vector3.UnitX : Vector3.UnitY;
            Vector3 t1 = Vector3.Normalize(Vector3.Cross(up, n));
            Vector3 t2 = Vector3.Cross(n, t1);

            for (int ring = 0; ring < 3; ring++)
            {
                float delay = 0.3f * ring;
                float u = (ev.Age - delay) / MathF.Max(0.2f, ev.Life - delay - 0.2f);
                if (u <= 0f || u >= 1f) continue;

                float ang = maxAng * (1f - (1f - u) * (1f - u)) * (1f - 0.12f * ring);
                float alpha = MathF.Pow(1f - u, 1.5f) * (0.95f - 0.22f * ring);
                float cd = MathF.Cos(ang), sd = MathF.Sin(ang);

                Vector2 prev = default;
                bool havePrev = false;
                for (int i = 0; i <= Seg; i++)
                {
                    float th = i * MathF.Tau / Seg;
                    var pt = (n * cd + (t1 * MathF.Cos(th) + t2 * MathF.Sin(th)) * sd) * 1.004f;
                    var r = Rot(pt);
                    if (r.Z < 0.02f) { havePrev = false; continue; }

                    var p = Proj(r);
                    float a = alpha * MathF.Min(1f, r.Z * 3f);
                    if (havePrev && Jumped(prev, p)) havePrev = false;
                    if (havePrev)
                    {
                        ds.DrawLine(prev, p, ToColor(col, a * 0.28f), 5f - 2f * u);
                        ds.DrawLine(prev, p, ToColor(Vector4.Lerp(col, Vector4.One, 0.35f), a), 1.5f);
                    }
                    prev = p;
                    havePrev = true;
                }
            }
        }
    }

    private void DrawEventSpikes(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        if (!_showEvents || _events.Count == 0) return;

        for (int e = 0; e < _events.Count; e++)
        {
            var ev = _events[e];
            var col = EventColor(ev);
            var rgb = new Vector3(col.X, col.Y, col.Z);
            var hot = Vector3.Lerp(rgb, Vector3.One, 0.6f);
            float fade = EventFade(ev);
            float h = SpikeHeight(ev);

            // impact flash at the base
            var baseV = Rot(ev.Center * 1.004f);
            if (baseV.Z > 0.02f)
            {
                float flash = MathF.Exp(-ev.Age * 3.2f);
                var bp = Proj(baseV);
                batch.Draw(_dot, bp, Tint(rgb, (0.35f * fade + 0.8f * flash)), origin, 0f, new Vector2(sizeBoost * (14f + 26f * ev.Mag01 + 30f * flash)), CanvasSpriteFlip.None);
            }

            int pts = 12 + (int)(14f * ev.Mag01);
            for (int k = 0; k <= pts; k++)
            {
                float f = k / (float)pts;
                var r = Rot(ev.Center * (1.004f + h * f));
                if (r.Z < 0.02f) continue;

                var p = Proj(r);
                if (OffScreen(p, 40f * sizeBoost)) continue;
                float flick = 0.7f + 0.3f * MathF.Sin(t * 28f + k * 1.9f + e);
                float a = (1f - 0.65f * f) * fade * flick * MathF.Min(1f, r.Z * 3f);
                batch.Draw(_dot, p, Tint(hot, a), origin, 0f, new Vector2(sizeBoost * (3.6f - 1.6f * f)), CanvasSpriteFlip.None);
                if ((k & 1) == 0) batch.Draw(_dot, p, Tint(rgb, a * 0.35f), origin, 0f, new Vector2(sizeBoost * 10f), CanvasSpriteFlip.None);

                if (k == pts)
                {
                    batch.Draw(_dot, p, Tint(rgb, 0.8f * fade), origin, 0f, new Vector2(sizeBoost * (14f + 8f * ev.Mag01)), CanvasSpriteFlip.None);
                    batch.Draw(_dot, p, Tint(Vector3.One, fade), origin, 0f, new Vector2(sizeBoost * 5f), CanvasSpriteFlip.None);
                }
            }
        }
    }

    private void DrawEventLabels(CanvasDrawingSession ds)
    {
        if (!_showEvents || !_evLabels || _events.Count == 0) return;

        _eventFormat ??= new CanvasTextFormat { FontFamily = "Consolas", FontSize = 12f, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };

        foreach (var ev in _events)
        {
            float fade = EventFade(ev) * Smooth01((ev.Age - 0.1f) / 0.3f);
            if (fade <= 0.05f) continue;

            var r = Rot(ev.Center * (1.004f + SpikeHeight(ev)));
            if (r.Z < 0.1f) continue;

            var p = Proj(r) + new Vector2(8f, -18f);
            string text = ev.Label ?? "M " + ev.Magnitude.ToString("0.0", CultureInfo.InvariantCulture);
            var c = Vector4.Lerp(EventColor(ev), Vector4.One, 0.5f);
            byte A(float f) => (byte)(Math.Clamp(f, 0f, 1f) * 255f);
            ds.DrawText(text, p + Vector2.One, Color.FromArgb(A(0.6f * fade), 0, 0, 0), _eventFormat);
            ds.DrawText(text, p, Color.FromArgb(A(fade), A(c.X), A(c.Y), A(c.Z)), _eventFormat);
        }
    }
}
