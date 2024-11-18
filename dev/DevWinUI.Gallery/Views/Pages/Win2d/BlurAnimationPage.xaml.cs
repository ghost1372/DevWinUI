namespace DevWinUIGallery.Views;
public sealed partial class BlurAnimationPage : Page
{
    public BlurAnimationPage()
    {
        this.InitializeComponent();
    }

    private void btnLogin_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        BlurAnimationHelper.StartBlurAnimation(grd, slFrom1.Value, slTo1.Value, TimeSpan.FromSeconds(slDuration1.Value));
    }

    private void btnLogin_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        BlurAnimationHelper.StartBlurAnimation(grd, slTo1.Value, slFrom1.Value, TimeSpan.FromSeconds(slDuration1.Value));
    }

    private void btnRegister_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        BlurAnimationHelper.StartBlurAnimation(grd2, slFrom2.Value, slTo2.Value, TimeSpan.FromSeconds(slDuration2.Value));

    }

    private void btnRegister_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        BlurAnimationHelper.StartBlurAnimation(grd2, slTo2.Value, slFrom2.Value, TimeSpan.FromSeconds(slDuration2.Value));
    }
}
