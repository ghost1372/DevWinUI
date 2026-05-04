using Microsoft.UI.Xaml.Media;

namespace DevWinUIGallery.Views;

public sealed partial class EyeDropperPage : Page
{
    public EyeDropperPage()
    {
        InitializeComponent();
    }

    private void BtnPick_Click(object sender, RoutedEventArgs e)
    {
        var eyeDropper = new EyeDropper();
        eyeDropper.Start();
        eyeDropper.ColorPicked += EyeDropper_ColorPicked;
    }

    private void EyeDropper_ColorPicked(object sender, Windows.UI.Color e)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            PreviewBox.Background = new SolidColorBrush(e);
            TxtHex.Text = $"Color picked: {DevWinUI.ColorHelper.GetHexFromColor(e)}";
        });
    }
}
