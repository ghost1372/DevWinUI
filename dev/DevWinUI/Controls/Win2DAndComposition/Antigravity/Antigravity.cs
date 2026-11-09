using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

[TemplatePart(Name = RootPartName, Type = typeof(Grid))]
public sealed partial class Antigravity : Control
{
    private const string RootPartName = "PART_Root";
    private const int LutSize = 256;
    private const int MaxParticles = 5000;
    private const float MaxFrameTime = 0.033f;
    private const long IdleThresholdMs = 2000;

    private static readonly Vector2[] TriangleVertices =
    {
        new(0f, -1f),
        new(0.866f, 0.5f),
        new(-0.866f, 0.5f),
    };

    private readonly object _inputLock = new();
    private readonly object _resourceLock = new();

    private Grid? _root;
    private CanvasAnimatedControl? _canvas;
    private CanvasGeometry[]? _triangleFaces;
    private CanvasStrokeStyle? _capStyle;

    private Color[] _lutLinear = Array.Empty<Color>();
    private Color[] _lutCyclic = Array.Empty<Color>();

    private Brush? _watchedBrush;
    private long _solidBrushToken;
    private readonly List<(GradientStop Stop, long ColorToken, long OffsetToken)> _watchedStops = new();

    private volatile AntigravitySettings _settings = new();
    private volatile bool _rebuildRequested = true;
    private volatile bool _burstPending;
    private volatile bool _pointerDown;

    private Vector2 _pointer;
    private bool _hasPointer;
    private long _lastMoveTicks;

    private AntigravityParticle[] _particles = Array.Empty<AntigravityParticle>();
    private Vector2 _mouse;
    private double _time;
    private float _sinceBuild;
    private float _ringMix;
    private float _press;
    private float _orbit;
    private float _colorPhase;

    public Antigravity()
    {
        DefaultStyleKey = typeof(Antigravity);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        RegisterPropertyChangedCallback(ForegroundProperty, OnForegroundChanged);
        RegisterPropertyChangedCallback(VisibilityProperty, OnVisibilityChanged);
        RefreshPalette();
    }

    public void Replay()
    {
        _burstPending = false;
        _rebuildRequested = true;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        DetachCanvas();
        _root = GetTemplateChild(RootPartName) as Grid;

        if (IsLoaded)
        {
            AttachCanvas();
        }
    }

    protected override void OnPointerMoved(PointerRoutedEventArgs e)
    {
        base.OnPointerMoved(e);
        if (IsInteractive)
        {
            TrackPointer(e);
        }
    }

    protected override void OnPointerPressed(PointerRoutedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!IsInteractive) return;

        TrackPointer(e);
        _pointerDown = true;
        CapturePointer(e.Pointer);
    }

    protected override void OnPointerReleased(PointerRoutedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_pointerDown && ClickBurst && IsInteractive)
        {
            _burstPending = true;
        }

        _pointerDown = false;
        ReleasePointerCapture(e.Pointer);
    }

    protected override void OnPointerCanceled(PointerRoutedEventArgs e)
    {
        base.OnPointerCanceled(e);
        _pointerDown = false;
        SetPointerPresence(false);
    }

    protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _pointerDown = false;
    }

    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);
        SetPointerPresence(false);
    }

    private static void OnSettingsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((Antigravity)sender).RefreshSettings();
    }

    private static void OnPaletteChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((Antigravity)sender).RefreshPalette();
    }

    private static void OnPausedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((Antigravity)sender).UpdatePausedState();
    }

    private void OnVisibilityChanged(DependencyObject sender, DependencyProperty property)
    {
        UpdatePausedState();
    }

    private void UpdatePausedState()
    {
        if (_canvas is not null)
        {
            _canvas.Paused = IsPaused || Visibility == Visibility.Collapsed;
        }
    }

    private void OnForegroundChanged(DependencyObject sender, DependencyProperty property)
    {
        if (Palette is null)
        {
            RefreshPalette();
        }
    }

    private void RefreshPalette()
    {
        var brush = Palette ?? Foreground;
        WatchBrush(brush);
        RebuildLuts(brush);
    }

    private void RebuildLuts(Brush? brush)
    {
        ReadPalette(brush, out var offsets, out var colors);
        _lutLinear = BuildLut(offsets, colors, cyclic: false);
        _lutCyclic = BuildLut(offsets, colors, cyclic: true);
        RefreshSettings();
    }

    private void WatchBrush(Brush? brush)
    {
        UnwatchBrush();

        switch (brush)
        {
            case SolidColorBrush solid:
                _watchedBrush = solid;
                _solidBrushToken = solid.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnWatchedBrushChanged);
                break;
            case GradientBrush gradient:
                _watchedBrush = gradient;
                foreach (var stop in gradient.GradientStops)
                {
                    var colorToken = stop.RegisterPropertyChangedCallback(GradientStop.ColorProperty, OnWatchedBrushChanged);
                    var offsetToken = stop.RegisterPropertyChangedCallback(GradientStop.OffsetProperty, OnWatchedBrushChanged);
                    _watchedStops.Add((stop, colorToken, offsetToken));
                }
                break;
        }
    }

    private void UnwatchBrush()
    {
        switch (_watchedBrush)
        {
            case SolidColorBrush solid:
                solid.UnregisterPropertyChangedCallback(SolidColorBrush.ColorProperty, _solidBrushToken);
                break;
            case GradientBrush:
                foreach (var (stop, colorToken, offsetToken) in _watchedStops)
                {
                    stop.UnregisterPropertyChangedCallback(GradientStop.ColorProperty, colorToken);
                    stop.UnregisterPropertyChangedCallback(GradientStop.OffsetProperty, offsetToken);
                }
                _watchedStops.Clear();
                break;
        }

        _watchedBrush = null;
    }

    private void OnWatchedBrushChanged(DependencyObject sender, DependencyProperty property)
    {
        RebuildLuts(Palette ?? Foreground);
    }

    private void RefreshSettings()
    {
        _settings = new AntigravitySettings
        {
            Count = Math.Clamp(ParticleCount, 0, MaxParticles),
            Magnet = MathF.Max(1f, (float)MagnetRadius),
            Ring = MathF.Max(0f, (float)RingRadius),
            WaveSpeed = (float)WaveSpeed,
            WaveAmp = (float)WaveAmplitude,
            WaveCount = WaveCount,
            Size = MathF.Max(0.05f, (float)ParticleSize),
            Lerp = Math.Clamp((float)LerpSpeed, 0.005f, 1f),
            Mode = ColorMode,
            ColorSpeed = (float)ColorSpeed,
            Auto = AutoAnimate,
            Variance = Math.Clamp((float)ParticleVariance, 0f, 1f),
            Rot = (float)RotationSpeed,
            Depth = (float)DepthFactor,
            Pulse = (float)PulseSpeed,
            Shape = ParticleShape,
            FieldStrength = MathF.Max(0f, (float)FieldStrength),
            Ambient = Math.Clamp((float)Ambient, 0f, 1f),
            Interactive = IsInteractive,
            ClickBurst = ClickBurst,
            Intro = Intro,
            LutLinear = _lutLinear,
            LutCyclic = _lutCyclic,
        };
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        WatchBrush(Palette ?? Foreground);
        AttachCanvas();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        DetachCanvas();
        UnwatchBrush();
    }

    private void AttachCanvas()
    {
        if (_canvas is not null || _root is null) return;

        var canvas = new CanvasAnimatedControl
        {
            ClearColor = Microsoft.UI.Colors.Transparent,
            Paused = IsPaused || Visibility == Visibility.Collapsed,
        };
        canvas.CreateResources += AnimatedCanvas_CreateResources;
        canvas.Update += AnimatedCanvas_Update;
        canvas.Draw += AnimatedCanvas_Draw;

        _canvas = canvas;
        _rebuildRequested = true;
        _root.Children.Add(canvas);
    }

    private void DetachCanvas()
    {
        var canvas = _canvas;
        if (canvas is null) return;

        _canvas = null;
        _pointerDown = false;
        SetPointerPresence(false);

        canvas.Paused = true;
        canvas.CreateResources -= AnimatedCanvas_CreateResources;
        canvas.Update -= AnimatedCanvas_Update;
        canvas.Draw -= AnimatedCanvas_Draw;
        canvas.RemoveFromVisualTree();
        _root?.Children.Remove(canvas);

        DisposeResources();
    }

    private void DisposeResources()
    {
        lock (_resourceLock)
        {
            if (_triangleFaces is not null)
            {
                foreach (var face in _triangleFaces)
                {
                    face.Dispose();
                }
                _triangleFaces = null;
            }

            _capStyle?.Dispose();
            _capStyle = null;
        }
    }

    private void AnimatedCanvas_CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        DisposeResources();

        var faces = new CanvasGeometry[3];
        for (var i = 0; i < faces.Length; i++)
        {
            faces[i] = CanvasGeometry.CreatePolygon(sender,
                new[] { Vector2.Zero, TriangleVertices[i], TriangleVertices[(i + 1) % 3] });
        }

        var capStyle = new CanvasStrokeStyle { StartCap = CanvasCapStyle.Round, EndCap = CanvasCapStyle.Round };

        lock (_resourceLock)
        {
            _triangleFaces = faces;
            _capStyle = capStyle;
        }
    }

    private void TrackPointer(PointerRoutedEventArgs e)
    {
        var position = e.GetCurrentPoint(this).Position;
        lock (_inputLock)
        {
            _pointer = new Vector2((float)position.X, (float)position.Y);
            _hasPointer = true;
            _lastMoveTicks = Environment.TickCount64;
        }
    }

    private void SetPointerPresence(bool present)
    {
        lock (_inputLock)
        {
            _hasPointer = present;
        }
    }

    private void Build(float w, float h, AntigravitySettings s)
    {
        _rebuildRequested = false;
        _sinceBuild = s.Intro ? 0f : 10f;
        _ringMix = 0f;
        _press = 0f;
        _mouse = new Vector2(w * 0.5f, h * 0.5f);

        var particles = new AntigravityParticle[s.Count];
        for (var i = 0; i < particles.Length; i++)
        {
            particles[i] = CreateParticle(w, h, s.Intro);
        }
        _particles = particles;
    }

    private void Resize(float w, float h, AntigravitySettings s)
    {
        var next = new AntigravityParticle[s.Count];
        Array.Copy(_particles, next, Math.Min(_particles.Length, next.Length));
        for (var i = _particles.Length; i < next.Length; i++)
        {
            next[i] = CreateParticle(w, h, intro: false);
        }
        _particles = next;
    }

    private static AntigravityParticle CreateParticle(float w, float h, bool intro)
    {
        var home = new Vector2(Rnd(), Rnd());
        var p = new AntigravityParticle
        {
            Home = home,
            Size = 0.6f + Rnd(),
            Phase = Rnd() * MathF.Tau,
            Depth = Rnd() * 2f - 1f,
            ColorT = Rnd(),
            RadiusOffset = Rnd() * 2f - 1f,
            Rot = Rnd() * MathF.Tau,
            Spin = (Rnd() - 0.5f) * 4f,
        };

        if (intro)
        {
            p.Pos = new Vector2(home.X * w + (Rnd() - 0.5f) * 60f, h + 30f + Rnd() * h * 0.15f);
            p.Age = -((1f - home.Y) * 0.9f + Rnd() * 0.6f);
        }
        else
        {
            p.Pos = new Vector2(home.X * w, home.Y * h);
            p.Age = 10f;
        }

        return p;
    }

    private void AnimatedCanvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        var s = _settings;
        var size = sender.Size;
        float w = (float)size.Width, h = (float)size.Height;
        if (w <= 1f || h <= 1f) return;

        var dt = Math.Clamp((float)args.Timing.ElapsedTime.TotalSeconds, 0.001f, MaxFrameTime);

        if (_rebuildRequested)
        {
            Build(w, h, s);
        }
        else if (_particles.Length != s.Count)
        {
            Resize(w, h, s);
        }

        _time += dt;
        var time = (float)_time;
        _sinceBuild += dt;
        _colorPhase += s.ColorSpeed * dt;
        _colorPhase -= MathF.Floor(_colorPhase);

        Vector2 pointer;
        bool hasPointer;
        long lastMove;
        lock (_inputLock)
        {
            pointer = _pointer;
            hasPointer = _hasPointer;
            lastMove = _lastMoveTicks;
        }

        var idle = Environment.TickCount64 - lastMove > IdleThresholdMs;
        var autoReady = s.Auto && _sinceBuild > (s.Intro ? 3f : 0.5f);

        Vector2 target;
        bool ringWanted;
        if (s.Interactive && hasPointer && (!idle || !autoReady))
        {
            target = pointer;
            ringWanted = true;
        }
        else if (autoReady)
        {
            target = new Vector2(
                w * (0.5f + 0.28f * MathF.Sin(time * 0.45f)),
                h * (0.5f + 0.22f * MathF.Sin(time * 0.31f + 1.3f)));
            ringWanted = true;
        }
        else
        {
            target = _mouse;
            ringWanted = false;
        }

        _mouse = Vector2.Lerp(_mouse, target, 1f - MathF.Pow(1f - s.Lerp, dt * 60f));
        _ringMix += ((ringWanted ? 1f : 0f) - _ringMix) * MathF.Min(1f, dt * 3f);

        var pressTarget = _pointerDown && s.ClickBurst && s.Interactive ? 1f : 0f;
        _press += (pressTarget - _press) * MathF.Min(1f, dt * 8f);
        _orbit += (s.Rot + _press * 1.2f) * dt;

        var burst = _burstPending;
        _burstPending = false;

        var mouse = _mouse;
        var effMagnet = s.Magnet * _ringMix * (1f + 0.15f * _press);
        var ringScale = 1f - 0.45f * _press;
        var stiffness = 1800f * MathF.Min(s.Lerp, 0.5f);
        var burstReach = s.Magnet * 1.6f;
        var fieldDeviation = 50f / (s.FieldStrength + 0.1f);
        var waveBase = s.WaveAmp * s.Ring * 0.07f;
        var waveTime = time * s.WaveSpeed * 4f;
        var isCapsule = s.Shape == AntigravityParticleShape.Capsule;

        var particles = _particles;
        for (var i = 0; i < particles.Length; i++)
        {
            ref var p = ref particles[i];
            p.Age += dt;
            if (p.Age < 0f) continue;

            var home = new Vector2(p.Home.X * w, p.Home.Y * h);
            home += new Vector2(MathF.Sin(time * 0.6f + p.Phase), MathF.Cos(time * 0.5f + p.Phase * 1.3f)) * 5f;

            var tgt = home;
            var inside = false;
            var rel = home - mouse;
            if (effMagnet > 1f && rel.LengthSquared() < effMagnet * effMagnet)
            {
                inside = true;
                var angle = MathF.Atan2(rel.Y, rel.X) + _orbit;
                var wave = MathF.Sin(waveTime + angle * s.WaveCount) * waveBase;
                var r = s.Ring * ringScale + wave + p.RadiusOffset * fieldDeviation;
                tgt = mouse + new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * r;
            }

            if (burst)
            {
                var d = p.Pos - mouse;
                var len = d.Length();
                if (len < burstReach && len > 0.01f)
                {
                    p.Vel += d / len * ((700f + Rnd() * 900f) * (1f - len / burstReach));
                }
            }

            var m = Math.Clamp(p.Age / 2f, 0f, 1f);
            m = m * m * (3f - 2f * m);
            var kk = Lerp(MathF.Min(18f, stiffness), stiffness, m);
            var damp = 2f * MathF.Sqrt(kk) * Lerp(1f, 0.55f, m);
            p.Vel += ((tgt - p.Pos) * kk - p.Vel * damp) * dt;
            p.Pos += p.Vel * dt;
            p.Speed = p.Vel.Length();

            p.Glow += ((inside ? 1f : 0f) - p.Glow) * MathF.Min(1f, dt * 6f);

            if (isCapsule)
            {
                float desired;
                if (p.Glow > 0.3f) desired = MathF.Atan2(mouse.Y - p.Pos.Y, mouse.X - p.Pos.X);
                else if (p.Speed > 20f) desired = MathF.Atan2(p.Vel.Y, p.Vel.X);
                else desired = p.Rot;
                p.Rot = LerpAngle(p.Rot, desired, MathF.Min(1f, dt * 10f));
            }
            else
            {
                p.Rot += p.Spin * dt * (1f + p.Glow * 2f + p.Speed * 0.004f);
            }
        }
    }

    private void AnimatedCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var particles = _particles;
        if (particles.Length == 0) return;

        var session = args.DrawingSession;
        var s = _settings;
        var shape = s.Shape;
        var mouse = _mouse;
        var time = (float)_time;

        lock (_resourceLock)
        {
            DrawParticles(session, particles, s, shape, mouse, time);
        }
    }

    private void DrawParticles(CanvasDrawingSession session, AntigravityParticle[] particles, AntigravitySettings s, AntigravityParticleShape shape, Vector2 mouse, float time)
    {
        var faces = _triangleFaces;
        var capStyle = _capStyle;

        if (shape == AntigravityParticleShape.Tetrahedron && faces is null) return;
        if (shape == AntigravityParticleShape.Capsule && capStyle is null) return;

        session.Transform = Matrix3x2.Identity;

        for (var i = 0; i < particles.Length; i++)
        {
            ref var p = ref particles[i];
            if (p.Age < 0f) continue;

            var pulse = 1f + s.Variance * 0.35f * MathF.Sin(time * s.Pulse + p.Phase);
            var depthScale = MathF.Max(0.2f, 1f + s.Depth * 0.45f * p.Depth);
            var radius = 2.1f * s.Size * p.Size * pulse * depthScale;
            var alpha = (s.Ambient + (1f - s.Ambient) * p.Glow) * Math.Clamp(1f + s.Depth * 0.4f * p.Depth, 0.25f, 1f);
            var baseColor = ColorFor(in p, s, mouse);

            switch (shape)
            {
                case AntigravityParticleShape.Capsule:
                    {
                        var dir = new Vector2(MathF.Cos(p.Rot), MathF.Sin(p.Rot));
                        var len = radius * 1.6f + MathF.Min(p.Speed * 0.03f, radius * 6f);
                        session.DrawLine(p.Pos - dir * (len * 0.5f), p.Pos + dir * (len * 0.5f),
                            Tint(baseColor, alpha), radius * 1.4f, capStyle);
                        break;
                    }
                case AntigravityParticleShape.Sphere:
                    {
                        session.FillCircle(p.Pos, radius, Tint(baseColor, alpha));
                        session.FillCircle(p.Pos + new Vector2(-0.28f, -0.28f) * radius, radius * 0.45f,
                            Tint(ColorHelper.GetInterpolatedColor(0.75, baseColor, Microsoft.UI.Colors.White), alpha * 0.6f));
                        break;
                    }
                case AntigravityParticleShape.Box:
                    {
                        var half = radius * 0.8f;
                        session.Transform = Matrix3x2.CreateRotation(p.Rot) * Matrix3x2.CreateTranslation(p.Pos);
                        session.FillRectangle(-half, -half, half * 2f, half * 2f, Tint(baseColor, alpha));
                        session.Transform = Matrix3x2.Identity;
                        break;
                    }
                case AntigravityParticleShape.Tetrahedron:
                    {
                        session.Transform = Matrix3x2.CreateScale(radius * 1.4f) *
                                            Matrix3x2.CreateRotation(p.Rot) *
                                            Matrix3x2.CreateTranslation(p.Pos);
                        session.FillGeometry(faces![0], Tint(baseColor, alpha, 1.0f));
                        session.FillGeometry(faces[1], Tint(baseColor, alpha, 0.78f));
                        session.FillGeometry(faces[2], Tint(baseColor, alpha, 0.58f));
                        session.Transform = Matrix3x2.Identity;
                        break;
                    }
            }
        }
    }

    private Color ColorFor(in AntigravityParticle p, AntigravitySettings s, Vector2 mouse)
    {
        float t;
        Color[] lut;
        switch (s.Mode)
        {
            case AntigravityColorMode.Conic:
                {
                    var d = p.Pos - mouse;
                    t = MathF.Atan2(d.Y, d.X) / MathF.Tau + 0.5f + _colorPhase;
                    t -= MathF.Floor(t);
                    lut = s.LutCyclic;
                    break;
                }
            case AntigravityColorMode.Radial:
                {
                    var d = (p.Pos - mouse).Length();
                    t = Math.Clamp(MathF.Abs(d - s.Ring) / s.Magnet, 0f, 1f);
                    lut = s.LutLinear;
                    break;
                }
            default:
                t = p.ColorT;
                lut = s.LutLinear;
                break;
        }

        return lut[(int)(Math.Clamp(t, 0f, 1f) * (LutSize - 1))];
    }

    private static float Rnd() => Random.Shared.NextSingle();

    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    private static float LerpAngle(float a, float b, float t)
    {
        var d = (b - a) % MathF.Tau;
        if (d > MathF.PI) d -= MathF.Tau;
        else if (d < -MathF.PI) d += MathF.Tau;
        return a + d * t;
    }

    private static Color Tint(Color c, float alpha, float shade = 1f) =>
        Color.FromArgb(
            (byte)Math.Clamp(alpha * 255f, 0f, 255f),
            (byte)(c.R * shade), (byte)(c.G * shade), (byte)(c.B * shade));

    private static void ReadPalette(Brush? brush, out float[] offsets, out Color[] colors)
    {
        if (brush is GradientBrush gradient && gradient.GradientStops.Count > 0)
        {
            var stops = new GradientStop[gradient.GradientStops.Count];
            gradient.GradientStops.CopyTo(stops, 0);
            Array.Sort(stops, static (a, b) => a.Offset.CompareTo(b.Offset));

            offsets = new float[stops.Length];
            colors = new Color[stops.Length];
            for (var i = 0; i < stops.Length; i++)
            {
                offsets[i] = (float)stops[i].Offset;
                colors[i] = stops[i].Color.WithAlpha(255);
            }
            return;
        }

        offsets = new[] { 0f };
        colors = new[] { (brush is SolidColorBrush solid ? solid.Color : Microsoft.UI.Colors.White).WithAlpha(255) };
    }

    private static Color[] BuildLut(float[] offsets, Color[] colors, bool cyclic)
    {
        var lut = new Color[LutSize];
        var count = colors.Length;
        if (count == 1)
        {
            Array.Fill(lut, colors[0]);
            return lut;
        }

        var total = cyclic ? count + 1 : count;
        var pos = new float[total];
        var col = new Color[total];
        var span = offsets[count - 1] - offsets[0];
        var scale = cyclic ? (count - 1f) / count : 1f;

        for (var i = 0; i < count; i++)
        {
            var normalized = span > 1e-6f ? (offsets[i] - offsets[0]) / span : i / (count - 1f);
            pos[i] = normalized * scale;
            col[i] = colors[i];
        }

        if (cyclic)
        {
            pos[count] = 1f;
            col[count] = colors[0];
        }

        var segment = 0;
        for (var i = 0; i < LutSize; i++)
        {
            var t = i / (LutSize - 1f);
            while (segment < total - 2 && t > pos[segment + 1]) segment++;

            var a = pos[segment];
            var b = pos[segment + 1];
            var f = b - a > 1e-6f ? Math.Clamp((t - a) / (b - a), 0f, 1f) : 1f;
            lut[i] = ColorHelper.GetInterpolatedColor(f, col[segment], col[segment + 1]);
        }

        return lut;
    }
}
