namespace DevWinUIGallery.Views;

public sealed partial class OpacityMaskViewPage : Page
{
    public OpacityMaskViewPage()
    {
        this.InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (EffectButtonAnimation.GetCurrentState() != Microsoft.UI.Xaml.Media.Animation.ClockState.Active)
        {
            EffectButtonAnimation.Begin();
        }
    }
}
