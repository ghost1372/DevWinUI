namespace DevWinUIGallery.Views;

public sealed partial class StatusMarkPage : Page
{
    public ObservableCollection<StatusMarkStatus> StatusItems { get; set; } = new ObservableCollection<StatusMarkStatus>(Enum.GetValues<StatusMarkStatus>());

    public StatusMarkPage()
    {
        InitializeComponent();
    }

    public double GetProgress(bool isDeterminate, double value) => isDeterminate ? value : double.NaN;

    private async void OnSimulateClick(object sender, RoutedEventArgs e)
    {
        TgDeterminate.IsOn = true;
        SliderProgress.Value = 0;
        CmbStatus.SelectedItem = StatusMarkStatus.Running;

        for (int i = 0; i <= 100; i += 4)
        {
            SliderProgress.Value = i;
            await Task.Delay(60);
        }

        CmbStatus.SelectedItem = StatusMarkStatus.Done;
    }
}
