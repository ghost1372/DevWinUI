using ComputeSharp;
using ComputeSharp.D2D1;

namespace DevWinUI;

/// <summary>
/// Matrix rain (digital rain) pixel shader inspired by the iconic effect from "The Matrix".
/// </summary>
[D2DInputCount(0)]
[D2DShaderProfile(D2D1ShaderProfile.PixelShader50)]
[D2DGeneratedPixelShaderDescriptor]
[D2DRequiresScenePosition]
public readonly partial struct MatrixRainShader(
    float time,
    float2 dispatchSize,
    float speed,
    float density,
    float glyphSize,
    float4 color) : ID2D1PixelShader
{
    // Simple pseudo-random helpers
    private static float Hash11(float p)
    {
        p = Hlsl.Frac(p * 0.1031f);
        p *= p + 33.33f;
        p *= p + p;
        return Hlsl.Frac(p);
    }

    private static float Hash21(float2 p)
    {
        float3 p3 = Hlsl.Frac(new float3(p.X, p.Y, p.X) * 0.1031f);
        p3 += Hlsl.Dot(p3, p3.YZX + 33.33f);
        return Hlsl.Frac((p3.X + p3.Y) * p3.Z);
    }

    // Read a single bitmap pixel — returns 0 for out-of-bounds coords
    private static float SampleBit(int charIndex, int col, int row)
    {
        if (col < 0 || col >= 5 || row < 0 || row >= 5) return 0.0f;
        uint pattern = GetCharPattern(charIndex);
        return ((pattern >> (row * 5 + col)) & 1u) != 0u ? 1.0f : 0.0f;
    }

    // 5x5 bitmap font rendered with bilinear + smoothstep — smooth, anti-aliased output
    private static float DrawChar(int charIndex, float2 uv)
    {
        float2 gridUV = (uv - 0.1f) / 0.8f;
        if (gridUV.X < 0.0f || gridUV.X > 1.0f || gridUV.Y < 0.0f || gridUV.Y > 1.0f)
            return 0.0f;

        // Sub-pixel position within the 5x5 grid (pixel centers at 0.5, 1.5, ...)
        float2 pixelPos = gridUV * 5.0f - 0.5f;
        int2 p0 = (int2)Hlsl.Floor(pixelPos);
        float2 frac = Hlsl.Frac(pixelPos);

        // Fetch 4 surrounding bitmap pixels
        float s00 = SampleBit(charIndex, p0.X,     p0.Y);
        float s10 = SampleBit(charIndex, p0.X + 1, p0.Y);
        float s01 = SampleBit(charIndex, p0.X,     p0.Y + 1);
        float s11 = SampleBit(charIndex, p0.X + 1, p0.Y + 1);

        // Smoothstep gives soft anti-aliased stroke edges instead of hard pixels
        float2 t = Hlsl.SmoothStep(0.2f, 0.8f, frac);
        return Hlsl.Lerp(Hlsl.Lerp(s00, s10, t.X), Hlsl.Lerp(s01, s11, t.X), t.Y);
    }

    // Each pattern encodes a 5x5 character bitmap.
    // Bits 0-4   = row 0 (top),  bit N = col N (left=0, right=4)
    // Bits 5-9   = row 1, bits 10-14 = row 2, bits 15-19 = row 3, bits 20-24 = row 4 (bottom)
    // pattern = r0 | (r1<<5) | (r2<<10) | (r3<<15) | (r4<<20)
    private static uint GetCharPattern(int charIndex)
    {
        // 0: .###. / #...# / #...# / #...# / .###.
        if (charIndex == 0)  return 15255086u;
        // 1: ..#.. / .##.. / ..#.. / ..#.. / .###.
        if (charIndex == 1)  return 14815428u;
        // 2: .###. / #...# / ..##. / .#... / #####
        if (charIndex == 2)  return 32584238u;
        // 3: .###. / ....# / ..##. / ....# / .###.
        if (charIndex == 3)  return 15217166u;
        // 4: #...# / #...# / ##### / ....# / ....#
        if (charIndex == 4)  return 17333809u;
        // 5: ##### / #.... / ####. / ....# / ####.
        if (charIndex == 5)  return 16268351u;
        // 6: .###. / #.... / ####. / #...# / .###.
        if (charIndex == 6)  return 15252526u;
        // 7: ##### / ....# / ...#. / ..#.. / ..#..
        if (charIndex == 7)  return 4334111u;
        // 8: .###. / #...# / .###. / #...# / .###.
        if (charIndex == 8)  return 15252014u;
        // 9: .###. / #...# / .#### / ....# / .###.
        if (charIndex == 9)  return 15235630u;
        // A: .###. / #...# / ##### / #...# / #...#
        if (charIndex == 10) return 18415150u;
        // B: ####. / #...# / ####. / #...# / ####.
        if (charIndex == 11) return 16301615u;
        // C: .###. / #.... / #.... / #.... / .###.
        if (charIndex == 12) return 14713902u;
        // D: ####. / #...# / #...# / #...# / ####.
        if (charIndex == 13) return 16303663u;
        // E: ##### / #.... / ####. / #.... / #####
        if (charIndex == 14) return 32554047u;
        // F: ##### / #.... / ####. / #.... / #....
        if (charIndex == 15) return 1096767u;
        // G: .###. / #.... / #.##. / #...# / .####
        if (charIndex == 16) return 32027694u;
        // H: #...# / #...# / ##### / #...# / #...#
        if (charIndex == 17) return 18415153u;
        // I: .###. / ..#.. / ..#.. / ..#.. / .###.
        if (charIndex == 18) return 14815374u;
        // J: .#### / ...#. / ...#. / #..#. / .##..
        if (charIndex == 19) return 6594846u;
        // K: #...# / #..#. / ###.. / #..#. / #...#
        if (charIndex == 20) return 18128177u;
        // L: #.... / #.... / #.... / #.... / #####
        if (charIndex == 21) return 32539681u;
        // M: #...# / ##.## / #.#.# / #...# / #...#
        if (charIndex == 22) return 18405233u;
        // N: #...# / ##..# / #.#.# / #..## / #...#
        if (charIndex == 23) return 18667121u;
        // O: .###. / #...# / #...# / #...# / .###.
        if (charIndex == 24) return 15255086u;
        // P: ####. / #...# / ####. / #.... / #....
        if (charIndex == 25) return 1097263u;
        // Q: .###. / #...# / #...# / #.##. / .####
        if (charIndex == 26) return 31901230u;
        // R: ####. / #...# / ####. / #.#.. / #..##
        if (charIndex == 27) return 26394159u;
        // S: .###. / #.... / .###. / ....# / .###.
        if (charIndex == 28) return 15218734u;
        // T: ##### / ..#.. / ..#.. / ..#.. / ..#..
        if (charIndex == 29) return 4329631u;
        // U: #...# / #...# / #...# / #...# / .###.
        if (charIndex == 30) return 15255089u;
        // V: #...# / #...# / #...# / .#.#. / ..#..
        if (charIndex == 31) return 4539953u;
        // W: #...# / #...# / #.#.# / ##.## / #...#
        if (charIndex == 32) return 18732593u;
        // X: #...# / .#.#. / ..#.. / .#.#. / #...#
        if (charIndex == 33) return 18157905u;
        // Y: #...# / .#.#. / ..#.. / ..#.. / ..#..
        if (charIndex == 34) return 4329809u;
        // Z: ##### / ....# / ..##. / .#... / #####
        if (charIndex == 35) return 32584223u;

        return 15255086u; // fallback: '0'
    }

    public float4 Execute()
    {
        int2 xy = (int2)D2D.GetScenePosition().XY;
        float2 fragCoord = new(xy.X, xy.Y);

        // Normalize coords into glyph-cell grid
        float cellW = Hlsl.Max(glyphSize, 4.0f);
        float cellH = cellW * 2.0f; // characters are roughly 1:2

        float2 cell = Hlsl.Floor(new float2(fragCoord.X / cellW, fragCoord.Y / cellH));
        float2 cellUV = Hlsl.Frac(new float2(fragCoord.X / cellW, fragCoord.Y / cellH));

        // Per-column random properties
        float colSeed = Hash11(cell.X * 73.1f + 19.7f);
        float streamSpeed = (0.5f + colSeed * 0.8f) * speed;

        // Head position of the rain stream (scrolls downward over time)
        float totalRows = dispatchSize.Y / cellH;
        float headY = Hlsl.Frac(colSeed + time * streamSpeed * 0.15f) * (totalRows + 20.0f) - 10.0f;

        // Distance from head of the stream (positive = above the head = tail trails upward)
        float dist = headY - cell.Y;

        // Stream length varies per column (density controls average length)
        float streamLen = 6.0f + Hash11(cell.X * 47.3f) * 18.0f * density;

        // Brightness: bright head, fading tail
        float brightness = 0.0f;
        if (dist >= 0.0f && dist < streamLen)
        {
            brightness = Hlsl.Pow(Hlsl.Abs(1.0f - dist / streamLen), 2.5f);
            if (dist < 1.0f)
                brightness = 1.0f; // head always full bright
        }

        // Second, dimmer stream offset by half period for layered look
        float colSeed2 = Hash11(cell.X * 131.7f + 5.3f);
        float streamSpeed2 = (0.3f + colSeed2 * 0.5f) * speed;
        float headY2 = Hlsl.Frac(colSeed2 * 0.5f + time * streamSpeed2 * 0.15f) * (totalRows + 20.0f) - 10.0f;
        float dist2 = headY2 - cell.Y;
        float streamLen2 = 4.0f + Hash11(cell.X * 23.9f) * 12.0f * density;

        if (dist2 >= 0.0f && dist2 < streamLen2)
        {
            float b2 = Hlsl.Pow(Hlsl.Abs(1.0f - dist2 / streamLen2), 2.5f) * 0.4f;
            if (dist2 < 1.0f) b2 = 0.4f;
            brightness = Hlsl.Max(brightness, b2);
        }

        if (brightness <= 0.001f)
            return new float4(0, 0, 0, 0);

        // Animate the glyph inside each cell — flicker by changing every few frames
        float glyphSeed = Hash21(cell + Hlsl.Floor(time * 8.0f * Hash11(cell.X + cell.Y * 57.0f)));

        // Draw alphanumeric characters (0-9, A-Z = 36 chars total)
        int charIndex = (int)Hlsl.Floor(glyphSeed * 36.0f);

        float2 uv = cellUV;
        // Slight glow halo around glyph
        float2 uvC = uv - 0.5f;
        float glow = Hlsl.Exp(-Hlsl.Dot(uvC, uvC) * 8.0f);

        // Draw character using simple 5x7 bitmap font
        float glyph = DrawChar(charIndex, uv);

        float alpha = (glyph * 0.85f + glow * 0.15f) * brightness;

        // Head cell is white-ish, rest follows the chosen color
        float3 finalColor;
        if (dist < 1.0f)
            finalColor = Hlsl.Lerp(color.XYZ, new float3(1, 1, 1), 0.85f);
        else
            finalColor = color.XYZ * brightness;

        return new float4(finalColor * alpha, alpha);
    }
}
