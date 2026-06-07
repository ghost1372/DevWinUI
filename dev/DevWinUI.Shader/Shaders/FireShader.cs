using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

//https://www.shadertoy.com/view/ssfXD4

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct FireShader(
    float time,
    float2 dispatchSize,
    float speed,            // Overall animation speed multiplier (default: 1.0)
    float smokeSpeed,       // Speed of smoke movement independent of base speed (default: 1.0)
    float particleSize,     // Scales the size of individual fire particles (default: 1.0)
    float particleDensity,  // Number of particle layers rendered (default: 15, range: 1–15)
    float smokeAmount,      // Intensity/density of the smoke layer (default: 1.0)
    float flameHeight,      // Controls how high the flame/smoke rises (default: 1.0)
    float vignetteStrength  // Strength of the vignette darkening at edges (default: 1.0)
) : ID2D1PixelShader
{
    private static float Hash1_2(float2 x)
    {
        return Hlsl.Frac(Hlsl.Sin(Hlsl.Dot(x, new float2(52.127f, 61.2871f))) * 521.582f);
    }

    private static float2 Hash2_2(float2 x)
    {
        float2 r = Hlsl.Sin(x * new float2x2(
            20.52f, 24.1994f,
            70.291f, 80.171f));

        return Hlsl.Frac(r * 492.194f);
    }

    private static float2 Noise2_2(float2 uv)
    {
        float2 f = Hlsl.SmoothStep(0f, 1f, Hlsl.Frac(uv));

        float2 uv00 = Hlsl.Floor(uv);
        float2 uv01 = uv00 + new float2(0f, 1f);
        float2 uv10 = uv00 + new float2(1f, 0f);
        float2 uv11 = uv00 + 1f;

        float2 v00 = Hash2_2(uv00);
        float2 v01 = Hash2_2(uv01);
        float2 v10 = Hash2_2(uv10);
        float2 v11 = Hash2_2(uv11);

        float2 v0 = Hlsl.Lerp(v00, v01, f.Y);
        float2 v1 = Hlsl.Lerp(v10, v11, f.Y);

        return Hlsl.Lerp(v0, v1, f.X);
    }

    private static float Noise1_2(float2 uv)
    {
        float2 f = Hlsl.Frac(uv);

        float2 uv00 = Hlsl.Floor(uv);
        float2 uv01 = uv00 + new float2(0f, 1f);
        float2 uv10 = uv00 + new float2(1f, 0f);
        float2 uv11 = uv00 + 1f;

        float v00 = Hash1_2(uv00);
        float v01 = Hash1_2(uv01);
        float v10 = Hash1_2(uv10);
        float v11 = Hash1_2(uv11);

        float v0 = Hlsl.Lerp(v00, v01, f.Y);
        float v1 = Hlsl.Lerp(v10, v11, f.Y);

        return Hlsl.Lerp(v0, v1, f.X);
    }

    private static float2 Rotate(float2 p, float a)
    {
        float s = Hlsl.Sin(a);
        float c = Hlsl.Cos(a);
        return Hlsl.Mul(p, new float2x2(s, c, -c, s));
    }

    private static float2 VoronoiPointFromRoot(float2 root, float deg)
    {
        float2 point = Hash2_2(root) - 0.5f;

        float s = Hlsl.Sin(deg);
        float c = Hlsl.Cos(deg);

        point = Hlsl.Mul(point, new float2x2(s, c, -c, s)) * 0.66f;
        return point + root + 0.5f;
    }

    private static float DegFromRootUV(float2 uv, float time)
    {
        return time * 1.5f * (Hash1_2(uv) - 0.5f) * 2f;
    }

    private static float2 RandomAround2_2(float2 point, float2 range, float2 uv)
    {
        return point + (Hash2_2(uv) - 0.5f) * range;
    }

    private static float LayeredNoise1_2(float2 uv, float sizeMod, float alphaMod, int layers, float animation, float time)
    {
        float noise = 0f;
        float alpha = 1f;
        float size = 1f;
        float2 offset = 0f;

        for (int i = 0; i < 6; i++) // fixed safe upper bound (original = 6)
        {
            if (i >= layers) break;

            offset += Hash2_2(new float2(alpha, size)) * 10f;

            noise += Noise1_2(uv * size + time * animation * 8f + offset) * alpha;

            alpha *= alphaMod;
            size *= sizeMod;
        }

        return noise;
    }

    // particleScale: derived from particleSize — smaller value = larger particles
    private static float3 FireParticles(float2 uv, float2 originalUV, float time, float particleScale)
    {
        float3 particles = 0f;

        float2 rootUV = Hlsl.Floor(uv);
        float deg = DegFromRootUV(rootUV, time);

        float2 pointUV = VoronoiPointFromRoot(rootUV, deg);

        float2 tempUV = uv + (Noise2_2(uv * 2f) - 0.5f) * 0.1f;
        tempUV += -(Noise2_2(uv * 3f + time) - 0.5f) * 0.07f;

        // particleScale shrinks the threshold distances → effectively scales particle size
        float dist = Hlsl.Length(Rotate(tempUV - pointUV, 0.7f) * RandomAround2_2(new float2(0.5f, 1.6f), new float2(0.25f, 0.2f), rootUV));
        float distBloom = Hlsl.Length(Rotate(tempUV - pointUV, 0.7f) * RandomAround2_2(new float2(0.5f, 0.8f), new float2(0.3f, 0.1f), rootUV));

        float3 sparkColor = new float3(1.0f, 0.4f, 0.05f) * 1.5f;
        float3 bloomColor = new float3(1.0f, 0.4f, 0.05f) * 0.8f;

        float threshold = 0.009f * particleScale;
        particles += (1f - Hlsl.SmoothStep(threshold * 0.6f, threshold * 3f, dist)) * sparkColor;
        particles += Hlsl.Pow((1f - Hlsl.SmoothStep(0f, threshold * 6f, distBloom)), 3f) * bloomColor;

        return particles;
    }

    private float3 LayeredParticles(float2 uv, int layers, float smoke, float time)
    {
        float3 particles = 0f;

        float size = 1f;
        float alpha = 1f;
        float2 offset = 0f;

        // particleSize < 1 → larger particles; > 1 → smaller/tighter particles
        float particleScale = 1f / particleSize;

        for (int i = 0; i < 15; i++)
        {
            if (i >= layers) break;

            float2 noiseOffset = (Noise2_2(uv * size * 2f + 0.5f) - 0.5f) * 0.15f;
            float2 bokehUV = uv * size + time + offset + noiseOffset;

            particles += FireParticles(bokehUV, uv, time, particleScale) * alpha;

            offset += Hash2_2(new float2(alpha, alpha)) * 10f;

            alpha *= 0.9f;
            size *= 1.05f;
        }

        return particles;
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;

        float2 uv = (2f * fragCoord - dispatchSize) / dispatchSize.X;

        // vignetteStrength scales the vignette fade (1.0 = original, 0.0 = no vignette)
        float rawVignette = 1f - Hlsl.SmoothStep(0.4f, 1.4f, Hlsl.Length(uv + new float2(0f, 0.3f)));
        float vignette = Hlsl.Lerp(1f, rawVignette, vignetteStrength);

        uv *= 1.8f;

        // Scaled time for base motion and individual smoke motion
        float scaledTime = time * speed;
        float scaledSmokeTime = time * speed * smokeSpeed;

        // flameHeight shifts the vertical cutoff so smoke/flame rises higher or lower
        float heightBias = (flameHeight - 1f) * 0.8f;

        float smokeIntensity =
            LayeredNoise1_2(uv * 10f + scaledSmokeTime * 4f, 1.7f, 0.7f, 6, 0.2f, scaledTime);

        // flameHeight: larger value pushes the vertical fade upward (taller flame)
        smokeIntensity *= Hlsl.Pow(1f - Hlsl.SmoothStep(-1f - heightBias, 1.6f - heightBias, uv.Y), 2f);

        // smokeAmount scales the overall smoke density
        float3 smoke = smokeIntensity * new float3(1f, 0.43f, 0.1f) * 0.8f * smokeAmount;

        smoke *= Hlsl.Pow(
            LayeredNoise1_2(uv * 4f + scaledSmokeTime * 0.5f, 1.8f, 0.5f, 3, 0.2f, scaledTime),
            2f) * 1.5f;

        // particleDensity: clamp to [1, 15] and cast to int for layer count
        int layerCount = (int)Hlsl.Clamp(particleDensity, 1f, 15f);
        float3 particles = LayeredParticles(uv, layerCount, smokeIntensity, scaledTime);

        float3 col = particles + smoke + new float3(1f, 0.43f, 0.1f) * 0.02f;
        col *= vignette;

        col = Hlsl.SmoothStep(-0.08f, 1f, col);

        return new float4(col, 1f);
    }
}
