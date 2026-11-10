namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    private const int MaxRipples = 6;
    private const float RippleSpeed = 1.25f;
    private const float RippleLife = 3.2f;

    private bool _rippleEnabled = true;
    private readonly List<ParticleGlobeRipple> _ripples = new();
    private readonly ParticleGlobeRippleFrame[] _ripFrame = new ParticleGlobeRippleFrame[MaxRipples];
    private int _ripFrameCount;

    private void SpawnRipple(Vector3 center)
    {
        if (_ripples.Count >= MaxRipples) _ripples.RemoveAt(0);
        _ripples.Add(new ParticleGlobeRipple { Center = Vector3.Normalize(center) });
    }

    private void AdvanceRipples(float dt)
    {
        for (int i = _ripples.Count - 1; i >= 0; i--)
        {
            _ripples[i].Age += dt;
            if (_ripples[i].Age >= RippleLife) _ripples.RemoveAt(i);
        }
    }

    private void BuildRippleFrames()
    {
        int n = 0;
        foreach (var ripple in _ripples)
        {
            float u = ripple.Age / RippleLife;
            float d = RippleSpeed * ripple.Age;
            float width = 0.07f + 0.02f * d;
            float amp = MathF.Pow(1f - u, 1.4f) * (1f - 0.3f * MathF.Min(d, MathF.PI) / MathF.PI);

            _ripFrame[n++] = new ParticleGlobeRippleFrame
            {
                Center = ripple.Center,
                Distance = d,
                Width = width,
                Lo = MathF.Cos(MathF.Min(MathF.PI, d + 2.5f * width)),
                Hi = MathF.Cos(MathF.Max(0f, d - 2.5f * width)),
                Amp = amp,
                Lon = MathF.Atan2(ripple.Center.X, ripple.Center.Z),
                Merc = MercatorN(MathF.Asin(Math.Clamp(ripple.Center.Y, -1f, 1f))) * MathF.PI
            };
        }
        _ripFrameCount = n;
    }

    private void DrawRippleRings(CanvasDrawingSession ds)
    {
        if (_morphT > 0f)
        {
            DrawFlatRippleRings(ds);
            return;
        }

        const int Seg = 72;

        for (int k = 0; k < _ripFrameCount; k++)
        {
            var f = _ripFrame[k];
            if (f.Distance <= 0.01f || f.Distance >= MathF.PI - 0.02f) continue;

            Vector3 n = f.Center;
            Vector3 up = MathF.Abs(n.Y) > 0.95f ? Vector3.UnitX : Vector3.UnitY;
            Vector3 t1 = Vector3.Normalize(Vector3.Cross(up, n));
            Vector3 t2 = Vector3.Cross(n, t1);
            float cd = MathF.Cos(f.Distance), sd = MathF.Sin(f.Distance);

            Vector2 prev = default;
            bool havePrev = false;
            for (int i = 0; i <= Seg; i++)
            {
                float th = i * MathF.Tau / Seg;
                var pt = (n * cd + (t1 * MathF.Cos(th) + t2 * MathF.Sin(th)) * sd) * 1.004f;
                var r = Rot(pt);
                if (r.Z < 0f)
                {
                    havePrev = false;
                    continue;
                }

                var p = Proj(r);
                float a = f.Amp * MathF.Min(1f, r.Z * 3f);
                if (havePrev && Jumped(prev, p)) havePrev = false;
                if (havePrev)
                {
                    ds.DrawLine(prev, p, ToColor(_accent, a * 0.25f), 4f);
                    ds.DrawLine(prev, p, ToColor(_accent, a * 0.9f), 1.4f);
                }
                prev = p;
                havePrev = true;
            }
        }
    }
}
