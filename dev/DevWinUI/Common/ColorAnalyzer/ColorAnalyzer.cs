using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUI;


[ContentProperty(Name = nameof(Analyzers))]
public partial class ColorAnalyzer : DependencyObject
{
    public ColorAnalyzer()
    {
        Analyzers = [];
    }

    public void UpdateAnalyzer()
    {
        _ = UpdateAnalyzerAsync();
    }

    public async Task UpdateAnalyzerAsync()
    {
        // No Analyzer to update.
        // Skip a lot of unnecessary computation
        if (Analyzers.Count is 0)
            return;

        const int sampleCount = 4096;
        const int k = 8;

        // Retreive pixel samples from source
        var samples = await SampleSourcePixelColorsAsync(sampleCount);

        // Failed to retreive pixel data. Cancel
        if (samples.Length == 0)
            return;

        // Cluster samples in RGB floating-point color space
        // With Euclidean Squared distance function, then construct analyzer data.
        var clusters = KMeansCluster(samples, k, out var sizes);
        var colorData = clusters.Select((vectorColor, i) => new AnalyzedColor(vectorColor.ToColor(), (float)sizes[i] / samples.Length));

        // Update analyzers on the UI thread
        foreach (var analyzer in Analyzers)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                analyzer.SelectColors(colorData);
            });
        }

        // Update analyzer property
        // Not a dependency property, so no need to update from the UI Thread
        AnalyzedColors = [.. colorData];


        // Invoke analyzer updated event from the UI thread
        DispatcherQueue.TryEnqueue(() =>
        {
            AnalyzerUpdated?.Invoke(this, EventArgs.Empty);
        });
    }

    private async Task<Vector3[]> SampleSourcePixelColorsAsync(int sampleCount)
    {
        // Ensure the source is populated
        if (Source is null)
            return [];

        // Grab actual size
        // If actualSize is 0, replace with 1:1 aspect ratio
        var sourceSize = Source.ActualSize;
        sourceSize = sourceSize != Vector2.Zero ? sourceSize : Vector2.One;
        
        // Calculate size of scaled rerender using the actual size
        // scaled down to the sample count, maintaining aspect ration
        var sourceArea = sourceSize.X * sourceSize.Y;
        var sampleScale = MathF.Sqrt(sampleCount / sourceArea);
        var sampleSize = sourceSize * sampleScale;

        // Rerender the UIElement to a bitmap of about sampleCount pixels
        // Note: RenderTargetBitmap is not supported with Uno Platform.
        var bitmap = new RenderTargetBitmap();
        await bitmap.RenderAsync(Source, (int)sampleSize.X, (int)sampleSize.Y);

        // Create a stream from the bitmap
        var pixels = await bitmap.GetPixelsAsync();
        var pixelByteStream = pixels.AsStream();

        // Something went wrong
        if (pixelByteStream.Length == 0)
            return [];

        // Read the stream into a a color array
        const int bytesPerPixel = 4;
        var samples = new Vector3[(int)pixelByteStream.Length / bytesPerPixel];

        // Iterate through the stream reading a pixel (4 bytes) at a time
        // and storing them as a Vector3. Opacity info is dropped.
        int colorIndex = 0;
        Span<byte> pixelBytes = stackalloc byte[bytesPerPixel];
        while (pixelByteStream.Read(pixelBytes) == bytesPerPixel)
        {
            // Skip fully transparent pixels
            if (pixelBytes[3] == 0)
                continue;

            // Take the red, green, and blue channels to make a floating-point space color.
            samples[colorIndex] = new Vector3(pixelBytes[2], pixelBytes[1], pixelBytes[0]) / byte.MaxValue;
            colorIndex++;
        }

        // If we skipped any pixels, trim the span
        samples = samples[..colorIndex];

        return samples;
    }
}
