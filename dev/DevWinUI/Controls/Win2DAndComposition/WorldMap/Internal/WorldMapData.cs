using System.Text;

namespace DevWinUI;

internal static partial class WorldMapData
{
    private const int Magic = 0x50414D57;

    public const string DefaultPath = "ms-appx:///Assets/WorldMap/WorldMap.bin";

    public static async Task<WorldMapDataSet> LoadAsync(Uri source)
    {
        var uri = source ?? new Uri(DefaultPath);
        if (source != null) uri = PathHelper.GetFilePath(source);

        var assembly = typeof(WorldMapData).Assembly;
        using var stream = FileHelper.GetFileFromEmbededResourcesOrUri(uri, assembly)
            ?? throw new FileNotFoundException("World map data could not be found.", uri.ToString());

        var bytes = new byte[stream.Length];
        await stream.ReadExactlyAsync(bytes);

        return await Task.Run(() => Read(bytes));
    }

    private static WorldMapDataSet Read(byte[] bytes)
    {
        using var reader = new BinaryReader(new MemoryStream(bytes), Encoding.UTF8);

        if (reader.ReadInt32() != Magic)
        {
            throw new InvalidDataException("Invalid world map data file.");
        }

        reader.ReadInt32();
        float width = reader.ReadSingle();
        float height = reader.ReadSingle();
        int countryCount = reader.ReadInt32();

        var countries = new List<WorldMapCountry>(countryCount);
        var points = new List<float>(4096);

        for (int i = 0; i < countryCount; i++)
        {
            string code = reader.ReadString();
            string name = reader.ReadString();
            var main = new Rect(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

            int shapeCount = reader.ReadInt32();
            var shapes = new List<WorldMapShapeData>(shapeCount);

            for (int s = 0; s < shapeCount; s++)
            {
                var shape = new WorldMapShapeData();
                int figureCount = reader.ReadInt32();

                for (int f = 0; f < figureCount; f++)
                {
                    int commandCount = reader.ReadInt32();
                    var ops = new byte[commandCount];
                    points.Clear();

                    for (int c = 0; c < commandCount; c++)
                    {
                        byte op = reader.ReadByte();
                        ops[c] = op;

                        int pairs = op switch { 0 => 1, 1 => 1, 2 => 3, _ => 0 };
                        for (int p = 0; p < pairs * 2; p++)
                        {
                            points.Add(reader.ReadUInt16() / 10f);
                        }
                    }

                    shape.Figures.Add(new WorldMapFigureData(ops, points.ToArray()));
                }

                shapes.Add(shape);
            }

            countries.Add(new WorldMapCountry(code, name, main, shapes));
        }

        return new WorldMapDataSet(new Size(width, height), countries);
    }
}
