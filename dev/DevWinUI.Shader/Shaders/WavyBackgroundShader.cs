using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

// https://www.shadertoy.com/view/ltGSWD

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct WavyBackgroundShader(
    float time,
    float2 dispatchSize
) : ID2D1PixelShader
{
    private static float Gradient(float p)
    {
        float2 pt0 = new float2(0.00f, 0.0f);
        float2 pt1 = new float2(0.86f, 0.1f);
        float2 pt2 = new float2(0.955f, 0.40f);
        float2 pt3 = new float2(0.99f, 1.0f);
        float2 pt4 = new float2(1.00f, 0.0f);

        if (p < pt0.X) return pt0.Y;

        if (p < pt1.X)
            return Hlsl.Lerp(pt0.Y, pt1.Y, (p - pt0.X) / (pt1.X - pt0.X));

        if (p < pt2.X)
            return Hlsl.Lerp(pt1.Y, pt2.Y, (p - pt1.X) / (pt2.X - pt1.X));

        if (p < pt3.X)
            return Hlsl.Lerp(pt2.Y, pt3.Y, (p - pt2.X) / (pt3.X - pt2.X));

        if (p < pt4.X)
            return Hlsl.Lerp(pt3.Y, pt4.Y, (p - pt3.X) / (pt4.X - pt3.X));

        return pt4.Y;
    }

    private float WaveN(
        float2 uv,
        float2 s12,
        float2 t12,
        float2 f12,
        float2 h12)
    {
        float2 x12 =
            Hlsl.Sin((time * s12 + t12 + uv.X) * f12) * h12;

        float g = Gradient(
            uv.Y / (0.5f + x12.X + x12.Y));

        return g * 0.27f;
    }

    private float Wave1(float2 uv)
    {
        return WaveN(
            new float2(uv.X, uv.Y - 0.25f),
            new float2(0.03f, 0.06f),
            new float2(0.00f, 0.02f),
            new float2(8.0f, 3.7f),
            new float2(0.06f, 0.05f));
    }

    private float Wave2(float2 uv)
    {
        return WaveN(
            new float2(uv.X, uv.Y - 0.25f),
            new float2(0.04f, 0.07f),
            new float2(0.16f, -0.37f),
            new float2(6.7f, 2.89f),
            new float2(0.06f, 0.05f));
    }

    private float Wave3(float2 uv)
    {
        return WaveN(
            new float2(uv.X, 0.75f - uv.Y),
            new float2(0.035f, 0.055f),
            new float2(-0.09f, 0.27f),
            new float2(7.4f, 2.51f),
            new float2(0.06f, 0.05f));
    }

    private float Wave4(float2 uv)
    {
        return WaveN(
            new float2(uv.X, 0.75f - uv.Y),
            new float2(0.032f, 0.09f),
            new float2(0.08f, -0.22f),
            new float2(6.5f, 3.89f),
            new float2(0.06f, 0.05f));
    }

    public float4 Execute()
    {
        float2 uv = D2D.GetScenePosition().XY / dispatchSize;

        float waves =
            Wave1(uv) +
            Wave2(uv) +
            Wave3(uv) +
            Wave4(uv);

        float x = uv.X;
        float y = 1.0f - uv.Y;

        float3 bg = Hlsl.Lerp(
            new float3(0.05f, 0.05f, 0.3f),
            new float3(0.1f, 0.65f, 0.85f),
            (x + y) * 0.55f);

        float3 color = bg + waves;

        return new float4(color, 1f);
    }
}
