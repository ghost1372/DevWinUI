namespace DevWinUI;

internal sealed record MotionWallItemInfo(MotionWallItemKind Kind, string? Key, Uri? Uri, string? Text, string? Title, object? Original);

internal readonly record struct MotionWallTileRef(int Lane, int K);

internal readonly record struct MotionWallLiftedTile(MotionWallCard Card, float X, float Y, float Alpha, float Light, float Overlay, float Lift);

internal readonly record struct MotionWallMetrics(
    bool Horizontal,
    int TotalLanes,
    int ItemCount,
    int LoopCount,
    int Stride,
    float TileWidth,
    float TileHeight,
    float Gap,
    float StageWidth,
    float StageHeight,
    float Travel)
{
    public float TileMain => Horizontal ? TileWidth : TileHeight;

    public float TileCross => Horizontal ? TileHeight : TileWidth;

    public float CellMain => TileMain + Gap;

    public float CellCross => TileCross + Gap;

    public float Loop => CellMain * LoopCount;
}

internal readonly struct MotionWallHomography
{
    private readonly float _a, _b, _c, _d, _e, _f, _g, _h, _i;

    private MotionWallHomography(float a, float b, float c, float d, float e, float f, float g, float h, float i, bool valid)
    {
        _a = a;
        _b = b;
        _c = c;
        _d = d;
        _e = e;
        _f = f;
        _g = g;
        _h = h;
        _i = i;
        IsValid = valid;
    }

    public bool IsValid { get; }

    public static MotionWallHomography Invert(in Matrix4x4 t)
    {
        float a = t.M11, b = t.M12, c = t.M14;
        float d = t.M21, e = t.M22, f = t.M24;
        float g = t.M41, h = t.M42, i = t.M44;

        float det = a * (e * i - f * h) - b * (d * i - f * g) + c * (d * h - e * g);

        if (MathF.Abs(det) < 1e-12f)
        {
            return new MotionWallHomography(0, 0, 0, 0, 0, 0, 0, 0, 0, false);
        }

        float k = 1f / det;

        return new MotionWallHomography(
            (e * i - f * h) * k, (c * h - b * i) * k, (b * f - c * e) * k,
            (f * g - d * i) * k, (a * i - c * g) * k, (c * d - a * f) * k,
            (d * h - e * g) * k, (b * g - a * h) * k, (a * e - b * d) * k,
            true);
    }

    public float W(float sx, float sy)
    {
        return sx * _c + sy * _f + _i;
    }

    public bool TryApply(float sx, float sy, out float x, out float y)
    {
        x = 0f;
        y = 0f;

        if (!IsValid)
        {
            return false;
        }

        float w = W(sx, sy);

        if (MathF.Abs(w) < 1e-6f)
        {
            return false;
        }

        x = (sx * _a + sy * _d + _g) / w;
        y = (sx * _b + sy * _e + _h) / w;
        return true;
    }
}
