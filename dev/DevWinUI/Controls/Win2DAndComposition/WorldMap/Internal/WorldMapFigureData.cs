namespace DevWinUI;

internal sealed partial class WorldMapFigureData
{
    public WorldMapFigureData(byte[] ops, float[] points)
    {
        Ops = ops;
        Points = points;
    }

    public byte[] Ops { get; }
    public float[] Points { get; }
}
