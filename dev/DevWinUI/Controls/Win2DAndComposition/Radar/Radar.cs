using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using Windows.Graphics.DirectX;
using DirectXPixelFormat = Windows.Graphics.DirectX.DirectXPixelFormat;

namespace DevWinUI;

[ContentProperty(Name = nameof(Blips))]
[TemplatePart(Name = "PART_Canvas", Type = typeof(CanvasControl))]
public sealed partial class Radar : Control
{
    private const float Tau = 2f * MathF.PI;
    private const float Omega0 = 1.05f;
    private const double FallbackSize = 240;
    private const float TargetSize = 2.4f;
    private const float MarkerScale = 2.5f;

    private static readonly Vector2[][] MarkerShapes =
    {
        Array.Empty<Vector2>(),
        new[]
        {
            new Vector2(0f, -1f), new Vector2(0.7f, 0f), new Vector2(0f, 1f), new Vector2(-0.7f, 0f),
        },
        new[]
        {
            new Vector2(-0.7f, -0.7f), new Vector2(0.7f, -0.7f), new Vector2(0.7f, 0.7f), new Vector2(-0.7f, 0.7f),
        },
        new[]
        {
            new Vector2(0f, -1f), new Vector2(0.7f, 0.8f), new Vector2(0f, 0.4f), new Vector2(-0.7f, 0.8f),
        },
        new[]
        {
            new Vector2(0f, -1f), new Vector2(0.12f, -0.6f), new Vector2(0.12f, -0.15f), new Vector2(1f, 0.25f),
            new Vector2(1f, 0.45f), new Vector2(0.12f, 0.25f), new Vector2(0.12f, 0.65f), new Vector2(0.45f, 0.9f),
            new Vector2(0.45f, 1f), new Vector2(0f, 0.85f), new Vector2(-0.45f, 1f), new Vector2(-0.45f, 0.9f),
            new Vector2(-0.12f, 0.65f), new Vector2(-0.12f, 0.25f), new Vector2(-1f, 0.45f), new Vector2(-1f, 0.25f),
            new Vector2(-0.12f, -0.15f), new Vector2(-0.12f, -0.6f),
        },
    };

    private readonly record struct Geo(Vector2 C, float R, float W, float H);

    private struct Contact
    {
        public Vector2 Pos;
        public float Ang, Dist, Size, Heading;
        public int Index;
        public bool Cursor;
        public bool ScanOnly;
        public string? Label;
        public RadarBlipState State;
        public Color Tone;
        public RadarMarker Marker;
        public RadarBlip? Blip;
    }

    private readonly PointerEventHandler _movedHandler;
    private readonly PointerEventHandler _exitedHandler;
    private readonly Action _blipChanged;
    private readonly Random _rng = new();
    private readonly List<Contact> _contacts = new();
    private readonly List<RadarBlip> _attached = new();
    private readonly List<IDisposable> _temps = new();
    private readonly CanvasGeometry?[] _markerGeo = new CanvasGeometry?[MarkerShapes.Length];
    private readonly float[] _ringR = new float[3];
    private readonly Vector2[] _wedge = new Vector2[4];
    private readonly CanvasGradientStop[] _maskStops = new CanvasGradientStop[3];
    private readonly CanvasGradientStop[] _rimStops = new CanvasGradientStop[3];
    private readonly PropertyChangedEventHandler _itemChanged;
    private readonly Dictionary<object, RadarBlip> _bound = new(ReferenceEqualityComparer.Instance);
    private readonly List<RadarBlip> _boundList = new();
    private readonly List<RadarZone> _zonesAttached = new();
    private readonly List<RadarJam> _jamsAttached = new();

    private static readonly string[] CompassLetters = { "N", "E", "S", "W" };

    private CanvasTextFormat? _text;
    private string? _textFamily;
    private float _textSize;
    private CanvasStrokeStyle? _dash;
    private CanvasRenderTarget? _phosphorA, _phosphorB;
    private float _phosphorW, _phosphorH;
    private Vector2[] _polyBuf = Array.Empty<Vector2>();
    private string[] _ringText = Array.Empty<string>();
    private double _ringTextMax = double.NaN;
    private string? _ringTextUnit;
    private double _dt;

    private CanvasControl? _canvas;
    private UIElement? _root;
    private XamlRoot? _xamlRoot;
    private CanvasBitmap? _noise;

    private bool _loaded, _offscreen, _rendering;
    private bool _hasMouse, _mouseIn;
    private double _lastTime, _t, _intro = 1;
    private double _warm, _warmTarget;
    private double _grainClock;
    private float _angle = -MathF.PI / 2f;
    private Vector2 _mouse, _mouseS;
    private Vector2 _grainOffset;
    private Color _col;

    private float[] _cR = Array.Empty<float>();
    private float[] _cA = Array.Empty<float>();
    private float[] _cS = Array.Empty<float>();
    private float[] _cB = Array.Empty<float>();

    public Radar()
    {
        DefaultStyleKey = typeof(Radar);
        _movedHandler = OnPointerMoved;
        _exitedHandler = OnPointerExited;
        _blipChanged = RequestFrame;
        _itemChanged = OnItemPropertyChanged;

        SetValue(BlipsProperty, new ObservableCollection<RadarBlip>());
        SetValue(ZonesProperty, new ObservableCollection<RadarZone>());
        SetValue(JamsProperty, new ObservableCollection<RadarJam>());

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        SizeChanged += OnSizeChanged;
        EffectiveViewportChanged += OnEffectiveViewportChanged;
        ActualThemeChanged += OnActualThemeChanged;
        RegisterPropertyChangedCallback(ForegroundProperty, OnBrushChanged);
        RegisterPropertyChangedCallback(BackgroundProperty, OnBrushChanged);
        RegisterPropertyChangedCallback(FontSizeProperty, OnBrushChanged);
        RegisterPropertyChangedCallback(FontFamilyProperty, OnBrushChanged);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_canvas != null)
        {
            _canvas.Draw -= OnDraw;
            _canvas.CreateResources -= OnCreateResources;
        }

        _canvas = GetTemplateChild("PART_Canvas") as CanvasControl;

        if (_canvas != null)
        {
            _canvas.Draw += OnDraw;
            _canvas.CreateResources += OnCreateResources;
        }

        ApplyDpi();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double width = availableSize.Width, height = availableSize.Height;
        bool unboundedWidth = double.IsInfinity(width), unboundedHeight = double.IsInfinity(height);

        if (unboundedWidth && unboundedHeight)
        {
            width = MinWidth > 0 ? MinWidth : FallbackSize;
            height = MinHeight > 0 ? MinHeight : width;
        }
        else if (unboundedWidth)
        {
            width = height;
        }
        else if (unboundedHeight)
        {
            height = width;
        }

        var size = new Size(width, height);
        base.MeasureOverride(size);
        return size;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        _intro = Intro ? 0 : 1;
        HookRoot();

        _xamlRoot = XamlRoot;
        if (_xamlRoot != null) _xamlRoot.Changed += OnXamlRootChanged;

        ApplyDpi();
        RequestFrame();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        UnhookRoot();

        if (_xamlRoot != null)
        {
            _xamlRoot.Changed -= OnXamlRootChanged;
            _xamlRoot = null;
        }

        StopLoop();
        DisposeResources();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        RequestFrame();
    }

    private void OnEffectiveViewportChanged(FrameworkElement sender, EffectiveViewportChangedEventArgs e)
    {
        Rect view = e.EffectiveViewport;
        bool off = view.Width <= 0 || view.Height <= 0 ||
                   view.Right <= 0 || view.Bottom <= 0 ||
                   view.Left >= ActualWidth || view.Top >= ActualHeight;
        if (off == _offscreen) return;

        _offscreen = off;
        if (!off) RequestFrame();
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        RequestFrame();
    }

    private void OnBrushChanged(DependencyObject sender, DependencyProperty dp)
    {
        RequestFrame();
    }

    private void OnXamlRootChanged(XamlRoot sender, XamlRootChangedEventArgs args)
    {
        ApplyDpi();
        RequestFrame();
    }

    private void OnCreateResources(CanvasControl sender, object args)
    {
        DisposeResources();
    }

    private void DisposeResources()
    {
        SafeDispose(_noise);
        _noise = null;

        SafeDispose(_text);
        _text = null;
        SafeDispose(_dash);
        _dash = null;
        ReleasePhosphor();

        for (int i = 0; i < _markerGeo.Length; i++)
        {
            SafeDispose(_markerGeo[i]);
            _markerGeo[i] = null;
        }
    }

    private static void SafeDispose(IDisposable? resource)
    {
        try
        {
            resource?.Dispose();
        }
        catch (Exception ex) when (ex is ObjectDisposedException or COMException)
        {
        }
    }

    private void OnPropsChanged()
    {
        if (!Intro) _intro = 1;
        ApplyDpi();
        UpdateTargets();
        RequestFrame();
    }

    private void OnBlipsChanged(IList<RadarBlip>? oldValue, IList<RadarBlip>? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldNotify) oldNotify.CollectionChanged -= OnBlipsCollectionChanged;
        DetachAllBlips();

        if (newValue is INotifyCollectionChanged newNotify) newNotify.CollectionChanged += OnBlipsCollectionChanged;
        if (newValue != null)
        {
            foreach (RadarBlip blip in newValue) AttachBlip(blip);
        }

        RequestFrame();
    }

    private void OnBlipsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                DetachAllBlips();
                foreach (RadarBlip blip in Blips) AttachBlip(blip);
                break;

            default:
                if (e.OldItems != null)
                {
                    foreach (RadarBlip blip in e.OldItems) DetachBlip(blip);
                }

                if (e.NewItems != null)
                {
                    foreach (RadarBlip blip in e.NewItems) AttachBlip(blip);
                }
                break;
        }

        RequestFrame();
    }

    private void ApplyItemsSource(object? oldValue, object? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldNotify) oldNotify.CollectionChanged -= OnSourceCollectionChanged;
        if (newValue is INotifyCollectionChanged newNotify) newNotify.CollectionChanged += OnSourceCollectionChanged;

        RebuildSource();
    }

    private void RebuildSource()
    {
        ClearBound();

        if (ItemsSource is System.Collections.IEnumerable items)
        {
            foreach (object? item in items) AddBound(item);
        }

        RequestFrame();
    }

    private void OnSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Move:
                break;

            case NotifyCollectionChangedAction.Reset:
                RebuildSource();
                return;

            default:
                if (e.OldItems != null)
                {
                    foreach (object? item in e.OldItems) RemoveBound(item);
                }

                if (e.NewItems != null)
                {
                    foreach (object? item in e.NewItems) AddBound(item);
                }
                break;
        }

        RequestFrame();
    }

    private void AddBound(object? item)
    {
        if (item is null || _bound.ContainsKey(item)) return;

        RadarBlip? blip = item as RadarBlip;
        if (blip is null)
        {
            IRadarBlipMapper? mapper = ItemMapper;
            if (mapper is null) return;

            blip = new RadarBlip();
            mapper.Map(item, blip);
        }

        blip.Changed += _blipChanged;
        blip.Reset = true;
        _bound.Add(item, blip);
        _boundList.Add(blip);

        if (item is INotifyPropertyChanged notify) notify.PropertyChanged += _itemChanged;
    }

    private void RemoveBound(object? item)
    {
        if (item is null || !_bound.Remove(item, out RadarBlip? blip)) return;

        blip.Changed -= _blipChanged;
        _boundList.Remove(blip);

        if (item is INotifyPropertyChanged notify) notify.PropertyChanged -= _itemChanged;
    }

    private void ClearBound()
    {
        foreach (KeyValuePair<object, RadarBlip> pair in _bound)
        {
            pair.Value.Changed -= _blipChanged;
            if (pair.Key is INotifyPropertyChanged notify) notify.PropertyChanged -= _itemChanged;
        }

        _bound.Clear();
        _boundList.Clear();
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender is null || !_bound.TryGetValue(sender, out RadarBlip? blip)) return;

        if (!ReferenceEquals(sender, blip)) ItemMapper?.Map(sender, blip);
        RequestFrame();
    }

    private void OnZonesChanged(IList<RadarZone>? oldValue, IList<RadarZone>? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldNotify) oldNotify.CollectionChanged -= OnZonesCollectionChanged;
        if (newValue is INotifyCollectionChanged newNotify) newNotify.CollectionChanged += OnZonesCollectionChanged;

        SyncElements(_zonesAttached, newValue);
        RequestFrame();
    }

    private void OnZonesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncElements(_zonesAttached, Zones);
        RequestFrame();
    }

    private void OnJamsChanged(IList<RadarJam>? oldValue, IList<RadarJam>? newValue)
    {
        if (oldValue is INotifyCollectionChanged oldNotify) oldNotify.CollectionChanged -= OnJamsCollectionChanged;
        if (newValue is INotifyCollectionChanged newNotify) newNotify.CollectionChanged += OnJamsCollectionChanged;

        SyncElements(_jamsAttached, newValue);
        RequestFrame();
    }

    private void OnJamsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SyncElements(_jamsAttached, Jams);
        RequestFrame();
    }

    private void SyncElements<T>(List<T> attached, IList<T>? source) where T : RadarElement
    {
        for (int i = 0; i < attached.Count; i++) attached[i].Changed -= _blipChanged;
        attached.Clear();

        if (source is null) return;

        for (int i = 0; i < source.Count; i++)
        {
            T element = source[i];
            element.Changed += _blipChanged;
            attached.Add(element);
        }
    }

    private void AttachBlip(RadarBlip blip)
    {
        blip.Changed += _blipChanged;
        blip.Reset = true;
        _attached.Add(blip);
    }

    private void DetachBlip(RadarBlip blip)
    {
        blip.Changed -= _blipChanged;
        _attached.Remove(blip);
    }

    private void DetachAllBlips()
    {
        for (int i = 0; i < _attached.Count; i++) _attached[i].Changed -= _blipChanged;
        _attached.Clear();
    }

    private void ApplyDpi()
    {
        if (_canvas == null || XamlRoot == null) return;

        double cap = Math.Min(2.0, Math.Max(0.5, MaxDevicePixelRatio));
        _canvas.DpiScale = (float)Math.Clamp(cap / Math.Max(0.5, XamlRoot.RasterizationScale), 0.25, 1.0);
    }

    private void HookRoot()
    {
        UnhookRoot();

        _root = XamlRoot?.Content;
        if (_root == null) return;

        _root.AddHandler(PointerMovedEvent, _movedHandler, true);
        _root.AddHandler(PointerExitedEvent, _exitedHandler, true);
    }

    private void UnhookRoot()
    {
        if (_root == null) return;

        _root.RemoveHandler(PointerMovedEvent, _movedHandler);
        _root.RemoveHandler(PointerExitedEvent, _exitedHandler);
        _root = null;
    }

    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        Point p = e.GetCurrentPoint(this).Position;
        _mouse = new Vector2((float)p.X, (float)p.Y);

        bool wasIn = _mouseIn;
        _mouseIn = p.X >= 0 && p.Y >= 0 && p.X <= ActualWidth && p.Y <= ActualHeight;
        if (!_hasMouse || (!wasIn && _mouseIn)) _mouseS = _mouse;
        _hasMouse = true;

        UpdateTargets();
        RequestFrame();
    }

    private void OnPointerExited(object sender, PointerRoutedEventArgs e)
    {
        _mouseIn = false;
        UpdateTargets();
        RequestFrame();
    }

    private void UpdateTargets()
    {
        _warmTarget = MouseInteraction && _hasMouse && _mouseIn ? 1 : 0;
    }

    private bool CanRender =>
        _loaded && !_offscreen && Visibility == Visibility.Visible &&
        ActualWidth > 2 && ActualHeight > 2 && XamlRoot?.IsHostVisible != false;

    private bool NeedsFrames()
    {
        return !Paused || _intro < 1 ||
               Math.Abs(_warm - _warmTarget) > 0.003 ||
               (_mouseIn && Vector2.DistanceSquared(_mouse, _mouseS) > 0.25f);
    }

    private void RequestFrame()
    {
        if (!CanRender) return;

        _canvas?.Invalidate();
        if (_rendering || !NeedsFrames()) return;

        _lastTime = -1;
        CompositionTarget.Rendering += OnRendering;
        _rendering = true;
    }

    private void StopLoop()
    {
        if (!_rendering) return;

        CompositionTarget.Rendering -= OnRendering;
        _rendering = false;
    }

    private void OnRendering(object? sender, object e)
    {
        double now = ((RenderingEventArgs)e).RenderingTime.TotalSeconds;
        double dt = _lastTime < 0 ? 0 : Math.Min(now - _lastTime, 0.05);
        _lastTime = now;

        if (!CanRender)
        {
            StopLoop();
            return;
        }

        Update(dt);
        _canvas?.Invalidate();
        if (!NeedsFrames()) StopLoop();
    }

    private void Update(double dt)
    {
        if (_intro < 1) _intro = Math.Min(1, _intro + dt / 2.4);

        if (!Paused)
        {
            _t += dt;
            _dt += dt;
            float introE = 1f - MathF.Pow(1f - (float)_intro, 3f);
            float gate = Smooth((introE - 0.3f) / 0.7f);
            _angle += (float)(dt * Speed * Omega0) * gate;
            if (_angle > 1000f || _angle < -1000f) _angle %= Tau;

            _grainClock += dt;
            if (_grainClock > 0.07)
            {
                _grainClock = 0;
                _grainOffset = new Vector2(_rng.Next(128), _rng.Next(128));
            }
        }

        _warm += (_warmTarget - _warm) * (1 - Math.Exp(-8 * dt));
        if (Math.Abs(_warm - _warmTarget) < 0.003) _warm = _warmTarget;

        if (_mouseIn) _mouseS += (_mouse - _mouseS) * (float)(1 - Math.Exp(-14 * dt));
    }

    private static float Smooth(float x)
    {
        x = Math.Clamp(x, 0, 1);
        return x * x * (3 - 2 * x);
    }

    private static float Frac(float x) => x - MathF.Floor(x);

    private static Vector2 Pt(Vector2 c, float r, float a) => c + new Vector2(MathF.Cos(a), MathF.Sin(a)) * r;

    private static float Hash(int i, int salt)
    {
        double x = Math.Sin(i * 12.9898 + salt * 78.233) * 43758.5453;
        return (float)(x - Math.Floor(x));
    }

    private Color Tint(Color c)
    {
        return LightMode
            ? Color.FromArgb(255, (byte)(c.R * 0.55), (byte)(c.G * 0.55), (byte)(c.B * 0.55))
            : c;
    }

    private Color Resolve(Brush? brush)
    {
        return brush is SolidColorBrush solid ? Tint(solid.Color) : _col;
    }

    private Color Col(float a) => Col(_col, a);

    private Color Col(Color c, float a)
    {
        return Color.FromArgb((byte)Math.Clamp(a * (float)Brightness * 255f, 0, 255), c.R, c.G, c.B);
    }

    private static float AgeBehind(float beam, float a, float sgn)
    {
        float d = sgn * (beam - a);
        d %= Tau;
        if (d < 0) d += Tau;
        return d;
    }

    private Geo ComputeGeo()
    {
        float w = (float)ActualWidth, h = (float)ActualHeight;
        float r = (float)Math.Clamp(RadarScale, 0.05, 2) * Math.Min(w, h) / 2f;
        var c = new Vector2((float)Math.Clamp(CenterX, 0, 1) * w, (float)Math.Clamp(CenterY, 0, 1) * h);
        return new Geo(c, r, w, h);
    }

    private Matrix4x4? TiltMatrix(Geo g)
    {
        double tilt = Math.Clamp(Tilt, -80, 80);
        if (Math.Abs(tilt) < 0.1) return null;

        var persp = Matrix4x4.Identity;
        persp.M34 = -1f / (3.2f * g.R);

        return Matrix4x4.CreateTranslation(-g.C.X, -g.C.Y, 0) *
               Matrix4x4.CreateRotationX((float)(tilt * Math.PI / 180.0)) *
               persp *
               Matrix4x4.CreateTranslation(g.C.X, g.C.Y, 0);
    }

    private static Vector2? ToFlat(Vector2 p, Matrix4x4? tilt)
    {
        if (tilt is null) return p;

        var m = tilt.Value;
        float a = m.M11, b = m.M12, c = m.M14;
        float d = m.M21, e = m.M22, f = m.M24;
        float g = m.M41, h = m.M42, i = m.M44;

        float det = a * (e * i - f * h) - b * (d * i - f * g) + c * (d * h - e * g);
        if (MathF.Abs(det) < 1e-9f) return null;

        float i11 = e * i - f * h, i12 = c * h - b * i, i13 = b * f - c * e;
        float i21 = f * g - d * i, i22 = a * i - c * g, i23 = c * d - a * f;
        float i31 = d * h - e * g, i32 = b * g - a * h, i33 = a * e - b * d;

        float vx = p.X * i11 + p.Y * i21 + i31;
        float vy = p.X * i12 + p.Y * i22 + i32;
        float vz = p.X * i13 + p.Y * i23 + i33;
        if (MathF.Abs(vz) < 1e-9f) return null;

        return new Vector2(vx / vz, vy / vz);
    }

    private void EnsureClutter(int n)
    {
        if (_cR.Length == n) return;

        var rng = new Random(1234);
        _cR = new float[n];
        _cA = new float[n];
        _cS = new float[n];
        _cB = new float[n];

        for (int i = 0; i < n; i++)
        {
            _cR[i] = MathF.Sqrt((float)rng.NextDouble()) * 0.97f;
            _cA[i] = (float)rng.NextDouble() * Tau;
            _cS[i] = 0.6f + (float)rng.NextDouble() * 1.1f;
            _cB[i] = 0.35f + (float)rng.NextDouble() * 0.65f;
        }
    }

    private static Vector2 TargetPos(int i, Geo g, double tt)
    {
        float h1 = Hash(i, 21), h2 = Hash(i, 22), h3 = Hash(i, 23);
        float r = g.R * (0.15f + 0.72f * (0.5f + 0.5f * MathF.Sin((float)tt * 0.045f * (0.6f + h1 * 1.2f) + h2 * Tau)));
        float a = h3 * Tau + (float)tt * (0.04f + 0.08f * h1) * (h2 > 0.5f ? 1f : -1f);
        return Pt(g.C, r, a);
    }

    private static Vector2 Place(Geo g, double angleDegrees, double distance)
    {
        float a = (float)(angleDegrees * Math.PI / 180.0) - MathF.PI / 2f;
        return Pt(g.C, g.R * (float)Math.Clamp(distance, 0, 2), a);
    }

    private static void Evaluate(RadarBlip blip, Geo g, double time, out Vector2 pos, out float heading)
    {
        double angle = blip.Angle, distance = blip.Distance;
        bool autoHeading = double.IsNaN(blip.Heading);

        pos = Place(g, angle, distance);
        heading = autoHeading ? 0f : (float)(blip.Heading * Math.PI / 180.0);
        if (blip.Motion != RadarMotion.Linear) return;

        Vector2 start = pos;
        Vector2 end = Place(g,
            double.IsNaN(blip.EndAngle) ? angle : blip.EndAngle,
            double.IsNaN(blip.EndDistance) ? distance : blip.EndDistance);

        double x = Math.Max(0, time - blip.Origin) / Math.Max(0.05, blip.Duration);
        bool back = false;

        switch (blip.Repeat)
        {
            case RadarRepeat.Once:
                x = Math.Min(x, 1);
                break;

            case RadarRepeat.Loop:
                x -= Math.Floor(x);
                break;

            default:
                x %= 2;
                if (x > 1)
                {
                    x = 2 - x;
                    back = true;
                }
                break;
        }

        pos = Vector2.Lerp(start, end, (float)x);

        if (!autoHeading) return;

        Vector2 v = back ? start - end : end - start;
        if (v.LengthSquared() > 1e-6f) heading = MathF.Atan2(v.X, -v.Y);
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        if (ActualWidth < 2 || ActualHeight < 2) return;

        CanvasDrawingSession ds = args.DrawingSession;
        Geo g = ComputeGeo();
        bool light = LightMode;

        Color fg = Foreground is SolidColorBrush foreground ? foreground.Color : default;
        _col = Tint(fg);

        float introE = 1f - MathF.Pow(1f - (float)_intro, 3f);
        float gate = Smooth((introE - 0.3f) / 0.7f);
        Matrix4x4? tilt = TiltMatrix(g);
        Vector2? mouseFlat = _hasMouse && _mouseIn ? ToFlat(_mouseS, tilt) : null;

        try
        {
            if (Background is SolidColorBrush { Color.A: not 0 } background)
            {
                ds.FillRectangle(0, 0, g.W, g.H, background.Color);
            }

            float fade = (float)Math.Clamp(Fade, 0, 1);
            if (fade > 0.001f)
            {
                _maskStops[0] = new CanvasGradientStop { Position = 0f, Color = Color.FromArgb(255, 255, 255, 255) };
                _maskStops[1] = new CanvasGradientStop { Position = 1f - 0.75f * fade, Color = Color.FromArgb(255, 255, 255, 255) };
                _maskStops[2] = new CanvasGradientStop { Position = 1f, Color = Color.FromArgb((byte)(255 * (1f - fade)), 255, 255, 255) };

                var mask = new CanvasRadialGradientBrush(sender, _maskStops)
                {
                    Center = new Vector2(g.W / 2, g.H / 2),
                    RadiusX = g.W / 2,
                    RadiusY = g.H / 2,
                };
                _temps.Add(mask);
                _temps.Add(ds.CreateLayer(mask));
            }

            var cl = new CanvasCommandList(sender);
            _temps.Add(cl);
            using (CanvasDrawingSession cds = cl.CreateDrawingSession())
            {
                cds.Blend = light ? CanvasBlend.SourceOver : CanvasBlend.Add;
                DrawRadar(cds, sender, g, introE, gate, mouseFlat);
            }

            ICanvasImage img = cl;

            double frameDt = _dt;
            _dt = 0;
            float phosphor = (float)Math.Clamp(Phosphor, 0, 1);
            if (!light && phosphor > 0.001f) img = Accumulate(sender, cl, phosphor, frameDt);
            else ReleasePhosphor();

            float glow = (float)Math.Clamp(Glow, 0, 3);
            if (!light && glow > 0.001f)
            {
                var blur = new GaussianBlurEffect { Source = img, BlurAmount = 5f };
                var bloom = new ArithmeticCompositeEffect
                {
                    Source1 = img,
                    Source2 = blur,
                    Source1Amount = 1f,
                    Source2Amount = Math.Min(1.6f, glow * 0.9f),
                    MultiplyAmount = 0f,
                    Offset = 0f,
                    ClampOutput = true,
                };
                _temps.Add(blur);
                _temps.Add(bloom);
                img = bloom;
            }

            if (tilt is Matrix4x4 m)
            {
                var t3 = new Transform3DEffect
                {
                    Source = img,
                    TransformMatrix = m,
                    InterpolationMode = CanvasImageInterpolation.Anisotropic,
                };
                _temps.Add(t3);
                img = t3;
            }

            float gr = (float)Math.Clamp(Grain * 3.0, 0, 1);
            if (gr > 0.001f)
            {
                _noise ??= MakeNoise(sender);
                var tile = new TileEffect { Source = _noise, SourceRectangle = new Rect(0, 0, 128, 128) };
                var move = new Transform2DEffect { Source = tile, TransformMatrix = Matrix3x2.CreateTranslation(_grainOffset) };
                var grain = new ArithmeticCompositeEffect
                {
                    Source1 = img,
                    Source2 = move,
                    Source1Amount = 1f - gr,
                    Source2Amount = 0f,
                    MultiplyAmount = gr,
                    Offset = 0f,
                    ClampOutput = true,
                };
                _temps.Add(tile);
                _temps.Add(move);
                _temps.Add(grain);
                img = grain;
            }

            ds.DrawImage(img);
        }
        finally
        {
            for (int i = _temps.Count - 1; i >= 0; i--) _temps[i].Dispose();
            _temps.Clear();
        }
    }

    private static CanvasBitmap MakeNoise(ICanvasResourceCreator rc)
    {
        const int n = 128;
        var rng = new Random(7);
        var bytes = new byte[n * n * 4];

        for (int i = 0; i < n * n; i++)
        {
            byte v = (byte)rng.Next(256);
            bytes[i * 4] = v;
            bytes[i * 4 + 1] = v;
            bytes[i * 4 + 2] = v;
            bytes[i * 4 + 3] = 255;
        }

        return CanvasBitmap.CreateFromBytes(rc, bytes, n, n, DirectXPixelFormat.B8G8R8A8UIntNormalized);
    }

    private void DrawRadar(CanvasDrawingSession cds, ICanvasResourceCreator rc, Geo g, float introE, float gate, Vector2? mouseFlat)
    {
        bool pulse = Mode == RadarMode.Pulse;
        float sgn = Speed >= 0 ? 1f : -1f;
        float trail = (float)Math.Clamp(Trail, 0, 1);
        float glow = (float)Math.Clamp(Glow, 0, 3);

        DrawGrid(cds, g, 1f, introE);
        if (_warm > 0.003 && mouseFlat is Vector2 mf)
        {
            using var mb = new CanvasRadialGradientBrush(rc,
                Color.FromArgb((byte)(255 * _warm), 255, 255, 255), Color.FromArgb(0, 255, 255, 255))
            {
                Center = mf,
                RadiusX = 0.3f * g.R,
                RadiusY = 0.3f * g.R,
            };
            using var layer = cds.CreateLayer(mb);
            DrawGrid(cds, g, 2.4f, introE);
        }

        bool expanding = Speed >= 0;
        if (pulse)
        {
            float phase = (float)(_t * Speed * 0.25);
            for (int k = 0; k < 3; k++)
            {
                _ringR[k] = g.R * Frac(phase + k / 3f);
                float t01 = expanding ? _ringR[k] / g.R : 1f - _ringR[k] / g.R;
                float a = 0.85f * MathF.Pow(1f - t01, 1.4f) * MathF.Min(1f, t01 * 8f) * gate;
                if (a <= 0.003f || _ringR[k] < 1f) continue;

                cds.DrawCircle(g.C, _ringR[k], Col(a), 2f);
                cds.DrawCircle(g.C, _ringR[k], Col(a * 0.25f), 6f);
            }
        }
        else if (gate > 0.003f)
        {
            DrawSweep(cds, rc, g, sgn, trail, glow, gate);
        }

        int cn = (int)Math.Round(Math.Clamp(Clutter, 0, 1) * 500);
        if (cn > 0)
        {
            EnsureClutter(cn);
            float tauC = 0.25f + trail * 2.5f;
            for (int i = 0; i < cn; i++)
            {
                float lit = pulse
                    ? PulseLit(_ringR, _cR[i] * g.R, g.R, expanding, trail)
                    : MathF.Exp(-AgeBehind(_angle, _cA[i], sgn) / tauC);
                float intensity = _cB[i] * lit * gate;
                if (intensity < 0.03f) continue;

                cds.FillCircle(Pt(g.C, _cR[i] * g.R, _cA[i]), _cS[i], Col(intensity * 0.9f));
            }
        }

        int jamFrame = (int)(_t * 14);
        DrawZones(cds, rc, g, gate);
        DrawJams(cds, g, gate, jamFrame);
        DrawScale(cds, g, introE);

        BuildContacts(g, mouseFlat);
        float tau = 0.3f + trail * 3f;
        float omegaS = MathF.Abs((float)Speed) * Omega0;
        float reachR2 = g.R * g.R * 1.1236f;
        float scanFade = (float)Math.Clamp(ScanFade, 0.1, 10);
        for (int ci = 0; ci < _contacts.Count; ci++)
        {
            Contact c = _contacts[ci];
            float reach = c.Cursor ? 1f : 1f - Smooth((c.Dist - g.R) / (0.04f * g.R));
            if (reach <= 0.003f) continue;
            if (!c.Cursor && IsJammed(c, ci, jamFrame)) continue;

            float decay = c.ScanOnly ? scanFade : 1f;
            float intensity, age = 0;
            if (pulse)
            {
                intensity = PulseLit(_ringR, c.Dist, g.R, expanding, trail, decay);
            }
            else
            {
                age = AgeBehind(_angle, c.Ang, sgn);
                intensity = MathF.Exp(-age * decay / tau);
            }

            float vis = gate * reach;
            intensity *= vis;

            if (c.ScanOnly && intensity < 0.01f) continue;

            bool ghosts = !pulse && !c.Cursor && omegaS > 0.01f &&
                          (c.Blip is null || c.Blip.Motion == RadarMotion.Linear);
            if (ghosts)
            {
                double dtCross = age / omegaS;
                for (int k = 1; k <= 3; k++)
                {
                    double tk = _t - dtCross - k * Tau / omegaS;
                    Vector2 pk;
                    if (c.Blip is null) pk = TargetPos(c.Index, g, tk);
                    else Evaluate(c.Blip, g, tk, out pk, out _);

                    if (Vector2.DistanceSquared(pk, g.C) > reachR2) continue;

                    float a = MathF.Pow(0.55f, k) * 0.8f * MathF.Exp(-age * decay / (2f * tau)) * vis;
                    if (a > 0.02f) cds.FillCircle(pk, 1.8f, Col(c.Tone, a));
                }
            }

            if (c.Blip is { Motion: RadarMotion.Linear } tracked && tracked.TrailLength > 0)
            {
                DrawTrail(cds, tracked, g, c.Tone, c.ScanOnly ? intensity : vis);
            }

            float markerAlpha = c.ScanOnly ? intensity : (0.12f + 0.88f * intensity) * vis;
            DrawMarker(cds, rc, c, Col(c.Tone, markerAlpha));

            if (c.State != RadarBlipState.Normal)
            {
                DrawState(cds, c, c.ScanOnly ? intensity : MathF.Max(markerAlpha, 0.6f * vis));
            }

            if (!string.IsNullOrEmpty(c.Label))
            {
                float extent = MarkerExtent(c);
                DrawText(cds, c.Label, c.Pos + new Vector2(extent + 4f, -extent), Col(c.Tone, markerAlpha * 0.9f), CanvasHorizontalAlignment.Left);
            }

            if (intensity > 0.05f) cds.FillCircle(c.Pos, MathF.Max(6f, c.Size * 2.5f), Col(c.Tone, 0.25f * intensity));
            if (!pulse && age < 0.9f)
            {
                cds.DrawCircle(c.Pos, 3f + age * 22f, Col(c.Tone, 0.8f * (1f - age / 0.9f) * vis), 1f);
            }
        }

        cds.FillCircle(g.C, 2.5f, Col(0.9f * introE));

        if (ShowReadout && mouseFlat is Vector2 cursor)
        {
            Vector2 offset = cursor - g.C;
            float dist = offset.Length();
            if (dist < g.R)
            {
                float bearing = MathF.Atan2(offset.X, -offset.Y) * 180f / MathF.PI;
                if (bearing < 0) bearing += 360f;

                double range = dist / g.R * MaxRange;
                string text = string.Create(CultureInfo.InvariantCulture, $"{bearing:000}°  {range:0.#} {RangeUnit}");
                DrawText(cds, text.TrimEnd(), cursor + new Vector2(12f, 14f), Col(0.9f * gate), CanvasHorizontalAlignment.Left);
            }
        }
    }

    private void DrawMarker(CanvasDrawingSession cds, ICanvasResourceCreator rc, Contact c, Color color)
    {
        int shape = (int)c.Marker;
        if (shape <= 0 || shape >= MarkerShapes.Length)
        {
            cds.FillCircle(c.Pos, c.Size, color);
            return;
        }

        CanvasGeometry geometry = _markerGeo[shape] ??= CanvasGeometry.CreatePolygon(rc, MarkerShapes[shape]);

        Matrix3x2 saved = cds.Transform;
        cds.Transform = Matrix3x2.CreateScale(c.Size * MarkerScale) *
                        Matrix3x2.CreateRotation(c.Heading) *
                        Matrix3x2.CreateTranslation(c.Pos) *
                        saved;
        cds.FillGeometry(geometry, color);
        cds.Transform = saved;
    }

    private void AddBlipContact(RadarBlip blip, Geo g, RadarVisibility fallback)
    {
        if (blip.Reset)
        {
            blip.Origin = _t;
            blip.Reset = false;
        }

        Evaluate(blip, g, _t, out Vector2 p, out float heading);
        Vector2 v = p - g.C;
        RadarVisibility visibility = blip.Visibility == RadarVisibility.Inherit ? fallback : blip.Visibility;

        _contacts.Add(new Contact
        {
            Pos = p,
            Ang = MathF.Atan2(v.Y, v.X),
            Dist = v.Length(),
            Index = -1,
            Size = (float)Math.Clamp(blip.Size, 0.5, 24),
            Heading = heading,
            Marker = blip.Marker,
            ScanOnly = visibility == RadarVisibility.ScanOnly,
            Label = blip.Label,
            State = blip.State,
            Tone = Resolve(blip.Foreground),
            Blip = blip,
        });
    }

    private static float MarkerExtent(Contact c)
    {
        return c.Marker == RadarMarker.Dot ? c.Size : c.Size * MarkerScale;
    }

    private CanvasTextFormat EnsureText()
    {
        string family = FontFamily?.Source ?? "Segoe UI";
        float size = (float)Math.Max(1, FontSize);
        if (_text != null && _textSize == size && _textFamily == family) return _text;

        SafeDispose(_text);
        _text = new CanvasTextFormat
        {
            FontFamily = family,
            FontSize = size,
            WordWrapping = CanvasWordWrapping.NoWrap,
        };
        _textFamily = family;
        _textSize = size;
        return _text;
    }

    private void DrawText(CanvasDrawingSession cds, string text, Vector2 p, Color color, CanvasHorizontalAlignment align)
    {
        CanvasTextFormat format = EnsureText();
        format.HorizontalAlignment = align;
        format.VerticalAlignment = CanvasVerticalAlignment.Center;

        const float width = 256f;
        float height = _textSize * 3f;
        float x = align switch
        {
            CanvasHorizontalAlignment.Center => p.X - width / 2f,
            CanvasHorizontalAlignment.Right => p.X - width,
            _ => p.X,
        };

        cds.DrawText(text, new Rect(x, p.Y - height / 2f, width, height), color, format);
    }

    private CanvasStrokeStyle EnsureDash()
    {
        return _dash ??= new CanvasStrokeStyle { DashStyle = CanvasDashStyle.Dash };
    }

    private void EnsureRingText(int rings)
    {
        double max = MaxRange;
        string unit = RangeUnit ?? string.Empty;
        if (_ringText.Length == rings && _ringTextMax == max && _ringTextUnit == unit) return;

        _ringText = new string[rings];
        for (int i = 1; i <= rings; i++)
        {
            string value = (max * i / rings).ToString("0.##", CultureInfo.InvariantCulture);
            _ringText[i - 1] = i == rings && unit.Length > 0 ? value + " " + unit : value;
        }

        _ringTextMax = max;
        _ringTextUnit = unit;
    }

    private void DrawScale(CanvasDrawingSession cds, Geo g, float introE)
    {
        bool labels = ShowRangeLabels, compass = ShowCompass;
        if (!labels && !compass) return;

        float a = Smooth(introE) * (float)Math.Clamp(GridOpacity, 0, 2);
        if (a <= 0.003f) return;

        if (labels)
        {
            int rings = Math.Clamp(RingCount, 0, 24);
            EnsureRingText(rings);

            for (int i = 1; i <= rings; i++)
            {
                Vector2 p = Pt(g.C, g.R * i / rings, -MathF.PI / 4f) + new Vector2(3f, 0f);
                DrawText(cds, _ringText[i - 1], p, Col(0.55f * a), CanvasHorizontalAlignment.Left);
            }
        }

        if (!compass) return;

        for (int k = 0; k < CompassLetters.Length; k++)
        {
            float angle = k * MathF.PI / 2f - MathF.PI / 2f;
            Vector2 outer = Pt(g.C, g.R * 1.14f, angle);
            bool fits = outer.X > 10f && outer.X < g.W - 10f && outer.Y > 10f && outer.Y < g.H - 10f;
            Vector2 p = fits ? outer : Pt(g.C, g.R * 0.9f, angle);
            DrawText(cds, CompassLetters[k], p, Col((k == 0 ? 0.9f : 0.6f) * a), CanvasHorizontalAlignment.Center);
        }
    }

    private void DrawTrail(CanvasDrawingSession cds, RadarBlip blip, Geo g, Color tone, float alpha)
    {
        if (alpha <= 0.01f) return;

        const int steps = 24;
        double duration = Math.Max(0.05, blip.Duration);
        double step = Math.Clamp(blip.TrailLength, 0, 120) / steps;
        double cycle = Math.Floor(Math.Max(0, _t - blip.Origin) / duration);
        float lw = (float)Math.Max(0.25, LineWidth);

        Evaluate(blip, g, _t, out Vector2 prev, out _);
        for (int i = 1; i <= steps; i++)
        {
            double time = _t - i * step;
            if (time < blip.Origin) break;
            if (blip.Repeat == RadarRepeat.Loop && Math.Floor((time - blip.Origin) / duration) != cycle) break;

            Evaluate(blip, g, time, out Vector2 p, out _);
            float a = 0.6f * (1f - (i - 1f) / steps) * alpha;
            cds.DrawLine(prev, p, Col(tone, a), lw);
            prev = p;
        }
    }

    private void DrawState(CanvasDrawingSession cds, Contact c, float alpha)
    {
        if (alpha <= 0.01f) return;

        float r = MathF.Max(8f, MarkerExtent(c) * 1.6f);
        float lw = (float)Math.Max(0.25, LineWidth);

        if (c.State == RadarBlipState.Selected)
        {
            Color color = Col(c.Tone, alpha);
            float len = r * 0.5f;
            for (int sx = -1; sx <= 1; sx += 2)
            {
                for (int sy = -1; sy <= 1; sy += 2)
                {
                    Vector2 corner = c.Pos + new Vector2(sx * r, sy * r);
                    cds.DrawLine(corner, corner - new Vector2(sx * len, 0f), color, lw * 1.4f);
                    cds.DrawLine(corner, corner - new Vector2(0f, sy * len), color, lw * 1.4f);
                }
            }

            return;
        }

        float phase = Frac((float)_t * 1.2f);
        cds.DrawCircle(c.Pos, r * 0.6f, Col(c.Tone, alpha * 0.5f), lw);
        cds.DrawCircle(c.Pos, r * (0.6f + 0.9f * phase), Col(c.Tone, alpha * (1f - phase)), lw * 1.5f);
    }

    private static bool InSector(float bearing, double start, double end)
    {
        double width = ((end - start) % 360 + 360) % 360;
        if (width < 0.01) return false;

        double d = ((bearing - start) % 360 + 360) % 360;
        return d <= width;
    }

    private bool IsJammed(Contact c, int index, int frame)
    {
        IList<RadarJam>? jams = Jams;
        if (jams is null || jams.Count == 0) return false;

        float bearing = c.Ang * 180f / MathF.PI + 90f;
        for (int i = 0; i < jams.Count; i++)
        {
            RadarJam jam = jams[i];
            if (!InSector(bearing, jam.StartAngle, jam.EndAngle)) continue;
            if (Hash(index + 31 * i, frame) < Math.Clamp(jam.Intensity, 0, 1)) return true;
        }

        return false;
    }

    private void DrawJams(CanvasDrawingSession cds, Geo g, float gate, int frame)
    {
        IList<RadarJam>? jams = Jams;
        if (jams is null || jams.Count == 0 || gate <= 0.003f) return;

        float lw = (float)Math.Max(0.25, LineWidth);
        for (int i = 0; i < jams.Count; i++)
        {
            RadarJam jam = jams[i];
            float intensity = (float)Math.Clamp(jam.Intensity, 0, 1);
            double width = ((jam.EndAngle - jam.StartAngle) % 360 + 360) % 360;
            if (intensity < 0.01f || width < 0.01) continue;

            Color tone = Resolve(jam.Foreground);
            float start = (float)(jam.StartAngle * Math.PI / 180.0) - MathF.PI / 2f;
            float span = (float)(width * Math.PI / 180.0);

            cds.DrawLine(g.C, Pt(g.C, g.R, start), Col(tone, 0.12f * gate), lw);
            cds.DrawLine(g.C, Pt(g.C, g.R, start + span), Col(tone, 0.12f * gate), lw);

            int count = (int)(intensity * (30f + 400f * span / Tau));
            for (int k = 0; k < count; k++)
            {
                float h1 = Hash(k + 97 * i, frame * 3 + 1);
                float h2 = Hash(k + 97 * i, frame * 3 + 2);
                float h3 = Hash(k + 97 * i, frame * 3 + 3);

                float r = MathF.Sqrt(h1) * g.R;
                float a = start + h2 * span;
                Vector2 p0 = Pt(g.C, r, a);
                Vector2 p1 = Pt(g.C, MathF.Min(g.R, r + 2f + h3 * 12f), a);
                cds.DrawLine(p0, p1, Col(tone, (0.15f + 0.5f * h3) * intensity * gate), lw);
            }
        }
    }

    private CanvasGeometry? CreateZoneGeometry(ICanvasResourceCreator rc, RadarZone zone, Geo g, out Vector2 anchor)
    {
        if (zone.Shape == RadarZoneShape.Circle)
        {
            anchor = Place(g, zone.Angle, zone.Distance);
            return CanvasGeometry.CreateCircle(rc, anchor, g.R * (float)Math.Clamp(zone.Radius, 0.005, 2));
        }

        anchor = g.C;
        PointCollection? points = zone.Points;
        if (points is null || points.Count < 3) return null;

        if (_polyBuf.Length != points.Count) _polyBuf = new Vector2[points.Count];

        Vector2 sum = Vector2.Zero;
        for (int i = 0; i < points.Count; i++)
        {
            Point point = points[i];
            _polyBuf[i] = Place(g, point.X, point.Y);
            sum += _polyBuf[i];
        }

        anchor = sum / points.Count;
        return CanvasGeometry.CreatePolygon(rc, _polyBuf);
    }

    private void PaintZone(CanvasDrawingSession cds, CanvasGeometry shape, Color tone, float fill, float alpha, float lw, bool dashed)
    {
        if (fill > 0.001f) cds.FillGeometry(shape, Col(tone, fill * alpha));

        Color stroke = Col(tone, 0.7f * alpha);
        if (dashed) cds.DrawGeometry(shape, stroke, lw, EnsureDash());
        else cds.DrawGeometry(shape, stroke, lw);
    }

    private void DrawZones(CanvasDrawingSession cds, ICanvasResourceCreator rc, Geo g, float gate)
    {
        IList<RadarZone>? zones = Zones;
        if (zones is null || zones.Count == 0 || gate <= 0.003f) return;

        bool pulse = Mode == RadarMode.Pulse;
        bool expanding = Speed >= 0;
        float sgn = expanding ? 1f : -1f;
        float trail = (float)Math.Clamp(Trail, 0, 1);
        float tau = 0.3f + trail * 3f;
        float decay = (float)Math.Clamp(ScanFade, 0.1, 10);
        float lw = (float)Math.Max(0.25, LineWidth);
        float far = g.R * 2.6f;

        const int slices = 36;
        float da = Tau / slices;

        for (int i = 0; i < zones.Count; i++)
        {
            RadarZone zone = zones[i];
            using CanvasGeometry? shape = CreateZoneGeometry(rc, zone, g, out Vector2 anchor);
            if (shape is null) continue;

            Color tone = Resolve(zone.Foreground);
            float fill = (float)Math.Clamp(zone.FillOpacity, 0, 1);
            float labelAlpha = gate;

            if (zone.Visibility != RadarVisibility.ScanOnly)
            {
                PaintZone(cds, shape, tone, fill, gate, lw, zone.Dashed);
            }
            else if (pulse)
            {
                labelAlpha = PulseLit(_ringR, Vector2.Distance(anchor, g.C), g.R, expanding, trail, decay) * gate;
                if (labelAlpha >= 0.01f) PaintZone(cds, shape, tone, fill, labelAlpha, lw, zone.Dashed);
            }
            else
            {
                Vector2 d = anchor - g.C;
                labelAlpha = MathF.Exp(-AgeBehind(_angle, MathF.Atan2(d.Y, d.X), sgn) * decay / tau) * gate;

                for (int s = 0; s < slices; s++)
                {
                    float a0 = s * da;
                    float a = MathF.Exp(-AgeBehind(_angle, a0 + da * 0.5f, sgn) * decay / tau) * gate;
                    if (a < 0.01f) continue;

                    _wedge[0] = g.C;
                    _wedge[1] = Pt(g.C, far, a0);
                    _wedge[2] = Pt(g.C, far, a0 + da * 0.52f);
                    _wedge[3] = Pt(g.C, far, a0 + da * 1.04f);

                    using CanvasGeometry wedge = CanvasGeometry.CreatePolygon(rc, _wedge);
                    using CanvasActiveLayer layer = cds.CreateLayer(1f, wedge);
                    PaintZone(cds, shape, tone, fill, a, lw, zone.Dashed);
                }
            }

            if (!string.IsNullOrEmpty(zone.Label) && labelAlpha > 0.02f)
            {
                DrawText(cds, zone.Label, anchor, Col(tone, 0.8f * labelAlpha), CanvasHorizontalAlignment.Center);
            }
        }
    }

    private ICanvasImage Accumulate(CanvasControl sender, ICanvasImage frame, float amount, double dt)
    {
        Size size = sender.Size;
        float w = (float)size.Width, h = (float)size.Height, dpi = sender.Dpi;

        if (_phosphorA is null || _phosphorB is null ||
            MathF.Abs(_phosphorW - w) > 0.5f || MathF.Abs(_phosphorH - h) > 0.5f ||
            MathF.Abs(_phosphorA.Dpi - dpi) > 0.01f)
        {
            ReleasePhosphor();

            _phosphorA = new CanvasRenderTarget(sender, w, h, dpi, DirectXPixelFormat.R16G16B16A16Float, CanvasAlphaMode.Premultiplied);
            _phosphorB = new CanvasRenderTarget(sender, w, h, dpi, DirectXPixelFormat.R16G16B16A16Float, CanvasAlphaMode.Premultiplied);
            _phosphorW = w;
            _phosphorH = h;

            using (CanvasDrawingSession clear = _phosphorA.CreateDrawingSession()) clear.Clear(Color.FromArgb(0, 0, 0, 0));
        }

        CanvasRenderTarget prev = _phosphorA, next = _phosphorB;
        float keep = (float)Math.Exp(-dt / Math.Max(0.02, amount * 1.5));

        using (var faded = new OpacityEffect { Source = prev, Opacity = keep })
        using (var merged = new BlendEffect { Background = faded, Foreground = frame, Mode = BlendEffectMode.Lighten })
        using (CanvasDrawingSession s = next.CreateDrawingSession())
        {
            s.Clear(Color.FromArgb(0, 0, 0, 0));
            s.DrawImage(merged);
        }

        _phosphorA = next;
        _phosphorB = prev;
        return next;
    }

    private void ReleasePhosphor()
    {
        if (_phosphorA is null && _phosphorB is null) return;

        SafeDispose(_phosphorA);
        SafeDispose(_phosphorB);
        _phosphorA = null;
        _phosphorB = null;
    }

    private static float PulseLit(float[] rings, float dist, float radius, bool expanding, float trail, float decay = 1f)
    {
        float best = 0, len = radius * (0.05f + trail * 0.35f) / decay;
        for (int k = 0; k < rings.Length; k++)
        {
            float dr = expanding ? rings[k] - dist : dist - rings[k];
            if (dr < 0) continue;

            best = MathF.Max(best, MathF.Exp(-dr / len) * (1f - 0.5f * rings[k] / radius));
        }
        return best;
    }

    private void BuildContacts(Geo g, Vector2? mouseFlat)
    {
        _contacts.Clear();

        Color targetTone = Resolve(TargetBrush);
        bool targetsScanOnly = TargetVisibility == RadarVisibility.ScanOnly;
        RadarVisibility blipDefault = BlipVisibility;
        int n = Math.Clamp(Targets, 0, 16);
        for (int i = 0; i < n; i++)
        {
            Vector2 p = TargetPos(i, g, _t);
            Vector2 v = p - g.C;
            _contacts.Add(new Contact
            {
                Pos = p,
                Ang = MathF.Atan2(v.Y, v.X),
                Dist = v.Length(),
                Index = i,
                Size = TargetSize,
                ScanOnly = targetsScanOnly,
                Tone = targetTone,
            });
        }

        IList<RadarBlip>? blips = Blips;
        if (blips != null)
        {
            for (int i = 0; i < blips.Count; i++) AddBlipContact(blips[i], g, blipDefault);
        }

        for (int i = 0; i < _boundList.Count; i++) AddBlipContact(_boundList[i], g, blipDefault);

        if (mouseFlat is Vector2 m && MouseInteraction)
        {
            Vector2 v = m - g.C;
            if (v.Length() < g.R)
            {
                _contacts.Add(new Contact
                {
                    Pos = m,
                    Ang = MathF.Atan2(v.Y, v.X),
                    Dist = v.Length(),
                    Index = -1,
                    Cursor = true,
                    Size = TargetSize,
                    Tone = _col,
                });
            }
        }
    }

    private void DrawGrid(CanvasDrawingSession cds, Geo g, float aMul, float introE)
    {
        float lw = (float)Math.Max(0.25, LineWidth);
        float ga = (float)Math.Clamp(GridOpacity, 0, 2) * aMul;

        int rings = Math.Clamp(RingCount, 0, 24);
        for (int i = 1; i <= rings; i++)
        {
            float p = Smooth(introE * (1f + 0.5f * rings) - 0.5f * (i - 1));
            bool outer = i == rings;
            DrawRing(cds, g.C, g.R * i / rings, p, Col((outer ? 0.55f : 0.22f) * ga), lw * (outer ? 1.4f : 1f));
        }

        int spokes = Math.Clamp(SpokeCount, 0, 72);
        for (int k = 0; k < spokes; k++)
        {
            float a = k * Tau / spokes - MathF.PI / 2f;
            cds.DrawLine(g.C, Pt(g.C, g.R * introE, a), Col(0.10f * ga), lw);
        }

        if (!Ticks) return;

        for (int k = 0; k < 72; k++)
        {
            float a = k * Tau / 72f - MathF.PI / 2f;
            bool major = k % 6 == 0;
            float r0 = g.R * 1.02f, r1 = g.R * (major ? 1.075f : 1.045f);
            cds.DrawLine(Pt(g.C, r0, a), Pt(g.C, r1, a), Col((major ? 0.55f : 0.30f) * ga * introE), lw);
        }
    }

    private static void DrawRing(CanvasDrawingSession cds, Vector2 c, float r, float p, Color color, float lw)
    {
        if (p <= 0.003f || r < 1f) return;

        if (p >= 0.999f)
        {
            cds.DrawCircle(c, r, color, lw);
            return;
        }

        float a0 = -MathF.PI / 2f, a1 = a0 + p * Tau;
        using var pb = new CanvasPathBuilder(cds);
        pb.BeginFigure(Pt(c, r, a0));
        pb.AddArc(Pt(c, r, a1), r, r, 0f, CanvasSweepDirection.Clockwise,
                  p > 0.5f ? CanvasArcSize.Large : CanvasArcSize.Small);
        pb.EndFigure(CanvasFigureLoop.Open);
        using var geo = CanvasGeometry.CreatePath(pb);
        cds.DrawGeometry(geo, color, lw);
    }

    private void DrawSweep(CanvasDrawingSession cds, ICanvasResourceCreator rc, Geo g, float sgn, float trail, float glow, float gate)
    {
        float span = trail * Tau * 0.95f;
        if (span > 0.01f)
        {
            _rimStops[0] = new CanvasGradientStop { Position = 0f, Color = Col(0.15f) };
            _rimStops[1] = new CanvasGradientStop { Position = 0.6f, Color = Col(0.55f) };
            _rimStops[2] = new CanvasGradientStop { Position = 1f, Color = Col(1.00f) };

            using var rim = new CanvasRadialGradientBrush(rc, _rimStops)
            {
                Center = g.C,
                RadiusX = g.R,
                RadiusY = g.R,
            };

            const int slices = 64;
            float da = span / slices;
            for (int s = 0; s < slices; s++)
            {
                float t0 = s / (float)slices;
                float a0 = _angle - sgn * t0 * span;
                float a1 = a0 - sgn * da * 1.6f;

                rim.Opacity = Math.Clamp(MathF.Pow(1f - t0, 2.2f) * 0.55f * Math.Max(0.3f, glow) * gate, 0f, 1f);
                if (rim.Opacity < 0.004f) break;

                _wedge[0] = g.C;
                _wedge[1] = Pt(g.C, g.R, a0);
                _wedge[2] = Pt(g.C, g.R, (a0 + a1) / 2f);
                _wedge[3] = Pt(g.C, g.R, a1);

                using var wedge = CanvasGeometry.CreatePolygon(rc, _wedge);
                cds.FillGeometry(wedge, rim);
            }
        }

        Vector2 tip = Pt(g.C, g.R, _angle);
        using var beam = new CanvasLinearGradientBrush(rc, Col(0.10f * gate), Col(1.0f * gate)) { StartPoint = g.C, EndPoint = tip };
        cds.DrawLine(g.C, tip, beam, 6f);
        cds.DrawLine(g.C, tip, beam, 2f);
        cds.FillCircle(tip, 4f, Col(0.5f * gate));
    }
}
