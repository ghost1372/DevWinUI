namespace DevWinUI;

internal static partial class ColorHelperEx
{
    public static async Task<Color> GetImageEdgeColorWithWin2DAsync(CanvasDevice device, Uri path)
    {
        using var bitmap = await CanvasBitmap.LoadAsync(device, path);
        var colors = bitmap.GetPixelColors();

        uint width = bitmap.SizeInPixels.Width;
        uint height = bitmap.SizeInPixels.Height;

        var edgePixels = new List<Color>();

        for (int x = 0; x < width; x++)
        {
            edgePixels.Add(colors[x]);                    // top
            edgePixels.Add(colors[(height - 1) * width + x]); // bottom
        }
        for (int y = 0; y < height; y++)
        {
            edgePixels.Add(colors[y * width]);            // left
            edgePixels.Add(colors[y * width + (width - 1)]); // right
        }

        byte r = (byte)edgePixels.Average(c => c.R);
        byte g = (byte)edgePixels.Average(c => c.G);
        byte b = (byte)edgePixels.Average(c => c.B);

        return Color.FromArgb(255, r, g, b);
    }
}
