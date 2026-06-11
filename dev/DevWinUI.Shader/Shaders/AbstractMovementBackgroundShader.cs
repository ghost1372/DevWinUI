using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

// https://www.shadertoy.com/view/NlsyR7
//new float2(-1,-1) // TopLeft
//new float2( 1,-1) // TopRight
//new float2(-1, 1) // BottomLeft
//new float2( 1, 1) // BottomRight

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct AbstractMovementBackgroundShader(
    float time,
    float2 dispatchSize,
    float3 colorA,
    float3 colorB,
    float3 colorC,
    float3 colorD,
    float2 direction
) : ID2D1PixelShader
{
    private static float2 Rot(float2 p, float a)
    {
        float s = Hlsl.Sin(a);
        float c = Hlsl.Cos(a);
        return new float2(p.X * c - p.Y * s, p.X * s + p.Y * c);
    }

    private static float Rand(float i)
    {
        return Hlsl.Frac(Hlsl.Sin(i * 23325f) * 35543f);
    }

    private static float4 Rand4(float i)
    {
        float4 v = new float4(i, i, i, i) * new float4(23325f, 53464f, 76543f, 12312f);
        return Hlsl.Frac(Hlsl.Sin(v) * new float4(35543f, 63454f, 23454f, 87651f));
    }

    private static float DrawLine(float2 uv, float2 a, float2 b)
    {
        float2 ba = b - a;
        float2 pa = uv - a;
        float h = Hlsl.Clamp(Hlsl.Dot(pa, ba) / Hlsl.Dot(ba, ba), 0f, 1f);
        return Hlsl.Length(pa - h * ba);
    }

    private static float DrawLineSegment(float2 uv, float linesCount, float speed, float verticalAmplitude, float segmentSeed, float time)
    {
        float mask = 0f;
        float step = 1f / linesCount;
        float t = time * speed * 0.1f;

        float amp = 3.5f;
        float2 wmin = new float2(0.2f, 1.5f);
        float2 smin = new float2(0.005f, 0.035f);

        float baseSeed = Rand(segmentSeed);

        for (int i = 0; i <= linesCount; i++)
        {
            float fi = i * step;

            float unitSpeed = Hlsl.Lerp(0.5f, 2f, Rand(fi));
            float seed = t * unitSpeed + fi + baseSeed;

            float it = Hlsl.Frac(seed);
            float4 h = Rand4(fi);

            float n = it * 2f - 1f;

            float w = Hlsl.Lerp(wmin.X, wmin.Y, Hlsl.Pow(h.Y, 2f));

            float2 a = new float2(-amp * n, h.X * verticalAmplitude);
            float2 b = a + new float2(w, 0f);

            float d = DrawLine(uv, a, b);

            float s = Hlsl.Lerp(smin.X, smin.Y, Hlsl.Pow(h.Z, 4f));

            mask += Hlsl.SmoothStep(s + 0.002f, s - 0.002f, d);
        }

        return Hlsl.Clamp(mask, 0f, 1f);
    }

    private static float2 ApplyDirection(float2 uv, float2 dir)
    {
        if (dir.X > 0f) uv.X = 1f - uv.X;
        if (dir.Y > 0f) uv.Y = 1f - uv.Y;
        return uv;
    }

    public float4 Execute()
    {
        float2 p = D2D.GetScenePosition().XY;

        float2 uv = (2f * p - dispatchSize) / dispatchSize.Y;

        uv = ApplyDirection(uv, direction);

        uv = Rot(uv, -35f * 0.01745329252f);

        float3 colA = colorA;
        float3 colB = colorB;
        float3 colC = colorC;
        float3 colD = colorD;

        float3 col = Hlsl.Lerp(colB, colA, Hlsl.SmoothStep(-0.055f, -0.05f, uv.Y));
        col = Hlsl.Lerp(colC, col, Hlsl.SmoothStep(-0.655f, -0.65f, uv.Y));
        col = Hlsl.Lerp(colD, col, Hlsl.SmoothStep(-1.305f, -1.3f, uv.Y));

        col = Hlsl.Lerp(col, colA, DrawLineSegment(uv - new float2(0f, -0.3f), 9f, 0.5f, 0.35f, 0.2f, time));
        col = Hlsl.Lerp(col, colB, DrawLineSegment(uv - new float2(0f, 0f), 25f, -0.4f, 0.5f, 0.1f, time));

        col = Hlsl.Lerp(col, colB, DrawLineSegment(uv - new float2(0f, -1f), 25f, -0.4f, 0.45f, 0.4f, time));
        col = Hlsl.Lerp(col, colC, DrawLineSegment(uv - new float2(0f, -0.65f), 25f, 0.3f, 0.3f, 0.3f, time));

        col = Hlsl.Lerp(col, colC, DrawLineSegment(uv - new float2(0f, -1.8f), 25f, 0.3f, 0.55f, 0.6f, time));
        col = Hlsl.Lerp(col, colD, DrawLineSegment(uv - new float2(0f, -1.3f), 25f, -0.2f, 0.3f, 0.5f, time));

        return new float4(col, 1f);
    }
}
