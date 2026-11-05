namespace DevWinUI;

internal static partial class ColorHelperEx
{
    public static async Task<(Color Color, CanvasBitmap CanvasBitmap)> GetImageEdgeColorWithWin2DAsync(CanvasDevice device, Uri path)
    {
        var canvasBitmap = await LoadBitmapAsync(device, path);
        var colors = canvasBitmap.GetPixelColors();

        int width = (int)canvasBitmap.SizeInPixels.Width;
        int height = (int)canvasBitmap.SizeInPixels.Height;

        var (r, g, b) = GetEdgeAverage(colors, width, height);

        return (Color.FromArgb(255, r, g, b), canvasBitmap);
    }

    public static async Task<(Color Color, CanvasBitmap CanvasBitmap)> GetBalancedImageColorAsync(CanvasDevice device, Uri path, float edgeWeight = 0.7f)
    {
        var canvasBitmap = await LoadBitmapAsync(device, path);
        var colors = canvasBitmap.GetPixelColors();

        int width = (int)canvasBitmap.SizeInPixels.Width;
        int height = (int)canvasBitmap.SizeInPixels.Height;

        var (rEdge, gEdge, bEdge) = GetEdgeAverage(colors, width, height);

        int centerX = width / 2;
        int centerY = height / 2;
        int sampleSize = Math.Min(width, height) / 4;

        int startX = Math.Max(0, centerX - sampleSize);
        int endX = Math.Min(width, centerX + sampleSize);
        int startY = Math.Max(0, centerY - sampleSize);
        int endY = Math.Min(height, centerY + sampleSize);

        long count = (long)(endX - startX) * (endY - startY);
        byte rCenter = rEdge;
        byte gCenter = gEdge;
        byte bCenter = bEdge;

        if (count > 0)
        {
            long rSum = 0;
            long gSum = 0;
            long bSum = 0;

            for (int y = startY; y < endY; y++)
            {
                int rowStart = y * width;
                for (int x = startX; x < endX; x++)
                {
                    var color = colors[rowStart + x];
                    rSum += color.R;
                    gSum += color.G;
                    bSum += color.B;
                }
            }

            rCenter = (byte)(rSum / count);
            gCenter = (byte)(gSum / count);
            bCenter = (byte)(bSum / count);
        }

        edgeWeight = Math.Clamp(edgeWeight, 0f, 1f);

        byte rFinal = (byte)(rEdge * edgeWeight + rCenter * (1 - edgeWeight));
        byte gFinal = (byte)(gEdge * edgeWeight + gCenter * (1 - edgeWeight));
        byte bFinal = (byte)(bEdge * edgeWeight + bCenter * (1 - edgeWeight));

        return (Color.FromArgb(255, rFinal, gFinal, bFinal), canvasBitmap);
    }

    private static async Task<CanvasBitmap> LoadBitmapAsync(CanvasDevice device, Uri path)
    {
        if ((RuntimeHelper.IsPackaged() && !path.IsFile) || path.Scheme == Uri.UriSchemeHttp || path.Scheme == Uri.UriSchemeHttps)
        {
            return await CanvasBitmap.LoadAsync(device, path);
        }

        var localFilePath = path.OriginalString;
        if (!File.Exists(localFilePath))
        {
            localFilePath = PathHelper.GetFilePath(path).OriginalString;
        }
        using var fs = File.OpenRead(localFilePath);
        return await CanvasBitmap.LoadAsync(device, fs.AsRandomAccessStream());
    }

    private static (byte R, byte G, byte B) GetEdgeAverage(Color[] colors, int width, int height)
    {
        long rSum = 0;
        long gSum = 0;
        long bSum = 0;

        int lastRowStart = (height - 1) * width;
        for (int x = 0; x < width; x++)
        {
            var top = colors[x];
            var bottom = colors[lastRowStart + x];
            rSum += top.R + bottom.R;
            gSum += top.G + bottom.G;
            bSum += top.B + bottom.B;
        }

        for (int y = 0; y < height; y++)
        {
            int rowStart = y * width;
            var left = colors[rowStart];
            var right = colors[rowStart + width - 1];
            rSum += left.R + right.R;
            gSum += left.G + right.G;
            bSum += left.B + right.B;
        }

        long count = 2L * (width + height);

        return ((byte)(rSum / count), (byte)(gSum / count), (byte)(bSum / count));
    }
}
