namespace DevWinUI;

internal sealed partial class ParticleGlobeFlowGrid
{
    public readonly int W, H;
    public readonly float[] U, V;
    public readonly float Max;

    public ParticleGlobeFlowGrid(int w, int h, float[] u, float[] v)
    {
        W = w; H = h; U = u; V = v;
        float m = 0f;
        for (int i = 0; i < w * h; i++) m = MathF.Max(m, MathF.Sqrt(u[i] * u[i] + v[i] * v[i]));
        Max = MathF.Max(m, 1e-3f);
    }
}
