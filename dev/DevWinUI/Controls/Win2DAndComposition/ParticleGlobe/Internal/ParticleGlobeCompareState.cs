namespace DevWinUI;

internal sealed partial class ParticleGlobeCompareState
{
    public ParticleGlobeCountryEntry A = null!;     // always set when a compare starts
    public ParticleGlobeCountryEntry? B;            // null until the second country is picked
    public Vector3 Av, Bv;
    public ParticleGlobeArcState? Path;             // null when the two points are too close / antipodal
    public float DistanceKm, BearingDeg, Start;
}
