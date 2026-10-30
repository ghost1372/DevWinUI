namespace DevWinUI;

internal sealed partial class PlayStationParticlesRenderer
{
    private const int BaseParticleCount = 460;
    private const int PaletteSize = 3;
    private const float MaxSpeed = 4f;
    private const float MaxFrameTime = 0.05f;
    private const float FadeTime = 1.5f;
    private const float SwayRate = 9f;
    private const float VisibleOpacity = 0.01f;

    private readonly CanvasAnimatedControl _control;
    private readonly Random _random = new();

    private Color _particleColor;
    private float _colorOpacity;
    private volatile float _speed;
    private PlayStationRendererParticle[] _particles = [];
    private CanvasRadialGradientBrush[]? _softBrushes;
    private CanvasRadialGradientBrush[]? _sharpBrushes;
    private float _width;
    private float _height;
    private float _unit;
    private double _time;
    private volatile bool _disposed;

    public PlayStationParticlesRenderer(CanvasAnimatedControl control, Color particleColor, double speed)
    {
        _control = control;
        _particleColor = particleColor;
        _colorOpacity = particleColor.A / 255f;
        SetSpeed(speed);

        control.CreateResources += OnCreateResources;
        control.Update += OnUpdate;
        control.Draw += OnDraw;
        control.SizeChanged += OnSizeChanged;
    }

    public void SetParticleColor(Color color)
    {
        _ = _control.RunOnGameLoopThreadAsync(() => ApplyParticleColor(color));
    }

    public void SetSpeed(double speed)
    {
        _speed = double.IsFinite(speed) ? (float)Math.Clamp(speed, 0d, MaxSpeed) : 1f;
    }

    public void Dispose()
    {
        _disposed = true;

        _control.CreateResources -= OnCreateResources;
        _control.Update -= OnUpdate;
        _control.Draw -= OnDraw;
        _control.SizeChanged -= OnSizeChanged;

        DisposeBrushes();
    }

    private void ApplyParticleColor(Color color)
    {
        if (_disposed || _particleColor == color) return;

        _particleColor = color;
        _colorOpacity = color.A / 255f;

        if (_softBrushes is not null)
        {
            CreateBrushes(_control);
        }
    }

    private void OnCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        CreateBrushes(sender);
    }

    private void CreateBrushes(ICanvasResourceCreator creator)
    {
        DisposeBrushes();

        var softBrushes = new CanvasRadialGradientBrush[PaletteSize];
        var sharpBrushes = new CanvasRadialGradientBrush[PaletteSize];

        for (var i = 0; i < PaletteSize; i++)
        {
            var color = GetPaletteColor(_particleColor, i);
            softBrushes[i] = CreateSoftBrush(creator, color);
            sharpBrushes[i] = CreateSharpBrush(creator, color);
        }

        _softBrushes = softBrushes;
        _sharpBrushes = sharpBrushes;
    }

    private static Color GetPaletteColor(Color color, int index) => index switch
    {
        0 => Color.FromArgb(255, color.R, color.G, color.B),
        1 => Color.FromArgb(255, color.R, Offset(color.G, 28), Offset(color.B, 46)),
        _ => Color.FromArgb(255, color.R, Offset(color.G, 64), Offset(color.B, 124)),
    };

    private static byte Offset(byte component, int offset) => (byte)Math.Min(byte.MaxValue, component + offset);

    private static CanvasRadialGradientBrush CreateSoftBrush(ICanvasResourceCreator creator, Color color)
    {
        CanvasGradientStop[] stops =
        [
            new() { Position = 0f, Color = WithAlpha(color, 230) },
            new() { Position = 0.5f, Color = WithAlpha(color, 209) },
            new() { Position = 0.75f, Color = WithAlpha(color, 128) },
            new() { Position = 0.92f, Color = WithAlpha(color, 31) },
            new() { Position = 1f, Color = WithAlpha(color, 0) },
        ];

        return new CanvasRadialGradientBrush(creator, stops);
    }

    private static CanvasRadialGradientBrush CreateSharpBrush(ICanvasResourceCreator creator, Color color)
    {
        CanvasGradientStop[] stops =
        [
            new() { Position = 0f, Color = Colors.White },
            new() { Position = 0.18f, Color = WithAlpha(color, 242) },
            new() { Position = 0.42f, Color = WithAlpha(color, 56) },
            new() { Position = 1f, Color = WithAlpha(color, 0) },
        ];

        return new CanvasRadialGradientBrush(creator, stops);
    }

    private static Color WithAlpha(Color color, byte alpha) => Color.FromArgb(alpha, color.R, color.G, color.B);

    private void OnSizeChanged(object sender, SizeChangedEventArgs args)
    {
        var width = (float)args.NewSize.Width;
        var height = (float)args.NewSize.Height;

        _ = _control.RunOnGameLoopThreadAsync(() => Resize(width, height));
    }

    private void Resize(float width, float height)
    {
        if (_disposed || width <= 0 || height <= 0) return;

        _width = width;
        _height = height;
        _unit = height / 1080f;

        var area = width * height / (1920f * 1080f);
        var count = (int)Math.Round(BaseParticleCount * Math.Clamp(area, 0.6f, 1.6f));

        if (_particles.Length != count)
        {
            _particles = new PlayStationRendererParticle[count];
        }

        for (var i = 0; i < _particles.Length; i++)
        {
            _particles[i] = Spawn(first: true);
        }
    }

    private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        var deltaTime = (float)Math.Min(args.Timing.ElapsedTime.TotalSeconds, MaxFrameTime) * _speed;
        _time += deltaTime;

        var time = (float)_time;
        var particles = _particles;

        for (var i = 0; i < particles.Length; i++)
        {
            ref var particle = ref particles[i];

            particle.X += particle.VelocityX * deltaTime;
            particle.Y += particle.VelocityY * deltaTime + MathF.Sin(time * 0.5f + particle.Phase) * SwayRate * _unit * deltaTime;
            particle.Age += deltaTime;

            if (particle.Age > particle.Lifetime ||
                particle.X - particle.Size > _width ||
                particle.Y + particle.Size < 0)
            {
                particle = Spawn(first: false);
            }
        }
    }

    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var softBrushes = _softBrushes;
        var sharpBrushes = _sharpBrushes;
        if (softBrushes is null || sharpBrushes is null) return;

        var session = args.DrawingSession;
        session.Blend = CanvasBlend.Add;

        var time = (float)_time;
        var particles = _particles;

        for (var i = 0; i < particles.Length; i++)
        {
            ref readonly var particle = ref particles[i];

            var life = MathF.Min(1f, MathF.Min(particle.Age / FadeTime, (particle.Lifetime - particle.Age) / FadeTime));
            var twinkle = 0.65f + 0.35f * MathF.Sin(time * particle.TwinkleRate + particle.Phase);
            var opacity = particle.Opacity * twinkle * life * _colorOpacity;
            if (opacity < VisibleOpacity) continue;

            var radius = (particle.Soft ? particle.Size : particle.Size * 2.2f) * 0.5f;
            var center = new Vector2(particle.X, particle.Y);
            var brush = particle.Soft ? softBrushes[particle.Tint] : sharpBrushes[particle.Tint];

            brush.Center = center;
            brush.RadiusX = radius;
            brush.RadiusY = radius;
            brush.Opacity = opacity;

            session.FillCircle(center, radius, brush);
        }
    }

    private PlayStationRendererParticle Spawn(bool first)
    {
        var layer = _random.NextDouble();
        var depth = layer < 0.64 ? 0 : layer < 0.88 ? 1 : 2;
        var x = _width * (depth == 2 ? RandomUnit() : MathF.Pow(RandomUnit(), 0.62f));
        var position = x / _width;
        var center = _height * (0.9f - 0.42f * position);
        var spread = _height * (0.045f + 0.085f * position) * (depth == 2 ? 1.7f : 1f);
        var y = center + Gaussian() * spread * 1.7f;

        if (_random.NextDouble() < 0.06)
        {
            y = _height * (0.3f + RandomUnit() * 0.7f);
        }

        var sparkle = depth == 0 && _random.NextDouble() < 0.05;
        var size = sparkle
            ? 6 + RandomUnit() * 4
            : depth == 0
                ? 2 + RandomUnit() * 4
                : depth == 1
                    ? 5 + RandomUnit() * 9
                    : 16 + RandomUnit() * 34;

        return new PlayStationRendererParticle
        {
            X = x,
            Y = y,
            Soft = depth == 2 || depth == 1 && _random.NextDouble() < 0.6,
            Tint = sparkle ? 2 : _random.NextDouble() < 0.5 ? 0 : _random.NextDouble() < 0.7 ? 1 : 2,
            Size = size * _unit,
            VelocityX = (depth == 0 ? 5 : depth == 1 ? 9 : 16) * _unit * (0.6f + RandomUnit() * 0.8f),
            VelocityY = -(depth == 0 ? 1.2f : depth == 1 ? 2f : 3.5f) * _unit * (0.4f + RandomUnit()),
            Opacity = depth == 0
                ? 0.45f + RandomUnit() * 0.55f
                : depth == 1
                    ? 0.28f + RandomUnit() * 0.45f
                    : 0.1f + RandomUnit() * 0.2f,
            TwinkleRate = 0.4f + RandomUnit() * 1.6f,
            Phase = RandomUnit() * MathF.Tau,
            Age = first ? RandomUnit() * 8 : 0,
            Lifetime = 7 + RandomUnit() * 10,
        };
    }

    private float RandomUnit() => (float)_random.NextDouble();

    private float Gaussian() => (RandomUnit() + RandomUnit() + RandomUnit() - 1.5f) / 1.5f;

    private void DisposeBrushes()
    {
        var softBrushes = _softBrushes;
        var sharpBrushes = _sharpBrushes;

        _softBrushes = null;
        _sharpBrushes = null;

        if (softBrushes is not null)
        {
            foreach (var brush in softBrushes)
            {
                brush.Dispose();
            }
        }

        if (sharpBrushes is not null)
        {
            foreach (var brush in sharpBrushes)
            {
                brush.Dispose();
            }
        }
    }

    private struct PlayStationRendererParticle
    {
        public float X;
        public float Y;
        public float Size;
        public float VelocityX;
        public float VelocityY;
        public float Opacity;
        public float TwinkleRate;
        public float Phase;
        public float Age;
        public float Lifetime;
        public int Tint;
        public bool Soft;
    }
}
