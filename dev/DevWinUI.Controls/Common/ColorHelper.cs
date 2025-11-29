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

    public static async Task<Color> GetBalancedImageColorAsync(CanvasDevice device, Uri path, float edgeWeight = 0.7f)
    {
        using var bitmap = await CanvasBitmap.LoadAsync(device, path);
        var colors = bitmap.GetPixelColors();

        uint width = bitmap.SizeInPixels.Width;
        uint height = bitmap.SizeInPixels.Height;

        // --- 1. Edge Color ---
        var edgePixels = new List<Color>();
        for (int x = 0; x < width; x++)
        {
            edgePixels.Add(colors[x]); // top
            edgePixels.Add(colors[(height - 1) * width + x]); // bottom
        }
        for (int y = 0; y < height; y++)
        {
            edgePixels.Add(colors[y * width]); // left
            edgePixels.Add(colors[y * width + (width - 1)]); // right
        }

        byte rEdge = (byte)edgePixels.Average(c => c.R);
        byte gEdge = (byte)edgePixels.Average(c => c.G);
        byte bEdge = (byte)edgePixels.Average(c => c.B);
        var edgeColor = Color.FromArgb(255, rEdge, gEdge, bEdge);

        // --- 2. Dominant Color (central area) ---
        int centerX = (int)width / 2;
        int centerY = (int)height / 2;
        int sampleSize = Math.Min((int)width, (int)height) / 4; // sample 1/4 size square in center

        var centerPixels = new List<Color>();
        for (int y = centerY - sampleSize; y < centerY + sampleSize; y++)
        {
            for (int x = centerX - sampleSize; x < centerX + sampleSize; x++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                    centerPixels.Add(colors[y * width + x]);
            }
        }

        byte rCenter = (byte)centerPixels.Average(c => c.R);
        byte gCenter = (byte)centerPixels.Average(c => c.G);
        byte bCenter = (byte)centerPixels.Average(c => c.B);
        var centerColor = Color.FromArgb(255, rCenter, gCenter, bCenter);

        // --- 3. Blend Edge + Center ---
        byte rFinal = (byte)(rEdge * edgeWeight + rCenter * (1 - edgeWeight));
        byte gFinal = (byte)(gEdge * edgeWeight + gCenter * (1 - edgeWeight));
        byte bFinal = (byte)(bEdge * edgeWeight + bCenter * (1 - edgeWeight));

        return Color.FromArgb(255, rFinal, gFinal, bFinal);
    }
}
