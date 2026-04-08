using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class AudioWave : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private CanvasAnimatedControl canvas;

    private double songPos_ = 0;
    private double elapsedMs = 0;
    private float[] waveData;

    private double duration = TimeSpan.FromSeconds(4).TotalMilliseconds;
    private float barWidth = 6f;
    private float barSpacing = 3f;
    private Color barBackground = Colors.Gray;
    private List<CanvasGradientStop> barBackgroundGradientStops = null;
    private List<CanvasGradientStop> barForegroundGradientStops = null;
    private Color barForeground = Colors.DeepPink;
    private float barRadiusX = 3f;
    private float barRadiusY = 3f;
    private AudioWaveState state = AudioWaveState.Stopped;
    private int lastProgress = -1;
    private double manualProgress = -1; // -1 means no manual override
    private bool isDragging = false;
    private double dragPosition = 0;
    public Func<float[]> WaveDataProvider { get; set; }
    public event EventHandler<int> ProgressChanged;

    public AudioWave()
    {
        DefaultStyleKey = typeof(AudioWave);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        waveData = WaveDataProvider?.Invoke();
        canvas = GetTemplateChild(PART_Canvas) as CanvasAnimatedControl;

        canvas.Draw -= OnCanvasDraw;
        canvas.Draw += OnCanvasDraw;

        canvas.Update -= OnCanvasUpdate;
        canvas.Update += OnCanvasUpdate;

        canvas.PointerPressed -= Canvas_PointerPressed;
        canvas.PointerPressed += Canvas_PointerPressed;

        canvas.PointerMoved -= Canvas_PointerMoved;
        canvas.PointerMoved += Canvas_PointerMoved;

        canvas.PointerReleased -= Canvas_PointerReleased;
        canvas.PointerReleased += Canvas_PointerReleased;
    }

    private void Canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!CanSelectedByMouse)
            return;

        var point = e.GetCurrentPoint(canvas);
        isDragging = true;
        UpdateDragPosition(point.Position.X);
    }

    private void Canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (!isDragging || !CanSelectedByMouse) return;

        var point = e.GetCurrentPoint(canvas);
        UpdateDragPosition(point.Position.X);
    }

    private void UpdateDragPosition(double x)
    {
        dragPosition = Math.Clamp(x, 0, canvas.Size.Width);
        manualProgress = dragPosition / canvas.Size.Width;
        canvas?.Invalidate();
    }

    private void Canvas_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!isDragging || !CanSelectedByMouse) return;

        isDragging = false;

        dragPosition = Math.Clamp(dragPosition, 0, canvas.Size.Width);
        manualProgress = dragPosition / canvas.Size.Width;

        elapsedMs = manualProgress * duration;

        int progressPercent = (int)(manualProgress * 100);
        lastProgress = progressPercent;
        ProgressChanged?.Invoke(this, progressPercent);

        canvas?.Invalidate();
    }

    private void OnCanvasDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var s = args.DrawingSession;

        int totalBars = waveData.Length;
        float availableWidth = (float)sender.Size.Width;

        float totalSpacing = barSpacing * (totalBars - 1);
        float totalBarWidth = totalBars * barWidth;
        float scale = Math.Min(1f, availableWidth / (totalBarWidth + totalSpacing));
        float actualBarWidth = barWidth * scale;
        float actualBarSpacing = barSpacing * scale;

        float x = 0f;
        float centerY = (float)sender.Size.Height / 2f;

        double progress = (manualProgress >= 0)
            ? manualProgress
            : Math.Min(1.0, elapsedMs / duration);

        int highlightCount = (int)(totalBars * progress);

        for (int i = 0; i < totalBars; i++)
        {
            float h = Math.Max(0, waveData[i]);

            float top = centerY - h / 2f;
            Rect barRect = new Rect(x, top, actualBarWidth, h);

            Color color = barBackground;
            List<CanvasGradientStop> stopsToUse = barBackgroundGradientStops;

            if ((state == AudioWaveState.Playing || state == AudioWaveState.Paused || manualProgress >= 0)
                && i < highlightCount)
            {
                color = barForeground;
                stopsToUse = barForegroundGradientStops;
            }

            if (stopsToUse == null)
            {
                s.FillRoundedRectangle(barRect, barRadiusX, barRadiusY, color);
            }
            else
            {
                CanvasGradientStop[] canvasStops = stopsToUse
                    .Select(gs => new CanvasGradientStop { Color = gs.Color, Position = gs.Position })
                    .ToArray();

                var gradient = new CanvasLinearGradientBrush(args.DrawingSession, canvasStops)
                {
                    StartPoint = new Vector2(x, 0),
                    EndPoint = new Vector2(x + actualBarWidth, 0)
                };

                s.FillRoundedRectangle(barRect, barRadiusX, barRadiusY, gradient);
            }

            x += actualBarWidth + actualBarSpacing;

            if (x > sender.Size.Width) break;
        }
    }

    private void OnCanvasUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (state != AudioWaveState.Playing)
            return;

        elapsedMs += args.Timing.ElapsedTime.TotalMilliseconds;
        double progress = Math.Min(1.0, elapsedMs / duration);

        songPos_ = sender.Size.Width * progress;

        int progressPercent = (int)(progress * 100);
        if (progressPercent != lastProgress)
        {
            lastProgress = progressPercent;
            DispatcherQueue.TryEnqueue(() =>
            {
                Progress = progressPercent;
            });
            ProgressChanged?.Invoke(this, progressPercent);
        }
    }
    public void Start()
    {
        manualProgress = -1;

        if (state == AudioWaveState.Playing) return;
        state = AudioWaveState.Playing;
    }

    public void Pause()
    {
        if (state != AudioWaveState.Playing) return;
        state = AudioWaveState.Paused;
    }

    public void Stop()
    {
        state = AudioWaveState.Stopped;
        elapsedMs = 0;
        songPos_ = 0;
        lastProgress = -1;
    }
    private void OnProgressChanged()
    {
        int percent = Progress;

        percent = Math.Clamp(percent, 0, 100);

        manualProgress = percent / 100.0;
        elapsedMs = manualProgress * duration;

        if (lastProgress != percent)
        {
            lastProgress = percent;
            ProgressChanged?.Invoke(this, percent);
        }

        canvas?.Invalidate();
    }
    public float[] GenerateSampleWaveData2(int count)
    {
        float[] sample = new float[count];
        var rand = new Random();
        for (int i = 0; i < sample.Length; i++)
            sample[i] = rand.Next(20, 80);

        return sample;
    }

    public float[] GenerateSampleWaveData(int count)
    {
        float[] sample = new float[count];
        var rand = new Random();
        float last = 50f;
        for (int i = 0; i < sample.Length; i++)
        {
            float target = rand.Next(20, 80);

            float h = last + (target - last) * 0.3f;

            sample[i] = h;
            last = h;
        }
        return sample;
    }

    public void UpdateWaveData()
    {
        waveData = WaveDataProvider?.Invoke();
    }
}
