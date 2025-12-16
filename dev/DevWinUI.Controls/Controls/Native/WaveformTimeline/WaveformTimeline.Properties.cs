namespace DevWinUI;

public partial class WaveformTimeline
{

    public static readonly DependencyProperty LeftLevelBrushProperty =
        DependencyProperty.Register(nameof(LeftLevelBrush), typeof(Brush), typeof(WaveformTimeline), new PropertyMetadata(new SolidColorBrush(Colors.Blue), OnLeftLevelBrushChanged));

    private static void OnLeftLevelBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnLeftLevelBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }
    }

    protected virtual void OnLeftLevelBrushChanged(Brush oldValue, Brush newValue)
    {
        leftPath.Fill = newValue;
        UpdateWaveform();
    }

    public Brush LeftLevelBrush
    {
        get { return (Brush)GetValue(LeftLevelBrushProperty); }
        set { SetValue(LeftLevelBrushProperty, value); }
    }


    public static readonly DependencyProperty RightLevelBrushProperty =
        DependencyProperty.Register(nameof(RightLevelBrush), typeof(Brush), typeof(WaveformTimeline), new PropertyMetadata(new SolidColorBrush(Colors.Red), OnRightLevelBrushChanged));

    private static void OnRightLevelBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnRightLevelBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }
    }

    protected virtual void OnRightLevelBrushChanged(Brush oldValue, Brush newValue)
    {
        rightPath.Fill = newValue;
        UpdateWaveform();
    }

    public Brush RightLevelBrush
    {
        get { return (Brush)GetValue(RightLevelBrushProperty); }
        set { SetValue(RightLevelBrushProperty, value); }
    }

    public static readonly DependencyProperty ProgressBarBrushProperty =
        DependencyProperty.Register(nameof(ProgressBarBrush), typeof(Brush), typeof(WaveformTimeline), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0xCD, 0xBA, 0x00, 0xFF)), OnProgressBarBrushChanged));

    private static void OnProgressBarBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnProgressBarBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }
    }

    protected virtual void OnProgressBarBrushChanged(Brush oldValue, Brush newValue)
    {
        progressIndicator.Fill = newValue;
        progressLine.Stroke = newValue;

        CreateProgressIndicator();
    }

    public Brush ProgressBarBrush
    {
        get { return (Brush)GetValue(ProgressBarBrushProperty); }
        set { SetValue(ProgressBarBrushProperty, value); }
    }

    public static readonly DependencyProperty ProgressBarThicknessProperty =
        DependencyProperty.Register(nameof(ProgressBarThickness), typeof(double), typeof(WaveformTimeline), new PropertyMetadata(2.0d, OnProgressBarThicknessChanged));

    private static void OnProgressBarThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnProgressBarThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }
    }

    protected virtual void OnProgressBarThicknessChanged(double oldValue, double newValue)
    {
        progressLine.StrokeThickness = newValue;
        CreateProgressIndicator();
    }

    public double ProgressBarThickness
    {
        get { return (double)GetValue(ProgressBarThicknessProperty); }
        set { SetValue(ProgressBarThicknessProperty, value); }
    }

    public static readonly DependencyProperty CenterLineBrushProperty =
        DependencyProperty.Register(nameof(CenterLineBrush), typeof(Brush), typeof(WaveformTimeline), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnCenterLineBrushChanged));

    private static void OnCenterLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnCenterLineBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }
    }
    protected virtual void OnCenterLineBrushChanged(Brush oldValue, Brush newValue)
    {
        centerLine.Stroke = newValue;
        UpdateWaveform();
    }

    public Brush CenterLineBrush
    {
        get { return (Brush)GetValue(CenterLineBrushProperty); }
        set { SetValue(CenterLineBrushProperty, value); }
    }

    public static readonly DependencyProperty CenterLineThicknessProperty =
        DependencyProperty.Register(nameof(CenterLineThickness), typeof(double), typeof(WaveformTimeline), new PropertyMetadata(1.0d, OnCenterLineThicknessChanged));

    private static void OnCenterLineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnCenterLineThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }
    }

    protected virtual void OnCenterLineThicknessChanged(double oldValue, double newValue)
    {
        centerLine.StrokeThickness = newValue;
        UpdateWaveform();
    }

    public double CenterLineThickness
    {
        get { return (double)GetValue(CenterLineThicknessProperty); }
        set { SetValue(CenterLineThicknessProperty, value); }
    }

    public static readonly DependencyProperty RepeatRegionBrushProperty =
        DependencyProperty.Register(nameof(RepeatRegionBrush), typeof(Brush), typeof(WaveformTimeline), new PropertyMetadata(new SolidColorBrush(Color.FromArgb(0x81, 0xF6, 0xFF, 0x00)), OnRepeatRegionBrushChanged));

    private static void OnRepeatRegionBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnRepeatRegionBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }
    }

    protected virtual void OnRepeatRegionBrushChanged(Brush oldValue, Brush newValue)
    {
        repeatRegion.Fill = newValue;
        UpdateRepeatRegion();
    }

    public Brush RepeatRegionBrush
    {
        get { return (Brush)GetValue(RepeatRegionBrushProperty); }
        set { SetValue(RepeatRegionBrushProperty, value); }
    }

    public static readonly DependencyProperty AllowRepeatRegionsProperty =
        DependencyProperty.Register(nameof(AllowRepeatRegions), typeof(bool), typeof(WaveformTimeline), new PropertyMetadata(true, OnAllowRepeatRegionsChanged));

    private static void OnAllowRepeatRegionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnAllowRepeatRegionsChanged((bool)e.OldValue, (bool)e.NewValue);
        }
    }

    protected virtual void OnAllowRepeatRegionsChanged(bool oldValue, bool newValue)
    {
        if (!newValue && soundPlayer != null)
        {
            soundPlayer.SelectionBegin = TimeSpan.Zero;
            soundPlayer.SelectionEnd = TimeSpan.Zero;
        }
    }

    public bool AllowRepeatRegions
    {
        get { return (bool)GetValue(AllowRepeatRegionsProperty); }
        set { SetValue(AllowRepeatRegionsProperty, value); }
    }

    public static readonly DependencyProperty TimelineTickBrushProperty =
        DependencyProperty.Register(nameof(TimelineTickBrush), typeof(Brush), typeof(WaveformTimeline), new PropertyMetadata(new SolidColorBrush(Colors.Black), OnTimelineTickBrushChanged));

    private static void OnTimelineTickBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WaveformTimeline waveformTimeline)
        {
            waveformTimeline.OnTimelineTickBrushChanged((Brush)e.OldValue, (Brush)e.NewValue);
        }
    }

    protected virtual void OnTimelineTickBrushChanged(Brush oldValue, Brush newValue)
    {
        UpdateTimeline();
    }

    public Brush TimelineTickBrush
    {
        get { return (Brush)GetValue(TimelineTickBrushProperty); }
        set { SetValue(TimelineTickBrushProperty, value); }
    }
}
