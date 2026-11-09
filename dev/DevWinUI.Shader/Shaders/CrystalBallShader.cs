using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

/// <summary>
/// Draws the glass ball: braided electric rim, outward flares, halo, inner sparks,
/// fresnel / haze / specular on the glass. All geometry is in "unit space":
/// (0,0) = ball centre, length 1 = ball radius, +Y = down (same as screen).
/// </summary>
[D2DInputCount(0)]
[D2DRequiresScenePosition]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
public readonly partial struct CrystalBallShader(
    float time,
    float2 center,      // pixels
    float radius,       // pixels
    float3 color,       // 0..1
    float2 cursor,      // unit space
    float hover,        // 0..1 (already includes proximity * hover strength)
    int strands,
    float crackle,
    float flares,
    float glow,
    float sparks,
    float haze,
    float power,        // intro power-up 0..1
    float light         // 0 = dark theme, 1 = light theme
) : ID2D1PixelShader
{
    public float4 Execute()
    {
        float2 pos = D2D.GetScenePosition().XY;
        float2 p = (pos - center) / radius;
        float d = Hlsl.Length(p);

        if (d > 2.8f)
        {
            return new float4(0f, 0f, 0f, 0f);
        }

        float a = Hlsl.Atan2(p.Y, p.X);

        // ---- cursor heat: the side of the rim facing the cursor gets hotter ----
        float ac = Hlsl.Atan2(cursor.Y, cursor.X);
        float align = 0.5f + 0.5f * Hlsl.Cos(a - ac);
        float heat = hover * Hlsl.Pow(align, 3f);

        // ---- braided strands ----
        float rim = 0f;
        for (int i = 0; i < 8; i++)
        {
            if (i < strands)
            {
                float s = i * 1.713f + 0.3f;
                float w = Wave(a, time, s);
                float amp = crackle * 0.05f * (1f + heat * 1.6f);
                float r = 1f + amp * w;
                float dist = Hlsl.Abs(d - r);
                rim += 0.0045f / (dist + 0.0045f);
            }
        }
        rim *= 1.5f / (1f + 0.3f * (float)strands);

        // ---- flares: slow travelling glow licking outward ----
        float x = d - 1f;
        float flareLen = flares * (0.06f
            + 0.16f * (0.5f + 0.5f * Hlsl.Sin(a * 2f - time * 0.45f + 1.3f)) * (0.6f + 0.4f * Hlsl.Sin(a * 5f + time * 0.6f))
            + 0.35f * heat);
        float flare = 0f;
        if (x > 0f)
        {
            flare = Hlsl.Exp(-x / Hlsl.Max(flareLen, 0.01f)) * (0.25f + 0.6f * heat);
        }

        // ---- halo ----
        float halo = glow * Hlsl.Exp(-Hlsl.Abs(x) * (x > 0f ? 9f : 5f)) * 0.5f * (1f + heat);

        // ---- sparks discharging along the inside of the rim ----
        float spark = 0f;
        if (sparks > 0f)
        {
            for (int k = 0; k < 3; k++)
            {
                float st = time * sparks * 0.8f + k * 0.37f;
                float idx = Hlsl.Floor(st);
                float life = st - idx;
                float ang = Hash(idx * 17.31f + k * 91.7f) * 6.2831853f;
                float width = 0.12f + 0.25f * Hash(idx * 3.7f + k);
                float da = Hlsl.Acos(Hlsl.Clamp(Hlsl.Cos(a - ang), -1f, 1f));
                float along = 1f - Hlsl.SmoothStep(0f, width, da);
                float dd = Hlsl.Abs(d - (0.93f + 0.035f * Hlsl.Sin(a * 37f + time * 25f + k)));
                float f = 1f - life;
                spark += along * (0.004f / (dd + 0.004f)) * f * f * (0.6f + 0.4f * Hlsl.Sin(time * 60f + k * 9f));
            }
            spark *= 1f - Hlsl.SmoothStep(1f, 1.03f, d);
        }

        // ---- glass: fresnel edge, haze pooled low, specular highlight ----
        float inside = 1f - Hlsl.SmoothStep(0.985f, 1f, d);
        float fres = Hlsl.Pow(Hlsl.Saturate(d), 8f) * 0.16f;
        float pool = Hlsl.Pow(Hlsl.Saturate(p.Y * 0.5f + 0.5f), 3f);
        float hz = haze * (0.10f * Hlsl.Pow(Hlsl.Saturate(d), 3f) + 0.22f * pool * Hlsl.Saturate(1f - d * d));
        float2 hl = p - new float2(-0.38f, -0.44f);
        float spec = Hlsl.Exp(-Hlsl.Dot(hl, hl) * 16f) * 0.10f;
        float glass = (fres + hz + spec) * inside;

        // ---- combine ----
        float boost = 1f + heat * 1.2f;
        float intensity = (rim * boost + flare + halo + spark * 1.3f + glass) * power;

        float3 white = new float3(1f, 1f, 1f);

        // dark theme: emitted light (additive-ish: alpha well below the colour)
        float3 hot = Hlsl.Lerp(color, white, Hlsl.Saturate(rim * 0.45f));
        float4 darkOut = new float4(hot * intensity, Hlsl.Saturate(intensity) * 0.4f);

        // light theme: crisp ink with a white-hot rim core
        float3 ink = color * 0.55f;
        float3 inkHot = Hlsl.Lerp(ink, white, Hlsl.Saturate((rim - 1.1f) * 0.6f));
        float al = Hlsl.Saturate(intensity * 1.15f);
        float4 lightOut = new float4(inkHot * al, al);

        return Hlsl.Lerp(darkOut, lightOut, light);
    }

    // Periodic in 'a' (integer harmonics), so there is no seam at +-pi.
    private static float Wave(float a, float t, float s)
    {
        return 0.50f * Hlsl.Sin(a * 3f + t * 1.3f + s)
             + 0.30f * Hlsl.Sin(a * 7f - t * 2.1f + s * 2.3f)
             + 0.20f * Hlsl.Sin(a * 17f + t * 4.7f + s * 3.7f)
             + 0.12f * Hlsl.Sin(a * 41f - t * 13f + s * 5f);
    }

    private static float Hash(float n)
    {
        return Hlsl.Frac(Hlsl.Sin(n) * 43758.5453f);
    }
}
