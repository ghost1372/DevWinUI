using Microsoft.Windows.Storage.Pickers;
using Windows.Storage;

namespace DevWinUIGallery.Views;

public sealed partial class WaveformTimelinePage : Page
{
    private AudioGraphEngine soundEngine;
    public WaveformTimelinePage()
    {
        InitializeComponent();

        soundEngine = new AudioGraphEngine();
        WaveformTimelineSample.RegisterSoundPlayer(soundEngine);
    }

    private async void BtnOpen_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker(MainWindow.Instance.AppWindow.Id);
        picker.FileTypeFilter.Add(".wav");

        var result = await picker.PickSingleFileAsync();
        if (result != null)
        {
            TxtPath.Text = result.Path;
            await soundEngine.OpenFileAsync(await StorageFile.GetFileFromPathAsync(result.Path), true);
            soundEngine.Play();
        }
    }

    private void BtnPlay_Click(object sender, RoutedEventArgs e)
    {
        soundEngine.Play();
    }

    private void BtnPause_Click(object sender, RoutedEventArgs e)
    {
        soundEngine.Pause();
    }

    private void BtnStop_Click(object sender, RoutedEventArgs e)
    {
        soundEngine.Stop();
    }

    private async void BtnOpenBuiltIn_Click(object sender, RoutedEventArgs e)
    {
        StorageFile sampleStorageFile = null;

        WaveformTimelineSample.RegisterSoundPlayer(soundEngine);

        if (RuntimeHelper.IsPackaged())
        {
            sampleStorageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Media/sample.wav"));
        }
        else
        {
            string assetPath = Path.Combine(AppContext.BaseDirectory, "Assets", "Media", "sample.wav");
            sampleStorageFile = await StorageFile.GetFileFromPathAsync(assetPath);
        }

        await soundEngine.OpenFileAsync(sampleStorageFile, true);

        soundEngine.Play();
    }
}
