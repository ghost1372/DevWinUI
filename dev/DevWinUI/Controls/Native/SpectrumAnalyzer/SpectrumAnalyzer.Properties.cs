namespace DevWinUI;

public partial class SpectrumAnalyzer
{

    public static readonly DependencyProperty MaximumFrequencyProperty =
        DependencyProperty.Register( nameof(MaximumFrequency),typeof(int), typeof(SpectrumAnalyzer),new PropertyMetadata(20000, OnMaximumFrequencyChanged));

    private static void OnMaximumFrequencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        int newValue = (int)e.NewValue;
        int coercedValue = spectrumAnalyzer.OnCoerceMaximumFrequency(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(MaximumFrequencyProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnMaximumFrequencyChanged((int)e.OldValue, newValue);
    }

    protected virtual int OnCoerceMaximumFrequency(int value)
    {
        if (value < MinimumFrequency)
            return MinimumFrequency + 1;
        return value;
    }

    protected virtual void OnMaximumFrequencyChanged(int oldValue, int newValue)
    {
        UpdateBarLayout();
    }
    public int MaximumFrequency
    {
        get { return (int)GetValue(MaximumFrequencyProperty); }
        set { SetValue(MaximumFrequencyProperty, value); }
    }

    public static readonly DependencyProperty MinimumFrequencyProperty =
        DependencyProperty.Register(nameof(MinimumFrequency),typeof(int), typeof(SpectrumAnalyzer),new PropertyMetadata(20, OnMinimumFrequencyChanged));

    private static void OnMinimumFrequencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        int newValue = (int)e.NewValue;
        int coercedValue = spectrumAnalyzer.OnCoerceMinimumFrequency(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(MinimumFrequencyProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnMinimumFrequencyChanged((int)e.OldValue, newValue);
    }

    protected virtual int OnCoerceMinimumFrequency(int value)
    {
        if (value < 0)
            return 0;

        OnCoerceMaximumFrequency(MaximumFrequency);
        return value;
    }

    protected virtual void OnMinimumFrequencyChanged(int oldValue, int newValue)
    {
        UpdateBarLayout();
    }

    public int MinimumFrequency
    {
        get { return (int)GetValue(MinimumFrequencyProperty); }
        set { SetValue(MinimumFrequencyProperty, value); }
    }


    public static readonly DependencyProperty BarCountProperty =
        DependencyProperty.Register( nameof(BarCount),  typeof(int), typeof(SpectrumAnalyzer),new PropertyMetadata(32, OnBarCountChanged));

    private static void OnBarCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        int newValue = (int)e.NewValue;
        int coercedValue = spectrumAnalyzer.OnCoerceBarCount(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(BarCountProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnBarCountChanged((int)e.OldValue, newValue);
    }

    protected virtual int OnCoerceBarCount(int value)
    {
        return Math.Max(value, 1);
    }

    protected virtual void OnBarCountChanged(int oldValue, int newValue)
    {
        UpdateBarLayout();
    }

    public int BarCount
    {
        get { return (int)GetValue(BarCountProperty); }
        set { SetValue(BarCountProperty, value); }
    }

    public static readonly DependencyProperty BarSpacingProperty =
        DependencyProperty.Register( nameof(BarSpacing), typeof(double),typeof(SpectrumAnalyzer), new PropertyMetadata(5.0d, OnBarSpacingChanged));

    private static void OnBarSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        double newValue = (double)e.NewValue;
        double coercedValue = spectrumAnalyzer.OnCoerceBarSpacing(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(BarSpacingProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnBarSpacingChanged((double)e.OldValue, newValue);
    }

    protected virtual double OnCoerceBarSpacing(double value)
    {
        return Math.Max(value, 0);
    }

    protected virtual void OnBarSpacingChanged(double oldValue, double newValue)
    {
        UpdateBarLayout();
    }

    public double BarSpacing
    {
        get { return (double)GetValue(BarSpacingProperty); }
        set { SetValue(BarSpacingProperty, value); }
    }

    public static readonly DependencyProperty PeakFallDelayProperty =
        DependencyProperty.Register(nameof(PeakFallDelay), typeof(int), typeof(SpectrumAnalyzer), new PropertyMetadata(10, OnPeakFallDelayChanged));

    private static void OnPeakFallDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        int newValue = (int)e.NewValue;
        int coercedValue = spectrumAnalyzer.OnCoercePeakFallDelay(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(PeakFallDelayProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnPeakFallDelayChanged((int)e.OldValue, newValue);
    }

    protected virtual int OnCoercePeakFallDelay(int value)
    {
        return Math.Max(value, 0);
    }

    protected virtual void OnPeakFallDelayChanged(int oldValue, int newValue)
    {
    }
    public int PeakFallDelay
    {
        get { return (int)GetValue(PeakFallDelayProperty); }
        set { SetValue(PeakFallDelayProperty, value); }
    }

    public static readonly DependencyProperty IsFrequencyScaleLinearProperty =
        DependencyProperty.Register(nameof(IsFrequencyScaleLinear),typeof(bool), typeof(SpectrumAnalyzer), new PropertyMetadata(false, OnIsFrequencyScaleLinearChanged));

    private static void OnIsFrequencyScaleLinearChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        bool newValue = (bool)e.NewValue;
        bool coercedValue = spectrumAnalyzer.OnCoerceIsFrequencyScaleLinear(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(IsFrequencyScaleLinearProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnIsFrequencyScaleLinearChanged((bool)e.OldValue, newValue);
    }

    protected virtual bool OnCoerceIsFrequencyScaleLinear(bool value)
    {
        return value;
    }

    protected virtual void OnIsFrequencyScaleLinearChanged(bool oldValue, bool newValue)
    {
        UpdateBarLayout();
    }
    public bool IsFrequencyScaleLinear
    {
        get { return (bool)GetValue(IsFrequencyScaleLinearProperty); }
        set { SetValue(IsFrequencyScaleLinearProperty, value); }
    }

    public static readonly DependencyProperty BarHeightScalingProperty =
        DependencyProperty.Register(nameof(BarHeightScaling), typeof(BarHeightScalingStyles), typeof(SpectrumAnalyzer), new PropertyMetadata(BarHeightScalingStyles.Decibel, OnBarHeightScalingChanged));

    private static void OnBarHeightScalingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        var newValue = (BarHeightScalingStyles)e.NewValue;
        var coercedValue = spectrumAnalyzer.OnCoerceBarHeightScaling(newValue);

        if (!Equals(coercedValue, newValue))
        {
            spectrumAnalyzer.SetValue(BarHeightScalingProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnBarHeightScalingChanged(
            (BarHeightScalingStyles)e.OldValue,
            newValue);
    }

    protected virtual BarHeightScalingStyles OnCoerceBarHeightScaling(BarHeightScalingStyles value)
    {
        return value;
    }

    protected virtual void OnBarHeightScalingChanged(
        BarHeightScalingStyles oldValue,
        BarHeightScalingStyles newValue)
    {
    }

    public BarHeightScalingStyles BarHeightScaling
    {
        get { return (BarHeightScalingStyles)GetValue(BarHeightScalingProperty); }
        set { SetValue(BarHeightScalingProperty, value); }
    }

    public static readonly DependencyProperty AveragePeaksProperty =
        DependencyProperty.Register(nameof(AveragePeaks),typeof(bool), typeof(SpectrumAnalyzer), new PropertyMetadata(false, OnAveragePeaksChanged));

    private static void OnAveragePeaksChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        bool newValue = (bool)e.NewValue;
        bool coercedValue = spectrumAnalyzer.OnCoerceAveragePeaks(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(AveragePeaksProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnAveragePeaksChanged((bool)e.OldValue, newValue);
    }

    protected virtual bool OnCoerceAveragePeaks(bool value)
    {
        return value;
    }

    protected virtual void OnAveragePeaksChanged(bool oldValue, bool newValue)
    {
    }
    public bool AveragePeaks
    {
        get { return (bool)GetValue(AveragePeaksProperty); }
        set { SetValue(AveragePeaksProperty, value); }
    }

    public static readonly DependencyProperty BarStyleProperty =
        DependencyProperty.Register(nameof(BarStyle),typeof(Style),typeof(SpectrumAnalyzer), new PropertyMetadata(null, OnBarStyleChanged));

    private static void OnBarStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        var newValue = (Style)e.NewValue;
        var coercedValue = spectrumAnalyzer.OnCoerceBarStyle(newValue);

        if (!Equals(coercedValue, newValue))
        {
            spectrumAnalyzer.SetValue(BarStyleProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnBarStyleChanged((Style)e.OldValue, newValue);
    }

    protected virtual Style OnCoerceBarStyle(Style value)
    {
        return value;
    }

    protected virtual void OnBarStyleChanged(Style oldValue, Style newValue)
    {
        UpdateBarLayout();
    }

    public Style BarStyle
    {
        get { return (Style)GetValue(BarStyleProperty); }
        set { SetValue(BarStyleProperty, value); }
    }

    public static readonly DependencyProperty PeakStyleProperty =
        DependencyProperty.Register(nameof(PeakStyle),typeof(Style),typeof(SpectrumAnalyzer),new PropertyMetadata(null, OnPeakStyleChanged));

    private static void OnPeakStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        var newValue = (Style)e.NewValue;
        var coercedValue = spectrumAnalyzer.OnCoercePeakStyle(newValue);

        if (!Equals(coercedValue, newValue))
        {
            spectrumAnalyzer.SetValue(PeakStyleProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnPeakStyleChanged((Style)e.OldValue, newValue);
    }

    protected virtual Style OnCoercePeakStyle(Style value)
    {
        return value;
    }

    protected virtual void OnPeakStyleChanged(Style oldValue, Style newValue)
    {
        UpdateBarLayout();
    }

    public Style PeakStyle
    {
        get { return (Style)GetValue(PeakStyleProperty); }
        set { SetValue(PeakStyleProperty, value); }
    }

    public static readonly DependencyProperty ActualBarWidthProperty =
        DependencyProperty.Register(nameof(ActualBarWidth),typeof(double), typeof(SpectrumAnalyzer), new PropertyMetadata(0.0d, OnActualBarWidthChanged));

    private static void OnActualBarWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        double newValue = (double)e.NewValue;
        double coercedValue = spectrumAnalyzer.OnCoerceActualBarWidth(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(ActualBarWidthProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnActualBarWidthChanged((double)e.OldValue, newValue);
    }

    protected virtual double OnCoerceActualBarWidth(double value)
    {
        return value;
    }

    protected virtual void OnActualBarWidthChanged(double oldValue, double newValue)
    {
    }

    public double ActualBarWidth
    {
        get { return (double)GetValue(ActualBarWidthProperty); }
        protected set { SetValue(ActualBarWidthProperty, value); }
    }

    public static readonly DependencyProperty RefreshIntervalProperty =
        DependencyProperty.Register(nameof(RefreshInterval), typeof(int), typeof(SpectrumAnalyzer), new PropertyMetadata(defaultUpdateInterval, OnRefreshIntervalChanged));

    private static void OnRefreshIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        int newValue = (int)e.NewValue;
        int coercedValue = spectrumAnalyzer.OnCoerceRefreshInterval(newValue);

        if (coercedValue != newValue)
        {
            spectrumAnalyzer.SetValue(RefreshIntervalProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnRefreshIntervalChanged((int)e.OldValue, newValue);
    }

    protected virtual int OnCoerceRefreshInterval(int value)
    {
        return Math.Min(1000, Math.Max(10, value));
    }

    protected virtual void OnRefreshIntervalChanged(int oldValue, int newValue)
    {
        animationTimer.Interval = TimeSpan.FromMilliseconds(newValue);
    }

    public int RefreshInterval
    {
        get { return (int)GetValue(RefreshIntervalProperty); }
        set { SetValue(RefreshIntervalProperty, value); }
    }

    public static readonly DependencyProperty FFTComplexityProperty =
        DependencyProperty.Register(nameof(FFTComplexity), typeof(FFTDataSize), typeof(SpectrumAnalyzer),new PropertyMetadata(FFTDataSize.FFT2048, OnFFTComplexityChanged));

    private static void OnFFTComplexityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var spectrumAnalyzer = d as SpectrumAnalyzer;
        if (spectrumAnalyzer == null)
            return;

        var newValue = (FFTDataSize)e.NewValue;
        var coercedValue = spectrumAnalyzer.OnCoerceFFTComplexity(newValue);

        if (!Equals(coercedValue, newValue))
        {
            spectrumAnalyzer.SetValue(FFTComplexityProperty, coercedValue);
            return;
        }

        spectrumAnalyzer.OnFFTComplexityChanged((FFTDataSize)e.OldValue, newValue);
    }

    protected virtual FFTDataSize OnCoerceFFTComplexity(FFTDataSize value)
    {
        return value;
    }

    protected virtual void OnFFTComplexityChanged(FFTDataSize oldValue, FFTDataSize newValue)
    {
        channelData = new float[((int)newValue / 2)];
    }

    public FFTDataSize FFTComplexity
    {
        get { return (FFTDataSize)GetValue(FFTComplexityProperty); }
        set { SetValue(FFTComplexityProperty, value); }
    }
}
