namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const float FlatZ = 0.5f;            // view-space depth of the flat map, keeps all "facing the viewer" checks valid
    private const float MercClampLat = 85f * MathF.PI / 180f;
    private const float MercVisibleN = 0.7755f;  // |mercator y| / pi at 80 degrees; particles beyond dissolve while flattening

    public bool IsFlatMapEnabled
    {
        get { return (bool)GetValue(IsFlatMapEnabledProperty); }
        set { SetValue(IsFlatMapEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsFlatMapEnabledProperty =
        DependencyProperty.Register(nameof(IsFlatMapEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnSettingsPropertyChanged));

    public ParticleGlobeFlatMapMode FlatMapMode
    {
        get { return (ParticleGlobeFlatMapMode)GetValue(FlatMapModeProperty); }
        set { SetValue(FlatMapModeProperty, value); }
    }

    public static readonly DependencyProperty FlatMapModeProperty =
        DependencyProperty.Register(nameof(FlatMapMode), typeof(ParticleGlobeFlatMapMode), typeof(ParticleGlobe), new PropertyMetadata(ParticleGlobeFlatMapMode.Scroll, OnSettingsPropertyChanged));

    public double MorphDuration
    {
        get { return (double)GetValue(MorphDurationProperty); }
        set { SetValue(MorphDurationProperty, value); }
    }

    public static readonly DependencyProperty MorphDurationProperty =
        DependencyProperty.Register(nameof(MorphDuration), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(2.2, OnSettingsPropertyChanged));

    private bool _flatTarget;
    private ParticleGlobeFlatMapMode _flatMode;
    private float _morphDuration = 2.2f;
    private float _morphT, _morphE, _mapHalfW = 1.5f;

    private static float MercatorN(float lat)
    {
        lat = Math.Clamp(lat, -MercClampLat, MercClampLat);
        return MathF.Log(MathF.Tan(MathF.PI * 0.25f + lat * 0.5f)) / MathF.PI;
    }

    private float FlatScale => 1f - FlatZ * Perspective;

    // Map position shown at the screen centre: Mercator y and longitude (both normalized). Dragging pans them once the map is larger than the view.
    private float _flatCenterN, _flatPanX;

    private void AdvanceFlatPan(float dt, float dragPixelsX, float dragPixelsY)
    {
        if (!_flatTarget || _flatMode == ParticleGlobeFlatMapMode.Fixed)
        {
            float decay = MathF.Exp(-dt * 3f);
            _flatCenterN *= decay;
            _flatPanX *= decay;
            if (MathF.Abs(_flatCenterN) < 1e-4f) _flatCenterN = 0f;
            if (MathF.Abs(_flatPanX) < 1e-4f) _flatPanX = 0f;
            return;
        }

        float pxPerN = _radius * _mapHalfW;
        if (pxPerN <= 1f) return;

        float limit = MathF.Max(0f, MercVisibleN - _center.Y / pxPerN);
        _flatCenterN = Math.Clamp(_flatCenterN + dragPixelsY / pxPerN, -limit, limit);

        if (_flatMode == ParticleGlobeFlatMapMode.FixedZoom)
        {
            float limitX = MathF.Max(0f, 1f - _center.X / pxPerN);
            _flatPanX = Math.Clamp(_flatPanX - dragPixelsX / pxPerN, -limitX, limitX);
        }
        else
        {
            _flatPanX = 0f;
        }
    }

    private void AdvanceMorph(float dt)
    {
        float target = _flatTarget ? 1f : 0f;
        float step = dt / MathF.Max(0.05f, _morphDuration);
        _morphT += Math.Clamp(target - _morphT, -step, step);

        float t = _morphT;
        _morphE = t * t * t * (t * (t * 6f - 15f) + 10f);
        AdvanceSun(dt);
        AdvanceBorders(dt);
        AdvanceIntro(dt);
        AdvanceFlow(dt);
        AdvanceEvents(dt);
    }

    // View-space position of a world point on the flat map; the map is centred on the current yaw.
    private Vector3 FlatView(Vector3 p)
    {
        float len = p.Length();
        if (len < 1e-5f) return new Vector3(0f, 0f, FlatZ);

        float lat = MathF.Asin(Math.Clamp(p.Y / len, -1f, 1f));
        float lonN = WrapPi(MathF.Atan2(p.X, p.Z) + _yaw) / MathF.PI;
        float s = FlatScale;
        return new Vector3((lonN - _flatPanX) * _mapHalfW * s, ((MercatorN(lat) - _flatCenterN) * _mapHalfW + (len - 1f)) * s, FlatZ);
    }

    // Map centred on ~11E so the wrap-around seam falls in the Pacific instead of slicing Asia or Europe.
    private const float FlatCenterYaw = -11f * MathF.PI / 180f;

    private void DrawFlatRippleRings(Microsoft.Graphics.Canvas.CanvasDrawingSession ds)
    {
        float e = _morphE;
        if (e <= 0.01f) return;

        float halfWpx = _radius * _mapHalfW;
        float pxPerRad = halfWpx / MathF.PI;
        var rect = new Windows.Foundation.Rect(_center.X - (1f + _flatPanX) * halfWpx, _center.Y - (MercVisibleN - _flatCenterN) * halfWpx, 2f * halfWpx, 2f * MercVisibleN * halfWpx);
        using var layer = ds.CreateLayer(1f, rect);

        for (int k = 0; k < _ripFrameCount; k++)
        {
            var f = _ripFrame[k];
            if (f.Distance <= 0.01f) continue;

            float cx = _center.X + (WrapPi(f.Lon + _yaw) / MathF.PI - _flatPanX) * halfWpx;
            float cy = _center.Y - (f.Merc - _flatCenterN * MathF.PI) * pxPerRad;
            float rad = f.Distance * pxPerRad;

            for (int off = 0; off <= 0; off++)
            {
                var c = new Vector2(cx + off * 2f * halfWpx, cy);
                ds.DrawCircle(c, rad, ToColor(_accent, f.Amp * 0.25f * e), 4f);
                ds.DrawCircle(c, rad, ToColor(_accent, f.Amp * 0.9f * e), 1.4f);
            }
        }
    }

    private bool TryUnprojectFlat(Vector2 point, out Vector3 world)
    {
        world = default;
        float lonN = (point.X - _center.X) / _radius / _mapHalfW + _flatPanX;
        float mercN = -(point.Y - _center.Y) / _radius / _mapHalfW + _flatCenterN;
        if (MathF.Abs(lonN) > 1f || MathF.Abs(mercN) > MercVisibleN) return false;

        float lat = MathF.Atan(MathF.Sinh(mercN * MathF.PI));
        float lon = lonN * MathF.PI - _yaw;
        float cl = MathF.Cos(lat);
        world = new Vector3(cl * MathF.Sin(lon), MathF.Sin(lat), cl * MathF.Cos(lon));
        return true;
    }

    // Screen-space segment that wraps across the map seam shouldn't be drawn.
    private bool Jumped(Vector2 a, Vector2 b) => _morphE > 0.001f && Vector2.DistanceSquared(a, b) > _radius * _radius * 0.5f;

}
