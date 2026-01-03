using Microsoft.Windows.Storage.Pickers;
using Windows.Storage;

namespace DevWinUIGallery.Views;

public sealed partial class SpectrumAnalyzerPage : Page
{
    private AudioGraphEngine soundEngine;
    public SpectrumAnalyzerPage()
    {
        InitializeComponent();

        soundEngine = new AudioGraphEngine();
        SpectrumAnalyzerSample.RegisterSoundPlayer(soundEngine);

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        soundEngine.Dispose();
    }

    private async void BtnOpen_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker(MainWindow.Instance.AppWindow.Id);
        picker.FileTypeFilter.Add(".mp3");
        picker.FileTypeFilter.Add(".wav");

        var result = await picker.PickSingleFileAsync();
        if (result != null)
        {
            TxtPath.Text = result.Path;
            await soundEngine.OpenFileAsync(await StorageFile.GetFileFromPathAsync(result.Path), false);
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
}
