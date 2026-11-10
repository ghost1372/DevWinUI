using System.Globalization;
using System.Text;

namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    #region Properties

    public bool ShowSatellites
    {
        get { return (bool)GetValue(ShowSatellitesProperty); }
        set { SetValue(ShowSatellitesProperty, value); }
    }

    public static readonly DependencyProperty ShowSatellitesProperty =
        DependencyProperty.Register(nameof(ShowSatellites), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public int SatelliteCount
    {
        get { return (int)GetValue(SatelliteCountProperty); }
        set { SetValue(SatelliteCountProperty, value); }
    }

    public static readonly DependencyProperty SatelliteCountProperty =
        DependencyProperty.Register(nameof(SatelliteCount), typeof(int), typeof(ParticleGlobe), new PropertyMetadata(6, OnSettingsPropertyChanged));

    public bool ShowCoordinateTicker
    {
        get { return (bool)GetValue(ShowCoordinateTickerProperty); }
        set { SetValue(ShowCoordinateTickerProperty, value); }
    }

    public static readonly DependencyProperty ShowCoordinateTickerProperty =
        DependencyProperty.Register(nameof(ShowCoordinateTicker), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowScanlines
    {
        get { return (bool)GetValue(ShowScanlinesProperty); }
        set { SetValue(ShowScanlinesProperty, value); }
    }

    public static readonly DependencyProperty ShowScanlinesProperty =
        DependencyProperty.Register(nameof(ShowScanlines), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowGlitch
    {
        get { return (bool)GetValue(ShowGlitchProperty); }
        set { SetValue(ShowGlitchProperty, value); }
    }

    public static readonly DependencyProperty ShowGlitchProperty =
        DependencyProperty.Register(nameof(ShowGlitch), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    #endregion

    private bool _showSats = true, _showTicker = true, _showScan = true, _showGlitch = true;
    private int _satWanted = 6;
    private int _satBrushVersion = -1;

    private ParticleGlobeSatellite[] _sats = Array.Empty<ParticleGlobeSatellite>();
    private readonly List<Vector4> _satHeads = new();

    private void SyncHudFxSettings()
    {
        _showSats = ShowSatellites;
        _satWanted = Math.Clamp(SatelliteCount, 0, 24);
        _showTicker = ShowCoordinateTicker;
        _showScan = ShowScanlines;
        _showGlitch = ShowGlitch;
    }

    private void EnsureSatellites()
    {
        if (_sats.Length == _satWanted && _satBrushVersion == _brushVersion) return;
        _satBrushVersion = _brushVersion;

        var rng = new Random(20240);
        var sats = new ParticleGlobeSatellite[_satWanted];
        var palette = new[]
        {
            new Vector4(_accent.X, _accent.Y, _accent.Z, 1f),
            new Vector4(_arc.X, _arc.Y, _arc.Z, 1f),
            new Vector4(1f, 0.85f, 0.45f, 1f),
        };

        for (int i = 0; i < sats.Length; i++)
        {
            float incl = (float)(rng.NextDouble() * 150.0 - 75.0);
            float node = (float)(rng.NextDouble() * 360.0);
            float radius = 1.16f + (float)rng.NextDouble() * 0.7f;
            float speed = (0.28f + 0.35f / radius) * (rng.Next(4) == 0 ? -1f : 1f);

            sats[i] = new ParticleGlobeSatellite
            {
                Basis = Matrix4x4.CreateRotationZ(incl * MathF.PI / 180f) * Matrix4x4.CreateRotationY(node * MathF.PI / 180f),
                Radius = radius,
                Speed = speed,
                Phase = (float)(rng.NextDouble() * Math.Tau),
                Color = palette[i % palette.Length],
            };
        }

        _sats = sats;
    }

    private static Vector3 SatLocal(ParticleGlobeSatellite s, float t, float back = 0f)
    {
        float a = s.Phase + s.Speed * t - MathF.Sign(s.Speed) * back;
        return new Vector3(MathF.Cos(a) * s.Radius, 0f, MathF.Sin(a) * s.Radius);
    }

    private void DrawSatelliteTrails(CanvasDrawingSession ds, float t)
    {
        _satHeads.Clear();
        float vis = 1f - _morphE;
        if (!_showSats || vis <= 0.01f) return;

        EnsureSatellites();
        var view = Matrix4x4.CreateRotationY(_yaw * 0.25f) * Matrix4x4.CreateRotationX(_tilt + _pitch);
        const int Steps = 26;
        const float TrailLen = 0.75f;

        for (int si = 0; si < _sats.Length; si++)
        {
            var s = _sats[si];
            var m = s.Basis * view;
            Vector2 prev = default;
            bool havePrev = false;

            for (int k = 0; k <= Steps; k++)
            {
                float u = k / (float)Steps;
                var r = Vector3.Transform(SatLocal(s, t, TrailLen * (1f - u)), m);
                if (Occluded(r)) { havePrev = false; continue; }

                var p = Proj(r);
                float depth = Math.Clamp(0.6f + 0.4f * r.Z, 0.3f, 1f);
                if (havePrev) ds.DrawLine(prev, p, ToColor(s.Color, 0.85f * u * u * depth * vis), 1.2f + 0.8f * u);
                prev = p;
                havePrev = true;

                if (k == Steps) _satHeads.Add(new Vector4(p.X, p.Y, depth * vis, si));
            }
        }
    }

    private void DrawSatelliteHeads(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        foreach (var h in _satHeads)
        {
            int idx = (int)h.W;
            float pulse = 0.75f + 0.25f * MathF.Sin(t * 5f + idx * 1.7f);
            var pos = new Vector2(h.X, h.Y);
            var col = _sats[idx].Color;
            var rgb = new Vector3(col.X, col.Y, col.Z);
            batch.Draw(_dot, pos, Tint(rgb, 0.55f * h.Z * pulse), origin, 0f, new Vector2(sizeBoost * 15f), CanvasSpriteFlip.None);
            batch.Draw(_dot, pos, Tint(Vector3.One, 0.95f * h.Z), origin, 0f, new Vector2(sizeBoost * 4.2f), CanvasSpriteFlip.None);
        }
    }

    #region HUD readout and ticker

    private CanvasTextFormat _tickFormat;
    private float _charW, _lineH;
    private string _tickUnit = "";
    private string _tickText, _tickTextFor;
    private int _tickReps;
    private float _tickStamp = -10f;
    private float _ctrLat, _ctrLon;
    private bool _haveCtr;

    private void EnsureTickFormat(CanvasDrawingSession ds)
    {
        if (_tickFormat != null && _charW > 0f) return;

        _tickFormat = new CanvasTextFormat { FontFamily = "Consolas", FontSize = 11f, WordWrapping = CanvasWordWrapping.NoWrap };
        using var layout = new CanvasTextLayout(ds, "0000000000", _tickFormat, 400f, 40f);
        _charW = (float)layout.LayoutBounds.Width / 10f;
        _lineH = (float)layout.LayoutBounds.Height;
    }

    private static string Coord(float latRad, float lonRad, string latFmt = "00.00", string lonFmt = "000.00")
    {
        float la = latRad * 180f / MathF.PI, lo = lonRad * 180f / MathF.PI;
        return $"{MathF.Abs(la).ToString(latFmt, CultureInfo.InvariantCulture)}\u00B0{(la >= 0f ? 'N' : 'S')} {MathF.Abs(lo).ToString(lonFmt, CultureInfo.InvariantCulture)}\u00B0{(lo >= 0f ? 'E' : 'W')}";
    }

    private string BuildTickerUnit()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < _sats.Length; i++)
        {
            var w = Vector3.Transform(SatLocal(_sats[i], _time), _sats[i].Basis);
            float len = w.Length();
            float lat = MathF.Asin(Math.Clamp(w.Y / len, -1f, 1f));
            float lon = MathF.Atan2(w.X, w.Z);
            float alt = (len - 1f) * 6371f;
            sb.Append("SAT-").Append((i + 1).ToString("00", CultureInfo.InvariantCulture)).Append(' ')
              .Append(Coord(lat, lon, "00.0", "000.0")).Append(" ALT ").Append(alt.ToString("0000", CultureInfo.InvariantCulture)).Append("KM   \u25C6   ");
        }

        return sb.ToString();
    }

    private void DrawCoordinateTicker(CanvasDrawingSession ds, float w, float h, float t)
    {
        if (!_showHud || !_showTicker) return;
        EnsureTickFormat(ds);
        EnsureSatellites();

        if (TryUnproject(_center, out var wp))
        {
            _ctrLat = MathF.Asin(Math.Clamp(wp.Y / MathF.Max(wp.Length(), 1e-5f), -1f, 1f));
            _ctrLon = MathF.Atan2(wp.X, wp.Z);
            _haveCtr = true;
        }

        float pad = 8f, stripH = _lineH + 6f;
        float stripY = h - stripH - 8f;
        float left = 12f, right = w - 12f;
        float stripW = right - left;
        if (stripW < 80f) return;

        // scrolling satellite ticker
        if (_sats.Length > 0)
        {
            if (t - _tickStamp > 0.25f || _tickUnit.Length == 0)
            {
                _tickUnit = BuildTickerUnit();
                _tickStamp = t;
            }

            float unitW = _tickUnit.Length * _charW;
            if (unitW > 1f)
            {
                int reps = (int)MathF.Ceiling(stripW / unitW) + 2;
                if (_tickText == null || _tickTextFor != _tickUnit || _tickReps != reps)
                {
                    var sb = new StringBuilder(_tickUnit.Length * reps);
                    for (int i = 0; i < reps; i++) sb.Append(_tickUnit);
                    _tickText = sb.ToString();
                    _tickTextFor = _tickUnit;
                    _tickReps = reps;
                }
                var text = _tickText;

                float off = (t * 48f) % unitW;
                ds.DrawLine(left, stripY, right, stripY, ToColor(_accent, 0.25f), 1f);
                using (ds.CreateLayer(1f, new Windows.Foundation.Rect(left, stripY + 1f, stripW, stripH)))
                {
                    ds.DrawText(text, left - off, stripY + 4f, ToColor(Vector4.Lerp(_accent, Vector4.One, 0.2f), 0.7f), _tickFormat);
                }
            }
        }

        // readout panel, bottom left above the ticker
        string utc = DateTime.UtcNow.ToString("HH:mm:ss", CultureInfo.InvariantCulture);
        string cursor = ((int)(t * 2f) & 1) == 0 ? "_" : " ";
        string[] lines =
        {
            "CTR  " + (_haveCtr ? Coord(_ctrLat, _ctrLon) : "--.--\u00B0- ---.--\u00B0-"),
            "UTC  " + utc + cursor,
            "SAT  " + _sats.Length.ToString("00", CultureInfo.InvariantCulture) + (_showSats ? " TRACKING" : " OFFLINE"),
        };

        float pw = 26f * _charW + pad * 2f, ph = lines.Length * _lineH + pad * 1.5f;
        float py = stripY - ph - 6f;
        DrawPanel(ds, new Windows.Foundation.Rect(left, py, pw, ph), 0.8f);
        var col = ToColor(Vector4.Lerp(_accent, Vector4.One, 0.35f), 0.9f);
        for (int i = 0; i < lines.Length; i++)
        {
            ds.DrawText(lines[i], left + pad + 3f, py + pad * 0.75f + i * _lineH, col, _tickFormat);
        }
    }

    #endregion

    #region Scanlines and glitch

    private CanvasImageBrush _scanBrush;
    private int _scanBrushVersion = -1;

    private CanvasImageBrush GetScanBrush(CanvasDrawingSession ds)
    {
        if (_scanBrush != null && _scanBrushVersion == _brushVersion) return _scanBrush;

        _scanBrush?.Dispose();
        using (var tile = new CanvasRenderTarget(ds, 1f, 3f, 96f))
        {
            using (var tds = tile.CreateDrawingSession())
            {
                tds.Clear(Colors.Transparent);
                tds.FillRectangle(0f, 0f, 1f, 1f, ToColor(_accent, 0.035f));
            }
            _scanBrush = new CanvasImageBrush(ds, tile)
            {
                ExtendX = CanvasEdgeBehavior.Wrap,
                ExtendY = CanvasEdgeBehavior.Wrap,
                SourceRectangle = new Windows.Foundation.Rect(0, 0, 1, 3)
            };
        }
        _scanBrushVersion = _brushVersion;
        return _scanBrush;
    }

    private void DrawScreenFx(CanvasDrawingSession ds, float w, float h, float t)
    {
        if (_showScan)
        {
            ds.FillRectangle(0f, 0f, w, h, GetScanBrush(ds));

            // slow bright band sweeping downwards
            float bandH = MathF.Max(40f, h * 0.12f);
            float by = ((t * 0.09f) % 1.4f - 0.2f) * h;
            const int Bands = 10;
            for (int i = 0; i < Bands; i++)
            {
                float u = (i + 0.5f) / Bands;
                float a = 0.035f * MathF.Sin(u * MathF.PI);
                ds.FillRectangle(0f, by + u * bandH, w, bandH / Bands + 0.5f, ToColor(_accent, a));
            }
        }

        if (_showGlitch)
        {
            // a glitch fires in the first moments of roughly one in five 1.1 s windows
            float window = t / 1.1f;
            int idx = (int)MathF.Floor(window);
            float local = (window - idx) * 1.1f;
            if (Hash01(idx + 0.5f, 12.9898f) > 0.8f && local < 0.16f)
            {
                int bars = 2 + (int)(Hash01(idx + 0.5f, 78.233f) * 4f);
                for (int b = 0; b < bars; b++)
                {
                    float sd = idx * 7.31f + b * 3.17f + (int)(local * 40f) * 0.13f;
                    float y = Hash01(sd, 12.9898f) * h;
                    float bh = 2f + Hash01(sd, 4.1414f) * 9f;
                    float x = Hash01(sd, 93.9898f) * w * 0.6f;
                    float bw = w * (0.2f + 0.5f * Hash01(sd, 54.321f));
                    ds.FillRectangle(x, y, bw, bh, ToColor(_accent, 0.07f));
                    ds.FillRectangle(x + 4f, y, bw, bh, Color.FromArgb(14, 255, 60, 90));
                    ds.FillRectangle(x - 4f, y + 1f, bw, bh, Color.FromArgb(14, 60, 200, 255));
                }
            }
        }
    }

    #endregion
}
