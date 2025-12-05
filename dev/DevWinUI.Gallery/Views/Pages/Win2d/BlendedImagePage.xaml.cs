namespace DevWinUIGallery.Views;

public sealed partial class BlendedImagePage : Page
{
    public BlendedImagePage()
    {
        InitializeComponent();

        BlendedImageSample.PrimaryImageSource = PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/Image1.jpg"));
        BlendedImageSample.SecondaryImageSource = PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/Image2.jpg"));
    }
}
