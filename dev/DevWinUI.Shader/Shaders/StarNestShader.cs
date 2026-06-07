using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

//https://www.shadertoy.com/view/Mll3zj

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct StarNestShader(
    float time,
    float2 dispatchSize,

    float speed,
    float zoom,
    float tile,

    float rotationSpeed1,
    float rotationSpeed2,

    float stepScale,
    float fadePower,

    float colorIntensity,

    float3 baseColor
) : ID2D1PixelShader
{
    private static float2 Rotate(float2 v, float a)
    {
        float s = Hlsl.Sin(a);
        float c = Hlsl.Cos(a);
        return new float2(c * v.X - s * v.Y, s * v.X + c * v.Y);
    }

    private static float3 Abs(float3 x)
    {
        return new float3(Hlsl.Abs(x.X), Hlsl.Abs(x.Y), Hlsl.Abs(x.Z));
    }

    private static float Length(float3 v)
    {
        return Hlsl.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
    }

    private static float Mod(float x, float y)
    {
        return x - y * Hlsl.Floor(x / y);
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;

        float2 uv = fragCoord / dispatchSize - 0.5f;
        uv.Y *= dispatchSize.Y / dispatchSize.X;

        float t = time * speed + 0.25f;

        float3 dir = new float3(uv * zoom, 1f);

        dir.XZ = Rotate(dir.XZ, t * rotationSpeed1);
        dir.XY = Rotate(dir.XY, t * rotationSpeed2);

        float3 from = new float3(1f, 0.5f, 0.5f);
        from += new float3(t * 2f, t, -2f);

        from.XZ = Rotate(from.XZ, rotationSpeed1);
        from.XY = Rotate(from.XY, rotationSpeed2);

        float s = 0.1f;
        float fade = 1f;

        float3 v = new float3(0f, 0f, 0f);

        const int volsteps = 20;
        const int iterations = 17;

        for (int r = 0; r < volsteps; r++)
        {
            float3 p = from + s * dir * 0.5f;

            float3 tvec = new float3(tile, tile, tile * 2f);

            float3 m = new float3(
                Mod(p.X, tvec.X),
                Mod(p.Y, tvec.Y),
                Mod(p.Z, tvec.Z)
            );

            p = Abs(new float3(tile, tile, tile) - m);

            float pa = 0f;
            float a = 0f;

            for (int i = 0; i < iterations; i++)
            {
                float dotp = p.X * p.X + p.Y * p.Y + p.Z * p.Z;

                p = Abs(p) / dotp - 0.53f;

                float lenp = Length(p);
                a += Hlsl.Abs(lenp - pa);
                pa = lenp;
            }

            float dm = Hlsl.Max(0f, 0.3f - a * a * stepScale);

            a *= a * a;

            if (r > 6)
                fade *= (1f - dm);

            v += fade;
            v += baseColor * (s * s * s * s) * a * 0.0015f * fade;

            fade *= fadePower;
            s += 0.1f;
        }

        float l = Length(v);

        v = Hlsl.Lerp(new float3(l, l, l), v, 0.85f);
        v *= colorIntensity;

        return new float4(v, 1f);
    }
}
