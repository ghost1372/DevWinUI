using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

//https://www.shadertoy.com/view/MdfBzl

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct IceAndFireShader(
    float time,
    float2 dispatchSize
) : ID2D1PixelShader
{
    // s3 = sqrt(3), i3 = 1/sqrt(3)
    private const float S3 = 1.7320508075688772f;
    private const float I3 = 0.5773502691896258f;

    // tri2cart columns: (1, 0), (-0.5, 0.5*s3)
    // cart2tri columns: (1, 0), (i3, 2*i3)
    private static float2 Tri2Cart(float2 p)
        => new float2(p.X - 0.5f * p.Y,
                      0.5f * S3 * p.Y);

    private static float2 Cart2Tri(float2 p)
        => new float2(p.X + I3 * p.Y,
                      2f * I3 * p.Y);

    private static float3 Pal(float t)
    {
        float3 a = new float3(0.5f, 0.5f, 0.5f);
        float3 b = new float3(0.5f, 0.5f, 0.5f);
        float3 c = new float3(0.8f, 0.8f, 0.5f);
        float3 d = new float3(0.0f, 0.2f, 0.5f);
        return Hlsl.Clamp(a + b * Hlsl.Cos(6.28318f * (c * t + d)), 0f, 1f);
    }

    private static float Hash12(float2 p)
    {
        float3 p3 = Hlsl.Frac(new float3(p.X, p.Y, p.X) * 0.1031f);
        p3 += Hlsl.Dot(p3, new float3(p3.Y, p3.Z, p3.X) + 19.19f);
        return Hlsl.Frac((p3.X + p3.Y) * p3.Z);
    }

    private static float2 Hash23(float3 p3)
    {
        float3 scale = new float3(443.897f, 441.423f, 437.195f);
        p3 = Hlsl.Frac(p3 * scale);
        p3 += Hlsl.Dot(p3, new float3(p3.Y, p3.Z, p3.X) + 19.19f);
        return Hlsl.Frac(new float2(p3.X + p3.Y, p3.X + p3.Z) *
                         new float2(p3.Z, p3.Y));
    }

    private static float3 Bary(float2 v0, float2 v1, float2 v2)
    {
        float invDenom = 1f / (v0.X * v1.Y - v1.X * v0.Y);
        float v = (v2.X * v1.Y - v1.X * v2.Y) * invDenom;
        float w = (v0.X * v2.Y - v2.X * v0.Y) * invDenom;
        float u = 1f - v - w;
        return new float3(u, v, w);
    }

    private static float Dseg(float2 xa, float2 ba)
        => Hlsl.Length(xa - ba * Hlsl.Clamp(Hlsl.Dot(xa, ba) / Hlsl.Dot(ba, ba), 0f, 1f));

    private static float2 RandCircle(float3 p)
    {
        float2 rt = Hash23(p);
        float r = Hlsl.Sqrt(rt.X);
        float theta = 6.283185307179586f * rt.Y;
        return r * new float2(Hlsl.Cos(theta), Hlsl.Sin(theta));
    }

    private static float2 RandCircleSpline(float2 p, float t)
    {
        float t1 = Hlsl.Floor(t);
        t -= t1;

        float2 pa = RandCircle(new float3(p.X, p.Y, t1 - 1f));
        float2 p0 = RandCircle(new float3(p.X, p.Y, t1));
        float2 p1 = RandCircle(new float3(p.X, p.Y, t1 + 1f));
        float2 pb = RandCircle(new float3(p.X, p.Y, t1 + 2f));

        float2 m0 = 0.5f * (p1 - pa);
        float2 m1 = 0.5f * (pb - p0);

        float2 c3 = 2f * p0 - 2f * p1 + m0 + m1;
        float2 c2 = -3f * p0 + 3f * p1 - 2f * m0 - m1;
        float2 c1 = m0;
        float2 c0 = p0;

        return (((c3 * t + c2) * t + c1) * t + c0) * 0.8f;
    }

    private float2 TriPoint(float2 p)
    {
        float t0 = Hash12(p);
        return Tri2Cart(p) + 0.45f * RandCircleSpline(p, 0.15f * time + t0);
    }

    private void TriColor(
        float2 p,
        float4 t0, float4 t1, float4 t2,
        float scl,
        ref float4 cw)
    {
        float2 p0 = p - t0.XY;
        float2 p10 = t1.XY - t0.XY;
        float2 p20 = t2.XY - t0.XY;

        float3 b = Bary(p10, p20, p0);

        float d10 = Dseg(p0, p10);
        float d20 = Dseg(p0, p20);
        float d21 = Dseg(p - t1.XY, t2.XY - t1.XY);

        float d = Hlsl.Min(Hlsl.Min(d10, d20), d21);
        d *= -Hlsl.Sign(Hlsl.Min(b.X, Hlsl.Min(b.Y, b.Z)));

        if (d < 0.5f * scl)
        {
            float2 tsum = t0.ZW + t1.ZW + t2.ZW;

            float3 hTri = new float3(
                Hash12(tsum + t0.ZW),
                Hash12(tsum + t1.ZW),
                Hash12(tsum + t2.ZW));

            float2 pctr = (t0.XY + t1.XY + t2.XY) / 3f;
            float theta = 1f + 0.01f * time;
            float2 dir = new float2(Hlsl.Cos(theta), Hlsl.Sin(theta));

            float gradInput = Hlsl.Dot(pctr, dir) - Hlsl.Sin(0.05f * time);
            float h0 = Hlsl.Sin(0.7f * gradInput) * 0.5f + 0.5f;

            hTri = Hlsl.Lerp(new float3(h0, h0, h0), hTri, 0.4f);

            float h = Hlsl.Dot(hTri, b);
            float3 c = Pal(h);
            float w = Hlsl.SmoothStep(0.5f * scl, -0.5f * scl, d);

            cw += new float4(w * c, w);
        }
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;

        float scl = 4.1f / dispatchSize.Y;
        float2 p = (fragCoord - 0.5f - 0.5f * dispatchSize) * scl;

        float2 tfloor = Hlsl.Floor(Cart2Tri(p) + 0.5f);

        // precompute 3×3 neighbourhood of perturbed lattice points
        float2 pts0 = TriPoint(tfloor + new float2(-1, -1));
        float2 pts1 = TriPoint(tfloor + new float2(-1, 0));
        float2 pts2 = TriPoint(tfloor + new float2(-1, 1));
        float2 pts3 = TriPoint(tfloor + new float2(0, -1));
        float2 pts4 = TriPoint(tfloor + new float2(0, 0));
        float2 pts5 = TriPoint(tfloor + new float2(0, 1));
        float2 pts6 = TriPoint(tfloor + new float2(1, -1));
        float2 pts7 = TriPoint(tfloor + new float2(1, 0));
        float2 pts8 = TriPoint(tfloor + new float2(1, 1));

        float4 cw = new float4(0, 0, 0, 0);

        // 2×2 quads, each split into 2 triangles  (mirrors the original loops)
        // quad (i=0, j=0)
        float4 t00 = new float4(pts0, tfloor + new float2(-1, -1));
        float4 t10 = new float4(pts3, tfloor + new float2(0, -1));
        float4 t01 = new float4(pts1, tfloor + new float2(-1, 0));
        float4 t11 = new float4(pts4, tfloor + new float2(0, 0));
        TriColor(p, t00, t10, t11, scl, ref cw);
        TriColor(p, t00, t11, t01, scl, ref cw);

        // quad (i=0, j=1)
        t00 = new float4(pts1, tfloor + new float2(-1, 0));
        t10 = new float4(pts4, tfloor + new float2(0, 0));
        t01 = new float4(pts2, tfloor + new float2(-1, 1));
        t11 = new float4(pts5, tfloor + new float2(0, 1));
        TriColor(p, t00, t10, t11, scl, ref cw);
        TriColor(p, t00, t11, t01, scl, ref cw);

        // quad (i=1, j=0)
        t00 = new float4(pts3, tfloor + new float2(0, -1));
        t10 = new float4(pts6, tfloor + new float2(1, -1));
        t01 = new float4(pts4, tfloor + new float2(0, 0));
        t11 = new float4(pts7, tfloor + new float2(1, 0));
        TriColor(p, t00, t10, t11, scl, ref cw);
        TriColor(p, t00, t11, t01, scl, ref cw);

        // quad (i=1, j=1)
        t00 = new float4(pts4, tfloor + new float2(0, 0));
        t10 = new float4(pts7, tfloor + new float2(1, 0));
        t01 = new float4(pts5, tfloor + new float2(0, 1));
        t11 = new float4(pts8, tfloor + new float2(1, 1));
        TriColor(p, t00, t10, t11, scl, ref cw);
        TriColor(p, t00, t11, t01, scl, ref cw);

        return cw / cw.W;
    }
}
