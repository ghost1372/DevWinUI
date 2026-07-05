namespace DevWinUIGallery.Views;

public sealed partial class SpeedGraphPage : Page
{
    public ObservableCollection<SpeedGraphBackgroundMode> SpeedGraphBackgroundModeItems { get; set; } = new ObservableCollection<SpeedGraphBackgroundMode>(Enum.GetValues<SpeedGraphBackgroundMode>());

    private bool _isSimulating;
    private bool _isSimulating2;
    private Random _random = new();
    private Random _random2 = new();
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

    private async void BtnCopy_Click2(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample2.Points.Clear();
        ulong _totalBytes = 1024UL * 1024 * (ulong)NBFileSize2.Value;

        SpeedGraphSample2.SetGraphNormal();

        if (_isSimulating2)
            return;

        _isSimulating2 = true;

        ulong copiedBytes = 0;

        while (copiedBytes < _totalBytes)
        {
            // Simulate varying copy speed (8–100 MB/s)
            ulong speed = (ulong)(_random2.Next(8, 100) * 1024 * 1024);

            copiedBytes += speed / 3;
            if (copiedBytes > _totalBytes)
                copiedBytes = _totalBytes;

            double percent = (double)copiedBytes / _totalBytes * 100.0;

            SpeedGraphSample2.Points.Add(new System.Numerics.Vector2((float)percent, speed));
            await Task.Delay(100);
        }

        _isSimulating2 = false;
    }

    private void BtnError_Click2(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample2.SetGraphError();
    }

    private void BtnPause_Click2(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample2.SetGraphWarning();
    }

    private void BtnNormal_Click2(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample2.SetGraphNormal();
    }
    private void BtnSuccess_Click2(object sender, RoutedEventArgs e)
    {
        SpeedGraphSample2.SetGraphSuccess();
    }
}
