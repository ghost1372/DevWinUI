using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;

namespace DevWinUIGallery.Views;

public sealed partial class BetterLyricPage : Page
{
    private DispatcherQueueTimer? _playbackTimer = App.Current.Resources.DispatcherQueue.CreateTimer();
    private bool _isDraggingSlider = false;

    public BetterLyricPage()
    {
        InitializeComponent();
        MyBetterLyric.CurrentLyricsData = LyricData.GetEnglishWithTranslationSample();

        MyBetterLyric.LineClicked += OnLineClicked;
        ApplyLyricsDataToSlider();
        StartPlaybackTimer();
    }

    private void OnLineClicked(object? sender, int lineIndex)
    {
        var lines = MyBetterLyric.CurrentLyricsData?.LyricsLines;
        if (lines != null && lineIndex >= 0 && lineIndex < lines.Count)
            ProgressSlider.Value = lines[lineIndex].StartMs;
    }

    private void ApplyLyricsDataToSlider()
    {
        var totalMs = (int)MyBetterLyric?.CurrentLyricsData?.Duration.TotalMilliseconds;
        ProgressSlider.Minimum = 0;
        ProgressSlider.Maximum = totalMs;
        ProgressSlider.StepFrequency = 100;
        ProgressSlider.Value = 0;

        var total = TimeSpan.FromMilliseconds(totalMs);
        PositionText.Text = $"0:00 / {(int)total.TotalMinutes}:{total.Seconds:D2}";
    }

    private void StartPlaybackTimer()
    {
        try
        {
            _playbackTimer!.Interval = TimeSpan.FromMilliseconds(100);
            _playbackTimer.Tick += (s, e) =>
            {
                if (_isDraggingSlider) return;

                var newMs = ProgressSlider.Value + 100;
                if (newMs > (int)MyBetterLyric?.CurrentLyricsData?.Duration.TotalMilliseconds) newMs = 0;

                ProgressSlider.Value = newMs;
            };
            _playbackTimer.Start();
        }
        catch (Exception)
        {
        }
    }

    private void ProgressSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        var ms = (int)e.NewValue;
        MyBetterLyric.CurrentPosition = TimeSpan.FromMilliseconds(ms);

        var current = TimeSpan.FromMilliseconds(ms);
        PositionText.Text = $"{(int)current.TotalMinutes}:{current.Seconds:D2} / {(int)(int)MyBetterLyric?.CurrentLyricsData?.Duration.TotalMinutes}:{(int)MyBetterLyric?.CurrentLyricsData?.Duration.Seconds:D2}";
    }

    private void ProgressSlider_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        _isDraggingSlider = true;
    }

    private void ProgressSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        _isDraggingSlider = false;
    }
}
