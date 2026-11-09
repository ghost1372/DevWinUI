using Microsoft.UI.Xaml.Input;
using WinRT;

namespace DevWinUI;

/// <summary>
/// An animated gradient border with a cursor-following glow and three self-animating modes.
/// Rendered with Win2D: WinUI has no conic gradient, so sweeps are pre-rendered bitmaps that are
/// rotated, masked to a ring geometry and blurred for the glow. Padding creates an outer frame.
/// </summary>
[TemplatePart(Name = PartRoot, Type = typeof(FrameworkElement))]
[TemplatePart(Name = PartCanvas, Type = typeof(CanvasControl))]
[TemplatePart(Name = PartContentBox, Type = typeof(Border))]
public sealed partial class GlowingBorder : ContentControl
{
    private const string PartRoot = "PART_Root";
    private const string PartCanvas = "PART_Canvas";
    private const string PartContentBox = "PART_ContentBox";

    private const int SweepSize = 512;
    private const double MaxFrameTime = 0.1;
    private const double IntensitySmoothing = 0.12;
    private const double PoolSmoothing = 0.06;
    private const double IntensityEpsilon = 0.003;
    private const double BeamTrail = 0.38;
    private const double BeamRestingOpacity = 0.14;
    private const double InnerGlowGain = 0.9;
    private const float MaskCoverage = 1.05f;

    private static readonly Color MaskColor = Colors.White;

    private static readonly CanvasGradientStop[] PoolStops =
    {
        new() { Position = 0.00f, Color = Color.FromArgb(255, 255, 255, 255) },
        new() { Position = 0.40f, Color = Color.FromArgb(190, 255, 255, 255) },
        new() { Position = 0.75f, Color = Color.FromArgb(50, 255, 255, 255) },
        new() { Position = 1.00f, Color = Color.FromArgb(0, 255, 255, 255) },
    };

    private static float[]? s_sweepFractions;

    private readonly List<Color> _palette = new();
    private readonly PointerEventHandler _pointerMovedHandler;
    private readonly PointerEventHandler _pointerExitedHandler;

    private CanvasControl? _canvas;
    private FrameworkElement? _root;
    private Border? _contentBox;

    private CanvasBitmap? _colorBitmap;
    private CanvasBitmap? _beamBitmap;
    private CanvasImageBrush? _colorBrush;
    private CanvasImageBrush? _beamBrush;
    private CanvasRadialGradientBrush? _poolBrush;
    private CanvasGeometry? _ring;
    private CanvasGeometry? _inside;
    private CanvasCommandList? _outsideMask;
    private CanvasCommandList? _insideMask;
    private (float W, float H, float R, float T, float Pad) _geometryKey;
    private double _pad;
    private bool _bitmapsDirty = true;

    private UIElement? _pointerHost;
    private TimeSpan _lastFrame;
    private double _time;
    private double _intensity;
    private Vector2 _pointer;
    private Vector2 _pool;
    private bool _pointerInside;
    private bool _hasLastFrame;
    private bool _rendering;
    private bool _loaded;

    public GlowingBorder()
    {
        DefaultStyleKey = typeof(GlowingBorder);

        _pointerMovedHandler = OnHostPointerMoved;
        _pointerExitedHandler = OnHostPointerExited;

        RegisterPropertyChangedCallback(ForegroundProperty, OnForegroundChanged);
        ParsePalette();

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        if (_canvas is not null)
        {
            _canvas.Draw -= OnDraw;
            _canvas.CreateResources -= OnCreateResources;
        }

        base.OnApplyTemplate();

        _root = GetTemplateChild(PartRoot) as FrameworkElement;
        _canvas = GetTemplateChild(PartCanvas) as CanvasControl;
        _contentBox = GetTemplateChild(PartContentBox) as Border;

        if (_canvas is not null)
        {
            _canvas.Draw += OnDraw;
            _canvas.CreateResources += OnCreateResources;
        }

        ApplyLayout();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;

        _pointerHost = XamlRoot?.Content;
        if (_pointerHost is not null)
        {
            _pointerHost.AddHandler(PointerMovedEvent, _pointerMovedHandler, true);
            _pointerHost.AddHandler(PointerExitedEvent, _pointerExitedHandler, true);
        }

        RequestRedraw();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        StopFrames();

        if (_pointerHost is not null)
        {
            _pointerHost.RemoveHandler(PointerMovedEvent, _pointerMovedHandler);
            _pointerHost.RemoveHandler(PointerExitedEvent, _pointerExitedHandler);
            _pointerHost = null;
        }

        DisposeResources();
    }

    private void OnCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args) => DisposeResources();

    private void OnForegroundChanged(DependencyObject sender, DependencyProperty property) => RefreshPalette();

    private void ApplyLayout()
    {
        _pad = Glow > 0 || GlowSize > 0 ? Math.Ceiling(GlowSize * 2) + 2 : 2;

        if (_canvas is not null) _canvas.Margin = new Thickness(-_pad);

        if (_contentBox is not null)
        {
            _contentBox.Margin = new Thickness(RingThickness);
            _contentBox.CornerRadius = new CornerRadius(Math.Max(0, Radius - RingThickness));
        }

        RequestRedraw();
    }

    private void RefreshPalette()
    {
        ParsePalette();
        _bitmapsDirty = true;
        RequestRedraw();
    }

    private void ParsePalette()
    {
        _palette.Clear();

        AddPaletteColor(GlowBrush1);
        AddPaletteColor(GlowBrush2);
        AddPaletteColor(GlowBrush3);
        AddPaletteColor(GlowBrush4);

        if (_palette.Count == 0)
        {
            _palette.Add(Foreground is SolidColorBrush brush ? brush.Color : MaskColor);
        }
    }

    private void AddPaletteColor(Brush? brush)
    {
        if (brush is SolidColorBrush solid) _palette.Add(solid.Color);
    }

    private void OnHostPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (Kind != GlowingBorderKind.Cursor || _root is null) return;

        var point = e.GetCurrentPoint(_root).Position;
        _pointer = new Vector2((float)point.X, (float)point.Y);
        _pointerInside = true;
        RequestFrame();
    }

    private void OnHostPointerExited(object sender, PointerRoutedEventArgs e)
    {
        if (Kind != GlowingBorderKind.Cursor) return;

        _pointerInside = false;
        RequestFrame();
    }

    private float TargetIntensity()
    {
        if (!_pointerInside || _root is null || Range <= 0) return 0;

        float dx = Math.Max(Math.Max(-_pointer.X, _pointer.X - (float)_root.ActualWidth), 0);
        float dy = Math.Max(Math.Max(-_pointer.Y, _pointer.Y - (float)_root.ActualHeight), 0);
        float t = 1f - (float)Math.Min(1.0, Math.Sqrt(dx * dx + dy * dy) / Range);

        return t * t * (3f - 2f * t);
    }

    private void RequestRedraw()
    {
        _canvas?.Invalidate();
        if (Kind != GlowingBorderKind.Cursor) RequestFrame();
    }

    private void RequestFrame()
    {
        if (_rendering || !_loaded) return;

        _rendering = true;
        _hasLastFrame = false;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopFrames()
    {
        if (!_rendering) return;

        _rendering = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        TimeSpan frameTime = e.As<RenderingEventArgs>().RenderingTime;
        double dt = _hasLastFrame ? Math.Clamp((frameTime - _lastFrame).TotalSeconds, 0, MaxFrameTime) : 0;
        _lastFrame = frameTime;
        _hasLastFrame = true;
        _time += dt;

        bool settled = true;

        if (Kind == GlowingBorderKind.Cursor)
        {
            float target = TargetIntensity();

            if (_intensity < 0.01 && target > 0) _pool = _pointer;

            _intensity += (target - _intensity) * (1 - Math.Exp(-dt / IntensitySmoothing));
            _pool += (_pointer - _pool) * (float)(1 - Math.Exp(-dt / PoolSmoothing));

            settled = Math.Abs(target - _intensity) < IntensityEpsilon && Vector2.Distance(_pool, _pointer) < 0.5f;
            if (settled)
            {
                _intensity = target;
                _pool = _pointer;
            }
        }

        _canvas?.Invalidate();

        if (Kind == GlowingBorderKind.Cursor && settled) StopFrames();
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        float pad = (float)_pad;
        float w = (float)(sender.ActualWidth - 2 * pad);
        float h = (float)(sender.ActualHeight - 2 * pad);
        if (w <= 0 || h <= 0) return;

        EnsureResources(sender, w, h, pad);
        if (_ring is null || _colorBrush is null) return;

        var ds = args.DrawingSession;
        double phase = (_time / Math.Max(0.1, Duration)) % 1.0;
        double breath = Kind == GlowingBorderKind.Pulse ? 0.65 - 0.35 * Math.Cos(2 * Math.PI * phase) : 1.0;

        using var lit = new CanvasCommandList(sender);
        using (var cds = lit.CreateDrawingSession())
        {
            DrawLit(cds, w, h, pad, (float)(phase * 360.0), (float)breath);
        }

        float glowAlpha = (float)Math.Clamp(Glow * breath, 0, 1);
        if (glowAlpha > 0.001f && _outsideMask is not null)
        {
            using var blur = new GaussianBlurEffect
            {
                Source = lit,
                BlurAmount = (float)Math.Max(0.1, GlowSize / 2),
                BorderMode = EffectBorderMode.Soft,
            };
            using var masked = new AlphaMaskEffect { Source = blur, AlphaMask = _outsideMask };
            using var faded = new OpacityEffect { Source = masked, Opacity = glowAlpha };
            ds.DrawImage(faded);
        }

        float innerAlpha = (float)Math.Clamp(InnerGlow, 0, 1);
        if (innerAlpha > 0.001f && _insideMask is not null)
        {
            using var blur = new GaussianBlurEffect
            {
                Source = lit,
                BlurAmount = (float)Math.Max(5, GlowSize * 0.75),
                BorderMode = EffectBorderMode.Soft,
            };
            using var masked = new AlphaMaskEffect { Source = blur, AlphaMask = _insideMask };
            using var faded = new OpacityEffect { Source = masked, Opacity = innerAlpha * (float)InnerGlowGain };
            ds.DrawImage(faded);
        }

        ds.DrawImage(lit);
    }

    private void DrawLit(CanvasDrawingSession cds, float w, float h, float pad, float angleDegrees, float breath)
    {
        var ring = _ring!;
        var color = _colorBrush!;
        var center = new Vector2(pad + w / 2f, pad + h / 2f);
        float scale = (float)Math.Sqrt(w * w + h * h) / SweepSize * MaskCoverage;

        switch (Kind)
        {
            case GlowingBorderKind.Cursor:
                {
                    if (_intensity < 0.002 || _poolBrush is null) break;

                    float radius = (float)Math.Max(1, Spot / 2);
                    _poolBrush.Opacity = (float)_intensity;
                    _poolBrush.Center = _pool + new Vector2(pad, pad);
                    _poolBrush.RadiusX = radius;
                    _poolBrush.RadiusY = radius;

                    color.Opacity = 1f;
                    color.Transform = SweepTransform(0, center, scale);
                    cds.FillGeometry(ring, color, _poolBrush);
                    break;
                }

            case GlowingBorderKind.Beam:
                {
                    var transform = SweepTransform(angleDegrees, center, scale);

                    color.Transform = transform;
                    color.Opacity = (float)BeamRestingOpacity;
                    cds.FillGeometry(ring, color);

                    if (_beamBrush is not null)
                    {
                        _beamBrush.Transform = transform;
                        color.Opacity = 1f;
                        cds.FillGeometry(ring, color, _beamBrush);
                    }
                    break;
                }

            case GlowingBorderKind.Spin:
                {
                    color.Opacity = 1f;
                    color.Transform = SweepTransform(angleDegrees, center, scale);
                    cds.FillGeometry(ring, color);
                    break;
                }

            default:
                {
                    color.Opacity = breath;
                    color.Transform = SweepTransform(0, center, scale);
                    cds.FillGeometry(ring, color);
                    break;
                }
        }
    }
    private static Matrix3x2 SweepTransform(float angleDegrees, Vector2 center, float scale) =>
        Matrix3x2.CreateTranslation(-SweepSize / 2f, -SweepSize / 2f)
        * Matrix3x2.CreateScale(scale)
        * Matrix3x2.CreateRotation(angleDegrees * (float)Math.PI / 180f)
        * Matrix3x2.CreateTranslation(center);

    private void EnsureResources(ICanvasResourceCreator creator, float w, float h, float pad)
    {
        if (_bitmapsDirty || _colorBrush is null)
        {
            DisposeBitmaps();

            int count = Kind == GlowingBorderKind.Cursor ? _palette.Count : Math.Min(3, _palette.Count);

            _colorBitmap = CanvasBitmap.CreateFromColors(creator, BuildColorSweep(count), SweepSize, SweepSize);
            _colorBrush = new CanvasImageBrush(creator, _colorBitmap)
            {
                Interpolation = CanvasImageInterpolation.Linear,
            };

            _poolBrush = new CanvasRadialGradientBrush(creator, PoolStops);

            if (Kind == GlowingBorderKind.Beam)
            {
                _beamBitmap = CanvasBitmap.CreateFromColors(creator, BuildBeamSweep(), SweepSize, SweepSize);
                _beamBrush = new CanvasImageBrush(creator, _beamBitmap)
                {
                    Interpolation = CanvasImageInterpolation.Linear,
                };
            }

            _bitmapsDirty = false;
        }

        float radius = (float)Radius;
        float thickness = (float)RingThickness;
        var key = (w, h, radius, thickness, pad);
        if (_ring is not null && key == _geometryKey) return;

        DisposeGeometry();
        _geometryKey = key;

        float innerRadius = Math.Max(0, radius - thickness);
        using var outerShape = CanvasGeometry.CreateRoundedRectangle(creator, pad, pad, w, h, radius, radius);
        var innerShape = CanvasGeometry.CreateRoundedRectangle(
            creator, pad + thickness, pad + thickness,
            Math.Max(0, w - 2 * thickness), Math.Max(0, h - 2 * thickness), innerRadius, innerRadius);

        _ring = outerShape.CombineWith(innerShape, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
        _inside = innerShape;

        using var everything = CanvasGeometry.CreateRectangle(creator, 0, 0, w + 2 * pad, h + 2 * pad);
        using var outside = everything.CombineWith(outerShape, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);

        _outsideMask = new CanvasCommandList(creator);
        using (var session = _outsideMask.CreateDrawingSession())
        {
            session.FillGeometry(outside, MaskColor);
        }

        _insideMask = new CanvasCommandList(creator);
        using (var session = _insideMask.CreateDrawingSession())
        {
            session.FillGeometry(innerShape, MaskColor);
        }
    }

    private static float[] GetSweepFractions()
    {
        if (s_sweepFractions is not null) return s_sweepFractions;

        var fractions = new float[SweepSize * SweepSize];
        double center = (SweepSize - 1) / 2.0;

        for (int y = 0; y < SweepSize; y++)
        {
            for (int x = 0; x < SweepSize; x++)
            {
                double angle = Math.Atan2(x - center, -(y - center));
                if (angle < 0) angle += 2 * Math.PI;
                fractions[y * SweepSize + x] = (float)(angle / (2 * Math.PI));
            }
        }

        s_sweepFractions = fractions;
        return fractions;
    }

    private Color[] BuildColorSweep(int count)
    {
        var fractions = GetSweepFractions();
        var pixels = new Color[SweepSize * SweepSize];

        for (int i = 0; i < pixels.Length; i++)
        {
            if (count == 1)
            {
                pixels[i] = _palette[0];
                continue;
            }

            double f = fractions[i] * count;
            int index = (int)f;
            float k = (float)(f - index);
            var a = _palette[index % count];
            var b = _palette[(index + 1) % count];

            pixels[i] = Color.FromArgb(
                255,
                (byte)(a.R + (b.R - a.R) * k),
                (byte)(a.G + (b.G - a.G) * k),
                (byte)(a.B + (b.B - a.B) * k));
        }

        return pixels;
    }

    private static Color[] BuildBeamSweep()
    {
        var fractions = GetSweepFractions();
        var pixels = new Color[SweepSize * SweepSize];

        for (int i = 0; i < pixels.Length; i++)
        {
            double behind = (1 - fractions[i]) % 1.0;
            double alpha = behind < BeamTrail ? Math.Pow(1 - behind / BeamTrail, 1.6) : 0;
            byte value = (byte)(alpha * 255);
            pixels[i] = Color.FromArgb(value, value, value, value);
        }

        return pixels;
    }

    private void DisposeGeometry()
    {
        _ring?.Dispose();
        _inside?.Dispose();
        _outsideMask?.Dispose();
        _insideMask?.Dispose();
        _ring = _inside = null;
        _outsideMask = _insideMask = null;
    }

    private void DisposeBitmaps()
    {
        _colorBrush?.Dispose();
        _beamBrush?.Dispose();
        _poolBrush?.Dispose();
        _colorBitmap?.Dispose();
        _beamBitmap?.Dispose();
        _colorBrush = _beamBrush = null;
        _poolBrush = null;
        _colorBitmap = _beamBitmap = null;
    }

    private void DisposeResources()
    {
        DisposeBitmaps();
        DisposeGeometry();
        _bitmapsDirty = true;
    }
}
