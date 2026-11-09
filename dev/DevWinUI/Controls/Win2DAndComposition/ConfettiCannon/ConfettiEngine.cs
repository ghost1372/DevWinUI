using System.Runtime.InteropServices;

namespace DevWinUI;

internal partial class ConfettiEngine
{
    private static readonly List<string> DefaultShapes = new() { "square", "circle" };

    private readonly object _lock = new();
    private readonly Random _random = new();
    private readonly List<ConfettiParticle> _particles = new();
    private readonly List<ConfettiParticle> _drawBuffer = new();
    private Size _lastSize;
    public int ParticleCount => _particles.Count;

    public void Enqueue(ConfettiCannonOptions options)
    {
        double radAngle = (270 - options.Angle) * (Math.PI / 180);
        double radSpread = options.Spread * (Math.PI / 180);

        var colors = options.GetFinalColors();
        var shapes = options.Shapes is { Count: > 0 } ? options.Shapes : DefaultShapes;
        int totalTicks = Math.Max(1, options.Ticks);

        lock (_lock)
        {
            _particles.EnsureCapacity(_particles.Count + Math.Max(0, options.ParticleCount));

            for (int i = 0; i < options.ParticleCount; i++)
            {
                var p = new ConfettiParticle
                {
                    Position = new Vector2(),
                    Wobble = _random.NextDouble() * 10,
                    WobbleSpeed = Math.Min(0.11, _random.NextDouble() * 0.1 + 0.05),
                    Velocity = (float)(options.StartVelocity * 0.5 + _random.NextDouble() * options.StartVelocity),
                    Angle2D = -(float)radAngle + (float)(0.5 * radSpread - _random.NextDouble() * radSpread),
                    TiltAngle = (float)((_random.NextDouble() * (0.75 - 0.25) + 0.25) * Math.PI),
                    Color = colors[i % colors.Count],
                    Shape = shapes[_random.Next(shapes.Count)],
                    TotalTicks = totalTicks,
                    Decay = (float)options.Decay,
                    Drift = (float)options.Drift,
                    RandomValue = (float)(_random.NextDouble() + 2),
                    Gravity = (float)(options.Gravity * 3),
                    OvalScalar = (float)(options.OvalScalar ?? 0.6),
                    Scalar = (float)options.Scalar,
                    Flat = options.Flat
                };

                p.Origin = options.Origin;
                _particles.Add(p);
            }
        }
    }

    public void Update(float dt, Size currentSize)
    {
        if (currentSize.Width <= 0 || currentSize.Height <= 0)
            return;

        _lastSize = currentSize;

        // Normalize to 60 ticks per second so speed is independent of the refresh rate
        float step = Math.Min(dt * 60f, 4f);

        lock (_lock)
        {
            var span = CollectionsMarshal.AsSpan(_particles);
            int length = span.Length;
            int write = 0;

            for (int i = 0; i < length; i++)
            {
                ref var c = ref span[i];

                if (!c.Initialized)
                {
                    c.Position = new Vector2(
                        (float)(c.Origin.X * _lastSize.Width),
                        (float)(c.Origin.Y * _lastSize.Height));
                    c.Initialized = true;
                }

                c.Position.X += (float)(Math.Sin(c.Angle2D) * c.Velocity + c.Drift) * step;
                c.Position.Y += (float)(Math.Cos(c.Angle2D) * c.Velocity + c.Gravity) * step;
                c.Velocity *= MathF.Pow(c.Decay, step);

                if (c.Flat)
                {
                    c.Wobble = 0;
                    c.WobbleX = c.Position.X + 10 * c.Scalar;
                    c.WobbleY = c.Position.Y + 10 * c.Scalar;

                    c.TiltSin = 0;
                    c.TiltCos = 0;
                    c.RandomValue = 1;
                }
                else
                {
                    c.Wobble += c.WobbleSpeed * step;
                    c.WobbleX = (float)(c.Position.X + 10 * c.Scalar * Math.Cos(c.Wobble));
                    c.WobbleY = (float)(c.Position.Y + 10 * c.Scalar * Math.Sin(c.Wobble));

                    c.TiltAngle += 0.1f * step;
                    c.TiltSin = (float)Math.Sin(c.TiltAngle);
                    c.TiltCos = (float)Math.Cos(c.TiltAngle);
                    c.RandomValue = (float)(_random.NextDouble() + 2);
                }

                c.Opacity = 1 - c.Tick / c.TotalTicks;
                c.Tick += step;

                if (c.IsCompleted)
                    continue;

                if (write != i)
                    span[write] = c;

                write++;
            }

            if (write < length)
                _particles.RemoveRange(write, length - write);
        }
    }
    public void Draw(CanvasDrawingSession ds)
    {
        lock (_lock)
        {
            if (_particles.Count == 0) return;
            _drawBuffer.Clear();
            _drawBuffer.AddRange(_particles);
        }

        foreach (ref readonly var c in CollectionsMarshal.AsSpan(_drawBuffer))
        {
            var baseColor = c.Color;
            var color = Color.FromArgb((byte)(c.Opacity * 255), baseColor.R, baseColor.G, baseColor.B);

            float x1 = c.Position.X + c.RandomValue * c.TiltCos;
            float y1 = c.Position.Y + c.RandomValue * c.TiltSin;
            float x2 = c.WobbleX + c.RandomValue * c.TiltCos;
            float y2 = c.WobbleY + c.RandomValue * c.TiltSin;

            switch (c.Shape)
            {
                case "circle":
                    {
                        float radiusX = Math.Abs(x2 - x1) * c.OvalScalar;
                        float radiusY = Math.Abs(y2 - y1) * c.OvalScalar;
                        float angle = (float)(18 * c.Wobble);

                        var center = new Vector2(c.Position.X, c.Position.Y);

                        var old = ds.Transform;
                        ds.Transform = Matrix3x2.CreateRotation((float)(angle * Math.PI / 180.0), center);
                        ds.FillEllipse(center, radiusX, radiusY, color);
                        ds.Transform = old;
                    }
                    break;

                case "star":
                    {
                        using var geo = CreateStar(ds, c.Position, 5, 4 * c.Scalar, 8 * c.Scalar, (float)c.Wobble);
                        ds.FillGeometry(geo, color);
                    }
                    break;

                default: // "square" (your stream-geometry triangle shape)
                    {
                        using var path = new CanvasPathBuilder(ds);

                        path.BeginFigure(new Vector2((float)Math.Floor(c.Position.X), (float)Math.Floor(c.Position.Y)));
                        path.AddLine(new Vector2((float)Math.Floor(c.WobbleX), (float)Math.Floor(y1)));
                        path.AddLine(new Vector2((float)Math.Floor(x2), (float)Math.Floor(y2)));
                        path.AddLine(new Vector2((float)Math.Floor(x1), (float)Math.Floor(c.WobbleY)));
                        path.EndFigure(Microsoft.Graphics.Canvas.Geometry.CanvasFigureLoop.Closed);

                        using var geo = CanvasGeometry.CreatePath(path);
                        ds.FillGeometry(geo, color);
                    }
                    break;
            }
        }
    }

    private static CanvasGeometry CreateStar(CanvasDrawingSession ds, Vector2 center,
        int spikes, float innerRadius, float outerRadius, float wobble)
    {
        float rot = (float)(Math.PI / 2 * 3);
        float step = (float)(Math.PI / spikes);

        using var pb = new CanvasPathBuilder(ds);

        bool first = true;
        while (spikes-- > 0)
        {
            float x = center.X + (float)Math.Cos(rot) * outerRadius;
            float y = center.Y + (float)Math.Sin(rot) * outerRadius;

            if (first)
            {
                pb.BeginFigure(new Vector2(x, y));
                first = false;
            }
            else
            {
                pb.AddLine(new Vector2(x, y));
            }

            rot += step;

            x = center.X + (float)Math.Cos(rot) * innerRadius;
            y = center.Y + (float)Math.Sin(rot) * innerRadius;
            pb.AddLine(new Vector2(x, y));
            rot += step;
        }

        pb.EndFigure(CanvasFigureLoop.Closed);
        return CanvasGeometry.CreatePath(pb);
    }
}
