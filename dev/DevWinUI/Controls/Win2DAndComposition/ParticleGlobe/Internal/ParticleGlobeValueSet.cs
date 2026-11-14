namespace DevWinUI;

internal sealed partial class ParticleGlobeValueSet
{
    public float[] T;
    public Vector3 Low, High;
    public int[] BarIds = Array.Empty<int>();
    public Vector3[] BarDir = Array.Empty<Vector3>();
    public Vector3[] BarE1 = Array.Empty<Vector3>();
    public Vector3[] BarE2 = Array.Empty<Vector3>();
}
