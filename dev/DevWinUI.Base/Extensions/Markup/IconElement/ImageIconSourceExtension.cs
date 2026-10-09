using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUI;

[MarkupExtensionReturnType(ReturnType = typeof(ImageIconSource))]
public sealed partial class ImageIconSourceExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the <see cref="Uri"/> representing the image to display.
    /// </summary>
    public Uri? Source { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return new ImageIconSource
        {
            ImageSource = new BitmapImage(Source)
        };
    }
}
