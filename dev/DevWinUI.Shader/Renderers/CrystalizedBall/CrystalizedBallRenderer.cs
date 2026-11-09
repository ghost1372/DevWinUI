using System.Numerics;
using ComputeSharp;
using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace DevWinUI;

public partial class CrystalizedBallRenderer : RendererBase
{
    private const float L = 0.93f;
    private const float MaxTop = L * 0.95f;

    private readonly record struct Look(
        int Strands, float Crackle, float Flares, float Glow, float Sparks, int Count,
        float Fill, CrystalizedBallMotion Motion, CrystalizedBallParticleShape Shape, float Depth, float Sway,
        float Twinkle, float Haze, float DustSpeed);

    private static Look GetLook(CrystalizedBallPreset p) => p switch
    {
        CrystalizedBallPreset.Plasma => new(4, .60f, .60f, .70f, .60f, 20000, .55f, CrystalizedBallMotion.Rise, CrystalizedBallParticleShape.Round, .60f, .50f, .40f, .50f, 1.0f),
        CrystalizedBallPreset.Aurora => new(3, .45f, .80f, .65f, .40f, 24000, .65f, CrystalizedBallMotion.Drift, CrystalizedBallParticleShape.Round, .70f, .60f, .35f, .55f, 0.8f),
        CrystalizedBallPreset.Nebula => new(5, .70f, .70f, .80f, .70f, 28000, .70f, CrystalizedBallMotion.Orbit, CrystalizedBallParticleShape.Round, .80f, .70f, .50f, .70f, 0.9f),
        CrystalizedBallPreset.Ember => new(4, .80f, .90f, .75f, .90f, 18000, .45f, CrystalizedBallMotion.Rise, CrystalizedBallParticleShape.Square, .50f, .30f, .60f, .45f, 1.2f),
        CrystalizedBallPreset.Frost => new(3, .35f, .40f, .55f, .30f, 22000, .60f, CrystalizedBallMotion.Fall, CrystalizedBallParticleShape.Round, .65f, .40f, .55f, .40f, 0.7f),
        CrystalizedBallPreset.Solar => new(6, .85f, 1.0f, .90f, .80f, 16000, .50f, CrystalizedBallMotion.Orbit, CrystalizedBallParticleShape.Square, .45f, .35f, .45f, .50f, 1.1f),
        CrystalizedBallPreset.Eclipse => new(2, .30f, .30f, .45f, .20f, 14000, .40f, CrystalizedBallMotion.Drift, CrystalizedBallParticleShape.Square, .55f, .45f, .30f, .30f, 0.6f),
        _ => new(5, .55f, .55f, .60f, .50f, 30000, .75f, CrystalizedBallMotion.Fall, CrystalizedBallParticleShape.Round, .90f, .55f, .50f, .80f, 0.7f),
    };

    private static Color GetPresetColor(CrystalizedBallPreset p) => p switch
    {
        CrystalizedBallPreset.Plasma => Color.FromArgb(255, 76, 194, 255),
        CrystalizedBallPreset.Aurora => Color.FromArgb(255, 16, 200, 140),
        CrystalizedBallPreset.Nebula => Color.FromArgb(255, 167, 139, 250),
        CrystalizedBallPreset.Ember => Color.FromArgb(255, 255, 99, 71),
        CrystalizedBallPreset.Frost => Color.FromArgb(255, 125, 211, 252),
        CrystalizedBallPreset.Solar => Color.FromArgb(255, 252, 196, 25),
        CrystalizedBallPreset.Eclipse => Color.FromArgb(255, 226, 232, 240),
        _ => Color.FromArgb(255, 99, 102, 241),
    };

    private sealed class Settings
    {
        public float R, G, B;
        public bool Light;
        public float Size, Crackle, Flares, Glow, Sparks, Fill, Depth, Sway, Twinkle, Haze, Speed, DustSpeed, HoverStrength;
        public int Strands, Count;
        public CrystalizedBallMotion Motion;
        public CrystalizedBallParticleShape Shape;
        public bool Interactive, Intro, Paused;
    }

    private volatile Settings? _s;
    private SolidColorBrush? _trackedBrush;
    private long _brushColorToken;

    private PixelShaderEffect<CrystalBallShader>? _effect;
    private CanvasBitmap? _round, _square;

    private float _time, _dustTime, _intro = 1f, _shake, _hover, _turn, _angle;
    private float _cx, _cy, _r, _cux, _cuy;
    private float _lcx, _lcy, _pvx, _pvy;
    private bool _hadPointer;

    private volatile float _px, _py;
    private volatile bool _pIn, _shakePending;

    private float[] _x = [], _y = [], _z = [], _vx = [], _vy = [], _vz = [], _sd = [], _ph = [], _ct = [];

    public CrystalizedBallRenderer()
    {
        Rebuild();
    }

    private static float F(double v, float d) => double.IsNaN(v) ? d : (float)v;
    private static float Rnd() => Random.Shared.NextSingle();
    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    private static float Smooth(float a, float b, float x)
    {
        float t = Math.Clamp((x - a) / (b - a), 0f, 1f);
        return t * t * (3f - 2f * t);
    }

    private static void OnSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CrystalizedBallRenderer)d).Rebuild();
    }

    private static Color? GetBrushColor(Brush? brush)
    {
        if (brush is SolidColorBrush solid && solid.Color.A > 0)
        {
            return Color.FromArgb(255, solid.Color.R, solid.Color.G, solid.Color.B);
        }

        return null;
    }

    private void TrackBallBrush(Brush? brush)
    {
        if (_trackedBrush is not null)
        {
            _trackedBrush.UnregisterPropertyChangedCallback(SolidColorBrush.ColorProperty, _brushColorToken);
            _trackedBrush = null;
        }

        if (brush is SolidColorBrush solid)
        {
            _trackedBrush = solid;
            _brushColorToken = solid.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnBallBrushColorChanged);
        }
    }

    private void OnBallBrushColorChanged(DependencyObject sender, DependencyProperty property)
    {
        Rebuild();
    }

    protected override void OnHostThemeChanged()
    {
        Rebuild();
    }

    private void Rebuild()
    {
        var l = GetLook(Preset);
        var brush = BallBrush;

        if (!ReferenceEquals(brush, _trackedBrush))
        {
            TrackBallBrush(brush);
        }

        Color c = BallColor != Colors.Transparent
            ? BallColor
            : GetBrushColor(brush) ?? GetPresetColor(Preset);

        _s = new Settings
        {
            R = c.R / 255f, G = c.G / 255f, B = c.B / 255f,
            Light = Theme switch
            {
                CrystalizedBallTheme.Light => true,
                CrystalizedBallTheme.Dark => false,
                _ => hostTheme == ElementTheme.Light,
            },
            Size = Math.Clamp((float)BallSize, 0.1f, 1.5f),
            Strands = Math.Clamp(Strands > 0 ? Strands : l.Strands, 1, 8),
            Crackle = F(Crackle, l.Crackle),
            Flares = F(Flares, l.Flares),
            Glow = F(Glow, l.Glow),
            Sparks = Math.Max(0f, F(Sparks, l.Sparks)),
            Count = Math.Clamp(ParticleCount >= 0 ? ParticleCount : l.Count, 0, 40000),
            Fill = Math.Clamp(F(Fill, l.Fill), 0f, 1f),
            Motion = Motion == CrystalizedBallMotion.Auto ? l.Motion : Motion,
            Shape = ParticleShape == CrystalizedBallParticleShape.Auto ? l.Shape : ParticleShape,
            Depth = F(Depth, l.Depth),
            Sway = F(Sway, l.Sway),
            Twinkle = Math.Clamp(F(Twinkle, l.Twinkle), 0f, 1f),
            Haze = F(Haze, l.Haze),
            Speed = (float)Speed,
            DustSpeed = F(DustSpeed, l.DustSpeed),
            HoverStrength = Math.Clamp((float)HoverStrength, 0f, 1f),
            Interactive = Interactive,
            Intro = Intro,
            Paused = IsPaused,
        };
    }

    public override void OnPointerMoved(Vector2 position)
    {
        _px = position.X;
        _py = position.Y;
        _pIn = true;
    }

    public override void OnPointerExited()
    {
        _pIn = false;
    }

    public override void OnPointerPressed(Vector2 position)
    {
        var s = _s;
        if (s is null || !s.Interactive) return;

        float dx = position.X - _cx, dy = position.Y - _cy;
        if (MathF.Sqrt(dx * dx + dy * dy) <= _r * 1.1f) _shakePending = true;
    }

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();

        _effect = new PixelShaderEffect<CrystalBallShader>();

        var round = new CanvasRenderTarget(sender, 16, 16, 96);
        using (var ds = round.CreateDrawingSession())
        {
            ds.Clear(Color.FromArgb(0, 255, 255, 255));
            var stops = new[]
            {
                new CanvasGradientStop { Position = 0.0f, Color = Color.FromArgb(255, 255, 255, 255) },
                new CanvasGradientStop { Position = 0.5f, Color = Color.FromArgb(140, 255, 255, 255) },
                new CanvasGradientStop { Position = 1.0f, Color = Color.FromArgb(0, 255, 255, 255) },
            };
            using var brush = new CanvasRadialGradientBrush(sender, stops)
            {
                Center = new Vector2(8, 8), RadiusX = 8, RadiusY = 8,
            };
            ds.FillRectangle(0, 0, 16, 16, brush);
        }
        _round = round;

        var square = new CanvasRenderTarget(sender, 4, 4, 96);
        using (var ds = square.CreateDrawingSession()) ds.Clear(Colors.White);
        _square = square;

        _intro = _s?.Intro == true ? 0f : 1f;
    }

    private float TopOf(int i, float yTop) => Lerp(yTop, MathF.Max(yTop, MaxTop), _ct[i]);

    private void Respawn(int i, float yTop, int where)
    {
        float y, rho;
        float ang = Rnd() * MathF.Tau;
        if (where == 1)
        {
            rho = L * 0.85f * MathF.Sqrt(Rnd());
            y = -MathF.Sqrt(MathF.Max(L * L - rho * rho, 0f)) * 0.98f + Rnd() * 0.05f;
        }
        else
        {
            y = where == 2 ? yTop - Rnd() * 0.04f : Lerp(-L * 0.95f, yTop, Rnd());
            float horiz = MathF.Sqrt(MathF.Max(L * L - y * y, 0.0001f));
            rho = horiz * 0.95f * MathF.Sqrt(Rnd());
        }
        _x[i] = rho * MathF.Cos(ang);
        _z[i] = rho * MathF.Sin(ang);
        _y[i] = y;
        _vx[i] = _vy[i] = _vz[i] = 0f;
    }

    private void Realloc(Settings s)
    {
        int n = s.Count;
        _x = new float[n]; _y = new float[n]; _z = new float[n];
        _vx = new float[n]; _vy = new float[n]; _vz = new float[n];
        _sd = new float[n]; _ph = new float[n]; _ct = new float[n];
        float yTop = Lerp(-0.85f, 0.93f, s.Fill);
        for (int i = 0; i < n; i++)
        {
            _sd[i] = Rnd();
            _ph[i] = Rnd();
            _ct[i] = MathF.Pow(Rnd(), 3f);
            Respawn(i, TopOf(i, yTop), 0);
        }
    }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        var s = _s;
        if (s is null || !isEnabled) return;

        UpdateBreathing(currentBassEnergy, breathingIntensity);

        float dt = Math.Min((float)args.Timing.ElapsedTime.TotalSeconds, 0.05f);
        float run = s.Paused ? 0f : 1f;

        _time += dt * s.Speed * run;
        _dustTime += dt * s.DustSpeed * run;
        _intro = s.Intro ? Math.Min(1f, _intro + dt / 2.2f) : 1f;

        float w = (float)sender.Size.Width, h = (float)sender.Size.Height;
        if (w < 2f || h < 2f) return;
        float R = Math.Min(w, h) * s.Size * 0.5f;

        if (_x.Length != s.Count) Realloc(s);
        int n = _x.Length;

        if (_shakePending)
        {
            _shakePending = false;
            _shake = 1f;
            for (int i = 0; i < n; i++)
            {
                _vx[i] += (Rnd() - 0.5f) * 3f;
                _vy[i] += (Rnd() - 0.5f) * 3f;
                _vz[i] += (Rnd() - 0.5f) * 3f;
            }
        }
        _shake *= MathF.Exp(-dt * 3f);
        float shT = (float)args.Timing.TotalTime.TotalSeconds;
        _cx = w * 0.5f + MathF.Sin(shT * 45f) * _shake * R * 0.035f;
        _cy = h * 0.5f + MathF.Cos(shT * 38f) * _shake * R * 0.035f;
        _r = R;

        bool active = s.Interactive && _pIn;
        float cpx = (_px - _cx) / R;
        float cpy = -(_py - _cy) / R;
        _cux = cpx;
        _cuy = -cpy;

        float dist = MathF.Sqrt(cpx * cpx + cpy * cpy);
        float reach = 1f - Smooth(1.0f, 1.9f, dist);
        float target = active ? reach * s.HoverStrength : 0f;
        _hover += (target - _hover) * (1f - MathF.Exp(-dt * 6f));

        float pvx = 0f, pvy = 0f;
        if (active && _hadPointer && dt > 0f)
        {
            pvx = (cpx - _lcx) / dt;
            pvy = (cpy - _lcy) / dt;
        }
        _hadPointer = active;
        _lcx = cpx; _lcy = cpy;
        float sm = 1f - MathF.Exp(-dt * 12f);
        _pvx += (pvx - _pvx) * sm;
        _pvy += (pvy - _pvy) * sm;
        float spd = MathF.Sqrt(_pvx * _pvx + _pvy * _pvy);

        float turnTarget = active ? Math.Clamp(cpx, -1.5f, 1.5f) * 0.45f * reach * s.HoverStrength : 0f;
        _turn += (turnTarget - _turn) * (1f - MathF.Exp(-dt * 2.5f));
        _angle = _turn + s.Sway * 0.6f * MathF.Sin(_dustTime * 0.35f);

        float cosA = MathF.Cos(_angle), sinA = MathF.Sin(_angle);
        float yTop = Lerp(-0.85f, 0.93f, s.Fill);
        float dd = dt * s.DustSpeed * run;
        float damp = MathF.Exp(-dt * 2.2f);
        float stirK = active ? s.HoverStrength : 0f;
        float t = _dustTime;
        var motion = s.Motion;

        for (int i = 0; i < n; i++)
        {
            float top = TopOf(i, yTop);
            float x = _x[i], y = _y[i], z = _z[i];
            float sd = _sd[i], ph = _ph[i] * MathF.Tau;
            float vx, vy, vz;

            switch (motion)
            {
                case CrystalizedBallMotion.Rise:
                    vy = 0.06f + 0.10f * sd;
                    vx = 0.03f * MathF.Sin(t * 0.7f + ph);
                    vz = 0.03f * MathF.Cos(t * 0.6f + ph);
                    break;
                case CrystalizedBallMotion.Fall:
                    vy = -(0.05f + 0.09f * sd);
                    vx = 0.03f * MathF.Sin(t * 0.7f + ph);
                    vz = 0.03f * MathF.Cos(t * 0.6f + ph);
                    break;
                case CrystalizedBallMotion.Drift:
                    vx = 0.08f * MathF.Sin(t * 0.37f + ph + y * 2f);
                    vy = 0.08f * MathF.Sin(t * 0.29f + ph * 1.7f + x * 2f);
                    vz = 0.08f * MathF.Sin(t * 0.31f + ph * 2.3f);
                    break;
                default:
                    float ow = 0.6f + 0.8f * sd;
                    vx = 0.12f * MathF.Cos(t * ow + ph);
                    vz = 0.12f * MathF.Sin(t * ow + ph);
                    vy = 0.07f * MathF.Sin(t * ow * 0.5f + ph * 0.5f);
                    break;
            }

            if (stirK > 0f)
            {
                float sx = x * cosA + z * sinA, sy = y;
                float qx = sx - cpx, qy = sy - cpy;
                float q2 = qx * qx + qy * qy;
                if (q2 < 0.5f)
                {
                    float f = MathF.Exp(-q2 / 0.18f) * stirK;
                    float inv = 1f / MathF.Sqrt(q2 + 1e-4f);
                    float tx = -qy * inv, ty = qx * inv;
                    float ax = (_pvx * 0.8f + tx * spd * 0.5f) * f * dt * 4f;
                    float ay = (_pvy * 0.8f + ty * spd * 0.5f) * f * dt * 4f;
                    _vx[i] = Math.Clamp(_vx[i] + ax * cosA, -2f, 2f);
                    _vz[i] = Math.Clamp(_vz[i] + ax * sinA, -2f, 2f);
                    _vy[i] = Math.Clamp(_vy[i] + ay, -2f, 2f);
                }
            }

            x += vx * dd + _vx[i] * dt;
            y += vy * dd + _vy[i] * dt;
            z += vz * dd + _vz[i] * dt;
            _vx[i] *= damp; _vy[i] *= damp; _vz[i] *= damp;

            float r2 = x * x + y * y + z * z;
            if (r2 > L * L)
            {
                if (motion == CrystalizedBallMotion.Rise) { Respawn(i, top, 1); continue; }
                if (motion == CrystalizedBallMotion.Fall) { Respawn(i, top, y < 0f ? 2 : 1); continue; }
                float sc = L / MathF.Sqrt(r2) * 0.98f;
                x *= sc; y *= sc; z *= sc;
                _vx[i] = _vy[i] = _vz[i] = 0f;
            }
            else if (y > top)
            {
                if (motion == CrystalizedBallMotion.Rise) { Respawn(i, top, 1); continue; }
                y = top;
                _vy[i] = 0f;
            }

            _x[i] = x; _y[i] = y; _z[i] = z;
        }
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var s = _s;
        var ds = args.DrawingSession;
        if (s is null || !isEnabled || _effect is null || _round is null || _square is null || _r <= 1f) return;

        var center = new Vector2((float)sender.Size.Width / 2f, (float)sender.Size.Height / 2f);
        ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);

        try
        {
            float scale = ds.Dpi / 96f;
            float power = Smooth(0f, 0.55f, _intro);
            _effect.ConstantBuffer = new CrystalBallShader(
                _time,
                new float2(_cx * scale, _cy * scale),
                _r * scale,
                new float3(s.R, s.G, s.B),
                new float2(_cux, _cuy),
                _hover,
                s.Strands, s.Crackle, s.Flares, s.Glow, s.Sparks, s.Haze,
                power,
                s.Light ? 1f : 0f);
            ds.DrawImage(_effect);

            DrawParticles(ds, s);
        }
        finally
        {
            ResetTransform(ds, isBreathingEffectEnabled);
        }
    }

    private void DrawParticles(CanvasDrawingSession ds, Settings s)
    {
        int n = Math.Min(_x.Length, s.Count);
        if (n == 0) return;

        bool square = s.Shape == CrystalizedBallParticleShape.Square;
        var tex = square ? _square! : _round!;
        float texSize = square ? 4f : 16f;
        var origin = new Vector2(texSize / 2f);

        float dr, dg, db;
        if (s.Light) { dr = s.R * 0.5f; dg = s.G * 0.5f; db = s.B * 0.5f; }
        else { dr = s.R + (1f - s.R) * 0.35f; dg = s.G + (1f - s.G) * 0.35f; db = s.B + (1f - s.B) * 0.35f; }

        float cosA = MathF.Cos(_angle), sinA = MathF.Sin(_angle);
        float R = _r;
        float pxScale = Math.Clamp(R / 220f, 0.7f, 2f);
        float depth = Math.Min(s.Depth, 1.2f);

        ds.Blend = s.Light ? CanvasBlend.SourceOver : CanvasBlend.Add;

        using (var batch = ds.CreateSpriteBatch(
            CanvasSpriteSortMode.None,
            square ? CanvasImageInterpolation.NearestNeighbor : CanvasImageInterpolation.Linear,
            CanvasSpriteOptions.None))
        {
            for (int i = 0; i < n; i++)
            {
                float x = _x[i], y = _y[i], z = _z[i];
                float sx = x * cosA + z * sinA;
                float zr = z * cosA - x * sinA;
                float depthT = Math.Clamp((zr + L) / (2f * L), 0f, 1f);

                float sizeMul = 1f - 0.45f * depth * (1f - depthT);
                float alpha = Math.Clamp(1f - 0.7f * depth * (1f - depthT), 0.05f, 1f);
                float sd = _sd[i];
                float tw = 1f - s.Twinkle * 0.5f * (1f - MathF.Sin(_dustTime * (2f + 4f * sd) + _ph[i] * 40f));
                float lit = Math.Clamp((_intro * 1.5f - (y + 1f) * 0.5f) * 4f, 0f, 1f);
                float a = alpha * tw * lit * 0.85f;
                if (a < 0.01f) continue;

                float px = _cx + sx * R;
                float py = _cy - y * R;
                if (square) { px = MathF.Round(px); py = MathF.Round(py); }

                float sz = (1.2f + 1.6f * sd * sd) * sizeMul * pxScale;
                float k = square ? sz / 4f : sz / 8f;

                batch.Draw(tex, new Vector2(px, py), new Vector4(dr * a, dg * a, db * a, a),
                    origin, 0f, new Vector2(k), CanvasSpriteFlip.None);
            }
        }

        ds.Blend = CanvasBlend.SourceOver;
    }

    public override void Dispose()
    {
        _effect?.Dispose();
        _effect = null;

        _round?.Dispose();
        _round = null;

        _square?.Dispose();
        _square = null;
    }
}
