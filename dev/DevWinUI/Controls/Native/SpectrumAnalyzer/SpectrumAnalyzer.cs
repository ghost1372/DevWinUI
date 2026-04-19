using System.ComponentModel;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_SpectrumCanvas), Type = typeof(Canvas))]
public partial class SpectrumAnalyzer : Control
{
    private const string PART_SpectrumCanvas = "PART_SpectrumCanvas";
    private Canvas spectrumCanvas;

    private readonly DispatcherTimer animationTimer;
    private ISpectrumPlayer soundPlayer;
    private readonly List<Shape> barShapes = new List<Shape>();
    private readonly List<Shape> peakShapes = new List<Shape>();
    private double[] barHeights;
    private double[] peakHeights;
    private float[] channelData = new float[2048];
    private float[] channelPeakData;
    private double bandWidth = 1.0;
    private double barWidth = 1;
    private int maximumFrequencyIndex = 2047;
    private int minimumFrequencyIndex;
    private int[] barIndexMax;
    private int[] barLogScaleIndexMax;

    private const int scaleFactorLinear = 9;
    private const int scaleFactorSqr = 2;
    private const double minDBValue = -90;
    private const double maxDBValue = 0;
    private const double dbScale = (maxDBValue - minDBValue);
    private const int defaultUpdateInterval = 25;

    public SpectrumAnalyzer()
    {
        DefaultStyleKey = typeof(SpectrumAnalyzer);
        animationTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(defaultUpdateInterval),
        };
        animationTimer.Tick += animationTimer_Tick;
    }
    protected override void OnApplyTemplate()
    {
        if (spectrumCanvas != null)
        {
            spectrumCanvas.Children.Clear();
        }
        
        spectrumCanvas = GetTemplateChild(PART_SpectrumCanvas) as Canvas;
        spectrumCanvas.SizeChanged -= OnSpectrumCanvasSizeChanged;
        spectrumCanvas.SizeChanged += OnSpectrumCanvasSizeChanged;

        UpdateBarLayout();
        UpdateSpectrum();
    }


    public void RegisterSoundPlayer(ISpectrumPlayer soundPlayer)
    {
        this.soundPlayer = soundPlayer;
        soundPlayer.PropertyChanged += soundPlayer_PropertyChanged;
        UpdateBarLayout();
        animationTimer.Start();
    }
    
    private void UpdateSpectrum()
    {
        if (soundPlayer == null || spectrumCanvas == null || spectrumCanvas.RenderSize.Width < 1 || spectrumCanvas.RenderSize.Height < 1)
            return;

        if (soundPlayer.IsPlaying && !soundPlayer.GetFFTData(channelData))
            return;

        UpdateSpectrumShapes();
    }

    private void UpdateSpectrumShapes()
    {
        bool allZero = true;
        double fftBucketHeight = 0f;
        double barHeight = 0f;
        double lastPeakHeight = 0f;
        double peakYPos = 0f;
        double height = spectrumCanvas.RenderSize.Height;
        int barIndex = 0;
        double peakDotHeight = Math.Max(barWidth / 2.0f, 1);
        double barHeightScale = (height - peakDotHeight);

        for (int i = minimumFrequencyIndex; i <= maximumFrequencyIndex; i++)
        {
            // If we're paused, keep drawing, but set the current height to 0 so the peaks fall.
            if (!soundPlayer.IsPlaying)
            {
                barHeight = 0f;
            }
            else // Draw the maximum value for the bar's band
            {
                switch (BarHeightScaling)
                {
                    case BarHeightScalingStyles.Decibel:
                        double dbValue = 20 * Math.Log10((double)channelData[i]);
                        fftBucketHeight = ((dbValue - minDBValue) / dbScale) * barHeightScale;
                        break;
                    case BarHeightScalingStyles.Linear:
                        fftBucketHeight = (channelData[i] * scaleFactorLinear) * barHeightScale;
                        break;
                    case BarHeightScalingStyles.Sqrt:
                        fftBucketHeight = (((Math.Sqrt((double)channelData[i])) * scaleFactorSqr) * barHeightScale);
                        break;
                }

                if (barHeight < fftBucketHeight)
                    barHeight = fftBucketHeight;
                if (barHeight < 0f)
                    barHeight = 0f;
            }

            // If this is the last FFT bucket in the bar's group, draw the bar.
            int currentIndexMax = IsFrequencyScaleLinear ? barIndexMax[barIndex] : barLogScaleIndexMax[barIndex];
            if (i == currentIndexMax)
            {
                // Peaks can't surpass the height of the control.
                if (barHeight > height)
                    barHeight = height;

                if (AveragePeaks && barIndex > 0)
                    barHeight = (lastPeakHeight + barHeight) / 2;

                peakYPos = barHeight;

                if (channelPeakData[barIndex] < peakYPos)
                    channelPeakData[barIndex] = (float)peakYPos;
                else
                    channelPeakData[barIndex] = (float)(peakYPos + (PeakFallDelay * channelPeakData[barIndex])) / ((float)(PeakFallDelay + 1));

                double xCoord = BarSpacing + (barWidth * barIndex) + (BarSpacing * barIndex) + 1;

                barShapes[barIndex].Margin = new Thickness(xCoord, (height - 1) - barHeight, 0, 0);
                barShapes[barIndex].Height = barHeight;
                peakShapes[barIndex].Margin = new Thickness(xCoord, (height - 1) - channelPeakData[barIndex] - peakDotHeight, 0, 0);
                peakShapes[barIndex].Height = peakDotHeight;

                if (channelPeakData[barIndex] > 0.05)
                    allZero = false;

                lastPeakHeight = barHeight;
                barHeight = 0f;
                barIndex++;
            }
        }

        if (allZero && !soundPlayer.IsPlaying)
            animationTimer.Stop();
    }

    private void UpdateBarLayout()
    {
        if (soundPlayer == null || spectrumCanvas == null)
            return;

        barWidth = Math.Max(((double)(spectrumCanvas.RenderSize.Width - (BarSpacing * (BarCount + 1))) / (double)BarCount), 1);
        maximumFrequencyIndex = Math.Min(soundPlayer.GetFFTFrequencyIndex(MaximumFrequency) + 1, 2047);
        minimumFrequencyIndex = Math.Min(soundPlayer.GetFFTFrequencyIndex(MinimumFrequency), 2047);
        bandWidth = Math.Max(((double)(maximumFrequencyIndex - minimumFrequencyIndex)) / spectrumCanvas.RenderSize.Width, 1.0);

        int actualBarCount;
        if (barWidth >= 1.0d)
            actualBarCount = BarCount;
        else
            actualBarCount = Math.Max((int)((spectrumCanvas.RenderSize.Width - BarSpacing) / (barWidth + BarSpacing)), 1);
        channelPeakData = new float[actualBarCount];

        int indexCount = maximumFrequencyIndex - minimumFrequencyIndex;
        int linearIndexBucketSize = (int)Math.Round((double)indexCount / (double)actualBarCount, 0);
        List<int> maxIndexList = new List<int>();
        List<int> maxLogScaleIndexList = new List<int>();
        double maxLog = Math.Log(actualBarCount, actualBarCount);
        for (int i = 1; i < actualBarCount; i++)
        {
            maxIndexList.Add(minimumFrequencyIndex + (i * linearIndexBucketSize));
            int logIndex = (int)((maxLog - Math.Log((actualBarCount + 1) - i, (actualBarCount + 1))) * indexCount) + minimumFrequencyIndex;
            maxLogScaleIndexList.Add(logIndex);
        }
        maxIndexList.Add(maximumFrequencyIndex);
        maxLogScaleIndexList.Add(maximumFrequencyIndex);
        barIndexMax = maxIndexList.ToArray();
        barLogScaleIndexMax = maxLogScaleIndexList.ToArray();

        barHeights = new double[actualBarCount];
        peakHeights = new double[actualBarCount];

        spectrumCanvas.Children.Clear();
        barShapes.Clear();
        peakShapes.Clear();

        double height = spectrumCanvas.RenderSize.Height;
        double peakDotHeight = Math.Max(barWidth / 2.0f, 1);
        for (int i = 0; i < actualBarCount; i++)
        {
            double xCoord = BarSpacing + (barWidth * i) + (BarSpacing * i) + 1;
            Rectangle barRectangle = new Rectangle()
            {
                Margin = new Thickness(xCoord, height, 0, 0),
                Width = barWidth,
                Height = 0,
                Style = BarStyle
            };
            barShapes.Add(barRectangle);
            Rectangle peakRectangle = new Rectangle()
            {
                Margin = new Thickness(xCoord, height - peakDotHeight, 0, 0),
                Width = barWidth,
                Height = peakDotHeight,
                Style = PeakStyle
            };
            peakShapes.Add(peakRectangle);
        }

        foreach (Shape shape in barShapes)
            spectrumCanvas.Children.Add(shape);
        foreach (Shape shape in peakShapes)
            spectrumCanvas.Children.Add(shape);

        ActualBarWidth = barWidth;
    }

    private void soundPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "IsPlaying":
                if (soundPlayer.IsPlaying && !animationTimer.IsEnabled)
                    animationTimer.Start();
                break;
        }
    }

    private void animationTimer_Tick(object sender, object e)
    {
        UpdateSpectrum();
    }

    private void OnSpectrumCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateBarLayout();
        UpdateSpectrum();
    }
}
