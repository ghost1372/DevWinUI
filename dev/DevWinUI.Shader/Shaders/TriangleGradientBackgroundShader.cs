using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;
// https://www.shadertoy.com/view/cldGDf

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct TriangleGradientBackgroundShader(float time, float2 dispatchSize)
    : ID2D1PixelShader
{
    private static float Hash12(float2 p)
    {
        float3 p3 = Hlsl.Frac(new float3(p.X, p.Y, p.X) * 0.1031f);
        p3 += Hlsl.Dot(p3, new float3(p3.Y, p3.Z, p3.X) + 19.19f);
        return Hlsl.Frac((p3.X + p3.Y) * p3.Z);
    }

    private static float2 Hash23(float3 p3)
    {
        p3 = Hlsl.Frac(p3 * new float3(443.897f, 441.423f, 437.195f));
        p3 += Hlsl.Dot(p3, new float3(p3.Y, p3.Z, p3.X) + 19.19f);
        return Hlsl.Frac(new float2(p3.X + p3.Y, p3.Z + p3.X) * new float2(p3.Z, p3.Y));
    }

    private static float2 Tri2Cart(float2 p)
        => new float2(p.X, -0.5f * p.X + 0.8660254f * p.Y);

    private static float2 Cart2Tri(float2 p)
        => new float2(p.X, 0.577350269f * p.X + 1.1547005f * p.Y);

    private static float2 RandCircle(float3 p)
    {
        float2 rt = Hash23(p);
        float r = Hlsl.Sqrt(rt.X);
        float theta = 6.2831853f * rt.Y;
        return r * new float2(Hlsl.Cos(theta), Hlsl.Sin(theta));
    }

    private static float2 RandSpline(float2 p, float t)
    {
        float t0 = Hlsl.Floor(t);
        t -= t0;

        float2 pa = RandCircle(new float3(p, t0 - 1));
        float2 p0 = RandCircle(new float3(p, t0));
        float2 p1 = RandCircle(new float3(p, t0 + 1));
        float2 pb = RandCircle(new float3(p, t0 + 2));

        float2 m0 = 0.5f * (p1 - pa);
        float2 m1 = 0.5f * (pb - p0);

        float2 c3 = 2 * p0 - 2 * p1 + m0 + m1;
        float2 c2 = -3 * p0 + 3 * p1 - 2 * m0 - m1;
        float2 c1 = m0;
        float2 c0 = p0;

        float2 t2 = new float2(t, t);
        return (((c3 * t2 + c2) * t2 + c1) * t2 + c0) * 0.8f;
    }

    private static float2 TriPoint(float2 p, float t)
    {
        float t0 = Hash12(p);
        return Tri2Cart(p) + 0.45f * RandSpline(p, t + t0);
    }

    private static float Sgn(float2 p1, float2 p2, float2 p3)
        => (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);

    private static bool PointInTriangle(float2 pt, float2 v1, float2 v2, float2 v3)
    {
        float d1 = Sgn(pt, v1, v2);
        float d2 = Sgn(pt, v2, v3);
        float d3 = Sgn(pt, v3, v1);

        bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

        return !(hasNeg && hasPos);
    }

    private static float3 ColourForPoint(float2 uv, float t)
    {
        float3 a = new float3(0.5f, 0.5f, 0.5f);
        float3 b = new float3(0.5f, 0.5f, 0.5f);
        float3 c = new float3(1.0f, 0.8f, 0.5f);
        float3 d = new float3(0.0f, 0.2f, 0.5f);

        float k = uv.X + uv.Y + t * 0.1f;

        float3 col = a + b * Hlsl.Cos(6.28318f * (c * k + d));
        return Hlsl.Clamp(col, 0, 1);
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;
        float2 iResolution = dispatchSize;

        float scl = 6.2f / iResolution.Y;

        float2 p = (fragCoord - 0.5f * iResolution) * scl;

        float2 tfloor = Hlsl.Floor(Cart2Tri(p) + 0.5f);

        float2 center = 0;

        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                float2 t00 = TriPoint(tfloor + new float2(i - 1, j - 1), time);
                float2 t10 = TriPoint(tfloor + new float2(i, j - 1), time);
                float2 t01 = TriPoint(tfloor + new float2(i - 1, j), time);
                float2 t11 = TriPoint(tfloor + new float2(i, j), time);

                if (PointInTriangle(p, t00, t10, t11))
                    center = (t00 + t10 + t11) / 3;

                if (PointInTriangle(p, t00, t11, t01))
                    center = (t00 + t11 + t01) / 3;
            }

        center = center / scl;
        center += 0.5f * iResolution;
        center = center / iResolution;

        float3 col = ColourForPoint(center, time);

        return new float4(col, 1.0f);
    }
}
