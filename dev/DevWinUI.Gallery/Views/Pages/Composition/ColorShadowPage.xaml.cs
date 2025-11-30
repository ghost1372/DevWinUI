namespace DevWinUIGallery.Views;

public sealed partial class ColorShadowPage : Page
{
    public ColorShadowPage()
    {
        InitializeComponent();

        ColorShadowCtrl.ImageUri = PathHelper.GetFilePath(new Uri("ms-appx:///Assets/Others/RainbowRose.png"));
    }
}
