using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

// https://www.shadertoy.com/view/4sfyzN

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct ResidualBackgroundRadiationShader(float time, float2 dispatchSize, float2 mouse) : ID2D1PixelShader
{
    private static float2 Rotate(float2 p, float a)
    {
        float s = Hlsl.Sin(a);
        float c = Hlsl.Cos(a);

        return new float2(
            p.X * c - p.Y * s,
            p.X * s + p.Y * c
        );
    }

    private static float2 Hash22(float2 p)
    {
        float3 p3 = Hlsl.Frac(new float3(p.X, p.X, p.Y) * new float3(443.897f, 441.423f, 437.195f));

        p3 += Hlsl.Dot(
            p3,
            new float3(p3.Y, p3.Z, p3.X) + 19.19f
        );

        return Hlsl.Frac(
            new float2(p3.X + p3.X, p3.Y + p3.Z) * new float2(p3.Z, p3.Y)
        );
    }

    private static float Cell(float2 x, float t)
    {
        float2 iPos = Hlsl.Floor(x);
        float2 fPos = Hlsl.Frac(x);
        float minDist1 = 2f;

        for (int y = -1; y <= 1; y++)
        {
            for (int x2 = -1; x2 <= 1; x2++)
            {
                float2 n = new float2(x2, y);

                float2 r = Hash22(iPos + n);

                r = 0.5f + Hlsl.Sin(t + r * 6.2831853f) * 0.5f;

                float2 d = n + r - fPos;

                float dist = Hlsl.Length(d);

                minDist1 = Hlsl.Min(dist, minDist1);
            }
        }

        return minDist1;
    }

    private static float Hsb2RgbV(float3 c)
    {
        float3 rgb = Hlsl.Clamp(
            Hlsl.Abs(
                Hlsl.Frac(c.X * 6f + new float3(0f, 4f, 2f)) - 3f
            ) - 1f,
            0f,
            1f
        );

        rgb = rgb * rgb * (3f - 2f * rgb);

        return c.Z * Hlsl.Lerp(new float3(1f, 1f, 1f), rgb, c.Y).X;
    }

    public float4 Execute()
    {
        float2 p = D2D.GetScenePosition().XY / dispatchSize;
        float2 m = 1f - mouse / dispatchSize;

        float2 pos = new float2(
            p.X * dispatchSize.X / dispatchSize.Y,
            p.Y
        );

        pos = Rotate(pos, (m.X + m.Y) * 0.1f);

        float brightness = 0f;

        float2 pp = pos - m * 8f;

        for (int i = 1; i <= 4; i++)
        {
            brightness += Cell(pp, time * 0.5f * i) / i;
            pp *= 4f;
        }

        brightness *= brightness;

        float hue = Hlsl.Length(p - m) * 0.2f + 0.5f;

        float3 col = new float3(hue, 0.5f, brightness);
        float3 rgb = Hsb2RgbV(col);

        return new float4(rgb, 1f);
    }
}
