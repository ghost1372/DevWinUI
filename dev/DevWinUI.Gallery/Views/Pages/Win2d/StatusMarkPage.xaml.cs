namespace DevWinUIGallery.Views;

public sealed partial class StatusMarkPage : Page
{
    public StatusMarkPage()
    {
        InitializeComponent();
    }

    public double GetProgress(bool isDeterminate, double value) => isDeterminate ? value : double.NaN;

    private async void OnSimulateClick(object sender, RoutedEventArgs e)
    {
        TgDeterminate.IsOn = true;
        SliderProgress.Value = 0;
        CmbStatus.Value = StatusMarkStatus.Running;

        for (int i = 0; i <= 100; i += 4)
        {
            SliderProgress.Value = i;
            await Task.Delay(60);
        }

        CmbStatus.Value = StatusMarkStatus.Done;
    }
}
