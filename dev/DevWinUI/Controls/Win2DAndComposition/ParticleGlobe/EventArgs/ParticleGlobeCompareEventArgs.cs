namespace DevWinUI;

public sealed partial class ParticleGlobeCompareEventArgs : EventArgs
{
    public ParticleGlobeCompareEventArgs(string codeA, string nameA, string codeB, string nameB, double distanceKm, TimeSpan flightTime)
    {
        CodeA = codeA;
        NameA = nameA;
        CodeB = codeB;
        NameB = nameB;
        DistanceKm = distanceKm;
        FlightTime = flightTime;
    }

    public string CodeA { get; }
    public string NameA { get; }
    public string CodeB { get; }
    public string NameB { get; }

    public double DistanceKm { get; }

    public TimeSpan FlightTime { get; }
}
