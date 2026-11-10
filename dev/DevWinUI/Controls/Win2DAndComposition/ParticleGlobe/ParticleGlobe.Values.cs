namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const int MaxBarParticles = 56;

    private readonly object _valLock = new();
    private Dictionary<string, double> _valRaw;
    private Color _valLowColor, _valHighColor;
    private bool _valDirty;
    private object _valResolvedFor;

    private volatile ParticleGlobeValueSet _values;
    private float _heatAmt, _barAmt;
    private bool _showBars;
    private float _barMaxHeight = 0.3f;

    public void SetCountryValues(IReadOnlyDictionary<string, double> values, Color low, Color high)
    {
        ArgumentNullException.ThrowIfNull(values);

        var copy = new Dictionary<string, double>(values.Count, StringComparer.OrdinalIgnoreCase);
        foreach (var pair in values)
        {
            if (!string.IsNullOrEmpty(pair.Key) && double.IsFinite(pair.Value)) copy[pair.Key] = pair.Value;
        }

        lock (_valLock)
        {
            _valRaw = copy.Count > 0 ? copy : null;
            _tlFrames = null;
            _tlCount = 0;
            _tlT = null;
            _tlPlaying = false;
            _valLowColor = low;
            _valHighColor = high;
            _valDirty = true;
        }
    }

    public IReadOnlyList<string> GetCountryCodes()
    {
        var list = new List<string>();
        foreach (var c in _countries)
        {
            if (c != null && !string.IsNullOrEmpty(c.Code)) list.Add(c.Code);
        }
        return list;
    }

    public void ClearCountryValues()
    {
        lock (_valLock)
        {
            _valRaw = null;
            _tlFrames = null;
            _tlCount = 0;
            _tlT = null;
            _tlPlaying = false;
            _valDirty = true;
        }
    }

    private static Vector3 ToRgb(Color c) => new(c.R / 255f, c.G / 255f, c.B / 255f);

    private void ResolveValues()
    {
        Dictionary<string, double> raw;
        Color lowC, highC;
        lock (_valLock)
        {
            raw = _valRaw;
            lowC = _valLowColor;
            highC = _valHighColor;
            _valDirty = false;
        }

        if (ResolveTimeline()) return;

        var map = _codeToId;
        var entries = _countries;
        _valResolvedFor = map;

        if (raw == null)
        {
            _values = null;
            return;
        }

        var resolved = new List<(int Id, double Value)>();
        foreach (var pair in raw)
        {
            if (map.TryGetValue(pair.Key, out int id) && id > 0 && id < entries.Length) resolved.Add((id, pair.Value));
        }

        if (resolved.Count == 0)
        {
            _values = null;
            return;
        }

        double min = double.MaxValue, max = double.MinValue;
        foreach (var (_, v) in resolved)
        {
            min = Math.Min(min, v);
            max = Math.Max(max, v);
        }
        double span = max - min;

        var t = new float[entries.Length];
        Array.Fill(t, float.NaN);

        var set = new ParticleGlobeValueSet
        {
            T = t,
            Low = ToRgb(lowC),
            High = ToRgb(highC),
            BarIds = new int[resolved.Count],
            BarDir = new Vector3[resolved.Count],
            BarE1 = new Vector3[resolved.Count],
            BarE2 = new Vector3[resolved.Count]
        };

        for (int i = 0; i < resolved.Count; i++)
        {
            var (id, v) = resolved[i];
            t[id] = span <= 0 ? 1f : (float)((v - min) / span);

            var e = entries[id];
            float cl = MathF.Cos(e.LatRad);
            var dir = new Vector3(cl * MathF.Sin(e.LonRad), MathF.Sin(e.LatRad), cl * MathF.Cos(e.LonRad));
            var up = MathF.Abs(dir.Y) > 0.95f ? Vector3.UnitX : Vector3.UnitY;
            var e1 = Vector3.Normalize(Vector3.Cross(dir, up));

            set.BarIds[i] = id;
            set.BarDir[i] = dir;
            set.BarE1[i] = e1;
            set.BarE2[i] = Vector3.Cross(dir, e1);
        }

        _values = set;
    }

    private void AdvanceValues(float dt)
    {
        if (_valDirty || !ReferenceEquals(_valResolvedFor, _codeToId) || (_values == null && (_valRaw != null || _tlFrames != null) && _countries.Length > 0))
        {
            bool hadValues = _values != null;
            ResolveValues();
            if (!hadValues && _values != null) _heatAmt = 0f;
        }

        AdvanceTimeline(dt);

        float heatTarget = _values != null ? 1f : 0f;
        _heatAmt += Math.Clamp(heatTarget - _heatAmt, -dt * 2f, dt * 1.5f);

        float barTarget = _showBars && _values != null ? 1f : 0f;
        _barAmt += Math.Clamp(barTarget - _barAmt, -dt * 2.5f, dt * 1.2f);
    }

    private void DrawBars(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        var set = _values;
        if (set == null || _barAmt <= 0.001f) return;

        float grow = _barAmt * _barAmt * (3f - 2f * _barAmt);

        for (int b = 0; b < set.BarIds.Length; b++)
        {
            float v = set.T[set.BarIds[b]];
            if (float.IsNaN(v)) continue;
            float h = _barMaxHeight * (0.04f + 0.96f * v) * grow;
            int n = 10 + (int)(v * (MaxBarParticles - 10));
            var rgb = Vector3.Lerp(set.Low, set.High, v);
            var top = Vector3.Lerp(rgb, Vector3.One, 0.45f);

            var dir = set.BarDir[b];
            var e1 = set.BarE1[b];
            var e2 = set.BarE2[b];
            float width = 0.010f + 0.012f * v;

            for (int j = 0; j < n; j++)
            {
                float seed = Hash(b * 131 + j, j * 7 + 3, 11);
                float u = (j + seed) / n + t * 0.18f;
                u -= MathF.Floor(u);

                float ang = j * 2.39996f;
                float lr = width * (0.25f + 0.75f * MathF.Sqrt(seed));
                float lift = 1f + 0.004f + u * h;
                var p = (dir + (e1 * MathF.Cos(ang) + e2 * MathF.Sin(ang)) * lr) * lift;

                var rv = Rot(p);
                float x1 = rv.X, y2 = rv.Y, z2 = rv.Z;

                float k = _radius / (1f - z2 * Perspective);
                var pos = new Vector2(_center.X + x1 * k, _center.Y - y2 * k);
                if (OffScreen(pos, 12f * sizeBoost)) continue;

                float face = z2 >= 0f ? 0.7f + 0.5f * (1f - z2) * (1f - z2) : (x1 * x1 + y2 * y2 < 1f ? 0.1f : 0.6f);
                float twinkle = 0.75f + 0.25f * MathF.Sin(t * 3f + seed * 40f);
                float a = (0.35f + 0.65f * u) * face * twinkle * _barAmt;
                float scale = sizeBoost * (2.6f + 1.8f * u);

                batch.Draw(_dot, pos, Tint(Vector3.Lerp(rgb, top, u), a), origin, 0f, new Vector2(scale), CanvasSpriteFlip.None);
            }
        }
    }
}
