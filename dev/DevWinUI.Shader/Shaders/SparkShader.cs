using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

//https://www.shadertoy.com/view/wl2Gzc

[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct SparkShader(
    float time,
    float2 dispatchSize,
    float speed,            // Overall animation speed multiplier (default: 1.0)
    float driftSpeed,       // Speed at which particles drift upward independently of base speed (default: 1.0)
    float particleScale,    // Size of individual spark particles — larger = bigger sparks (default: 1.0)
    float particleDensity,  // Number of particle layers rendered (default: 10, range: 1–10)
    float smokeSpeed,       // Speed of smoke noise scrolling independently of base speed (default: 1.0)
    float smokeIntensityVal,// Overall smoke density/brightness multiplier (default: 1.0)
    float trailStrength,    // Strength of the noise-based wobble that gives sparks their trail (default: 1.0)
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

    private static float2 VoronoiPoint(float2 root, float deg)
    {
        float2 p = Hash2_2(root) - 0.5f;

        float s = Hlsl.Sin(deg);
        float c = Hlsl.Cos(deg);

        p = Hlsl.Mul(p, new float2x2(s, c, -c, s)) * 0.66f;

        return p + root + 0.5f;
    }

    private static float DegFromUV(float2 uv, float scaledTime)
    {
        return scaledTime * 0.6f * (Hash1_2(uv) - 0.5f) * 2f;
    }

    private static float LayeredNoise1_2(float2 uv, float sizeMod, float alphaMod, int layers, float scaledTime)
    {
        float noise = 0.3f;
        float alpha = 1f;
        float size = 1f;
        float2 offset = 0f;

        int maxLayers = 6; // safety for ComputeSharp

        for (int i = 0; i < maxLayers; i++)
        {
            if (i >= layers) break;

            offset += Hash2_2(new float2(alpha, size)) * 10f;

            noise += Noise1_2(
                uv * size +
                scaledTime * 8f * (-1f * new float2(0f, 1f)) * 0.5f +
                offset) * alpha;

            alpha *= alphaMod;
            size *= sizeMod;
        }

        return noise;
    }

    // trailStrength: scales the noise-based UV wobble that gives sparks a trailing tail
    // particleThreshold: derived from particleScale — controls SmoothStep distances
    private static float3 FireParticles(float2 uv, float2 originalUV, float scaledTime, float trailStr, float particleThreshold)
    {
        float3 particles = 0f;

        float2 rootUV = Hlsl.Floor(uv);
        float deg = DegFromUV(rootUV, scaledTime);

        float2 pointUV = VoronoiPoint(rootUV, deg);

        float2 tempUV = uv + (Noise2_2(uv * 2f) - 0.5f) * 0.1f * trailStr;
        tempUV += -(Noise2_2(uv * 3f + scaledTime) - 0.5f) * 0.07f * trailStr;

        float dist = Hlsl.Length(Rotate(tempUV - pointUV, 0.7f) * new float2(0.5f, 1.6f));
        float distBloom = Hlsl.Length(Rotate(tempUV - pointUV, 0.7f) * new float2(0.5f, 0.8f));

        float3 spark = new float3(1f, 0.4f, 0.05f) * 1.5f;
        float3 bloom = new float3(1f, 0.4f, 0.05f) * 0.8f;

        // particleThreshold scales the distance thresholds — smaller = larger sparks
        particles += (1f - Hlsl.SmoothStep(particleThreshold * 0.6f, particleThreshold * 3f, dist)) * spark;
        particles += Hlsl.Pow((1f - Hlsl.SmoothStep(0f, particleThreshold * 6f, distBloom)), 3f) * bloom;

        return particles;
    }

    // driftSpeed: scales the upward drift velocity of the particle bokeh UV
    private static float3 LayeredParticles(float2 uv, int layers, float smoke, float scaledTime, float driftSpd, float trailStr, float particleThreshold)
    {
        float3 particles = 0f;

        float size = 1f;
        float alpha = 1f;
        float2 offset = 0f;

        int maxLayers = 10;

        for (int i = 0; i < maxLayers; i++)
        {
            if (i >= layers) break;

            float2 noiseOffset = (Noise2_2(uv * size * 2f + 0.5f) - 0.5f) * 0.15f;

            float2 bokehUV =
                uv * size +
                scaledTime * new float2(0f, -1f) * 0.5f * driftSpd +
                offset +
                noiseOffset;

            particles += FireParticles(bokehUV, uv, scaledTime, trailStr, particleThreshold) * alpha;

            offset += Hash2_2(new float2(alpha, alpha)) * 10f;

            alpha *= 0.9f;
            size *= 1.01f;
        }

        return particles;
    }

    public float4 Execute()
    {
        float2 fragCoord = D2D.GetScenePosition().XY;

        float2 uv = (2f * fragCoord - dispatchSize) / dispatchSize.X;

        // vignetteStrength: lerp between no vignette (0) and full vignette (1)
        float rawVignette = 1f - Hlsl.SmoothStep(0.4f, 1.4f, Hlsl.Length(uv + new float2(0f, 0.3f)));
        float vignette = Hlsl.Lerp(1f, rawVignette, vignetteStrength);

        uv *= 1.8f;

        // scaledTime drives all animation; smokeSpeed further scales smoke noise sampling
        float scaledTime = time * speed;
        float scaledSmokeTime = time * speed * smokeSpeed;

        float smokeNoise =
            LayeredNoise1_2(uv * 10f + scaledSmokeTime * 2f * new float2(0f, -1f), 1.7f, 0.7f, 6, scaledSmokeTime);

        smokeNoise *= Hlsl.Pow(1f - Hlsl.SmoothStep(-1f, 1.6f, uv.Y), 2f);

        // smokeIntensityVal scales the smoke color brightness
        float3 smoke = smokeNoise * new float3(1f, 0.43f, 0.1f) * 0.8f * smokeIntensityVal;

        smoke *= Hlsl.Pow(
            LayeredNoise1_2(uv * 4f + scaledSmokeTime * 0.5f, 1.8f, 0.5f, 3, scaledSmokeTime),
            2f) * 1.5f;

        // particleScale > 1 = larger sparks; < 1 = smaller/sharper sparks
        // internally inverted: smaller threshold = larger rendered particle
        float particleThreshold = 0.002f / particleScale;

        int layerCount = (int)Hlsl.Clamp(particleDensity, 1f, 10f);
        float3 particles = LayeredParticles(uv, layerCount, smokeNoise, scaledTime, driftSpeed, trailStrength, particleThreshold);

        float3 col = particles + smoke + new float3(1f, 0.43f, 0.1f) * 0.02f;
        col *= vignette;

        col = Hlsl.SmoothStep(-0.08f, 1f, col);

        return new float4(col, 1f);
    }
}
