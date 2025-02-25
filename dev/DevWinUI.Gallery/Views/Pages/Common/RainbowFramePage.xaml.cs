namespace DevWinUIGallery.Views;

public sealed partial class RainbowFramePage : Page
{
    private RainbowFrame rainbowFrameHelper;
    public RainbowFramePage()
    {
        this.InitializeComponent();
        rainbowFrameHelper = new RainbowFrame();
        rainbowFrameHelper.Initialize(App.MainWindow);
    }

    private void btnFixed_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        rainbowFrameHelper?.StopRainbowFrame();
        rainbowFrameHelper.ChangeFrameColor(Colors.Red);
    }

    private void btnReset_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        rainbowFrameHelper?.ResetFrameColorToDefault();
    }

    private void btnRainbow_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        rainbowFrameHelper?.StopRainbowFrame();
        rainbowFrameHelper?.StartRainbowFrame();
    }

    private void nbEffectSpeed_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        rainbowFrameHelper?.UpdateEffectSpeed((int)args.NewValue);
    }
}
