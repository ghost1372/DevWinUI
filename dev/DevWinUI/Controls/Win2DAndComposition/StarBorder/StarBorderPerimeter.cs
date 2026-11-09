namespace DevWinUI;

internal sealed partial class StarBorderPerimeter
{
    private readonly float W, H, R;
    private readonly float[] _len = new float[8];
    public readonly float Total;

    public StarBorderPerimeter(float w, float h, float r)
    {
        W = w; H = h; R = Math.Clamp(r, 0, Math.Min(w, h) / 2);
        float a = W - 2 * R, b = H - 2 * R, arc = MathF.PI * R / 2f;
        _len[0] = a; _len[1] = arc; _len[2] = b; _len[3] = arc;
        _len[4] = a; _len[5] = arc; _len[6] = b; _len[7] = arc;
        foreach (var l in _len) Total += l;
    }

    public Vector2 PointAt(float d)
    {
        if (Total <= 0) return Vector2.Zero;
        d %= Total; if (d < 0) d += Total;
        for (int i = 0; i < 8; i++)
        {
            if (d <= _len[i] || i == 7) return Seg(i, Math.Min(d, _len[i]));
            d -= _len[i];
        }
        return Vector2.Zero;
    }

    private Vector2 Seg(int i, float u) => i switch
    {
        0 => new(R + u, 0),
        1 => ArcPt(W - R, R, -MathF.PI / 2, u),
        2 => new(W, R + u),
        3 => ArcPt(W - R, H - R, 0, u),
        4 => new(W - R - u, H),
        5 => ArcPt(R, H - R, MathF.PI / 2, u),
        6 => new(0, H - R - u),
        _ => ArcPt(R, R, MathF.PI, u),
    };

    private Vector2 ArcPt(float cx, float cy, float a0, float u)
    {
        float a = a0 + (R > 0 ? u / R : 0);
        return new(cx + R * MathF.Cos(a), cy + R * MathF.Sin(a));
    }

    public float Nearest(Vector2 p)
    {
        float best = 0, bd = float.MaxValue;
        const int n = 720;
        for (int i = 0; i < n; i++)
        {
            float d = Total * i / n;
            float dd = Vector2.DistanceSquared(p, PointAt(d));
            if (dd < bd) { bd = dd; best = d; }
        }
        return best;
    }
}
