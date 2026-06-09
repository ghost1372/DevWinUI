using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Input;
using WinRT;

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

        Loaded -= OnLoaded;
        Loaded += OnLoaded;
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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        CmbLyricsAlignment.SelectedIndex = (int)MyBetterLyric.LyricsAlignmentType;
        CmbLyricsLineContentOrientation.SelectedIndex = (int)MyBetterLyric.LyricsLineContentOrientation;
        CmbWordByWord.SelectedIndex = (int)MyBetterLyric.WordByWordEffectMode;
        CmbLyricsGlowEffectScope.SelectedIndex = (int)MyBetterLyric.LyricsGlowEffectScope;
        CmbLyricsScrollEasingType.SelectedIndex = (int)MyBetterLyric.LyricsScrollEasingType;
        CmbLyricsScrollEasingMode.SelectedIndex = (int)MyBetterLyric.LyricsScrollEasingMode;
    }

    private void CmbLyricsAlignment_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbItem = CmbLyricsAlignment.SelectedItem.As<ComboBoxItem>();
        var item = GeneralHelper.GetEnum<LyricsTextAlignmentType>(cmbItem.Tag.ToString());
        if (item != null)
        {
            MyBetterLyric.LyricsAlignmentType = item;
        }
    }

    private void CmbLyricsLineContentOrientation_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {        
        var cmbItem = CmbLyricsLineContentOrientation.SelectedItem.As<ComboBoxItem>();
        var item = GeneralHelper.GetEnum<LyricsContentOrientation>(cmbItem.Tag.ToString());
        if (item != null)
        {
            MyBetterLyric.LyricsLineContentOrientation = item;
        }
    }

    private void CmbWordByWord_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbItem = CmbWordByWord.SelectedItem.As<ComboBoxItem>();
        var item = GeneralHelper.GetEnum<LyricsWordByWordEffectMode>(cmbItem.Tag.ToString());
        if (item != null)
        {
            MyBetterLyric.WordByWordEffectMode = item;
        }
    }

    private void CmbLyricsGlowEffectScope_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbItem = CmbLyricsGlowEffectScope.SelectedItem.As<ComboBoxItem>();
        var item = GeneralHelper.GetEnum<LyricEffectScope>(cmbItem.Tag.ToString());
        if (item != null)
        {
            MyBetterLyric.LyricsGlowEffectScope = item;
        }
    }

    private void CmbLyricsScrollEasingType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbItem = CmbLyricsScrollEasingType.SelectedItem.As<ComboBoxItem>();
        var item = GeneralHelper.GetEnum<AnimationEasingType>(cmbItem.Tag.ToString());
        if (item != null)
        {
            MyBetterLyric.LyricsScrollEasingType = item;
        }
    }

    private void CmbLyricsScrollEasingMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var cmbItem = CmbLyricsScrollEasingMode.SelectedItem.As<ComboBoxItem>();
        var item = GeneralHelper.GetEnum<AnimationEaseMode>(cmbItem.Tag.ToString());
        if (item != null)
        {
            MyBetterLyric.LyricsScrollEasingMode = item;
        }
    }
}
