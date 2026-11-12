namespace DevWinUI;

internal sealed partial class WorldMapDataSet
{
    public WorldMapDataSet(Size mapSize, List<WorldMapCountry> countries)
    {
        MapSize = mapSize;
        Countries = countries;
    }

    public Size MapSize { get; }
    public List<WorldMapCountry> Countries { get; }
}
