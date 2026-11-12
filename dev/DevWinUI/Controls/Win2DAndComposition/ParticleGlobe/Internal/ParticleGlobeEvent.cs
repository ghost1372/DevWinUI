namespace DevWinUI;

internal sealed partial class ParticleGlobeEvent
{
    public Vector3 Center;
    public float Magnitude, Age, Life;
    public Vector4? Color;
    public string Label;
    public float Mag01 => Math.Clamp(Magnitude / 9f, 0f, 1f);
}
