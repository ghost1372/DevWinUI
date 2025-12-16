using Microsoft.Windows.Storage.Pickers;

namespace DevWinUIGallery.Views;

public sealed partial class WaveformTimelinePage : Page
{
    private NAudioEngine soundEngine;
    public WaveformTimelinePage()
    {
        InitializeComponent();

        soundEngine = NAudioEngine.Instance;
        WaveformTimelineSample.RegisterSoundPlayer(soundEngine);
    }

    private async void BtnOpen_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker(MainWindow.Instance.AppWindow.Id);
        picker.FileTypeFilter.Add(".mp3");

        var result = await picker.PickSingleFileAsync();
        if (result != null)
        {
            TxtPath.Text = result.Path;
            soundEngine.OpenFile(result.Path);
            soundEngine.Play();
        }
    }

    private void BtnPlay_Click(object sender, RoutedEventArgs e)
    {
        if (soundEngine.CanPlay)
        {
            soundEngine.Play();
        }
    }

    private void BtnPause_Click(object sender, RoutedEventArgs e)
    {
        if (soundEngine.CanPause)
        {
            soundEngine.Pause();
        }
    }

    private void BtnStop_Click(object sender, RoutedEventArgs e)
    {
        if (soundEngine.CanStop)
        {
            soundEngine.Stop();
        }
    }
}
