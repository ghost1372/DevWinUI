using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

// https://www.shadertoy.com/view/wdyczG

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct GradientFlowShader(
    float time,
    float2 dispatchSize,
    float3 colorA,
    float3 colorB,
    float3 colorC,
    float3 colorD
) : ID2D1PixelShader
{
    const float PI = 3.14159265f;
    private static float S(float a, float b, float t)
    {
        float x = Hlsl.Saturate((t - a) / (b - a));
        return x * x * (3f - 2f * x);
    }

    private static float2 Rot(float2 p, float a)
    {
        float s = Hlsl.Sin(a);
        float c = Hlsl.Cos(a);

        return new float2(
            c * p.X - s * p.Y,
            s * p.X + c * p.Y
        );
    }

    private static float2 Hash(float2 p)
    {
        p = new float2(
            Hlsl.Dot(p, new float2(2127.1f, 81.17f)),
            Hlsl.Dot(p, new float2(1269.5f, 283.37f))
        );

        return Hlsl.Frac(Hlsl.Sin(p) * 43758.5453f);
    }

    private static float Noise(float2 p)
    {
        float2 i = Hlsl.Floor(p);
        float2 f = Hlsl.Frac(p);
        float2 u = f * f * (3.0f - 2.0f * f);

        float a = Hlsl.Dot(-1 + 2 * Hash(i + new float2(0, 0)), f - new float2(0, 0));
        float b = Hlsl.Dot(-1 + 2 * Hash(i + new float2(1, 0)), f - new float2(1, 0));
        float c = Hlsl.Dot(-1 + 2 * Hash(i + new float2(0, 1)), f - new float2(0, 1));
        float d = Hlsl.Dot(-1 + 2 * Hash(i + new float2(1, 1)), f - new float2(1, 1));

        return 0.5f + 0.5f * Hlsl.Lerp(
            Hlsl.Lerp(a, b, u.X),
            Hlsl.Lerp(c, d, u.X),
            u.Y
        );
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;
        float2 uv = fragCoord / dispatchSize;

        float ratio = dispatchSize.X / dispatchSize.Y;

        float2 tuv = uv - 0.5f;

        float degree = Noise(new float2(time * 0.1f, tuv.X * tuv.Y));

        tuv.Y *= 1.0f / ratio;

        float angle =
            ((degree - 0.5f) * 720.0f + 180.0f)
            * (PI / 180.0f);

        tuv = Rot(tuv, angle);

        tuv.Y *= ratio;

        float speed = time * 2.0f;

        tuv.X += Hlsl.Sin(tuv.Y * 5.0f + speed) / 30.0f;
        tuv.Y += Hlsl.Sin(tuv.X * 7.5f + speed) / 15.0f;

        float x = Rot(tuv, -5.0f * (PI / 180.0f)).X;

        float layer1Mask = S(-0.3f, 0.2f, x);
        float layer2Mask = S(0.5f, -0.3f, tuv.Y);

        float3 layer1 = Hlsl.Lerp(colorA, colorB, layer1Mask);
        float3 layer2 = Hlsl.Lerp(colorC, colorD, layer2Mask);

        float3 col = Hlsl.Lerp(layer1, layer2, layer2Mask);

        return new float4(col, 1.0f);
    }
}
