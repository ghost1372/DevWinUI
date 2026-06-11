using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

// https://www.shadertoy.com/view/fcf3Dn

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct PS3XMBBackgroundShader(float time, float2 dispatchSize) : ID2D1PixelShader
{
    private const float THRESHOLD = 0.99f;
    private const float MIN_DIST = 0.04f;
    private const float MAX_DIST = 40f;

    private static float Hash12(float2 p)
    {
        UInt2 q = new UInt2((uint)p.X, (uint)p.Y) * new UInt2(1597334673u, 3812015801u);
        uint n = (q.X ^ q.Y) * 1597334673u;
        return (float)n * 2.328306437080797e-10f;
    }

    private static float Value2D(float2 p)
    {
        float2 pg = Hlsl.Floor(p);
        float2 pc = p - pg;
        pc = pc * pc * (3f - 2f * pc);

        return Hlsl.Lerp(
            Hlsl.Lerp(Hash12(pg + new float2(0, 0)), Hash12(pg + new float2(1, 0)), pc.X),
            Hlsl.Lerp(Hash12(pg + new float2(0, 1)), Hash12(pg + new float2(1, 1)), pc.X),
            pc.Y
        );
    }

    private static float GetStarsRough(float2 p)
    {
        float s = Hlsl.SmoothStep(THRESHOLD, 1f, Hash12(p));

        if (s >= THRESHOLD)
        {
            float v = (s - THRESHOLD) / (1f - THRESHOLD);
            v = Hlsl.Clamp(v, 0f, 1f);
            s = Hlsl.Pow(v, 10f);
        }

        return s;
    }

    private static float GetStars(float2 p, float a, float t, float time)
    {
        float2 pg = Hlsl.Floor(p);
        float2 pc = p - pg;
        pc = pc * pc * (3f - 2f * pc);

        float s = Hlsl.Lerp(
            Hlsl.Lerp(GetStarsRough(pg), GetStarsRough(pg + new float2(1, 0)), pc.X),
            Hlsl.Lerp(GetStarsRough(pg + new float2(0, 1)), GetStarsRough(pg + new float2(1, 1)), pc.X),
            pc.Y
        );

        float anim = Value2D(p * 0.1f + time * new float2(20f, -10.1f));
        anim = Hlsl.Clamp(anim, 0f, 1f);
        anim = anim * 0.5f + 0.5f;

        return Hlsl.SmoothStep(a, a + t, s) * Hlsl.Pow(anim, 8.3f);
    }

    private static float GetDust(float2 p, float2 size, float f, float time, float2 dispatchSizeLocal)
    {
        float2 ar = new float2(dispatchSizeLocal.X / dispatchSizeLocal.Y, 1f);

        float2 pp = p * size * ar;

        float w = 0.64f + 0.46f * Hlsl.Cos(p.X * 6.2831f);
        w = Hlsl.Max(w, 0f);

        float s1 = GetStars(pp + time * new float2(20f, -10.1f), 0.11f, 0.71f, time);
        float s2 = GetStars(pp + time * new float2(30f, -10.1f), 0.1f, 0.31f, time);
        float s3 = GetStars(pp + time * new float2(40f, -10.1f), 0.1f, 0.91f, time);

        return w * f * (s1 * 4f + s2 * 5f + s3 * 2f);
    }

    private static float Sdf(float3 p, float time)
    {
        p *= 2f;

        float o =
            8.2f * Hlsl.Sin(0.05f * p.X + time * 0.25f) +
            (0.04f * p.Z) *
            Hlsl.Sin(p.X * 0.11f + time) *
            2f * Hlsl.Sin(p.Z * 0.2f + time) *
            Value2D(new float2(0.03f, 0.4f) * p.XZ + new float2(time * 0.5f, 0f));

        return Hlsl.Abs(Hlsl.Dot(p, new float3(0f, 1f, 0.05f)) + 2.5f + o * 0.5f);
    }

    private static float2 RayMarch(float3 o, float3 d, float jitter, float time)
    {
        float t = jitter * 2f;
        float a = 0f;
        float g = MAX_DIST;
        int dr = 0;

        for (int i = 0; i < 100; i++)
        {
            float3 p = o + d * t;
            float ndt = Sdf(p, time);

            if (t >= MAX_DIST) break;

            if (Hlsl.Abs(ndt) < MIN_DIST)
            {
                if (dr > 40) break;
                dr++;

                float f = Hlsl.SmoothStep(0f, 0.3f, (p.Z * 0.9f) / 100f);

                a += 0.015f * f;
                t += 0.05f;
            }
            else
            {
                t += Hlsl.Abs(ndt) * 0.8f;
            }
        }

        return new float2(a, Hlsl.Clamp(1f - g / 3f, 0f, 1f));
    }

    private static float Dither(float2 pos)
    {
        return Hlsl.Frac(52.9829189f * Hlsl.Frac(Hlsl.Dot(pos, new float2(0.06711056f, 0.00583715f))));
    }

    public float4 Execute()
    {
        float2 U = D2D.GetScenePosition().XY;
        float2 ires = dispatchSize;

        float3 o = 0f;
        float3 d = new float3((U - 0.5f * ires) / ires.Y, 1f);

        float2 mg = RayMarch(o, d, Dither(U), time);
        float m = mg.X;

        float q = Hlsl.Frac(time / 86400f);
        float p = Hlsl.Sin((6.2831853f * q) + (1.5707963f));

        float3 l1 = Hlsl.Lerp(new float3(0.149f, 0.471f, 0.569f), new float3(0.231f, 0.231f, 0.231f), p);
        float3 l2 = Hlsl.Lerp(new float3(0.075f, 0.333f, 0.412f), new float3(0.129f, 0.129f, 0.129f), p);
        float3 l3 = Hlsl.Lerp(new float3(0.063f, 0.329f, 0.412f), new float3(0.149f, 0.149f, 0.149f), p);
        float3 l4 = Hlsl.Lerp(new float3(0.169f, 0.482f, 0.580f), new float3(0.251f, 0.251f, 0.251f), p);

        float2 uv = U / ires;

        float3 c = Hlsl.Lerp(
            Hlsl.Lerp(l1, l2, uv.X),
            Hlsl.Lerp(l3, l4, uv.X),
            uv.Y
        );

        c = Hlsl.Lerp(c, 1f, Hlsl.Clamp(m, 0f, 1f));

        c += GetDust(uv, new float2(2000f, 2000f), mg.Y, time, ires) * 0.3f;

        return new float4(c, 1f);
    }
}
