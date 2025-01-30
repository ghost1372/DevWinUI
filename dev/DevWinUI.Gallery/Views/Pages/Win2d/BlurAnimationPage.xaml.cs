namespace DevWinUIGallery.Views;
public sealed partial class BlurAnimationPage : Page
{
    public BlurAnimationPage()
    {
        this.InitializeComponent();
        BlurAnimationHelper.ApplyBlurEffect(grd, 0);
        BlurAnimationHelper.ApplyBlurEffect(grd2, 0);
    }

    private void btnLogin_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        BlurAnimationHelper.StartBlurAnimation(grd, (float)slFrom1.Value, (float)slTo1.Value, TimeSpan.FromSeconds(slDuration1.Value));
    }

    private void btnLogin_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        BlurAnimationHelper.StartBlurAnimation(grd, (float)slTo1.Value, (float)slFrom1.Value, TimeSpan.FromSeconds(slDuration1.Value));
    }

    private void btnRegister_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        BlurAnimationHelper.StartBlurAnimation(grd2, (float)slFrom2.Value, (float)slTo2.Value, TimeSpan.FromSeconds(slDuration2.Value));

    }

    private void btnRegister_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        BlurAnimationHelper.StartBlurAnimation(grd2, (float)slTo2.Value, (float)slFrom2.Value, TimeSpan.FromSeconds(slDuration2.Value));
    }
}
