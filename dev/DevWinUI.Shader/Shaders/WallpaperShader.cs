using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

// https://www.shadertoy.com/view/XlGXWW

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct WallpaperShader(float time, float2 dispatchSize) : ID2D1PixelShader
{
    private static float N1(float x)
    {
        return Hlsl.Frac(Hlsl.Cos(x * 36.1f) * 924f);
    }

    private static float BoxN1(float2 x)
    {
        float2 c = Hlsl.Ceil(x);
        return Hlsl.Frac(Hlsl.Cos(c.X * 36.1f + c.Y * 47.9f) * 924f);
    }

    private static float3 N3(float n)
    {
        return Hlsl.Frac(Hlsl.Cos(n * new float3(36.1f, 47.9f, 29.8f)) * 924f);
    }

    private float3 ColorFunc(float2 fragCoord, float2 dispatchSizeLocal)
    {
        float2 cs = Hlsl.Cos(time * 0.01f + new float2(0f, 1.57f));

        float2 coord = (fragCoord * 2f - dispatchSizeLocal) / dispatchSizeLocal.Y;

        float2 rot = new float2(
            coord.X * cs.X - coord.Y * (-cs.Y),
            coord.X * cs.Y + coord.Y * cs.X
        ) * 2f;

        float2 r = rot;

        float box = BoxN1(r) * 0.5f;
        r *= 4f;
        box += BoxN1(r) * 0.3f;
        r *= 4f;
        box += BoxN1(r) * 0.1f;
        r *= 2f;
        box += BoxN1(r) * 0.1f;

        float seed = 0f;

        float grad = Hlsl.SmoothStep(-3f, 3f, coord.X + coord.Y * (N1(seed++) - 0.5f));

        float3 colA = N3(seed++);
        float3 colB = N3(seed++);

        float3 col = 1f - Hlsl.Lerp(colA, colB, grad) * (0.8f + 0.2f * Hlsl.Cos(box * 8f + time));

        float nA = N1(seed++);
        float nB = N1(seed++);

        float shade = 0.5f + 0.5f * Hlsl.SmoothStep(-0.8f - 1.2f * nA, 0.8f * nB, coord.Y);

        return col * col * col * col * shade;
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;

        float3 total = 0f;

        float AA = 2f;

        for (float y = 0f; y < AA; y++)
        {
            for (float x = 0f; x < AA; x++)
            {
                float2 offset = new float2(x, y) / AA;
                float2 uv = fragCoord + offset;
                total += ColorFunc(uv, dispatchSize);
            }
        }

        total /= (AA * AA);

        return new float4(total, 1f);
    }
}
