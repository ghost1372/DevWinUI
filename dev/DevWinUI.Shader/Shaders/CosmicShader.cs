using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

// https://www.shadertoy.com/view/XXyGzh
[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct CosmicShader(
    float time,
    float2 dispatchSize,

    float speed,
    float rotationSpeed,

    float frequency,
    float hotspotContrast,
    float hotspotStrength,

    float edgeFalloff,
    float exposure,

    float4 accentColor1,
    float4 accentColor2,
    float4 accentColor3,
    float4 accentColor4
) : ID2D1PixelShader
{
    private static float2 Stanh(float2 a)
    {
        return Hlsl.Tanh(Hlsl.Clamp(a, -40f, 40f));
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;

        float2 v = dispatchSize;

        float2 u = 0.2f * (fragCoord + fragCoord - v) / v.Y;

        float4 o = new(1f, 2f, 3f, 0f);
        float4 z = o;

        float a = 0.5f;
        float t = time * speed;

        for (int i = 1; i < 19; i++)
        {
            a += 0.03f;

            float4 swizzle = new(z.W, z.X, z.Z, z.W);

            float4 cosValues =
                Hlsl.Cos(i + rotationSpeed * t - swizzle * 11f);

            float2x2 rot = new(
                cosValues.X,
                cosValues.Y,
                cosValues.Z,
                cosValues.W);

            u = Hlsl.Mul(u, rot);

            float d = Hlsl.Dot(u, u);

            float2 tanhTerm =
                Stanh(
                    hotspotContrast *
                    d *
                    Hlsl.Cos(frequency * u.YX + t))
                / hotspotStrength;

            float oo = Hlsl.Dot(o, o);

            float2 extra =
                Hlsl.Cos(
                    4f / Hlsl.Exp(oo / 100f) + t)
                / 300f;

            u += tanhTerm + 0.2f * a * u + extra;

            t += 1f;

            v =
                Hlsl.Cos(
                    t - 7f * u * Hlsl.Pow(a, i))
                - 5f * u;

            float2 denom =
                (1f + i * Hlsl.Dot(v, v))
                * Hlsl.Sin(
                    1.5f * u / (0.5f - Hlsl.Dot(u, u))
                    - 9f * u.YX
                    + t);

            float len =
                Hlsl.Max(
                    Hlsl.Length(denom),
                    0.0001f);

            o += (1f + Hlsl.Cos(z + t)) / len;
        }

        o =
            25.6f /
            (Hlsl.Min(o, 13f) + 164f / o)
            - Hlsl.Dot(u, u) / edgeFalloff;

        float intensity =
            Hlsl.Saturate(
                (o.X + o.Y + o.Z) / 3f);

        float4 color;

        if (intensity < 0.33f)
        {
            float t1 = intensity / 0.33f;

            color =
                Hlsl.Lerp(
                    accentColor1,
                    accentColor2,
                    t1);
        }
        else if (intensity < 0.66f)
        {
            float t2 = (intensity - 0.33f) / 0.33f;

            color =
                Hlsl.Lerp(
                    accentColor2,
                    accentColor3,
                    t2);
        }
        else
        {
            float t3 = (intensity - 0.66f) / 0.34f;

            color =
                Hlsl.Lerp(
                    accentColor3,
                    accentColor4,
                    t3);
        }

        color *= intensity;
        color *= exposure;

        return Hlsl.Saturate(color);
    }
}
