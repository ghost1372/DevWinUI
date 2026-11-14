namespace DevWinUI;

public sealed partial class ParticleGlobeMarkerEventArgs : EventArgs
{
    public ParticleGlobeMarkerEventArgs(ParticleGlobeMarker marker) => Marker = marker;

    public ParticleGlobeMarker Marker { get; }
}
