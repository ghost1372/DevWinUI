namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const float MinZoom = 0.55f;
    private const float MaxZoom = 4f;
    private const float DefaultFlyZoom = 2f;
    private const int MaxOutlinePoints = 9000;
    private const int MinOutlinePoints = 900;

    private bool _flyOnSelect = true, _showOutline = true;
    private float _flyDuration = 1.4f, _flyZoomFixed;

    private bool _flyPending, _releasePending;
    private float _reqLat, _reqLon, _reqZoom;

    private bool _flyActive, _focused, _hasPreZoom;
    private float _flyT, _flyDur, _flyYaw0, _flyYawD, _flyPitch0, _flyPitch1, _flyZoom0, _flyZoom1, _preFlyZoom;

    private float[] _olX = Array.Empty<float>(), _olY = Array.Empty<float>(), _olZ = Array.Empty<float>(), _olSeed = Array.Empty<float>();
    private int _olCount, _olId;
    private int[] _olRef;

    public void FlyTo(double latitude, double longitude, double zoom = 0)
    {
        float z = zoom > 0 ? (float)zoom : _flyZoomFixed > 0f ? _flyZoomFixed : DefaultFlyZoom;
        RequestFly((float)(latitude * Math.PI / 180.0), (float)(longitude * Math.PI / 180.0), z);
    }

    public bool FlyToCountry(string code)
    {
        var entry = FindEntry(code);
        if (entry == null) return false;

        FlyToEntry(entry);
        return true;
    }

    public void ResetView()
    {
        lock (_inputLock) { _releasePending = true; }
    }

    private void FlyToEntry(ParticleGlobeCountryEntry entry)
    {
        float zoom = _flyZoomFixed > 0f
            ? _flyZoomFixed
            : entry.Spread > 0f
                ? Math.Clamp(0.7f / (entry.Spread * _globeScale), 1.4f, 3.4f)
                : DefaultFlyZoom;

        RequestFly(entry.LatRad, entry.LonRad, zoom);
    }

    private void RequestFly(float lat, float lon, float zoom)
    {
        lock (_inputLock)
        {
            _reqLat = lat;
            _reqLon = lon;
            _reqZoom = Math.Clamp(zoom, MinZoom, MaxZoom);
            _flyPending = true;
        }
    }

    private static float EaseInOutCubic(float k)
    {
        return k < 0.5f ? 4f * k * k * k : 1f - MathF.Pow(-2f * k + 2f, 3f) / 2f;
    }

    private bool StepFly(float dt, bool userMoved, bool flyPending, bool release, float lat, float lon, float zoom, ref float zoomT)
    {
        if (userMoved)
        {
            _flyActive = false;
            _focused = false;
        }

        if (release)
        {
            _flyActive = false;
            _focused = false;

            if (_hasPreZoom)
            {
                _hasPreZoom = false;
                zoomT = _preFlyZoom;
                lock (_inputLock) { _zoomTarget = zoomT; }
            }
        }

        if (flyPending)
        {
            if (!_hasPreZoom)
            {
                _preFlyZoom = zoomT;
                _hasPreZoom = true;
            }

            _flyYaw0 = _yaw;
            _flyYawD = WrapPi(-lon - _yaw);
            _flyPitch0 = _pitch;
            _flyPitch1 = Math.Clamp(lat - _tilt, -1.1f, 1.1f);
            _flyZoom0 = zoomT;
            _flyZoom1 = zoom;
            _flyDur = Math.Max(0.05f, _flyDuration);
            _flyT = 0f;
            _flyActive = true;
            _focused = true;
        }

        if (_flyActive)
        {
            _flyT = Math.Min(1f, _flyT + dt / _flyDur);
            float k = EaseInOutCubic(_flyT);

            _yaw = _flyYaw0 + _flyYawD * k;
            _pitch = _flyPitch0 + (_flyPitch1 - _flyPitch0) * k;
            zoomT = _flyZoom0 + (_flyZoom1 - _flyZoom0) * k;
            lock (_inputLock) { _zoomTarget = zoomT; }

            if (_flyT >= 1f) _flyActive = false;
        }

        return _focused;
    }

    private void BuildOutline(int id, int[] border)
    {
        _olId = id;
        _olRef = border;
        _olCount = 0;

        if (border == null || border.Length == 0 || _geoW <= 0) return;

        int stride = (border.Length + MaxOutlinePoints - 1) / MaxOutlinePoints;
        int n = (border.Length + stride - 1) / stride;
        int reps = n < MinOutlinePoints ? Math.Min(6, (MinOutlinePoints + n - 1) / n) : 1;
        int total = n * reps;

        var xs = new float[total];
        var ys = new float[total];
        var zs = new float[total];
        var seeds = new float[total];
        var rng = new Random(id * 7919 + 13);
        int k = 0;

        for (int r = 0; r < reps; r++)
        {
            for (int i = 0; i < border.Length; i += stride)
            {
                int pixel = border[i];
                int v = pixel / _geoW;
                int u = pixel - v * _geoW;

                float fu = u + (float)rng.NextDouble();
                float fv = v + (float)rng.NextDouble();

                float lon = (fu / GeoRasterScale - GeoOriginX) / GeoScale;
                float lat = 2f * MathF.Atan(MathF.Exp((GeoOriginY - fv / GeoRasterScale) / GeoScale)) - MathF.PI * 0.5f;
                float cl = MathF.Cos(lat);

                xs[k] = cl * MathF.Sin(lon) * 1.003f;
                ys[k] = MathF.Sin(lat) * 1.003f;
                zs[k] = cl * MathF.Cos(lon) * 1.003f;
                seeds[k] = (float)rng.NextDouble();
                k++;
            }
        }

        _olX = xs;
        _olY = ys;
        _olZ = zs;
        _olSeed = seeds;
        _olCount = k;
    }

    private void DrawOutline(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        int count = _olCount;
        if (!_showOutline || (_selFx & ParticleGlobeSelectionEffects.Outline) == 0 || count == 0 || _selDrawId == 0 || _selAmt <= 0f) return;

        var ox = _olX;
        var oy = _olY;
        var oz = _olZ;
        var seeds = _olSeed;
        if (ox.Length < count) return;

        var halo = new Vector3(_arc.X, _arc.Y, _arc.Z);
        var core = Vector3.Lerp(halo, Vector3.One, 0.45f);
        float cy = _cY, sy = _sY, cx = _cX, sx = _sX;
        float amt = _selAmt;

        for (int i = 0; i < count; i++)
        {
            float x = ox[i], y = oy[i], z = oz[i];

            var rv = Rot(new Vector3(x, y, z));
            float x1 = rv.X, y2 = rv.Y, z2 = rv.Z;
            if (z2 < 0.02f) continue;

            float k = _radius / (1f - z2 * Perspective);
            var pos = new Vector2(_center.X + x1 * k, _center.Y - y2 * k);

            float fade = MathF.Min(1f, z2 * 3f) * amt;
            float twinkle = 0.7f + 0.3f * MathF.Sin(t * 3.2f + seeds[i] * 40f);

            batch.Draw(_dot, pos, Tint(core, 0.9f * fade * twinkle), origin, 0f, new Vector2(sizeBoost * 4f), CanvasSpriteFlip.None);
            if ((i & 1) == 0)
            {
                batch.Draw(_dot, pos, Tint(halo, 0.35f * fade), origin, 0f, new Vector2(sizeBoost * 12f), CanvasSpriteFlip.None);
            }
        }
    }
}
