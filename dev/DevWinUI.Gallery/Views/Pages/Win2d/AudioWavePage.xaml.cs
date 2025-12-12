namespace DevWinUIGallery.Views;

public sealed partial class AudioWavePage : Page
{
    public AudioWavePage()
    {
        InitializeComponent();

        AudioWaveSample.WaveDataProvider = OnFakeWaveData2;
    }

    private void BtnFake2_Click(object sender, RoutedEventArgs e)
    {
        AudioWaveSample.WaveDataProvider = OnFakeWaveData;
        AudioWaveSample.UpdateWaveData();
    }

    private void BtnFake_Click(object sender, RoutedEventArgs e)
    {
        AudioWaveSample.WaveDataProvider = OnFakeWaveData2;
        AudioWaveSample.UpdateWaveData();
    }

    private float[] OnFakeWaveData()
    {
        return AudioWaveSample.GenerateSampleWaveData(200);
    }
    private float[] OnFakeWaveData2()
    {
        return AudioWaveSample.GenerateSampleWaveData2(200);
    }

    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        SldProgress.Value = 0;
        AudioWaveSample.Start();
    }

    private void BtnPause_Click(object sender, RoutedEventArgs e)
    {
        AudioWaveSample.Pause();
    }

    private void BtnStop_Click(object sender, RoutedEventArgs e)
    {
        SldProgress.Value = 0;
        AudioWaveSample.Stop();
    }
}
