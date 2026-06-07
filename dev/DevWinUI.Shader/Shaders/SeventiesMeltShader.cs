using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

//https://www.shadertoy.com/view/XsX3zl

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct SeventiesMeltShader(
    float time,
    float2 dispatchSize,
    float speed,            // Overall animation speed multiplier (default: 1.0)
    float warpSpeed,        // Speed of the inner warp/distortion loop independently of base speed (default: 1.0)
    float zoom,             // Number of distortion iterations — higher = more warped (default: 40, range: 1–40)
    float xAmplitude,       // Strength of horizontal warp displacement (default: 1.0)
    float yAmplitude,       // Strength of vertical warp displacement (default: 1.0)
    float scaleAmplitude,   // Amplitude of the fScale oscillation driving overall warp scale (default: 1.0)
    float vignetteStrength  // Strength of the vignette darkening at edges (default: 1.0)
) : ID2D1PixelShader
{
    private static float CosRange(float amt, float range, float minimum)
    {
        return ((1f + Hlsl.Cos(amt * 3.1415927f / 180f)) * 0.5f) * range + minimum;
    }

    public float4 Execute()
    {
        const float brightness = 0.975f;

        // speed scales the base time; warpSpeed further scales the inner warp offsets
        float t = time * 1.25f * speed;

        float2 fragCoord = D2D.GetScenePosition().XY;

        float2 uv = fragCoord / dispatchSize;
        float2 p = (2f * fragCoord - dispatchSize) / Hlsl.Max(dispatchSize.X, dispatchSize.Y);

        float ct = CosRange(t * 5f, 3f, 1.1f);
        float xBoost = CosRange(t * 0.2f, 5f, 5f);
        float yBoost = CosRange(t * 0.1f, 10f, 5f);

        // scaleAmplitude scales the oscillation range of fScale (original range was 1.25)
        float fScale = CosRange(t * 15.5f, 1.25f * scaleAmplitude, 0.5f);

        // zoom: clamp to [1, 40] and cast to int for loop iterations
        int zoomInt = (int)Hlsl.Clamp(zoom, 1f, 40f);

        for (int i = 1; i < 40; i++)
        {
            if (i >= zoomInt) break;

            float fi = (float)i;

            float2 newp = p;

            // xAmplitude scales the 0.25 horizontal displacement coefficient
            newp.X += (0.25f * xAmplitude) / fi *
                      Hlsl.Sin(fi * p.Y + t * warpSpeed * Hlsl.Cos(ct) * 0.025f + 0.005f * fi)
                      * fScale
                      + xBoost;

            // yAmplitude scales the 0.25 vertical displacement coefficient
            newp.Y += (0.25f * yAmplitude) / fi *
                      Hlsl.Sin(fi * p.X + t * warpSpeed * ct * 0.0075f + 0.03f * (fi + 15f))
                      * fScale
                      + yBoost;

            p = newp;
        }

        float3 col = new float3(
            0.5f * Hlsl.Sin(3f * p.X) + 0.5f,
            0.5f * Hlsl.Sin(3f * p.Y) + 0.5f,
            Hlsl.Sin(p.X + p.Y)
        );

        col *= brightness;

        // vignette
        float2 v = uv - 0.5f;
        float vigAmt = 5f * vignetteStrength;

        float vignette =
            (1f - vigAmt * v.Y * v.Y) *
            (1f - vigAmt * v.X * v.X);

        float extrusion = (col.X + col.Y + col.Z) / 4f;
        extrusion *= 1.5f;
        extrusion *= vignette;

        return new float4(col, extrusion);
    }
}
