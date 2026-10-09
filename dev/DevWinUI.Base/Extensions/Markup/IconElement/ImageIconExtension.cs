using Microsoft.UI.Xaml.Media.Imaging;

namespace DevWinUI;

[MarkupExtensionReturnType(ReturnType = typeof(ImageIcon))]
public sealed partial class ImageIconExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the <see cref="Uri"/> representing the image to display.
    /// </summary>
    public Uri? Source { get; set; }

    /// <inheritdoc/>
    protected override object ProvideValue()
    {
        return new ImageIcon()
        {
            Source = new BitmapImage(Source)
        };
    }
}
