using System.Diagnostics;
using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = "PART_Canvas", Type = typeof(CanvasAnimatedControl))]
public sealed partial class ParticleGlobe : Control
{
    private const string PartCanvas = "PART_Canvas";
    private const float Perspective = 0.10f;

    private CanvasAnimatedControl _canvas;
    private CanvasRenderTarget _dot;
    private CanvasRadialGradientBrush _glowBrush;
    private CanvasRadialGradientBrush _discBrush;
    private int _builtVersion = -1;

    // ---- settings cache (written on UI thread, read on render thread) ----
    private volatile bool _needsRebuild = true;
    private int _brushVersion;
    private int _particleCount = 26000, _orbitRings = 3;
    private float _particleSize = 1f, _rotationSpeed = 0.12f, _tilt = 0.31f, _globeScale = 0.74f;
    private float _landThreshold = 0.52f, _arcFrequency = 1.6f;
    private bool _autoRotate = true, _interactive = true;
    private bool _showStars = true, _showGrid = true, _showArcs = true, _showCities = true, _showOrbits = true, _showHud = true;
    private Uri _maskUri;

    private static readonly Vector4 DefLand = new(0.30f, 0.95f, 1.00f, 0.90f);
    private static readonly Vector4 DefOcean = new(0.10f, 0.45f, 0.85f, 0.55f);
    private static readonly Vector4 DefAtmos = new(0.20f, 0.60f, 1.00f, 0.60f);
    private static readonly Vector4 DefGrid = new(0.30f, 0.70f, 1.00f, 0.22f);
    private static readonly Vector4 DefArc = new(1.00f, 0.55f, 0.20f, 0.95f);
    private static readonly Vector4 DefAccent = new(0.55f, 1.00f, 0.85f, 1.00f);
    private static readonly Vector4 DefStar = new(0.80f, 0.90f, 1.00f, 0.90f);

    private Vector4 _land = DefLand, _ocean = DefOcean, _atmos = DefAtmos, _grid = DefGrid, _arc = DefArc, _accent = DefAccent, _star = DefStar;

    private byte[] _mask;
    private int _maskW, _maskH;

    private float[] _px = Array.Empty<float>(), _py = Array.Empty<float>(), _pz = Array.Empty<float>();
    private float[] _plon = Array.Empty<float>(), _pseed = Array.Empty<float>(), _pmy = Array.Empty<float>();
    private byte[] _pkind = Array.Empty<byte>(); // 0 ocean, 1 land, 2 haze
    private ushort[] _pcountry = Array.Empty<ushort>();
    private float[] _pphS = Array.Empty<float>(), _pphC = Array.Empty<float>();
    private int _count;

    private float[] _stX = Array.Empty<float>(), _stY = Array.Empty<float>(), _stD = Array.Empty<float>(), _stSeed = Array.Empty<float>();

    private ParticleGlobeOrbit[] _orbits = Array.Empty<ParticleGlobeOrbit>();
    private readonly List<ParticleGlobeArcState> _arcs = new();
    private readonly List<Vector3> _heads = new();
    private readonly Random _rng = new();
    private float _arcTimer;

    private float _time, _yaw, _pitch, _scanLon, _zoom = 1f, _zoomTarget = 1f;
    private float _cY, _sY, _cX, _sX, _radius;
    private Vector2 _center;
    private float _viewW, _viewH;

    private readonly object _inputLock = new();
    private bool _dragging;
    private float _pendingYaw, _pendingPitch, _velYaw;
    private Windows.Foundation.Point _lastPt;
    private long _lastMoveTs;

    public ParticleGlobe()
    {
        DefaultStyleKey = typeof(ParticleGlobe);
        Markers = ParticleGlobeMarker.CreateDefaults();

        Loaded += (s, e) =>
        {
            SyncSettings();
            HookHostVisibility();
            UpdatePaused();
        };
        Unloaded += (s, e) =>
        {
            UnhookHostVisibility();
            if (_canvas != null) _canvas.Paused = true;
        };
    }

    // Pauses rendering while the host window is hidden or minimized; uses XamlRoot so no Window reference is needed.
    private XamlRoot _hostRoot;

    private void HookHostVisibility()
    {
        var root = XamlRoot;
        if (ReferenceEquals(root, _hostRoot)) return;

        UnhookHostVisibility();
        _hostRoot = root;
        if (root != null) root.Changed += OnHostRootChanged;
    }

    private void UnhookHostVisibility()
    {
        if (_hostRoot != null) _hostRoot.Changed -= OnHostRootChanged;
        _hostRoot = null;
    }

    private void OnHostRootChanged(XamlRoot sender, XamlRootChangedEventArgs args) => UpdatePaused();

    private void UpdatePaused()
    {
        if (_canvas == null) return;
        _canvas.Paused = _hostRoot != null && !_hostRoot.IsHostVisible;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_canvas != null)
        {
            _canvas.CreateResources -= OnCreateResources;
            _canvas.Update -= OnUpdate;
            _canvas.Draw -= OnDraw;
        }

        _canvas = GetTemplateChild(PartCanvas) as CanvasAnimatedControl;

        if (_canvas != null)
        {
            _canvas.CreateResources += OnCreateResources;
            _canvas.Update += OnUpdate;
            _canvas.Draw += OnDraw;
        }

        SyncSettings();
    }

    #region Settings

    private void SyncSettings()
    {
        _particleCount = Math.Clamp(ParticleCount, 0, 150000);
        _particleSize = (float)Math.Max(0.1, ParticleSize);
        _rotationSpeed = (float)RotationSpeed;
        _autoRotate = IsAutoRotateEnabled;
        _interactive = IsInteractive;
        _tilt = (float)(TiltAngle * Math.PI / 180.0);
        _globeScale = (float)Math.Clamp(GlobeScale, 0.2, 1.2);
        _landThreshold = (float)LandThreshold;
        _arcFrequency = (float)Math.Clamp(ArcFrequency, 0.0, 20.0);
        _orbitRings = Math.Clamp(OrbitRingCount, 0, 8);

        _showStars = ShowStars;
        _showGrid = ShowGrid;
        _showArcs = ShowArcs;
        _showCities = ShowCities;
        _showMarkerLabels = ShowMarkerLabels;
        _flatTarget = IsFlatMapEnabled;
        _flatMode = FlatMapMode;
        SyncQualitySettings();
        SyncSunSettings();
        SyncBorderSettings();
        SyncIntroSettings();
        SyncHudFxSettings();
        SyncSelectionSettings();
        SyncFlowSettings();
        SyncEventSettings();
        SyncCompareSettings();
        SyncTourSettings();
        _morphDuration = (float)Math.Clamp(MorphDuration, 0.1, 10.0);
        _showOrbits = ShowOrbits;
        _showHud = ShowHud;
        _countryHover = IsCountryHoverEnabled;
        _showTooltip = ShowCountryTooltip;
        _flyOnSelect = IsFlyToOnSelectEnabled;
        _showOutline = ShowCountryOutline;
        _rippleEnabled = IsClickRippleEnabled;
        _showReticle = ShowCursorReticle;
        _forceField = IsCursorForceFieldEnabled;
        _clickToConnect = IsClickToConnectEnabled;
        _showBars = ShowCountryBars;
        _barMaxHeight = (float)Math.Clamp(BarMaxHeight, 0.05, 1.0);
        _flyDuration = (float)Math.Clamp(FlyToDuration, 0.05, 10.0);
        _flyZoomFixed = (float)Math.Clamp(FlyToZoom, 0.0, MaxZoom);

        var pal = GetPalette(Theme);
        bool themed = Theme != ParticleGlobeTheme.Default; // the default style sets every brush, so a preset must take precedence
        _land = ToVector(themed ? null : LandBrush, pal.Land);
        _ocean = ToVector(themed ? null : OceanBrush, pal.Ocean);
        _atmos = ToVector(themed ? null : AtmosphereBrush, pal.Atmos);
        _grid = ToVector(themed ? null : GridBrush, pal.Grid);
        _arc = ToVector(themed ? null : ArcBrush, pal.Arc);
        _accent = ToVector(themed ? null : AccentBrush, pal.Accent);
        _star = ToVector(themed ? null : StarBrush, pal.Star);

        _brushVersion++;
    }

    private static Vector4 ToVector(Brush brush, Vector4 fallback)
    {
        if (brush is SolidColorBrush solid)
        {
            Color c = solid.Color;
            return new Vector4(c.R / 255f, c.G / 255f, c.B / 255f, c.A / 255f * (float)solid.Opacity);
        }
        return fallback;
    }

    private static Color ToColor(Vector4 v, float alphaMul = 1f)
    {
        static byte B(float f) => (byte)(Math.Clamp(f, 0f, 1f) * 255f + 0.5f);
        return Color.FromArgb(B(v.W * alphaMul), B(v.X), B(v.Y), B(v.Z));
    }

    #endregion

    #region Land mask

    private async void ReloadMask()
    {
        var uri = _maskUri;
        if (uri == null)
        {
            _mask = null;
            _needsRebuild = true;
            return;
        }

        var device = _canvas?.Device;
        if (device == null) return; // OnCreateResources will load it

        await LoadMaskAsync(device, uri);
    }

    private async Task LoadMaskAsync(ICanvasResourceCreator rc, Uri uri)
    {
        try
        {
            using var bmp = await CanvasBitmap.LoadAsync(rc, uri);
            int w = (int)bmp.SizeInPixels.Width;
            int h = (int)bmp.SizeInPixels.Height;
            byte[] px = bmp.GetPixelBytes(); // BGRA

            var lum = new byte[w * h];
            for (int i = 0; i < lum.Length; i++)
            {
                int o = i * 4;
                float b = px[o], g = px[o + 1], r = px[o + 2], a = px[o + 3];
                lum[i] = (byte)MathF.Round((0.299f * r + 0.587f * g + 0.114f * b) * (a / 255f));
            }

            _mask = lum;
            _maskW = w;
            _maskH = h;
        }
        catch
        {
            _mask = null; // fall back to procedural continents
        }
        _needsRebuild = true;
    }

    #endregion

    #region Resources

    private void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        _dot?.Dispose();
        _dot = CreateDot(sender);

        _glowBrush?.Dispose();
        _discBrush?.Dispose();
        _glowBrush = null;
        _discBrush = null;
        _scanBrush?.Dispose();
        _scanBrush = null;
        _builtVersion = -1;
        ResetTooltipResources();

        var uri = _maskUri;
        if (uri != null)
        {
            args.TrackAsyncAction(LoadMaskAsync(sender.Device, uri).AsAsyncAction());
        }

        var geoSource = _geoSource;
        args.TrackAsyncAction(LoadGeoMaskAsync(sender.Device, geoSource).AsAsyncAction());
    }

    private static CanvasRenderTarget CreateDot(ICanvasResourceCreator rc)
    {
        var rt = new CanvasRenderTarget(rc, 64, 64, 96);
        using (var ds = rt.CreateDrawingSession())
        {
            ds.Clear(Colors.Transparent);
            var stops = new[]
            {
                new CanvasGradientStop { Position = 0.00f, Color = Color.FromArgb(255, 255, 255, 255) },
                new CanvasGradientStop { Position = 0.22f, Color = Color.FromArgb(210, 255, 255, 255) },
                new CanvasGradientStop { Position = 0.55f, Color = Color.FromArgb(55, 255, 255, 255) },
                new CanvasGradientStop { Position = 1.00f, Color = Color.FromArgb(0, 255, 255, 255) },
            };
            using var brush = new CanvasRadialGradientBrush(rc, stops)
            {
                Center = new Vector2(32, 32),
                RadiusX = 32,
                RadiusY = 32
            };
            ds.FillRectangle(0, 0, 64, 64, brush);
        }
        return rt;
    }

    private void EnsureBrushes(ICanvasResourceCreator rc)
    {
        if (_glowBrush != null && _discBrush != null && _builtVersion == _brushVersion) return;

        _glowBrush?.Dispose();
        _discBrush?.Dispose();

        // Atmosphere: transparent core, thin bright rim just outside the surface, soft fall-off.
        _glowBrush = new CanvasRadialGradientBrush(rc, new[]
        {
            new CanvasGradientStop { Position = 0.00f, Color = ToColor(_atmos, 0f) },
            new CanvasGradientStop { Position = 0.60f, Color = ToColor(_atmos, 0f) },
            new CanvasGradientStop { Position = 0.69f, Color = ToColor(_atmos, 0.35f) },
            new CanvasGradientStop { Position = 0.72f, Color = ToColor(_atmos, 0.80f) },
            new CanvasGradientStop { Position = 0.80f, Color = ToColor(_atmos, 0.22f) },
            new CanvasGradientStop { Position = 1.00f, Color = ToColor(_atmos, 0f) },
        });

        // Ocean body: very subtle tinted disc, lit from the top-left.
        _discBrush = new CanvasRadialGradientBrush(rc, new[]
        {
            new CanvasGradientStop { Position = 0.00f, Color = ToColor(_ocean, 0.30f) },
            new CanvasGradientStop { Position = 0.70f, Color = ToColor(_ocean, 0.10f) },
            new CanvasGradientStop { Position = 1.00f, Color = ToColor(_ocean, 0.02f) },
        });

        _builtVersion = _brushVersion;
    }

    #endregion

    #region Input

    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);

        var point = e.GetCurrentPoint(this);
        _pressPt = point.Position;
        _pressMoved = false;
        _pressed = point.Properties.IsLeftButtonPressed;

        if (!_interactive) return;

        lock (_inputLock)
        {
            _dragging = true;
            _velYaw = 0f;
        }
        _lastPt = point.Position;
        _lastMoveTs = Stopwatch.GetTimestamp();
        CapturePointer(e.Pointer);
        e.Handled = true;
    }

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);
        var p = e.GetCurrentPoint(this).Position;
        lock (_inputLock)
        {
            _pointerPos = new Vector2((float)p.X, (float)p.Y);
            _pointerInside = true;
        }
    }

    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        lock (_inputLock) { _pointerInside = false; }
    }

    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        base.OnPointerMoved(e);

        var p = e.GetCurrentPoint(this).Position;
        lock (_inputLock)
        {
            _pointerPos = new Vector2((float)p.X, (float)p.Y);
            _pointerInside = true;
        }

        if (_pressed)
        {
            float mx = (float)(p.X - _pressPt.X);
            float my = (float)(p.Y - _pressPt.Y);
            if (mx * mx + my * my > 36f) _pressMoved = true;
        }

        if (!_dragging) return;

        float dx = (float)(p.X - _lastPt.X);
        float dy = (float)(p.Y - _lastPt.Y);
        _lastPt = p;

        long now = Stopwatch.GetTimestamp();
        float dt = Math.Max((float)(now - _lastMoveTs) / Stopwatch.Frequency, 0.001f);
        _lastMoveTs = now;

        lock (_inputLock)
        {
            _pendingYaw += dx * 0.006f;
            _pendingPitch += dy * 0.006f;
            _velYaw = Math.Clamp(0.5f * _velYaw + 0.5f * (dx * 0.006f / dt), -6f, 6f);
        }
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_pressed)
        {
            _pressed = false;
            if (!_pressMoved)
            {
                var p = e.GetCurrentPoint(this).Position;
                lock (_inputLock)
                {
                    _clickPos = new Vector2((float)p.X, (float)p.Y);
                    _clickPending = true;
                }
            }
        }

        EndDrag(e.Pointer);
    }

    protected override void OnPointerCanceled(PointerRoutedEventArgs e)
    {
        base.OnPointerCanceled(e);
        _pressed = false;
        EndDrag(e.Pointer);
    }

    protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _pressed = false;
        EndDrag(null);
    }

    private void EndDrag(Pointer pointer)
    {
        lock (_inputLock) { _dragging = false; }
        if (pointer != null) ReleasePointerCapture(pointer);
    }

    protected override void OnPointerWheelChanged(PointerRoutedEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        if (!_interactive) return;

        int delta = e.GetCurrentPoint(this).Properties.MouseWheelDelta;
        lock (_inputLock)
        {
            _zoomTarget = Math.Clamp(_zoomTarget * (1f + delta * 0.0006f), MinZoom, MaxZoom);
        }
        e.Handled = true;
    }

    #endregion

    #region Build data

    private static float Hash(int x, int y, int z)    {
        unchecked
        {
            int h = x * 374761393 + y * 668265263 + z * 1274126177;
            h = (h ^ (h >> 13)) * 1274126177;
            h ^= h >> 16;
            return (h & 0x7fffffff) / 2147483647f;
        }
    }

    private static float Noise3(float x, float y, float z)
    {
        int xi = (int)MathF.Floor(x), yi = (int)MathF.Floor(y), zi = (int)MathF.Floor(z);
        float fx = x - xi, fy = y - yi, fz = z - zi;
        float u = fx * fx * (3f - 2f * fx), v = fy * fy * (3f - 2f * fy), w = fz * fz * (3f - 2f * fz);

        float c000 = Hash(xi, yi, zi), c100 = Hash(xi + 1, yi, zi);
        float c010 = Hash(xi, yi + 1, zi), c110 = Hash(xi + 1, yi + 1, zi);
        float c001 = Hash(xi, yi, zi + 1), c101 = Hash(xi + 1, yi, zi + 1);
        float c011 = Hash(xi, yi + 1, zi + 1), c111 = Hash(xi + 1, yi + 1, zi + 1);

        float x00 = c000 + (c100 - c000) * u, x10 = c010 + (c110 - c010) * u;
        float x01 = c001 + (c101 - c001) * u, x11 = c011 + (c111 - c011) * u;
        float y0 = x00 + (x10 - x00) * v, y1 = x01 + (x11 - x01) * v;
        return y0 + (y1 - y0) * w;
    }

    private static float Fbm(float x, float y, float z)
    {
        float sum = 0f, amp = 0.5f, f = 1f;
        for (int i = 0; i < 5; i++)
        {
            sum += amp * Noise3(x * f, y * f, z * f);
            f *= 2.03f;
            amp *= 0.5f;
        }
        return sum;
    }

    private bool IsLand(float x, float y, float z)
    {
        var mask = _mask;
        if (mask != null)
        {
            float lon = MathF.Atan2(x, z);
            float lat = MathF.Asin(Math.Clamp(y, -1f, 1f));
            int u = Math.Clamp((int)((lon / MathF.Tau + 0.5f) * _maskW), 0, _maskW - 1);
            int v = Math.Clamp((int)((0.5f - lat / MathF.PI) * _maskH), 0, _maskH - 1);
            return mask[v * _maskW + u] > _landThreshold * 255f;
        }
        var geo = _geoMask;
        if (geo != null) return IsGeoLand(geo, x, y, z);

        return Fbm(x * 1.9f + 7.3f, y * 1.9f + 1.7f, z * 1.9f + 4.1f) > _landThreshold;
    }

    private static float Rnd(Random r) => (float)(r.NextDouble() * 2.0 - 1.0);

    private void Rebuild()
    {
        _needsRebuild = false;

        var rng = new Random(20260101);
        int candidates = (int)(_particleCount * 1.6f);
        int cap = candidates + candidates / 8 + 16;

        var px = new float[cap];
        var py = new float[cap];
        var pz = new float[cap];
        var plon = new float[cap];
        var pseed = new float[cap];
        var pkind = new byte[cap];
        var pcountry = new ushort[cap];
        var pmy = new float[cap];
        var geoIds = _mask == null ? _geoIds : null;
        int n = 0;

        void Add(Vector3 v, float radius, byte kind)
        {
            px[n] = v.X * radius;
            py[n] = v.Y * radius;
            pz[n] = v.Z * radius;
            plon[n] = MathF.Atan2(v.X, v.Z);
            pmy[n] = MercatorN(MathF.Asin(Math.Clamp(v.Y, -1f, 1f)));
            pseed[n] = (float)rng.NextDouble();
            pkind[n] = kind;
            pcountry[n] = kind == 1 && geoIds != null ? (ushort)GeoCountryAt(geoIds, v.X, v.Y, v.Z) : (ushort)0;
            n++;
        }

        if (candidates > 0)
        {
            float spacing = MathF.Sqrt(4f * MathF.PI / candidates);
            float golden = MathF.PI * (3f - MathF.Sqrt(5f));

            for (int i = 0; i < candidates; i++)
            {
                float y = 1f - 2f * (i + 0.5f) / candidates;
                float r = MathF.Sqrt(MathF.Max(0f, 1f - y * y));
                float th = golden * i;

                var v = new Vector3(MathF.Cos(th) * r, y, MathF.Sin(th) * r);
                v += new Vector3(Rnd(rng), Rnd(rng), Rnd(rng)) * (spacing * 0.45f); // break the lattice pattern
                v = Vector3.Normalize(v);

                bool land = IsLand(v.X, v.Y, v.Z);
                if (!land && rng.NextDouble() > 0.38) continue; // thin the oceans so continents glow

                Add(v, 1f, land ? (byte)1 : (byte)0);
            }

            // Atmospheric haze shell
            int haze = n / 8;
            for (int i = 0; i < haze; i++)
            {
                var v = Vector3.Normalize(new Vector3(Rnd(rng), Rnd(rng), Rnd(rng)));
                Add(v, 1.03f + (float)rng.NextDouble() * 0.14f, 2);
            }
        }

        // buffers were sized for the worst case; keep only what is used
        if (n < cap)
        {
            Array.Resize(ref px, n); Array.Resize(ref py, n); Array.Resize(ref pz, n);
            Array.Resize(ref plon, n); Array.Resize(ref pseed, n); Array.Resize(ref pkind, n);
            Array.Resize(ref pcountry, n); Array.Resize(ref pmy, n);
        }

        // per-particle twinkle phase as sin/cos so the draw loop needs no trig: sin(a + b) = sin a cos b + cos a sin b
        var phS = new float[n];
        var phC = new float[n];
        for (int i = 0; i < n; i++)
        {
            phS[i] = MathF.Sin(pseed[i] * 40f);
            phC[i] = MathF.Cos(pseed[i] * 40f);
        }
        _pphS = phS; _pphC = phC;

        _px = px; _py = py; _pz = pz; _plon = plon; _pseed = pseed; _pkind = pkind; _pcountry = pcountry; _pmy = pmy;
        _count = n;
        OnParticlesRebuilt();

        // Stars
        int starCount = 520;
        _stX = new float[starCount]; _stY = new float[starCount]; _stD = new float[starCount]; _stSeed = new float[starCount];
        for (int i = 0; i < starCount; i++)
        {
            _stX[i] = (float)rng.NextDouble();
            _stY[i] = (float)rng.NextDouble();
            _stD[i] = 0.15f + (float)rng.NextDouble() * 0.85f;
            _stSeed[i] = (float)rng.NextDouble();
        }

        // Orbit belts
        var presets = new (float incl, float node, float radius, float speed, int count)[]
        {
            (62f, 0f,   1.32f,  0.36f, 900),
            (-28f, 70f, 1.52f, -0.22f, 1100),
            (80f, 140f, 1.78f,  0.15f, 1300),
        };

        _orbits = new ParticleGlobeOrbit[_orbitRings];
        for (int k = 0; k < _orbitRings; k++)
        {
            var (incl, node, radius, speed, count) = k < presets.Length
                ? presets[k]
                : (Rnd(rng) * 80f, (float)rng.NextDouble() * 360f, 1.9f + 0.25f * (k - 2), Rnd(rng) * 0.2f, 1400);

            var o = new ParticleGlobeOrbit
            {
                Count = count,
                Speed = speed,
                Basis = Matrix4x4.CreateRotationZ(incl * MathF.PI / 180f) * Matrix4x4.CreateRotationY(node * MathF.PI / 180f),
                Angle = new float[count],
                Radius = new float[count],
                Lift = new float[count]
            };

            for (int i = 0; i < count; i++)
            {
                o.Angle[i] = (float)rng.NextDouble() * MathF.Tau;
                float spread = (i % 7 == 0) ? 0.09f : 0.035f;
                o.Radius[i] = radius * (1f + Rnd(rng) * spread);
                o.Lift[i] = Rnd(rng) * 0.03f;
            }
            _orbits[k] = o;
        }
    }

    private void SpawnArc()
    {
        var markers = _markers;
        if (markers.Length < 2) return;

        for (int tries = 0; tries < 8; tries++)
        {
            var a = markers[_rng.Next(markers.Length)].ToVector();
            var b = markers[_rng.Next(markers.Length)].ToVector();
            float omega = MathF.Acos(Math.Clamp(Vector3.Dot(a, b), -1f, 1f));
            if (omega < 0.5f || omega > 2.9f) continue;

            _arcs.Add(new ParticleGlobeArcState
            {
                A = a,
                B = b,
                Omega = omega,
                Height = 0.10f + 0.28f * (omega / MathF.PI),
                Life = 2.6f + (float)_rng.NextDouble() * 1.8f
            });
            return;
        }
    }

    private static Vector3 ArcPoint(ParticleGlobeArcState a, float t)
    {
        float s = MathF.Sin(a.Omega);
        Vector3 p = (MathF.Sin((1f - t) * a.Omega) * a.A + MathF.Sin(t * a.Omega) * a.B) / s;
        return p * (1f + a.Height * MathF.Sin(MathF.PI * t));
    }

    #endregion

    #region Update

    private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (!_showProfiler) { OnUpdateCore(sender, args); return; }

        long t0 = Stopwatch.GetTimestamp();
        OnUpdateCore(sender, args);
        PfSmooth(ref _pfUpdateMs, PfMs(Stopwatch.GetTimestamp() - t0));
    }

    private void OnUpdateCore(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        float dt = Math.Min((float)args.Timing.ElapsedTime.TotalSeconds, 0.1f);

        if (_needsRebuild) Rebuild();

        _time += dt;
        AdvanceQuality(dt, _dragging);

        float pendYaw, pendPitch, vel = 0f, zoomT;
        float flyLat, flyLon, flyZoom;
        bool dragging, flyPending, release;
        lock (_inputLock)
        {
            pendYaw = _pendingYaw;
            pendPitch = _pendingPitch;
            _pendingYaw = _pendingPitch = 0f;
            zoomT = _zoomTarget;
            dragging = _dragging;
            if (!dragging)
            {
                vel = _velYaw;
                _velYaw *= MathF.Exp(-dt * 2.2f);
            }

            flyPending = _flyPending;
            release = _releasePending;
            flyLat = _reqLat;
            flyLon = _reqLon;
            flyZoom = _reqZoom;
            _flyPending = _releasePending = false;
            if (flyPending) _velYaw = 0f;
        }

        bool locked = StepFly(dt, pendYaw != 0f || pendPitch != 0f, flyPending, release, flyLat, flyLon, flyZoom, ref zoomT);
        AdvanceTour(dt, pendYaw != 0f || pendPitch != 0f || dragging, ref zoomT);

        if (!locked)
        {
            _yaw += pendYaw + vel * dt;
            if (!_flatTarget) _pitch = Math.Clamp(_pitch + pendPitch, -1.1f, 1.1f);
            if (!dragging) _pitch *= MathF.Exp(-dt * 0.4f);
            if (_autoRotate && !_flatTarget) _yaw += _rotationSpeed * dt;
            else if (_flatTarget && _morphT < 1f && !dragging) _yaw += WrapPi(FlatCenterYaw - _yaw) * (1f - MathF.Exp(-dt * 2.5f));
        }

        AdvanceFlatPan(dt, _flatMode == ParticleGlobeFlatMapMode.Scroll ? 0f : pendYaw / 0.006f, _flatMode == ParticleGlobeFlatMapMode.Fixed ? 0f : pendPitch / 0.006f);

        if (_flatTarget && _flatMode != ParticleGlobeFlatMapMode.Scroll)
        {
            // the map never wraps: lock the longitude and ignore momentum
            lock (_inputLock) { _velYaw = 0f; }
            _yaw = _morphT >= 1f ? FlatCenterYaw : _yaw + WrapPi(FlatCenterYaw - _yaw) * (1f - MathF.Exp(-dt * 8f));
        }

        _zoom += (zoomT - _zoom) * (1f - MathF.Exp(-dt * 8f));
        _scanLon = (_scanLon + dt * 0.9f) % MathF.Tau;

        // arcs
        for (int i = _arcs.Count - 1; i >= 0; i--)
        {
            _arcs[i].Age += dt;
            if (_arcs[i].Age >= _arcs[i].Life) _arcs.RemoveAt(i);
        }

        if (_showArcs && _arcFrequency > 0f)
        {
            float interval = 1f / _arcFrequency;
            _arcTimer += dt;
            while (_arcTimer >= interval)
            {
                _arcTimer -= interval;
                if (_arcs.Count < 28) SpawnArc();
            }
        }
        else
        {
            _arcTimer = 0f;
        }
    }

    #endregion

    #region Draw

    private Vector3 Rot(Vector3 p)
    {
        float x1 = p.X * _cY + p.Z * _sY;
        float z1 = p.Z * _cY - p.X * _sY;
        var s = new Vector3(x1, p.Y * _cX - z1 * _sX, p.Y * _sX + z1 * _cX);
        return _morphE > 0f ? Vector3.Lerp(s, FlatView(p), _morphE) : s;
    }

    private bool OffScreen(Vector2 p, float pad) => p.X < -pad || p.Y < -pad || p.X > _viewW + pad || p.Y > _viewH + pad;

    private Vector2 Proj(Vector3 r)
    {
        float k = _radius / (1f - r.Z * Perspective);
        return new Vector2(_center.X + r.X * k, _center.Y - r.Y * k);
    }

    private static bool Occluded(Vector3 r) => r.Z < 0f && (r.X * r.X + r.Y * r.Y) < 1f;

    private static float WrapPi(float d)
    {
        d -= MathF.Tau * MathF.Floor((d + MathF.PI) / MathF.Tau);
        return d;
    }

    // Tint is premultiplied manually so additive blending scales with alpha.
    private static Vector4 Tint(Vector3 rgb, float a)
    {
        a = Math.Clamp(a, 0f, 1f);
        return new Vector4(rgb.X * a, rgb.Y * a, rgb.Z * a, a);
    }

    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        long t0 = Stopwatch.GetTimestamp();
        if (_pfLastDraw != 0) QualitySample(PfMs(t0 - _pfLastDraw));
        _pfLastDraw = t0;

        if (!_showProfiler) { OnDrawCore(sender, args); return; }

        _pfFrames++;
        OnDrawCore(sender, args);
        PfSmooth(ref _pfDrawMs, PfMs(Stopwatch.GetTimestamp() - t0));
    }

    private void OnDrawCore(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var ds = args.DrawingSession;
        long pf = _showProfiler ? Stopwatch.GetTimestamp() : 0;
        float w = (float)sender.Size.Width, h = (float)sender.Size.Height;
        if (w < 8f || h < 8f || _dot == null) return;

        EnsureBrushes(sender);

        // in Fixed mode the map ignores zoom, so neither the map size nor the particle size grows with it
        float zEff = _flatMode == ParticleGlobeFlatMapMode.Fixed ? 1f + (_zoom - 1f) * (1f - _morphE) : _zoom;
        float R = MathF.Min(w, h) * 0.5f * _globeScale * zEff;
        var c = new Vector2(w * 0.5f, h * 0.5f);
        float t = _time;
        float tilt = _tilt + _pitch;

        _radius = R;
        _center = c;
        _viewW = w;
        _viewH = h;
        _mapHalfW = MathF.Min(w * 0.46f, h * 0.46f / MercVisibleN) * zEff / R;
        _cY = MathF.Cos(_yaw); _sY = MathF.Sin(_yaw);
        _cX = MathF.Cos(tilt); _sX = MathF.Sin(tilt);

        UpdateCountryState((float)args.Timing.ElapsedTime.TotalSeconds);

        ds.Blend = CanvasBlend.Add;

        var origin = new Vector2(32f, 32f);
        float sizeBoost = Math.Clamp(R / 260f, 0.7f, 2.2f) * _particleSize / 64f;

        // 1. Stars
        if (_showStars)
        {
            using var batch = ds.CreateSpriteBatch(CanvasSpriteSortMode.None, CanvasImageInterpolation.Linear, CanvasSpriteOptions.None);
            var rgb = new Vector3(_star.X, _star.Y, _star.Z);
            float shift = _yaw * 0.012f;
            for (int i = 0; i < _stX.Length; i++)
            {
                float d = _stD[i];
                float sx = _stX[i] + shift * d;
                sx -= MathF.Floor(sx);
                float sy = _stY[i] + MathF.Sin(_yaw * 0.5f) * 0.004f * d;
                float a = (0.2f + 0.6f * d) * (0.55f + 0.45f * MathF.Sin(t * 1.5f + _stSeed[i] * 50f)) * _star.W;
                float scale = (2f + 4f * d) / 64f;
                batch.Draw(_dot, new Vector2(sx * w, sy * h), Tint(rgb, a), origin, 0f, new Vector2(scale), CanvasSpriteFlip.None);
            }
        }
        PfMark(0, ref pf);

        // 2-4. Atmosphere, body, grid and HUD dissolve as the globe flattens
        float morph = _morphE;
        if (morph < 0.999f)
        {
            using var layer = morph > 0.001f ? ds.CreateLayer(1f - morph) : null;

            _glowBrush.Center = c;
            _glowBrush.RadiusX = _glowBrush.RadiusY = R * 1.42f;
            ds.FillCircle(c, R * 1.42f, _glowBrush);

            _discBrush.Center = c + new Vector2(-0.28f * R, -0.28f * R);
            _discBrush.RadiusX = _discBrush.RadiusY = R * 1.35f;
            ds.FillCircle(c, R, _discBrush);

            if (_showGrid) DrawGrid(ds);
            if (_showHud) DrawHud(ds, c, R, t);
        }
        PfMark(1, ref pf);

        // 5. Arcs (lines) + arrival rings
        _heads.Clear();
        if (_showArcs) DrawArcs(ds);
        DrawRippleRings(ds);
        DrawLinks(ds);
        DrawUserArcs(ds);
        DrawSatelliteTrails(ds, t);
        DrawEventRings(ds);
        DrawCompareArc(ds);
        PfMark(2, ref pf);

        // 6. Everything sprite-based in one additive batch
        using (var batch = ds.CreateSpriteBatch(CanvasSpriteSortMode.None, CanvasImageInterpolation.Linear, CanvasSpriteOptions.None))
        {
            DrawParticles(batch, origin, sizeBoost, t);
            PfMark(3, ref pf);
            DrawOutline(batch, origin, sizeBoost, t);
            DrawBorders(batch, origin, sizeBoost, t);
            DrawFlow(batch, origin, sizeBoost, t);
            PfMark(4, ref pf);
            DrawEventSpikes(batch, origin, sizeBoost, t);
            DrawCompareSprites(batch, origin, sizeBoost, t);
            DrawBars(batch, origin, sizeBoost, t);
            PfMark(5, ref pf);
            if (_showOrbits) DrawOrbits(batch, origin, sizeBoost, t);
            DrawSatelliteHeads(batch, origin, sizeBoost, t);
            if (_showCities) DrawMarkers(batch, origin, sizeBoost, t);
            DrawHeads(batch, origin, sizeBoost);
            DrawPackets(batch, origin, sizeBoost);
            DrawUserPackets(batch, origin, sizeBoost);
        }
        PfMark(6, ref pf);

        ds.Blend = CanvasBlend.SourceOver;
        DrawMarkerLabels(ds);
        DrawEventLabels(ds);
        DrawCompareLabels(ds);
        DrawCoordinateTicker(ds, w, h, t);
        DrawCompareHud(ds, w, h, t);
        DrawCursorHud(ds, w, h, t);
        DrawScreenFx(ds, w, h, t);
        PfMark(7, ref pf);
        if (_showProfiler) DrawProfiler(ds, w, h);
    }

    private void DrawParticles(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        float cy = _cY, sy = _sY, cx = _cX, sx = _sX;
        var land = new Vector3(_land.X, _land.Y, _land.Z);
        var ocean = new Vector3(_ocean.X, _ocean.Y, _ocean.Z);
        var haze = new Vector3(_atmos.X, _atmos.Y, _atmos.Z);
        var accent = new Vector3(_accent.X, _accent.Y, _accent.Z);

        var pc = _pcountry;
        int hov = _hoverId, fad = _fadeId, sel = _selDrawId;
        float hovAmt = _hoverAmt, fadAmt = _fadeAmt, selAmt = _selAmt;
        bool anyCountry = pc.Length >= _count && (hov != 0 || fad != 0 || sel != 0);
        var hoverRgb = Vector3.Lerp(accent, Vector3.One, 0.45f);
        var selRgb = Vector3.Lerp(new Vector3(_arc.X, _arc.Y, _arc.Z), Vector3.One, 0.25f);
        var fx = _selFx;
        bool selHighlight = (fx & ParticleGlobeSelectionEffects.Highlight) != 0;
        float selPulse = (fx & ParticleGlobeSelectionEffects.Pulse) != 0 ? 0.5f + 0.5f * MathF.Sin(t * 2.6f) : 0.3f;
        float dimAmt = (fx & ParticleGlobeSelectionEffects.DimOthers) != 0 && sel != 0 && pc.Length >= _count ? _selDim * selAmt : 0f;
        var rf = _ripFrame;
        int nRip = _ripFrameCount;
        var rippleRgb = Vector3.Lerp(accent, Vector3.One, 0.6f);
        var fc = _fieldCenter;
        float fieldAmt = _forceField ? _fieldAmt : 0f;
        var fieldRgb = Vector3.Lerp(accent, Vector3.One, 0.5f);
        var vset = _values;
        float morphT = _morphT, mapHalfW = _mapHalfW, flatScale = FlatScale;
        float heatAmt = _heatAmt;
        bool heat = vset != null && heatAmt > 0.001f && pc.Length >= _count;
        float dn = _dnAmt;
        var sun = _sunDir;
        if (dn > 0.001f) EnsureCityLights(); else dn = 0f;
        var lights = _plight;
        bool hasLights = dn > 0f && lights.Length >= _count;

        float cullPad = 40f * sizeBoost;
        var phS = _pphS; var phC = _pphC;
        float twS = MathF.Sin(t * 2.2f), twC = MathF.Cos(t * 2.2f);
        int loopCount = Math.Min(_count, Math.Min(phS.Length, phC.Length));
        bool intro = _introT < 1f;
        float dofAmt = _tourAmt * _tourDim;
        float dofR = MathF.Max(_radius * 0.6f, 60f);
        float introT = _introT;
        float keep = _qKeep;
        bool thin = keep < 0.999f;
        float thinA = 1f + (1f - keep) * 0.6f, thinS = 1f + (1f - keep) * 0.3f;
        var pkindArr = _pkind; var pseedArr = _pseed;

        // optional multi-threaded path: sprites are computed in parallel into buffers, then submitted in order
        bool par = _parallel && loopCount >= ParallelMinParticles;
        Vector2[] outPos = null; Vector4[] outTint = null; float[] outScale = null;
        if (par) EnsureParticleBuffers(loopCount, out outPos, out outTint, out outScale);

        int Work(int from, int to, bool buffered)
        {
        int drawn = 0;

        for (int i = from; i < to; i++)
        {
            if (buffered) outScale[i] = 0f;

            // adaptive quality: thin ocean/haze deterministically by seed, land is never dropped
            if (thin && pkindArr[i] != 1 && pseedArr[i] > keep) continue;

            float x = _px[i], y = _py[i], z = _pz[i];
            float introA = 1f;
            if (intro)
            {
                float s = _pseed[i];
                float q = Math.Clamp(introT * 1.6f - 0.6f * Hash01(s, 191.3f), 0f, 1f);
                float e = q * q * q * (q * (q * 6f - 15f) + 10f);
                var o = IntroOrigin(s);

                // spiral in: the start point swings round the Y axis as it settles
                float sw = (1f - e) * 2.4f, cs = MathF.Cos(sw), sn = MathF.Sin(sw);
                o = new Vector3(o.X * cs + o.Z * sn, o.Y, o.Z * cs - o.X * sn);
                x = o.X + (x - o.X) * e;
                y = o.Y + (y - o.Y) * e;
                z = o.Z + (z - o.Z) * e;
                introA = MathF.Min(1f, q * 4f);
            }
            float sunDot = dn > 0f ? x * sun.X + y * sun.Y + z * sun.Z : 1f;

            float rip = 0f;
            if (nRip > 0 && _pkind[i] != 2)
            {
                for (int r = 0; r < nRip; r++)
                {
                    float dot = x * rf[r].Center.X + y * rf[r].Center.Y + z * rf[r].Center.Z;
                    float dist;
                    if (morphT > 0f)
                    {
                        float dl = WrapPi(_plon[i] + _yaw) - WrapPi(rf[r].Lon + _yaw), dm = _pmy[i] * MathF.PI - rf[r].Merc;
                        float dMap = MathF.Sqrt(dl * dl + dm * dm);
                        float dSphere = MathF.Acos(Math.Clamp(dot, -1f, 1f));
                        dist = dSphere + (dMap - dSphere) * _morphE;
                        if (MathF.Abs(dist - rf[r].Distance) > 2.5f * rf[r].Width) continue;
                    }
                    else
                    {
                        if (dot < rf[r].Lo || dot > rf[r].Hi) continue;
                        dist = MathF.Acos(Math.Clamp(dot, -1f, 1f));
                    }

                    float q = (dist - rf[r].Distance) / rf[r].Width;
                    rip += rf[r].Amp * MathF.Exp(-q * q);
                }

                if (rip > 0f)
                {
                    float lift = 1f + 0.05f * MathF.Min(rip, 1.5f);
                    x *= lift;
                    y *= lift;
                    z *= lift;
                }
            }

            float ff = 0f;
            if (fieldAmt > 0f)
            {
                float fd = x * fc.X + y * fc.Y + z * fc.Z;
                if (fd > FieldCutoffCos)
                {
                    float ang = MathF.Acos(MathF.Min(fd, 1f));
                    float q = ang / FieldRadius;
                    ff = MathF.Exp(-2f * q * q) * fieldAmt;

                    float tx = x - fc.X * fd, ty = y - fc.Y * fd, tz = z - fc.Z * fd;
                    float tl = MathF.Sqrt(tx * tx + ty * ty + tz * tz);
                    float push = tl > 1e-5f ? 0.06f * ff * MathF.Min(1f, ang / (FieldRadius * 0.5f)) / tl : 0f;
                    float lift = 1f + 0.07f * ff;
                    x = (x + tx * push) * lift;
                    y = (y + ty * push) * lift;
                    z = (z + tz * push) * lift;
                }
            }

            float x1 = x * cy + z * sy;
            float z1 = z * cy - x * sy;
            float y2 = y * cx - z1 * sx;
            float z2 = y * sx + z1 * cx;

            float zs = z2;
            float eP = 0f;
            float edgeFade = 1f;
            if (morphT > 0f)
            {
                float lonN = WrapPi(_plon[i] + _yaw) / MathF.PI;
                float stagger = 0.4f * MathF.Abs(lonN) + 0.15f * _pseed[i];
                float q = Math.Clamp(morphT * 1.55f - stagger, 0f, 1f);
                eP = q * q * (3f - 2f * q);
                if (eP > 0f)
                {
                    float my = _pmy[i];
                    float fy = ((my - _flatCenterN) * mapHalfW + 0.05f * MathF.Min(rip, 1.5f)) * flatScale;
                    float hop = MathF.Sin(MathF.PI * eP) * (0.25f + 0.3f * _pseed[i]);
                    x1 += ((lonN - _flatPanX) * mapHalfW * flatScale - x1) * eP;
                    y2 += (fy - y2) * eP;
                    z2 += (FlatZ - z2) * eP + hop;
                    if (MathF.Abs(my) > MercVisibleN) edgeFade = 1f - eP;
                }
            }

            float k = _radius / (1f - z2 * Perspective);
            var pos = new Vector2(_center.X + x1 * k, _center.Y - y2 * k);

            byte kind = _pkind[i];
            Vector3 rgb;
            float baseA, px;
            if (kind == 1) { rgb = land; baseA = _land.W * 0.85f; px = 5.0f; }
            else if (kind == 0) { rgb = ocean; baseA = _ocean.W * 0.55f; px = 3.6f; }
            else { rgb = haze; baseA = _atmos.W * 0.30f * (1f - eP); px = 3.0f; }
            baseA *= edgeFade;
            if (thin && kind != 1) { baseA *= thinA; px *= thinS; }

            if (heat && kind == 1)
            {
                int vid = pc[i];
                if (vid < vset.T.Length && !float.IsNaN(vset.T[vid]))
                {
                    rgb = Vector3.Lerp(rgb, Vector3.Lerp(vset.Low, vset.High, vset.T[vid]), heatAmt);
                    baseA *= 1f + 0.15f * heatAmt;
                }
            }

            if (dn > 0f && kind != 2)
            {
                float u = Math.Clamp((sunDot + 0.1f) / 0.25f, 0f, 1f);
                float day = u * u * (3f - 2f * u);
                float night = 1f - day;
                float twi = 4f * day * night;
                rgb = Vector3.Lerp(rgb, rgb * new Vector3(0.5f, 0.65f, 1f), dn * night * 0.6f);
                rgb = Vector3.Lerp(rgb, new Vector3(1f, 0.55f, 0.25f), dn * twi * 0.35f);
                baseA *= 1f - dn * night * 0.6f + dn * day * 0.2f;
                if (hasLights && kind == 1)
                {
                    float nl = dn * night * night * lights[i];
                    if (nl > 0.01f)
                    {
                        rgb = Vector3.Lerp(rgb, new Vector3(1f, 0.8f, 0.4f), MathF.Min(1f, nl * 1.3f));
                        baseA += nl * 0.9f;
                        px *= 1f + 0.7f * nl;
                    }
                }
            }

            float face = zs >= 0f ? 0.65f + 0.6f * (1f - zs) * (1f - zs) : 0.14f;
            if (eP > 0f) face += (0.9f - face) * eP;
            float twinkle = 0.7f + 0.3f * (twS * phC[i] + twC * phS[i]);

            // scan beam
            float boost = 0f;
            float d = MathF.Abs(WrapPi(_plon[i] - _scanLon));
            if (d < 0.35f && kind != 2) boost = MathF.Exp(-d * d * 60f);

            if (boost > 0.01f) rgb = Vector3.Lerp(rgb, accent, boost);

            float a = baseA * face * twinkle * (1f + boost * 2f) * introA;
            float scale = sizeBoost * px * (1f + boost * 1.2f);

            if (rip > 0f)
            {
                rgb = Vector3.Lerp(rgb, rippleRgb, MathF.Min(1f, rip * 0.8f));
                a *= 1f + 2.4f * rip;
                scale *= 1f + 0.8f * rip;
            }

            if (ff > 0.001f)
            {
                rgb = Vector3.Lerp(rgb, fieldRgb, 0.55f * ff);
                a *= 1f + 1.1f * ff;
                scale *= 1f + 1.2f * ff;
            }

            if (anyCountry)
            {
                int cid = pc[i];
                if (cid != 0)
                {
                    float hl = cid == hov ? hovAmt : cid == fad ? fadAmt : 0f;
                    float sl = cid == sel ? selAmt : 0f;

                    if (hl > 0f)
                    {
                        float pulse = 0.5f + 0.5f * MathF.Sin(t * 5f - _plon[i] * 3f);
                        rgb = Vector3.Lerp(rgb, hoverRgb, hl * 0.9f);
                        a *= 1f + hl * (1.1f + 0.9f * pulse);
                        scale *= 1f + hl * (0.25f + 0.4f * pulse);
                    }

                    if (sl > 0f && selHighlight)
                    {
                        rgb = Vector3.Lerp(rgb, selRgb, sl * 0.85f);
                        a *= 1f + sl * (0.8f + 0.5f * selPulse);
                        scale *= 1f + sl * (0.2f + 0.2f * selPulse);
                    }
                }
            }

            if (dofAmt > 0.001f)
            {
                float dd = Vector2.Distance(pos, _center) / dofR;
                float blur = Math.Clamp((dd - 0.5f) / 1.0f, 0f, 1f) * dofAmt;
                a *= 1f - 0.75f * blur;
                scale *= 1f + 0.6f * blur;
            }

            if (dimAmt > 0f && pc[i] != sel) a *= 1f - 0.85f * dimAmt;

            // skip sprites that are off-screen or too faint to see
            if (a < 0.004f || pos.X < -cullPad || pos.Y < -cullPad || pos.X > _viewW + cullPad || pos.Y > _viewH + cullPad) continue;

            if (buffered)
            {
                outPos[i] = pos;
                outTint[i] = Tint(rgb, a);
                outScale[i] = scale;
            }
            else
            {
                batch.Draw(_dot, pos, Tint(rgb, a), origin, 0f, new Vector2(scale), CanvasSpriteFlip.None);
            }
            drawn++;
        }

        return drawn;
        }

        int total;
        if (par)
        {
            int workers = Math.Clamp(Environment.ProcessorCount - 1, 1, 8);
            int chunk = Math.Max(2048, (loopCount + workers * 3) / (workers * 4));
            int chunks = (loopCount + chunk - 1) / chunk;
            Parallel.For(0, chunks, new ParallelOptions { MaxDegreeOfParallelism = workers }, ci =>
            {
                int s = ci * chunk;
                Work(s, Math.Min(loopCount, s + chunk), true);
            });

            total = 0;
            var dot = _dot;
            for (int i = 0; i < loopCount; i++)
            {
                float sc = outScale[i];
                if (sc <= 0f) continue;
                batch.Draw(dot, outPos[i], outTint[i], origin, 0f, new Vector2(sc), CanvasSpriteFlip.None);
                total++;
            }
        }
        else
        {
            total = Work(0, loopCount, false);
        }

        _pfDrawn = total;
    }

    private void DrawOrbits(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost, float t)
    {
        float vis = 1f - _morphE;
        if (vis <= 0.01f) return;

        var view = Matrix4x4.CreateRotationY(_yaw * 0.25f) * Matrix4x4.CreateRotationX(_tilt + _pitch);
        var rgb = Vector3.Lerp(new Vector3(_atmos.X, _atmos.Y, _atmos.Z), new Vector3(_accent.X, _accent.Y, _accent.Z), 0.35f);
        var sat = new Vector3(_accent.X, _accent.Y, _accent.Z);

        foreach (var o in _orbits)
        {
            var m = o.Basis * view;
            for (int i = 0; i < o.Count; i++)
            {
                float a = o.Angle[i] + o.Speed * t;
                var local = new Vector3(MathF.Cos(a) * o.Radius[i], o.Lift[i], MathF.Sin(a) * o.Radius[i]);
                var r = Vector3.Transform(local, m);
                if (Occluded(r)) continue;

                var pos = Proj(r);
                float depth = Math.Clamp(0.55f + 0.3f * r.Z, 0.2f, 1f);

                if (i % 150 == 0)
                {
                    batch.Draw(_dot, pos, Tint(sat, 0.95f * vis), origin, 0f, new Vector2(sizeBoost * 11f), CanvasSpriteFlip.None);
                    batch.Draw(_dot, pos, Tint(new Vector3(1f, 1f, 1f), 0.9f * vis), origin, 0f, new Vector2(sizeBoost * 3.5f), CanvasSpriteFlip.None);
                }
                else
                {
                    batch.Draw(_dot, pos, Tint(rgb, 0.45f * depth * vis), origin, 0f, new Vector2(sizeBoost * 2.8f), CanvasSpriteFlip.None);
                }
            }
        }
    }

    private void DrawHeads(CanvasSpriteBatch batch, Vector2 origin, float sizeBoost)
    {
        var rgb = Vector3.Lerp(new Vector3(_arc.X, _arc.Y, _arc.Z), Vector3.One, 0.55f);
        foreach (var h in _heads)
        {
            var pos = new Vector2(h.X, h.Y);
            batch.Draw(_dot, pos, Tint(rgb, 0.8f * h.Z), origin, 0f, new Vector2(sizeBoost * 16f), CanvasSpriteFlip.None);
            batch.Draw(_dot, pos, Tint(Vector3.One, h.Z), origin, 0f, new Vector2(sizeBoost * 5f), CanvasSpriteFlip.None);
        }
    }

    private void DrawGrid(CanvasDrawingSession ds)
    {
        const int Seg = 72;

        void Poly(Func<float, Vector3> f)
        {
            Vector2 prev = default;
            float prevA = 0f;
            for (int i = 0; i <= Seg; i++)
            {
                var r = Rot(f(i * MathF.Tau / Seg));
                var p = Proj(r);
                float a = _grid.W * (r.Z > 0f ? 0.3f + 0.7f * MathF.Min(1f, r.Z * 2f) : 0.08f);
                if (i > 0) ds.DrawLine(prev, p, ToColor(_grid, (a + prevA) * 0.5f / Math.Max(_grid.W, 0.0001f)), 0.75f);
                prev = p;
                prevA = a;
            }
        }

        for (int lat = -60; lat <= 60; lat += 30)
        {
            float phi = lat * MathF.PI / 180f;
            float y = MathF.Sin(phi), rr = MathF.Cos(phi);
            Poly(lam => new Vector3(rr * MathF.Sin(lam), y, rr * MathF.Cos(lam)) * 1.002f);
        }

        for (int lon = 0; lon < 360; lon += 30)
        {
            float lam = lon * MathF.PI / 180f;
            float sl = MathF.Sin(lam), cl = MathF.Cos(lam);
            Poly(phi => new Vector3(MathF.Cos(phi) * sl, MathF.Sin(phi), MathF.Cos(phi) * cl) * 1.002f);
        }
    }

    private void DrawHud(CanvasDrawingSession ds, Vector2 c, float R, float t)
    {
        float hr = R * 1.17f;
        float rot = t * 0.05f;

        ds.DrawCircle(c, hr, ToColor(_accent, 0.18f), 0.75f);

        for (int i = 0; i < 180; i++)
        {
            float a = i * MathF.Tau / 180f + rot;
            bool major = i % 15 == 0, mid = i % 5 == 0;
            float len = major ? 0.045f : mid ? 0.025f : 0.012f;
            var d = new Vector2(MathF.Cos(a), MathF.Sin(a));
            ds.DrawLine(c + d * hr, c + d * (hr + R * len), ToColor(_accent, major ? 0.55f : 0.22f), major ? 1.5f : 1f);
        }

        // three counter-rotating arc spans on an outer ring
        for (int s = 0; s < 3; s++)
        {
            float start = -t * 0.18f + s * MathF.Tau / 3f;
            const int Steps = 28;
            float sweep = 1.1f;
            Vector2 prev = default;
            for (int k = 0; k <= Steps; k++)
            {
                float a = start + sweep * k / Steps;
                var p = c + new Vector2(MathF.Cos(a), MathF.Sin(a)) * (R * 1.27f);
                if (k > 0) ds.DrawLine(prev, p, ToColor(_atmos, 0.45f), 1.5f);
                prev = p;
            }
        }
    }

    private void DrawArcs(CanvasDrawingSession ds)
    {
        const int Seg = 36;

        foreach (var a in _arcs)
        {
            float ph = a.Age / a.Life;
            float head = Math.Clamp(ph / 0.55f, 0f, 1f);
            head = 1f - (1f - head) * (1f - head);
            float tail = Math.Clamp((ph - 0.35f) / 0.65f, 0f, 1f);
            float fade = ph > 0.8f ? (1f - ph) / 0.2f : 1f;

            if (tail < head)
            {
                Vector2 prev = default;
                bool havePrev = false;
                for (int k = 0; k <= Seg; k++)
                {
                    float u = k / (float)Seg;
                    var r = Rot(ArcPoint(a, tail + (head - tail) * u));
                    if (Occluded(r)) { havePrev = false; continue; }

                    var p = Proj(r);
                    if (havePrev && Jumped(prev, p)) havePrev = false;
                    if (havePrev) ds.DrawLine(prev, p, ToColor(_arc, fade * u * u), 1.6f);
                    prev = p;
                    havePrev = true;

                    if (k == Seg) _heads.Add(new Vector3(p.X, p.Y, fade));
                }
            }

            // arrival ring on the surface
            if (ph > 0.5f)
            {
                float rp = Math.Clamp((ph - 0.5f) / 0.5f, 0f, 1f);
                float ang = 0.02f + rp * 0.16f;
                Vector3 n = a.B;
                Vector3 up = MathF.Abs(n.Y) > 0.95f ? Vector3.UnitX : Vector3.UnitY;
                Vector3 t1 = Vector3.Normalize(Vector3.Cross(up, n));
                Vector3 t2 = Vector3.Cross(n, t1);
                float ca = MathF.Cos(ang), sa = MathF.Sin(ang);

                const int Ring = 28;
                Vector2 prev = default;
                bool havePrev = false;
                for (int k = 0; k <= Ring; k++)
                {
                    float th = k * MathF.Tau / Ring;
                    var pt = (n * ca + (t1 * MathF.Cos(th) + t2 * MathF.Sin(th)) * sa) * 1.004f;
                    var r = Rot(pt);
                    if (r.Z < 0f) { havePrev = false; continue; }

                    var p = Proj(r);
                    if (havePrev && Jumped(prev, p)) havePrev = false;
                    if (havePrev) ds.DrawLine(prev, p, ToColor(_arc, (1f - rp) * 0.8f), 1.2f);
                    prev = p;
                    havePrev = true;
                }
            }
        }
    }

    #endregion
}
