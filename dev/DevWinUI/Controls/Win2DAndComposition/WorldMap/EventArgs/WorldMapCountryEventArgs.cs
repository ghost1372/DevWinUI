namespace DevWinUI;

public sealed partial class WorldMapCountryEventArgs : EventArgs
{
    internal WorldMapCountryEventArgs(WorldMapCountry? country)
    {
        Country = country;
    }

    public WorldMapCountry? Country { get; }
}
