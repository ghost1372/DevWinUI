using ComputeSharp.D2D1.WinUI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;

namespace DevWinUI;

public partial class MatrixRainRenderer : RendererBase
{
    private PixelShaderEffect<MatrixRainShader>? _effect;
    private float _timeAccumulator = 0f;
    private bool useWin2DFont = true;

    // Cached per-frame resources — recreated only when font/size/canvas dimensions change.
    // Avoids allocating CanvasTextFormat + CanvasTextLayout + ch.ToString() on every Draw call.
    private CanvasTextFormat? _cachedTextFormat;
    private float _cachedFormatFontSize;
    private string? _cachedFormatFontFamily;

    private CanvasTextLayout?[]? _glyphLayouts;
    private float _cachedLayoutCellW;
    private float _cachedLayoutCellH;
    private string[]? _glyphStrings;

    public override void CreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        Dispose();
        _effect = new PixelShaderEffect<MatrixRainShader>();
    }

    private static float Hash11(float p)
    {
        p = Frac(p * 0.1031f);
        p *= p + 33.33f;
        p *= p + p;
        return Frac(p);
    }

    private static float Hash21(Vector2 p)
    {
        // Port of the shader Hash21: p3 = frac((p.X, p.Y, p.X) * 0.1031);
        float3 p3 = new float3(p.X, p.Y, p.X);
        p3.X = Frac(p3.X * 0.1031f);
        p3.Y = Frac(p3.Y * 0.1031f);
        p3.Z = Frac(p3.Z * 0.1031f);

        // scalar = dot(p3, p3.YZX + 33.33f)
        float scalar = p3.X * (p3.Y + 33.33f) + p3.Y * (p3.Z + 33.33f) + p3.Z * (p3.X + 33.33f);

        // add scalar to each component (matches shader p3 += dot(...)) and frac
        p3.X = Frac(p3.X + scalar);
        p3.Y = Frac(p3.Y + scalar);
        p3.Z = Frac(p3.Z + scalar);

        return Frac((p3.X + p3.Y) * p3.Z);
    }

    private static float Frac(float v) => v - MathF.Floor(v);
    private static float Frac(double v) => (float)(v - Math.Floor(v));

    private static float Dot(float3 a, float3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    private struct float3 { public float X, Y, Z; public float3(float x, float y, float z) { X = x; Y = y; Z = z; } }

    public override void Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (!useWin2DFont && _effect == null) return;

        UpdateBreathing(currentBassEnergy, breathingIntensity);

        TimeSpan elapsedTime = args.Timing.ElapsedTime;

        _timeAccumulator += (float)elapsedTime.TotalSeconds;
    }

    private CanvasTextFormat GetOrCreateTextFormat()
    {
        var fontFamily = matrixRainFontFamily ?? "Segoe UI";
        var fontSize = (float)matrixRainFontSize;
        if (_cachedTextFormat == null
            || _cachedFormatFontFamily != fontFamily
            || _cachedFormatFontSize != fontSize)
        {
            _cachedTextFormat?.Dispose();
            InvalidateGlyphLayouts();
            _cachedTextFormat = new CanvasTextFormat
            {
                FontFamily = fontFamily,
                FontSize = fontSize,
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                Options = CanvasDrawTextOptions.EnableColorFont
            };
            _cachedFormatFontFamily = fontFamily;
            _cachedFormatFontSize = fontSize;
        }
        return _cachedTextFormat;
    }

    private void InvalidateGlyphLayouts()
    {
        if (_glyphLayouts != null)
        {
            foreach (var l in _glyphLayouts)
                l?.Dispose();
        }
        _glyphLayouts = null;
        _cachedLayoutCellW = 0f;
        _cachedLayoutCellH = 0f;
    }

    private CanvasTextLayout GetOrCreateGlyphLayout(
        ICanvasResourceCreator device, int charIndex, float cellW, float cellH, CanvasTextFormat fmt)
    {
        int count = matrixRainGlyphs.Length;
        if (_glyphLayouts == null
            || _glyphLayouts.Length != count
            || _cachedLayoutCellW != cellW
            || _cachedLayoutCellH != cellH)
        {
            InvalidateGlyphLayouts();
            _glyphLayouts = new CanvasTextLayout[count];
            _cachedLayoutCellW = cellW;
            _cachedLayoutCellH = cellH;

            // Pre-build single-char strings once to avoid ToString() allocations per glyph per frame.
            _glyphStrings = new string[count];
            for (int i = 0; i < count; i++)
                _glyphStrings[i] = matrixRainGlyphs[i].ToString();
        }

        if (_glyphLayouts[charIndex] == null)
            _glyphLayouts[charIndex] = new CanvasTextLayout(device, _glyphStrings![charIndex], fmt, cellW, cellH);

        return _glyphLayouts[charIndex]!;
    }

    public override void Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        if (!isEnabled) return;

        var ds = args.DrawingSession;
        var currentSpeed = (float)matrixRainSpeed / 100f;
        var currentDensity = (float)matrixRainDensity / 100f;
        if (useWin2DFont)
        {
            // Draw matrix rain using Win2D text rendering for crisp glyphs
            float width = (float)sender.Size.Width; // DIPs
            float height = (float)sender.Size.Height; // DIPs

            float cellW = MathF.Max((float)matrixRainFontSize, 4f);
            float cellH = cellW * 2f;

            int cols = (int)MathF.Ceiling(width / cellW);
            int rows = (int)MathF.Ceiling(height / cellH);

            var textFormat = GetOrCreateTextFormat();

            // Precompute center for breathing transform
            var center = new Vector2(width / 2f, height / 2f);
            ApplyBreathingTransform(ds, center, isBreathingEffectEnabled);

            for (int x = 0; x < cols; x++)
            {
                float colSeed = Hash11(x * 73.1f + 19.7f);
                float streamSpeed = (0.5f + colSeed * 0.8f) * currentSpeed;
                float totalRows = rows;
                float headY = Frac(colSeed + _timeAccumulator * streamSpeed * 0.15f) * (totalRows + 20.0f) - 10.0f;
                int headRowPrimary = (int)MathF.Floor(headY);

                float colSeed2 = Hash11(x * 131.7f + 5.3f);
                float streamSpeed2 = (0.3f + colSeed2 * 0.5f) * currentSpeed;
                float headY2 = Frac(colSeed2 * 0.5f + _timeAccumulator * streamSpeed2 * 0.15f) * (totalRows + 20.0f) - 10.0f;

                for (int y = 0; y < rows; y++)
                {
                    float dist = headY - y;
                    bool isHead = (y == headRowPrimary);
                    float streamLen = 6.0f + Hash11(x * 47.3f) * 18.0f * currentDensity;

                    float brightness = 0.0f;
                    if (dist >= 0.0f && dist < streamLen)
                    {
                        brightness = MathF.Pow(MathF.Abs(1.0f - dist / streamLen), 2.5f);
                        if (dist < 1.0f) brightness = 1.0f;
                    }

                    float dist2 = headY2 - y;
                    float streamLen2 = 4.0f + Hash11(x * 23.9f) * 12.0f * currentDensity;
                    if (dist2 >= 0.0f && dist2 < streamLen2)
                    {
                        float b2 = MathF.Pow(MathF.Abs(1.0f - dist2 / streamLen2), 2.5f) * 0.4f;
                        if (dist2 < 1.0f) b2 = 0.4f;
                        brightness = MathF.Max(brightness, b2);
                    }

                    if (brightness <= 0.001f) continue;

                    int flicker = (int)MathF.Floor(_timeAccumulator * 8.0f * Hash11(x + y * 57.0f));
                    // Use a simple hashed mix for Win2D path to ensure varied glyphs
                    float glyphSeedVal = Hash11(x * 157.0f + y * 313.0f + flicker * 7.0f);
                    glyphSeedVal = Frac(glyphSeedVal);
                    int charIndex = (int)MathF.Floor(glyphSeedVal * 36.0f);
                    char ch = matrixRainGlyphs[Math.Clamp(charIndex, 0, matrixRainGlyphs.Length - 1)];

                    float px = x * cellW;
                    float py = y * cellH;

                    // Color and alpha
                    // Head (dist < 1.0) should be white-ish; other glyphs keep the chosen color tinted by brightness.
                    float3 baseColor = new float3(matrixRainColor.R / 255f, matrixRainColor.G / 255f, matrixRainColor.B / 255f);
                    float3 finalRgb;
                    byte finalA;

                    // Only treat as head when it's the primary stream head row. This ensures exactly one
                    // white-ish glyph per column (primary head), while brightness still includes the second stream.
                    if (isHead)
                    {
                        // mix toward white like the shader: lerp(color, white, 0.85)
                        const float whiteMix = 0.85f;
                        finalRgb = new float3(
                            baseColor.X * (1 - whiteMix) + 1.0f * whiteMix,
                            baseColor.Y * (1 - whiteMix) + 1.0f * whiteMix,
                            baseColor.Z * (1 - whiteMix) + 1.0f * whiteMix);
                        finalA = 255;
                    }
                    else
                    {
                        // non-head: use chosen color scaled by brightness (may be influenced by either stream)
                        finalRgb = new float3(baseColor.X * brightness, baseColor.Y * brightness, baseColor.Z * brightness);
                        float a = (matrixRainColor.A / 255f) * brightness;
                        finalA = (byte)Math.Max(0f, Math.Min(255f, a * 255f));
                    }

                    var col = Windows.UI.Color.FromArgb(finalA, (byte)Math.Max(0f, Math.Min(255f, finalRgb.X * 255f)), (byte)Math.Max(0f, Math.Min(255f, finalRgb.Y * 255f)), (byte)Math.Max(0f, Math.Min(255f, finalRgb.Z * 255f)));

                    var layout = GetOrCreateGlyphLayout(sender, charIndex, cellW, cellH, textFormat);
                    float offsetX = MathF.Max(0, (cellW - (float)layout.LayoutBounds.Width) / 2f);
                    float offsetY = MathF.Max(0, (cellH - (float)layout.LayoutBounds.Height) / 2f);

                    // Draw the glyph with computed color
                    ds.DrawTextLayout(layout, px + offsetX, py + offsetY, col);
                }
            }

            ResetTransform(ds, isBreathingEffectEnabled);
            return;
        }

        // Fallback: original shader-based rendering
        if (_effect == null) return;

        float widthPx = sender.ConvertDipsToPixels((float)sender.Size.Width, CanvasDpiRounding.Round);
        float heightPx = sender.ConvertDipsToPixels((float)sender.Size.Height, CanvasDpiRounding.Round);

        var center2 = new Vector2((float)sender.Size.Width / 2, (float)sender.Size.Height / 2);

        _effect.ConstantBuffer = new MatrixRainShader(
            _timeAccumulator,
            new float2(widthPx, heightPx),
            currentSpeed,
            currentDensity,
            (float)matrixRainGlyphSize,
            new float4(matrixRainColor.R / 255f, matrixRainColor.G / 255f, matrixRainColor.B / 255f, matrixRainColor.A / 255f)
        );

        ApplyBreathingTransform(ds, center2, isBreathingEffectEnabled);
        ds.DrawImage(_effect);
        ResetTransform(ds, isBreathingEffectEnabled);
    }

    public override void Dispose()
    {
        _cachedTextFormat?.Dispose();
        _cachedTextFormat = null;
        InvalidateGlyphLayouts();
        _glyphStrings = null;
        _effect?.Dispose();
        _effect = null;
    }
}
