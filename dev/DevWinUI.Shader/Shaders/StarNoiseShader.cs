using ComputeSharp;
using ComputeSharp.D2D1;
using System;
using System.Collections.Generic;
using System.Text;

namespace DevWinUI;

//https://www.shadertoy.com/view/wdyfDV

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct StarNoiseShader(
    float time,
    float2 dispatchSize,

    float starDensity,
    float starExposure,
    float starThreshold,
    float flickerSpeed,

    float3 skyTint
) : ID2D1PixelShader
{
    private static float3 Hash(float3 p)
    {
        p = new float3(
            Hlsl.Dot(p, new float3(127.1f, 311.7f, 74.7f)),
            Hlsl.Dot(p, new float3(269.5f, 183.3f, 246.1f)),
            Hlsl.Dot(p, new float3(113.5f, 271.9f, 124.6f))
        );

        return -1f + 2f * Frac3(Hlsl.Sin(p) * 43758.5453123f);
    }

    private static float3 Frac3(float3 v)
    {
        return new float3(
            Hlsl.Frac(v.X),
            Hlsl.Frac(v.Y),
            Hlsl.Frac(v.Z)
        );
    }

    private static float Noise(float3 p)
    {
        float3 i = Hlsl.Floor(p);
        float3 f = Hlsl.Frac(p);

        float3 u = f * f * (3f - 2f * f);

        float n000 = Hlsl.Dot(Hash(i + new float3(0f, 0f, 0f)), f - new float3(0f, 0f, 0f));
        float n100 = Hlsl.Dot(Hash(i + new float3(1f, 0f, 0f)), f - new float3(1f, 0f, 0f));
        float n010 = Hlsl.Dot(Hash(i + new float3(0f, 1f, 0f)), f - new float3(0f, 1f, 0f));
        float n110 = Hlsl.Dot(Hash(i + new float3(1f, 1f, 0f)), f - new float3(1f, 1f, 0f));

        float n001 = Hlsl.Dot(Hash(i + new float3(0f, 0f, 1f)), f - new float3(0f, 0f, 1f));
        float n101 = Hlsl.Dot(Hash(i + new float3(1f, 0f, 1f)), f - new float3(1f, 0f, 1f));
        float n011 = Hlsl.Dot(Hash(i + new float3(0f, 1f, 1f)), f - new float3(0f, 1f, 1f));
        float n111 = Hlsl.Dot(Hash(i + new float3(1f, 1f, 1f)), f - new float3(1f, 1f, 1f));

        float x00 = Hlsl.Lerp(n000, n100, u.X);
        float x10 = Hlsl.Lerp(n010, n110, u.X);
        float x01 = Hlsl.Lerp(n001, n101, u.X);
        float x11 = Hlsl.Lerp(n011, n111, u.X);

        float y0 = Hlsl.Lerp(x00, x10, u.Y);
        float y1 = Hlsl.Lerp(x01, x11, u.Y);

        return Hlsl.Lerp(y0, y1, u.Z);
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;

        float2 uv = fragCoord / dispatchSize;

        float3 dir = Hlsl.Normalize(new float3(uv * 2f - 1f, 1f));

        float n = Noise(dir * starDensity);

        float stars =
            Hlsl.Pow(Hlsl.Clamp(n, 0f, 1f), starThreshold) *
            starExposure;

        float flicker = Noise(dir * (starDensity * 0.5f) + new float3(time * flickerSpeed, time * flickerSpeed, time * flickerSpeed));

        stars *= Hlsl.Lerp(0.4f, 1.4f, flicker);

        float3 color = skyTint * stars;

        return new float4(color, 1f);
    }
}
