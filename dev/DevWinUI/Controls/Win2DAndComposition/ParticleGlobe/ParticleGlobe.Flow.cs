namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    #region Properties

    public bool ShowFlow
    {
        get { return (bool)GetValue(ShowFlowProperty); }
        set { SetValue(ShowFlowProperty, value); }
    }

    public static readonly DependencyProperty ShowFlowProperty =
        DependencyProperty.Register(nameof(ShowFlow), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnSettingsPropertyChanged));

    public int FlowParticleCount
    {
        get { return (int)GetValue(FlowParticleCountProperty); }
        set { SetValue(FlowParticleCountProperty, value); }
    }

    public static readonly DependencyProperty FlowParticleCountProperty =
        DependencyProperty.Register(nameof(FlowParticleCount), typeof(int), typeof(ParticleGlobe), new PropertyMetadata(2500, OnSettingsPropertyChanged));

    public double FlowSpeed
    {
        get { return (double)GetValue(FlowSpeedProperty); }
        set { SetValue(FlowSpeedProperty, value); }
    }

    public static readonly DependencyProperty FlowSpeedProperty =
        DependencyProperty.Register(nameof(FlowSpeed), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(1.0, OnSettingsPropertyChanged));

    public int FlowTrailLength
    {
        get { return (int)GetValue(FlowTrailLengthProperty); }
        set { SetValue(FlowTrailLengthProperty, value); }
    }

    public static readonly DependencyProperty FlowTrailLengthProperty =
        DependencyProperty.Register(nameof(FlowTrailLength), typeof(int), typeof(ParticleGlobe), new PropertyMetadata(10, OnSettingsPropertyChanged));

    #endregion

    #region Data API

    public void SetFlowField(Func<double, double, (double U, double V)> sampler, int columns = 180, int rows = 90)
    {
        ArgumentNullException.ThrowIfNull(sampler);
        columns = Math.Clamp(columns, 4, 1440);
        rows = Math.Clamp(rows, 3, 720);

        var u = new float[columns * rows];
        var v = new float[columns * rows];
        for (int r = 0; r < rows; r++)
        {
            double lat = 90.0 - 180.0 * r / (rows - 1);
            for (int c = 0; c < columns; c++)
            {
                double lon = -180.0 + 360.0 * c / columns;
                var (uu, vv) = sampler(lat, lon);
                u[r * columns + c] = double.IsFinite(uu) ? (float)uu : 0f;
                v[r * columns + c] = double.IsFinite(vv) ? (float)vv : 0f;
            }
        }

        _flowGrid = new ParticleGlobeFlowGrid(columns, rows, u, v);
    }

    public void SetFlowGrid(int columns, int rows, float[] u, float[] v)
    {
        ArgumentNullException.ThrowIfNull(u);
        ArgumentNullException.ThrowIfNull(v);
        if (columns < 2 || rows < 2) throw new ArgumentOutOfRangeException(nameof(columns), "The grid needs at least 2 columns and 2 rows.");
        if (u.Length < columns * rows || v.Length < columns * rows) throw new ArgumentException("u and v must hold columns * rows values.");

        _flowGrid = new ParticleGlobeFlowGrid(columns, rows, (float[])u.Clone(), (float[])v.Clone());
    }

    public void ClearFlowField() => _flowGrid = null;

    #endregion

    private const int FlowMaxTrail = 16;
    private const float FlowTrailStep = 0.05f;   // seconds between trail samples
    private const float FlowTimeScale = 30000f;  // m/s of data -> simulated metres per real second

    private volatile ParticleGlobeFlowGrid _flowGrid;
    private ParticleGlobeFlowGrid _demoFlow;

    private bool _showFlow;
    private int _flowWanted = 2500, _flowTrail = 10;
    private float _flowSpeed = 1f, _flowAmt;

    private float[] _flLat = Array.Empty<float>(), _flLon = Array.Empty<float>(), _flAge = Array.Empty<float>(), _flLife = Array.Empty<float>(), _flMag = Array.Empty<float>();
    private Vector3[] _flHist = Array.Empty<Vector3>();
    private int _flHead;
    private float _flStep;

    private void SyncFlowSettings()
    {
        _showFlow = ShowFlow;
        _flowWanted = Math.Clamp(FlowParticleCount, 100, 20000);
        _flowSpeed = (float)Math.Clamp(FlowSpeed, 0.0, 20.0);
        _flowTrail = Math.Clamp(FlowTrailLength, 3, FlowMaxTrail);
    }

    private static ParticleGlobeFlowGrid BuildDemoFlow()
    {
        const int W = 180, H = 91;
        var u = new float[W * H];
        var v = new float[W * H];
        for (int r = 0; r < H; r++)
        {
            float lat = (90f - 2f * r) * MathF.PI / 180f;
            for (int c = 0; c < W; c++)
            {
                float lon = (-180f + 2f * c) * MathF.PI / 180f;
                float cl = MathF.Cos(lat);
                u[r * W + c] = -10f * MathF.Cos(3f * lat) + 3f * MathF.Sin(4f * lat + 3f * lon);
                v[r * W + c] = (6f * MathF.Sin(2f * lon + 3f * lat) + 3f * MathF.Sin(5f * lon - 2f * lat)) * cl;
            }
        }

        return new ParticleGlobeFlowGrid(W, H, u, v);
    }

    private static (float U, float V) SampleFlow(ParticleGlobeFlowGrid g, float latDeg, float lonDeg)
    {
        float fy = (90f - latDeg) / 180f * (g.H - 1);
        fy = Math.Clamp(fy, 0f, g.H - 1.001f);
        float fx = (lonDeg + 180f) / 360f * g.W;
        fx -= MathF.Floor(fx / g.W) * g.W;

        int y0 = (int)fy, x0 = (int)fx;
        float ty = fy - y0, tx = fx - x0;
        int x1 = x0 + 1 >= g.W ? 0 : x0 + 1;
        int y1 = Math.Min(y0 + 1, g.H - 1);

        float Bi(float[] a)
        {
            float top = a[y0 * g.W + x0] + (a[y0 * g.W + x1] - a[y0 * g.W + x0]) * tx;
            float bot = a[y1 * g.W + x0] + (a[y1 * g.W + x1] - a[y1 * g.W + x0]) * tx;
            return top + (bot - top) * ty;
        }

        return (Bi(g.U), Bi(g.V));
    }

    private static Vector3 LatLonToVector(float lat, float lon)
    {
        float cl = MathF.Cos(lat);
        return new Vector3(cl * MathF.Sin(lon), MathF.Sin(lat), cl * MathF.Cos(lon)) * 1.012f;
    }

    private void RespawnFlow(int i, bool randomAge)
    {
        var rng = Random.Shared;
        float lat = MathF.Asin(rng.NextSingle() * 2f - 1f);
        float lon = (rng.NextSingle() * 2f - 1f) * MathF.PI;
        _flLat[i] = lat;
        _flLon[i] = lon;
        _flLife[i] = 2.5f + rng.NextSingle() * 4f;
        _flAge[i] = randomAge ? rng.NextSingle() * _flLife[i] : 0f;

        var p = LatLonToVector(lat, lon);
        int b = i * FlowMaxTrail;
        for (int k = 0; k < FlowMaxTrail; k++) _flHist[b + k] = p;
    }

    private void AdvanceFlow(float dt)
    {
        float target = _showFlow ? 1f : 0f;
        _flowAmt += Math.Clamp(target - _flowAmt, -dt / 0.7f, dt / 0.7f);
        if (_flowAmt <= 0f && !_showFlow) return;

        if (_flLat.Length != _flowWanted)
        {
            int n = _flowWanted;
            _flLat = new float[n]; _flLon = new float[n]; _flAge = new float[n]; _flLife = new float[n]; _flMag = new float[n];
            _flHist = new Vector3[n * FlowMaxTrail];
            for (int i = 0; i < n; i++) RespawnFlow(i, true);
        }

        var grid = _flowGrid ?? (_demoFlow ??= BuildDemoFlow());
        float k = FlowTimeScale * _flowSpeed / 6.371e6f * dt;
        float invMax = 1f / grid.Max;
        int n2 = _flLat.Length;

        for (int i = 0; i < n2; i++)
        {
            float lat = _flLat[i], lon = _flLon[i];
            var (u, v) = SampleFlow(grid, lat * 180f / MathF.PI, lon * 180f / MathF.PI);
            _flMag[i] = Math.Min(1f, MathF.Sqrt(u * u + v * v) * invMax);

            lat += v * k;
            lon += u * k / MathF.Max(MathF.Cos(lat), 0.15f);
            if (lon > MathF.PI) lon -= MathF.Tau; else if (lon < -MathF.PI) lon += MathF.Tau;

            _flAge[i] += dt;
            if (_flAge[i] >= _flLife[i] || MathF.Abs(lat) > 1.45f) { RespawnFlow(i, false); continue; }

            _flLat[i] = lat;
            _flLon[i] = lon;
        }

        _flStep += dt;
        if (_flStep >= FlowTrailStep)
        {
            _flStep = 0f;
            _flHead = (_flHead + 1) % FlowMaxTrail;
            for (int i = 0; i < n2; i++) _flHist[i * FlowMaxTrail + _flHead] = LatLonToVector(_flLat[i], _flLon[i]);
        }
    }

    private void DrawFlow(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        float amt = _flowAmt;
        if (amt <= 0.003f || _flLat.Length == 0) return;

        int n = _flLat.Length, trail = _flowTrail, head = _flHead;
        float morphE = _morphE;
        var slow = Vector3.Lerp(new Vector3(_atmos.X, _atmos.Y, _atmos.Z), Vector3.One, 0.2f);
        var fast = Vector3.Lerp(new Vector3(_accent.X, _accent.Y, _accent.Z), Vector3.One, 0.55f);

        for (int i = 0; i < n; i++)
        {
            float life = _flAge[i] / _flLife[i];
            float fade = MathF.Sin(MathF.PI * Math.Clamp(life, 0f, 1f)) * amt;
            if (fade <= 0.02f) continue;

            var rgb = Vector3.Lerp(slow, fast, _flMag[i]);
            int b = i * FlowMaxTrail;

            for (int j = 0; j < trail; j++)
            {
                var p = _flHist[b + (head - j + FlowMaxTrail) % FlowMaxTrail];
                float vis = fade;
                if (morphE > 0f && MathF.Abs(p.Y) > 0.985f) vis *= 1f - morphE;
                if (vis <= 0.02f) continue;

                var rv = Rot(p);
                if (rv.Z < 0.02f) continue;

                float f = 1f - j / (float)trail;
                float k = _radius / (1f - rv.Z * Perspective);
                var pos = new Vector2(_center.X + rv.X * k, _center.Y - rv.Y * k);
                if (OffScreen(pos, 8f * sizeBoost)) continue;
                float a = (0.15f + 0.6f * _flMag[i]) * f * f * vis * MathF.Min(1f, rv.Z * 3f);
                batch.Draw(_dot, pos, Tint(rgb, a), origin, 0f, new Vector2(sizeBoost * (1.6f + 1.8f * f)), CanvasSpriteFlip.None);
            }
        }
    }
}
