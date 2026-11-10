using System.Globalization;

namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const double EarthRadiusKm = 6371.0;

    private volatile ParticleGlobeCompareState _cmp;
    private float _cmpSpeed = 850f;
    private CanvasTextFormat _cmpFormat;
    private readonly List<Vector2> _cmpPts = new();

    public event EventHandler<ParticleGlobeCompareEventArgs> CompareChanged;

    public bool IsCompareModeEnabled
    {
        get { return (bool)GetValue(IsCompareModeEnabledProperty); }
        set { SetValue(IsCompareModeEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsCompareModeEnabledProperty =
        DependencyProperty.Register(nameof(IsCompareModeEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnCompareModeChanged));

    public double CompareFlightSpeedKmh
    {
        get { return (double)GetValue(CompareFlightSpeedKmhProperty); }
        set { SetValue(CompareFlightSpeedKmhProperty, value); }
    }

    public static readonly DependencyProperty CompareFlightSpeedKmhProperty =
        DependencyProperty.Register(nameof(CompareFlightSpeedKmh), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(850.0, OnSettingsPropertyChanged));

    private static void OnCompareModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (!(bool)e.NewValue) ((ParticleGlobe)d)._cmp = null;
    }

    private void SyncCompareSettings()
    {
        _cmpSpeed = (float)Math.Clamp(double.IsFinite(CompareFlightSpeedKmh) ? CompareFlightSpeedKmh : 850.0, 50.0, 100000.0);
    }

    public bool CompareCountries(string codeA, string codeB)
    {
        var a = FindEntry(codeA);
        var b = FindEntry(codeB);
        if (a == null || b == null || ReferenceEquals(a, b)) return false;

        StartCompare(a);
        FinishCompare(b);
        return true;
    }

    public void ClearCompare() => _cmp = null;

    private void HandleComparePick(int id)
    {
        var list = _countries;
        if (id <= 0 || id >= list.Length) return;

        var entry = list[id];
        var cur = _cmp;
        if (cur == null || cur.B != null) StartCompare(entry);
        else if (!ReferenceEquals(cur.A, entry)) FinishCompare(entry);
    }

    private static Vector3 EntryVector(ParticleGlobeCountryEntry e)
    {
        float cl = MathF.Cos(e.LatRad);
        return new Vector3(cl * MathF.Sin(e.LonRad), MathF.Sin(e.LatRad), cl * MathF.Cos(e.LonRad));
    }

    private void StartCompare(ParticleGlobeCountryEntry a)
    {
        _cmp = new ParticleGlobeCompareState { A = a, Av = EntryVector(a), Start = _time };
    }

    private void FinishCompare(ParticleGlobeCountryEntry b)
    {
        var cur = _cmp;
        if (cur == null) return;

        var bv = EntryVector(b);
        float omega = MathF.Acos(Math.Clamp(Vector3.Dot(cur.Av, bv), -1f, 1f));

        float la1 = cur.A.LatRad, la2 = b.LatRad, dl = b.LonRad - cur.A.LonRad;
        float y = MathF.Sin(dl) * MathF.Cos(la2);
        float x = MathF.Cos(la1) * MathF.Sin(la2) - MathF.Sin(la1) * MathF.Cos(la2) * MathF.Cos(dl);
        float bearing = (MathF.Atan2(y, x) * 180f / MathF.PI + 360f) % 360f;

        ParticleGlobeArcState path = null;
        if (omega > 0.01f && omega < MathF.PI - 0.01f)
        {
            path = new ParticleGlobeArcState { A = cur.Av, B = bv, Omega = omega, Height = 0.10f + 0.30f * (omega / MathF.PI) };
        }

        float km = omega * (float)EarthRadiusKm;
        var next = new ParticleGlobeCompareState
        {
            A = cur.A,
            B = b,
            Av = cur.Av,
            Bv = bv,
            Path = path,
            DistanceKm = km,
            BearingDeg = bearing,
            Start = _time
        };
        _cmp = next;

        var flight = TimeSpan.FromHours(km / _cmpSpeed);
        var args = new ParticleGlobeCompareEventArgs(cur.A.Code, cur.A.Name, b.Code, b.Name, km, flight);
        CompareChanged?.Invoke(this, args);
    }

    private static string ShortName(string name, int max = 14)
    {
        if (string.IsNullOrEmpty(name)) return "?";
        return name.Length <= max ? name : name.Substring(0, max - 1) + ".";
    }

    private static string FormatFlight(float hours)
    {
        int totalMin = (int)MathF.Round(hours * 60f);
        return (totalMin / 60).ToString("00", CultureInfo.InvariantCulture) + "H " + (totalMin % 60).ToString("00", CultureInfo.InvariantCulture) + "M";
    }

    private void DrawCompareArc(CanvasDrawingSession ds)
    {
        var s = _cmp;
        if (s?.Path == null) return;

        float grow = Smooth01((_time - s.Start) / 1.1f);
        const int Seg = 64;
        int n = (int)(Seg * grow);
        if (n < 2) return;

        _cmpPts.Clear();
        var invalid = new Vector2(float.NaN);
        for (int k = 0; k <= n; k++)
        {
            var r = Rot(ArcPoint(s.Path, k / (float)Seg));
            _cmpPts.Add(Occluded(r) ? invalid : Proj(r));
        }

        var col = Vector4.Lerp(_accent, Vector4.One, 0.3f);
        for (int pass = 0; pass < 2; pass++)
        {
            var color = ToColor(col, pass == 0 ? 0.14f : 0.9f);
            float width = pass == 0 ? 7f : 1.8f;
            for (int k = 1; k < _cmpPts.Count; k++)
            {
                var p0 = _cmpPts[k - 1];
                var p1 = _cmpPts[k];
                if (float.IsNaN(p0.X) || float.IsNaN(p1.X) || Jumped(p0, p1)) continue;
                ds.DrawLine(p0, p1, color, width);
            }
        }
    }

    private void DrawCompareSprites(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        var s = _cmp;
        if (s == null) return;

        float appear = Smooth01((_time - s.Start) / 0.4f);
        var rgb = Vector3.Lerp(new Vector3(_accent.X, _accent.Y, _accent.Z), Vector3.One, 0.3f);
        float pulse = 0.5f + 0.5f * MathF.Sin(t * 4f);

        void Endpoint(Vector3 v, float amt)
        {
            var r = Rot(v * 1.004f);
            if (r.Z < 0.02f) return;
            var p = Proj(r);
            float fade = Math.Min(1f, r.Z * 3f) * amt;
            batch.Draw(_dot, p, Tint(rgb, (0.35f + 0.35f * pulse) * fade), origin, 0f, new Vector2(sizeBoost * (20f + 10f * pulse)), CanvasSpriteFlip.None);
            batch.Draw(_dot, p, Tint(Vector3.One, fade), origin, 0f, new Vector2(sizeBoost * 5f), CanvasSpriteFlip.None);
        }

        Endpoint(s.Av, appear);
        if (s.B == null) return;

        float bAmt = Smooth01((_time - s.Start - 0.7f) / 0.4f);
        Endpoint(s.Bv, bAmt);

        if (s.Path == null) return;
        float grow = Smooth01((_time - s.Start) / 1.1f);
        for (int k = 0; k < 4; k++)
        {
            float u0 = (t * 0.3f + k * 0.25f) % 1f;
            for (int tr = 0; tr < 4; tr++)
            {
                float u = (u0 - tr * 0.012f);
                if (u < 0f) continue;
                u *= grow;
                var r = Rot(ArcPoint(s.Path, u));
                if (Occluded(r)) continue;
                float a = (1f - tr * 0.25f) * grow;
                batch.Draw(_dot, Proj(r), Tint(tr == 0 ? Vector3.One : rgb, a), origin, 0f, new Vector2(sizeBoost * (tr == 0 ? 6f : 4.5f - tr * 0.8f)), CanvasSpriteFlip.None);
            }
        }
    }

    private void DrawCompareLabels(CanvasDrawingSession ds)
    {
        var s = _cmp;
        if (s == null) return;

        _cmpFormat ??= new CanvasTextFormat { FontFamily = "Consolas", FontSize = 12f, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold };
        var c = Vector4.Lerp(_accent, Vector4.One, 0.55f);
        byte A(float f) => (byte)(Math.Clamp(f, 0f, 1f) * 255f);

        void Text(string text, Vector3 v, float amt, Vector2 off)
        {
            if (amt <= 0.02f) return;
            var r = Rot(v);
            if (r.Z < 0.05f) return;
            var p = Proj(r) + off;
            ds.DrawText(text, p + Vector2.One, Color.FromArgb(A(0.6f * amt), 0, 0, 0), _cmpFormat);
            ds.DrawText(text, p, Color.FromArgb(A(amt), A(c.X), A(c.Y), A(c.Z)), _cmpFormat);
        }

        Text(ShortName(s.A.Name).ToUpperInvariant(), s.Av * 1.004f, Smooth01((_time - s.Start) / 0.4f), new Vector2(10f, -18f));
        if (s.B == null) return;

        Text(ShortName(s.B.Name).ToUpperInvariant(), s.Bv * 1.004f, Smooth01((_time - s.Start - 0.7f) / 0.4f), new Vector2(10f, -18f));

        if (s.Path != null)
        {
            string label = s.DistanceKm.ToString("N0", CultureInfo.InvariantCulture) + " km \u00B7 " + FormatFlight(s.DistanceKm / _cmpSpeed).ToLowerInvariant();
            Text(label, ArcPoint(s.Path, 0.5f), Smooth01((_time - s.Start - 0.5f) / 0.6f), new Vector2(-label.Length * 3.4f, -20f));
        }
    }

    private void DrawCompareHud(CanvasDrawingSession ds, float w, float h, float t)
    {
        var s = _cmp;
        if (s == null) return;
        EnsureTickFormat(ds);

        string[] lines;
        if (s.B == null)
        {
            lines = new[]
            {
                "CMP  " + ShortName(s.A.Name, 16).ToUpperInvariant(),
                "     SELECT 2ND COUNTRY" + (((int)(t * 2f) & 1) == 0 ? "_" : " "),
            };
        }
        else
        {
            float km = s.DistanceKm;
            lines = new[]
            {
                "CMP  " + ShortName(s.A.Name, 14).ToUpperInvariant() + " > " + ShortName(s.B.Name, 14).ToUpperInvariant(),
                "DST  " + km.ToString("N0", CultureInfo.InvariantCulture) + " KM / " + (km * 0.621371f).ToString("N0", CultureInfo.InvariantCulture) + " MI",
                "ETA  " + FormatFlight(km / _cmpSpeed) + " @ " + _cmpSpeed.ToString("0", CultureInfo.InvariantCulture) + " KM/H",
                "BRG  " + s.BearingDeg.ToString("000", CultureInfo.InvariantCulture) + "\u00B0",
            };
        }

        int maxLen = 0;
        foreach (var l in lines) maxLen = Math.Max(maxLen, l.Length);

        float pad = 8f;
        float pw = maxLen * _charW + pad * 2f + 6f, ph = lines.Length * _lineH + pad * 1.5f;
        float bottom = h - ((_showHud && _showTicker) ? _lineH + 6f + 8f + 6f : 12f);
        float px = w - 12f - pw, py = bottom - ph;
        if (px < 0f || py < 0f) return;

        float appear = Smooth01((_time - s.Start) / 0.3f);
        DrawPanel(ds, new Windows.Foundation.Rect(px, py, pw, ph), 0.8f * appear);
        var col = ToColor(Vector4.Lerp(_accent, Vector4.One, 0.35f), 0.9f * appear);
        for (int i = 0; i < lines.Length; i++)
        {
            ds.DrawText(lines[i], px + pad + 3f, py + pad * 0.75f + i * _lineH, col, _tickFormat);
        }
    }
}
