namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    public bool ShowCountryBorders
    {
        get { return (bool)GetValue(ShowCountryBordersProperty); }
        set { SetValue(ShowCountryBordersProperty, value); }
    }

    public static readonly DependencyProperty ShowCountryBordersProperty =
        DependencyProperty.Register(nameof(ShowCountryBorders), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnSettingsPropertyChanged));

    public int BorderParticleCount
    {
        get { return (int)GetValue(BorderParticleCountProperty); }
        set { SetValue(BorderParticleCountProperty, value); }
    }

    public static readonly DependencyProperty BorderParticleCountProperty =
        DependencyProperty.Register(nameof(BorderParticleCount), typeof(int), typeof(ParticleGlobe), new PropertyMetadata(12000, OnSettingsPropertyChanged));

    public Brush BorderLineBrush
    {
        get { return (Brush)GetValue(BorderLineBrushProperty); }
        set { SetValue(BorderLineBrushProperty, value); }
    }

    public static readonly DependencyProperty BorderLineBrushProperty =
        DependencyProperty.Register(nameof(BorderLineBrush), typeof(Brush), typeof(ParticleGlobe), new PropertyMetadata(null, OnSettingsPropertyChanged));

    public double BorderIntensity
    {
        get { return (double)GetValue(BorderIntensityProperty); }
        set { SetValue(BorderIntensityProperty, value); }
    }

    public static readonly DependencyProperty BorderIntensityProperty =
        DependencyProperty.Register(nameof(BorderIntensity), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(1.5, OnSettingsPropertyChanged));

    public bool BorderScanOnly
    {
        get { return (bool)GetValue(BorderScanOnlyProperty); }
        set { SetValue(BorderScanOnlyProperty, value); }
    }

    public static readonly DependencyProperty BorderScanOnlyProperty =
        DependencyProperty.Register(nameof(BorderScanOnly), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnSettingsPropertyChanged));

    public double BorderFadeSeconds
    {
        get { return (double)GetValue(BorderFadeSecondsProperty); }
        set { SetValue(BorderFadeSecondsProperty, value); }
    }

    public static readonly DependencyProperty BorderFadeSecondsProperty =
        DependencyProperty.Register(nameof(BorderFadeSeconds), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(1.5, OnSettingsPropertyChanged));

    private bool _showBorders;
    private int _borderMax = 12000;
    private float _bdAmt, _bdIntensity = 1.5f, _bdFade = 1.5f;
    private bool _bdScanOnly;
    private Vector4 _bdColor = new(1f, 0.82f, 0.35f, 1f);

    private float[] _bdX = Array.Empty<float>(), _bdY = Array.Empty<float>(), _bdZ = Array.Empty<float>();
    private float[] _bdLon = Array.Empty<float>(), _bdSeed = Array.Empty<float>();
    private byte[] _bdShared = Array.Empty<byte>();
    private int _bdCount;
    private ushort[] _bdSrc;
    private int _bdBuiltMax;

    private void SyncBorderSettings()
    {
        _showBorders = ShowCountryBorders;
        _borderMax = Math.Clamp(BorderParticleCount, 1000, 120000);
        _bdIntensity = (float)Math.Clamp(BorderIntensity, 0.2, 4.0);
        _bdScanOnly = BorderScanOnly;
        _bdFade = (float)Math.Clamp(BorderFadeSeconds, 0.1, 10.0);
        _bdColor = ToVector(BorderLineBrush, GetPalette(Theme).Border);
    }

    private void AdvanceBorders(float dt)
    {
        float target = _showBorders ? 1f : 0f;
        _bdAmt += Math.Clamp(target - _bdAmt, -dt / 0.6f, dt / 0.6f);
    }

    private void EnsureBorders()
    {
        var ids = _geoIds;
        int w = _geoW, h = _geoH;
        if (ids == null || w <= 0 || h <= 0 || ids.Length < w * h) { _bdCount = 0; return; }
        if (ReferenceEquals(_bdSrc, ids) && _bdBuiltMax == _borderMax) return;

        var edge = new List<int>(65536);
        for (int v = 0; v < h; v++)
        {
            for (int u = 0; u < w; u++)
            {
                int i = v * w + u;
                int id = ids[i];
                if (id == 0) continue;

                int l = u > 0 ? ids[i - 1] : id, r = u < w - 1 ? ids[i + 1] : id;
                int up = v > 0 ? ids[i - w] : id, dn = v < h - 1 ? ids[i + w] : id;
                if (l != id || r != id || up != id || dn != id) edge.Add(i);
            }
        }

        int stride = Math.Max(1, (edge.Count + _borderMax - 1) / _borderMax);
        int n = (edge.Count + stride - 1) / stride;
        var xs = new float[n]; var ys = new float[n]; var zs = new float[n];
        var lons = new float[n]; var seeds = new float[n];
        var shared = new byte[n];
        var rng = new Random(4711);
        int k = 0;

        for (int e = 0; e < edge.Count && k < n; e += stride)
        {
            int i = edge[e];
            int v = i / w, u = i - v * w;
            int id = ids[i];

            // a shared border touches another country; a coastline only touches the ocean
            bool sh = false;
            int l = u > 0 ? ids[i - 1] : id, r = u < w - 1 ? ids[i + 1] : id;
            int up = v > 0 ? ids[i - w] : id, dn = v < h - 1 ? ids[i + w] : id;
            if ((l != id && l != 0) || (r != id && r != 0) || (up != id && up != 0) || (dn != id && dn != 0)) sh = true;

            float fu = u + (float)rng.NextDouble();
            float fv = v + (float)rng.NextDouble();
            float lon = (fu / GeoRasterScale - GeoOriginX) / GeoScale;
            float lat = 2f * MathF.Atan(MathF.Exp((GeoOriginY - fv / GeoRasterScale) / GeoScale)) - MathF.PI * 0.5f;
            float cl = MathF.Cos(lat);

            xs[k] = cl * MathF.Sin(lon) * 1.004f;
            ys[k] = MathF.Sin(lat) * 1.004f;
            zs[k] = cl * MathF.Cos(lon) * 1.004f;
            lons[k] = lon;
            seeds[k] = (float)rng.NextDouble();
            shared[k] = sh ? (byte)1 : (byte)0;
            k++;
        }

        _bdX = xs; _bdY = ys; _bdZ = zs; _bdLon = lons; _bdSeed = seeds; _bdShared = shared;
        _bdCount = k;
        _bdBuiltMax = _borderMax;
        _bdSrc = ids;
    }

    private void DrawBorders(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        float amt = _bdAmt;
        if (amt <= 0.003f) return;

        EnsureBorders();
        int count = _bdCount;
        if (count == 0) return;

        var bx = _bdX; var by = _bdY; var bz = _bdZ;
        var blon = _bdLon; var bseed = _bdSeed; var bshared = _bdShared;
        var accent = new Vector3(_bdColor.X, _bdColor.Y, _bdColor.Z);
        var core = Vector3.Lerp(accent, Vector3.One, 0.5f);
        float inten = _bdIntensity;
        float morphE = _morphE;
        bool scanOnly = _bdScanOnly;
        float trail = MathF.Max(0.05f, 0.9f * _bdFade); // the scan advances 0.9 rad/s
        float scanLon = _scanLon;

        for (int i = 0; i < count; i++)
        {
            float y = by[i];
            float vis = amt;
            if (morphE > 0f && MathF.Abs(y) > 0.985f) vis *= 1f - morphE; // polar rows are cut off on the flat map
            if (vis <= 0.003f) continue;

            float d = MathF.Abs(WrapPi(blon[i] - scanLon));
            float boost = d < 0.4f ? MathF.Exp(-d * d * 45f) : 0f;
            float lvl = 1f;
            if (scanOnly)
            {
                // angle travelled since the beam passed this particle
                float behind = (scanLon - blon[i]) % MathF.Tau;
                if (behind < 0f) behind += MathF.Tau;
                lvl = MathF.Max(MathF.Exp(-behind / trail), boost);
                if (lvl < 0.02f) continue;
            }

            var rv = Rot(new Vector3(bx[i], y, bz[i]));
            if (rv.Z < 0.02f) continue;

            float k = _radius / (1f - rv.Z * Perspective);
            var pos = new Vector2(_center.X + rv.X * k, _center.Y - rv.Y * k);
            if (OffScreen(pos, 20f * sizeBoost)) continue;

            float shimmer = 0.75f + 0.25f * MathF.Sin(t * 2.6f + bseed[i] * 40f);
            float strength = bshared[i] != 0 ? 1f : 0.55f;
            float fade = MathF.Min(1f, rv.Z * 3f) * vis * strength * lvl;

            float a = (0.6f + 0.9f * boost) * shimmer * fade * inten;
            var rgb = Vector3.Lerp(accent, core, MathF.Min(1f, 0.15f + boost));
            batch.Draw(_dot, pos, Tint(rgb, a), origin, 0f, new Vector2(sizeBoost * (3.4f + 2.4f * boost) * MathF.Sqrt(inten)), CanvasSpriteFlip.None);

            if (boost > 0.15f || (i & 3) == 0)
            {
                batch.Draw(_dot, pos, Tint(accent, (0.15f + 0.5f * boost) * fade * inten), origin, 0f, new Vector2(sizeBoost * (9f + 9f * boost)), CanvasSpriteFlip.None);
            }
        }
    }
}
