namespace DevWinUIGallery.Views;

public sealed partial class SpeedGraphPage : Page
{
    public ObservableCollection<SpeedGraphBackgroundMode> SpeedGraphBackgroundModeItems { get; set; } = new ObservableCollection<SpeedGraphBackgroundMode>(Enum.GetValues<SpeedGraphBackgroundMode>());

    private bool _isSimulating;
    private Random _random = new();
    public SpeedGraphPage()
    {
        InitializeComponent();
    }

    private async void BtnCopy_Click(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample.ResetGraph();

        ulong _totalBytes = 1024UL * 1024 * (ulong)NBFileSize.Value;

        SpeedGraphSample.NormalGraph();

        if (_isSimulating)
            return;

        _isSimulating = true;

        ulong copiedBytes = 0;

        while (copiedBytes < _totalBytes)
        {
            // Simulate varying copy speed (8–100 MB/s)
            ulong speed = (ulong)(_random.Next(8, 100) * 1024 * 1024);

            // Add progress (simulate ~1/3 second per chunk)
            copiedBytes += speed / 3;
            if (copiedBytes > _totalBytes)
                copiedBytes = _totalBytes;

            double percent = (double)copiedBytes / _totalBytes * 100.0;

            // Update SpeedGraph
            SpeedGraphSample.SetSpeed(percent, speed);
            // Update text

            // Wait before next tick
            await Task.Delay(100);
        }

        _isSimulating = false;
    }

    private void BtnError_Click(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample.ErrorGraph();
    }

    private void BtnPause_Click(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample.PauseGraph();
    }

    private void BtnNormal_Click(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample.NormalGraph();
    }
}
