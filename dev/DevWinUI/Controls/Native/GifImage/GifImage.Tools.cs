using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;

namespace DevWinUI;

public partial class GifImage
{
    private static async Task<byte[]> GetPixelsAsync(BitmapFrame frame)
    {
        var pixelData = await frame.GetPixelDataAsync(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, new BitmapTransform(), ExifOrientationMode.RespectExifOrientation, ColorManagementMode.DoNotColorManage);
        return pixelData.DetachPixelData();
    }

    private static WriteableBitmap LoadImage(byte[] pixels, int width, int height)
    {
        var bitmap = new WriteableBitmap(width, height);
        var buffer = bitmap.PixelBuffer;
        pixels.CopyTo(buffer);
        bitmap.Invalidate();
        return bitmap;
    }

    private static void MergePixels(byte[] pixels1, int width, byte[] pixels2, Rect rect)
    {
        try
        {
            int x0 = (int)rect.X;
            int y0 = (int)rect.Y;
            int stride = (int)rect.Width;

            for (int y = 0; y < rect.Height; y++)
            {
                for (int x = 0; x < rect.Width; x++)
                {
                    int index1 = (x0 + x + (y0 + y) * width) * 4;
                    int index2 = (x + y * stride) * 4;
                    if (pixels2[index2 + 3] > 0)
                    {
                        pixels1[index1 + 0] = pixels2[index2 + 0];
                        pixels1[index1 + 1] = pixels2[index2 + 1];
                        pixels1[index1 + 2] = pixels2[index2 + 2];
                        pixels1[index1 + 3] = pixels2[index2 + 3];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("MergePixels. {0}", ex.Message);
        }
    }

    private static async Task<FrameProperties> GetFramePropertiesAsync(BitmapFrame frame)
    {
        const string leftProperty = "/imgdesc/Left";
        const string topProperty = "/imgdesc/Top";
        const string widthProperty = "/imgdesc/Width";
        const string heightProperty = "/imgdesc/Height";
        const string delayProperty = "/grctlext/Delay";
        const string disposalProperty = "/grctlext/Disposal";

        var propertiesView = frame.BitmapProperties;
        var requiredProperties = new[] { leftProperty, topProperty, widthProperty, heightProperty };
        var properties = await propertiesView.GetPropertiesAsync(requiredProperties);

        var left = (ushort)properties[leftProperty].Value;
        var top = (ushort)properties[topProperty].Value;
        var width = (ushort)properties[widthProperty].Value;
        var height = (ushort)properties[heightProperty].Value;

        var delayMilliseconds = 30.0;
        var shouldDispose = false;

        try
        {
            var extensionProperties = new[] { delayProperty, disposalProperty };
            properties = await propertiesView.GetPropertiesAsync(extensionProperties);

            if (properties.ContainsKey(delayProperty) && properties[delayProperty].Type == PropertyType.UInt16)
            {
                var delayInHundredths = (ushort)properties[delayProperty].Value;
                if (delayInHundredths >= 3u) // Prevent degenerate frames with no delay time
                {
                    delayMilliseconds = 10.0 * delayInHundredths;
                }
            }

            if (properties.ContainsKey(disposalProperty) && properties[disposalProperty].Type == PropertyType.UInt8)
            {
                var disposal = (byte)properties[disposalProperty].Value;
                if (disposal == 2)
                {
                    // 0 = undefined
                    // 1 = none (compose next frame on top of this one, default)
                    // 2 = dispose
                    // 3 = revert to previous (not supported)
                    shouldDispose = true;
                }
            }
        }
        catch
        {
            // These properties are not required, so it's okay to ignore failure.
        }

        return new FrameProperties(
            new Rect(left, top, width, height),
            delayMilliseconds,
            shouldDispose
            );
    }
}
