namespace DevWinUIGallery.Views;

public sealed partial class BlendedImagePage : Page
{
    public BlendedImagePage()
    {
        InitializeComponent();

        BlendedImageSample.PrimaryImageSource = new Uri(PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/Image1.jpg")));
        BlendedImageSample.SecondaryImageSource = new Uri(PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/Image2.jpg")));
    }
}
