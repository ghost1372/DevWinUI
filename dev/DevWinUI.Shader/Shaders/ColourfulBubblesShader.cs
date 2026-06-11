using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

// https://www.shadertoy.com/view/4d3BWH

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct ColourfulBubblesShader(
    float time,
    float2 dispatchSize,
    float2 mouse,
    int direction
) : ID2D1PixelShader
{
    private static float S(float a, float b, float t)
    {
        float x = Hlsl.Saturate((t - a) / (b - a));
        return x * x * (3f - 2f * x);
    }

    private static float No(float x, float2 T)
    {
        return Hlsl.Frac(
            9667.5f *
            Hlsl.Sin(7983.75f * (x + T.X) + 297.0f + T.Y)
        );
    }

    private static float4 Rancol(float2 x, float2 T)
    {
        return new float4(
            No(x.X + x.Y, T),
            No(x.X * x.X + x.Y, T),
            No(x.X * x.X + x.Y * x.Y, T),
            1f
        );
    }

    public float4 Execute()
    {
        float2 uv = D2D.GetScenePosition().XY / dispatchSize;
        uv.Y *= dispatchSize.Y / dispatchSize.X;

        float2 T = mouse;
        float2 guv = uv * 20f;

        float2 id = new float2(
            Hlsl.Floor(guv.X),
            Hlsl.Floor(guv.Y)
        );

        float drift = (5f * No(id.X * id.X, T) + 1f) * time * 0.4f;

        if (direction == 0)
        {
            guv.Y += drift;
            guv.Y += No(id.X, T);
        }
        else if (direction == 1)
        {
            guv.Y -= drift;
            guv.Y += No(id.X, T);
        }

        id = new float2(
            Hlsl.Floor(guv.X),
            Hlsl.Floor(guv.Y)
        );

        float2 fuv = Hlsl.Frac(guv) - 0.5f;

        float d = Hlsl.Length(fuv);

        float2 nid = id + new float2(1f, 1f);

        float r =
            0.1f * Hlsl.Sin(time + Hlsl.Sin(time) * 0.5f)
            + 0.3f;

        float r1 =
            0.07f * Hlsl.Sin(2f * time + Hlsl.Sin(2f * time) * 0.5f)
            + 0.1f * No(id.X + id.Y, T);

        float4 color = new float4(0f, 0f, 0f, 1f);

        if (d < r && d > r - 0.1f)
        {
            float4 col =
                0.5f * Rancol(nid, T) +
                new float4(0.5f, 0.5f, 0.5f, 0.5f);

            float a = S(r - 0.12f, r, d);
            float b = 1f - S(r - 0.05f, r + 0.12f, d);

            color = col * a * b;
        }

        if (d < r1)
        {
            color =
                0.5f * Rancol(nid, T) +
                new float4(0.5f, 0.5f, 0.5f, 0.5f);
        }

        return color;
    }
}
