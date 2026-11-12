namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    public bool ShowDayNight
    {
        get { return (bool)GetValue(ShowDayNightProperty); }
        set { SetValue(ShowDayNightProperty, value); }
    }

    public static readonly DependencyProperty ShowDayNightProperty =
        DependencyProperty.Register(nameof(ShowDayNight), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnSettingsPropertyChanged));

    public bool IsRealTimeEnabled
    {
        get { return (bool)GetValue(IsRealTimeEnabledProperty); }
        set { SetValue(IsRealTimeEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsRealTimeEnabledProperty =
        DependencyProperty.Register(nameof(IsRealTimeEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public DateTimeOffset CustomTime
    {
        get { return (DateTimeOffset)GetValue(CustomTimeProperty); }
        set { SetValue(CustomTimeProperty, value); }
    }

    public static readonly DependencyProperty CustomTimeProperty =
        DependencyProperty.Register(nameof(CustomTime), typeof(DateTimeOffset), typeof(ParticleGlobe), new PropertyMetadata(DateTimeOffset.UtcNow, OnSettingsPropertyChanged));

    public double TimeSpeed
    {
        get { return (double)GetValue(TimeSpeedProperty); }
        set { SetValue(TimeSpeedProperty, value); }
    }

    public static readonly DependencyProperty TimeSpeedProperty =
        DependencyProperty.Register(nameof(TimeSpeed), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(0.0, OnSettingsPropertyChanged));

    public DateTimeOffset CurrentSunTime => new(DateTime.SpecifyKind(_sunTime, DateTimeKind.Utc));

    private bool _dnTarget, _dnReal;
    private float _dnTimeSpeed;
    private DateTime _customBaseUtc = DateTime.UtcNow;
    private double _simSeconds;
    private DateTime _sunTime = DateTime.UtcNow;
    private float _dnAmt;
    private Vector3 _sunDir = new(0f, 0f, 1f);

    private float[] _plight = Array.Empty<float>();
    private float[] _lightSrc;

    private void SyncSunSettings()
    {
        _dnTarget = ShowDayNight;
        _dnReal = IsRealTimeEnabled;
        _dnTimeSpeed = (float)Math.Clamp(TimeSpeed, -240.0, 240.0);

        var custom = CustomTime.UtcDateTime;
        if (custom != _customBaseUtc)
        {
            _customBaseUtc = custom;
            _simSeconds = 0;
        }
    }

    private void AdvanceSun(float dt)
    {
        float target = _dnTarget ? 1f : 0f;
        _dnAmt += Math.Clamp(target - _dnAmt, -dt / 0.8f, dt / 0.8f);
        if (_dnAmt <= 0f && !_dnTarget) return;

        if (_dnReal)
        {
            _sunTime = DateTime.UtcNow;
        }
        else
        {
            _simSeconds += dt * _dnTimeSpeed * 3600.0;
            try { _sunTime = _customBaseUtc.AddSeconds(_simSeconds); }
            catch (ArgumentOutOfRangeException) { _simSeconds = 0; _sunTime = _customBaseUtc; }
        }

        double doy = _sunTime.DayOfYear + (_sunTime.TimeOfDay.TotalHours - 12.0) / 24.0;
        double decl = -23.44 * Math.Cos(2.0 * Math.PI / 365.0 * (doy + 10.0));
        double b = 2.0 * Math.PI * (doy - 81.0) / 364.0;
        double eotMin = 9.87 * Math.Sin(2 * b) - 7.53 * Math.Cos(b) - 1.5 * Math.Sin(b);
        double sunLonDeg = -15.0 * (_sunTime.TimeOfDay.TotalHours - 12.0 + eotMin / 60.0);

        float phi = (float)(decl * Math.PI / 180.0);
        float lam = (float)(sunLonDeg * Math.PI / 180.0);
        _sunDir = new Vector3(MathF.Cos(phi) * MathF.Sin(lam), MathF.Sin(phi), MathF.Cos(phi) * MathF.Cos(lam));
    }

    private static readonly (float Lat, float Lon, float W)[] LightCities =
    {
        (35.7f,139.7f,1f),(28.6f,77.2f,1f),(31.2f,121.5f,1f),(-23.5f,-46.6f,.9f),(19.4f,-99.1f,.8f),(30.0f,31.2f,.8f),
        (19.1f,72.9f,.9f),(39.9f,116.4f,.9f),(23.8f,90.4f,.8f),(34.7f,135.5f,.7f),(40.7f,-74.0f,1f),(24.9f,67.0f,.7f),
        (-34.6f,-58.4f,.7f),(41.0f,29.0f,.8f),(6.5f,3.4f,.7f),(14.6f,121.0f,.7f),(55.8f,37.6f,.8f),(48.9f,2.35f,.8f),
        (51.5f,-0.1f,.9f),(35.7f,51.4f,.8f),(37.6f,127.0f,.8f),(-6.2f,106.8f,.8f),(34.0f,-118.2f,.8f),(41.9f,-87.6f,.6f),
        (13.8f,100.5f,.7f),(1.35f,103.8f,.6f),(-33.9f,151.2f,.6f),(-26.2f,28.0f,.6f),(-4.3f,15.3f,.5f),(-12.0f,-77.0f,.6f),
        (4.7f,-74.1f,.6f),(24.7f,46.7f,.5f),(52.5f,13.4f,.6f),(40.4f,-3.7f,.6f),(41.9f,12.5f,.5f),(43.7f,-79.4f,.5f),
        (-1.3f,36.8f,.4f),(22.6f,88.4f,.7f),(22.3f,114.2f,.7f),(10.8f,106.7f,.6f),(33.3f,44.4f,.5f),(13.1f,80.3f,.5f),
        (13.0f,77.6f,.5f),(31.5f,74.3f,.6f),(50.5f,30.5f,.4f),(-33.4f,-70.7f,.5f),(-22.9f,-43.2f,.7f),(9.0f,38.7f,.4f),
        (25.0f,121.5f,.5f),(25.8f,-80.2f,.4f),(32.8f,-96.8f,.4f),(29.8f,-95.4f,.4f),(38.9f,-77.0f,.5f),(-37.8f,145.0f,.4f),
        (33.6f,-7.6f,.4f),(36.8f,3.0f,.4f),(-8.8f,13.2f,.4f),(-6.8f,39.3f,.4f),(-33.9f,18.4f,.4f),(32.0f,34.8f,.4f),
        (52.2f,21.0f,.4f),(49.3f,-123.1f,.3f),(25.2f,55.3f,.5f),(50.1f,8.7f,.5f),(59.9f,30.3f,.4f),(45.5f,-73.6f,.3f),
    };

    private void EnsureCityLights()
    {
        var px = _px;
        if (ReferenceEquals(_lightSrc, px) && _plight.Length >= _count) return;

        int n = Math.Min(_count, Math.Min(px.Length, _pseed.Length));
        var light = new float[px.Length];
        var cv = new Vector3[LightCities.Length];
        var rad = new float[LightCities.Length];
        for (int c = 0; c < cv.Length; c++)
        {
            var (lat, lon, w) = LightCities[c];
            float phi = lat * MathF.PI / 180f, lam = lon * MathF.PI / 180f;
            cv[c] = new Vector3(MathF.Cos(phi) * MathF.Sin(lam), MathF.Sin(phi), MathF.Cos(phi) * MathF.Cos(lam));
            rad[c] = 0.028f + 0.03f * w;
        }

        var py = _py; var pz = _pz;
        for (int i = 0; i < n; i++)
        {
            if (_pkind[i] != 1) continue;
            float s = 0f;
            for (int c = 0; c < cv.Length; c++)
            {
                float dot = px[i] * cv[c].X + py[i] * cv[c].Y + pz[i] * cv[c].Z;
                if (dot < 0.9f) continue;
                float ang = MathF.Acos(Math.Min(dot, 1f)) / rad[c];
                if (ang > 2.2f) continue;
                s += LightCities[c].W * MathF.Exp(-ang * ang * 1.4f);
            }

            if (s > 0.02f) light[i] = Math.Min(1f, s * (0.45f + 0.75f * _pseed[i]));
        }

        _plight = light;
        _lightSrc = px;
    }
}
