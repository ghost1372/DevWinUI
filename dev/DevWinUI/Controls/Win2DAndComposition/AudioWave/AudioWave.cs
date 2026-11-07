using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public partial class AudioWave : Control
{
    private const string PART_Canvas = "PART_Canvas";

    private readonly object sync = new();
    private CanvasAnimatedControl canvas;
    private float[] waveData;

    private double elapsedMs;
    private double duration = TimeSpan.FromSeconds(4).TotalMilliseconds;
    private float barWidth = 6f;
    private float barSpacing = 3f;
    private float barRadiusX = 3f;
    private float barRadiusY = 3f;
    private Color barBackground = Colors.Gray;
    private Color barForeground = Colors.DeepPink;
    private CanvasGradientStop[] backgroundStops;
    private CanvasGradientStop[] foregroundStops;
    private CanvasLinearGradientBrush backgroundBrush;
    private CanvasLinearGradientBrush foregroundBrush;
    private volatile AudioWaveState state = AudioWaveState.Stopped;
    private volatile bool isDragging;
    private bool isSyncingProgress;
    private int lastProgress = -1;

    public Func<float[]> WaveDataProvider { get; set; }
    public event EventHandler<int> ProgressChanged;

    public AudioWave()
    {
        DefaultStyleKey = typeof(AudioWave);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        DetachCanvas();
        canvas = GetTemplateChild(PART_Canvas) as CanvasAnimatedControl;
        waveData = WaveDataProvider?.Invoke();

        if (IsLoaded)
            AttachCanvas();
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => AttachCanvas();

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        isDragging = false;
        DetachCanvas();
        ReleaseBrushes();
    }

    private void AttachCanvas()
    {
        if (canvas == null)
            return;

        DetachCanvas();

        canvas.CreateResources += OnCanvasCreateResources;
        canvas.Draw += OnCanvasDraw;
        canvas.Update += OnCanvasUpdate;
        canvas.SizeChanged += OnCanvasSizeChanged;
        canvas.PointerPressed += OnCanvasPointerPressed;
        canvas.PointerMoved += OnCanvasPointerMoved;
        canvas.PointerReleased += OnCanvasPointerReleased;
        canvas.PointerCanceled += OnCanvasPointerCanceled;
        canvas.PointerCaptureLost += OnCanvasPointerCaptureLost;

        UpdatePlaybackLoop();
    }

    private void DetachCanvas()
    {
        if (canvas == null)
            return;

        canvas.CreateResources -= OnCanvasCreateResources;
        canvas.Draw -= OnCanvasDraw;
        canvas.Update -= OnCanvasUpdate;
        canvas.SizeChanged -= OnCanvasSizeChanged;
        canvas.PointerPressed -= OnCanvasPointerPressed;
        canvas.PointerMoved -= OnCanvasPointerMoved;
        canvas.PointerReleased -= OnCanvasPointerReleased;
        canvas.PointerCanceled -= OnCanvasPointerCanceled;
        canvas.PointerCaptureLost -= OnCanvasPointerCaptureLost;
    }

    private void OnCanvasCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args) => ReleaseBrushes();

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e) => canvas?.Invalidate();

    private void OnCanvasPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (!CanSelectedByMouse || !canvas.CapturePointer(e.Pointer))
            return;

        isDragging = true;
        SeekToPointer(e);
    }

    private void OnCanvasPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if (isDragging)
            SeekToPointer(e);
    }

    private void OnCanvasPointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!isDragging)
            return;

        SeekToPointer(e);
        canvas.ReleasePointerCapture(e.Pointer);
    }

    private void OnCanvasPointerCanceled(object sender, PointerRoutedEventArgs e)
    {
        if (isDragging)
            canvas.ReleasePointerCapture(e.Pointer);
    }

    private void OnCanvasPointerCaptureLost(object sender, PointerRoutedEventArgs e) => CompleteDrag();

    private void SeekToPointer(PointerRoutedEventArgs e)
    {
        double width = canvas.ActualWidth;
        if (width <= 0)
            return;

        Seek(e.GetCurrentPoint(canvas).Position.X / width);
    }

    private void Seek(double ratio)
    {
        lock (sync)
            elapsedMs = Math.Clamp(ratio, 0, 1) * duration;

        canvas?.Invalidate();
    }

    private void CompleteDrag()
    {
        if (!isDragging)
            return;

        isDragging = false;

        int percent;
        lock (sync)
            percent = (int)(GetProgress() * 100);

        Volatile.Write(ref lastProgress, percent);
        PublishProgress(percent);
    }

    private double GetProgress() => duration > 0 ? Math.Clamp(elapsedMs / duration, 0, 1) : 0;

    private void OnCanvasDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var data = waveData;
        float width = (float)sender.Size.Width;
        float height = (float)sender.Size.Height;

        if (data is not { Length: > 0 } || width <= 0 || height <= 0)
            return;

        var session = args.DrawingSession;
        int totalBars = data.Length;

        float totalWidth = totalBars * barWidth + barSpacing * (totalBars - 1);
        float scale = Math.Min(1f, width / totalWidth);
        float actualBarWidth = barWidth * scale;
        float step = actualBarWidth + barSpacing * scale;
        float centerY = height / 2f;
        float x = 0f;

        lock (sync)
        {
            var background = GetBrush(ref backgroundBrush, backgroundStops, sender);
            var foreground = GetBrush(ref foregroundBrush, foregroundStops, sender);
            int highlightCount = (int)(totalBars * GetProgress());

            for (int i = 0; i < totalBars; i++)
            {
                float h = Math.Clamp(data[i], 0f, height);
                float y = centerY - h / 2f;

                bool active = i < highlightCount;
                var brush = active ? foreground : background;

                if (brush == null)
                {
                    session.FillRoundedRectangle(x, y, actualBarWidth, h, barRadiusX, barRadiusY, active ? barForeground : barBackground);
                }
                else
                {
                    brush.StartPoint = new Vector2(x, 0);
                    brush.EndPoint = new Vector2(x + actualBarWidth, 0);
                    session.FillRoundedRectangle(x, y, actualBarWidth, h, barRadiusX, barRadiusY, brush);
                }

                x += step;
            }
        }
    }

    private static CanvasLinearGradientBrush GetBrush(ref CanvasLinearGradientBrush brush, CanvasGradientStop[] stops, ICanvasResourceCreator creator)
        => stops == null ? null : brush ??= new CanvasLinearGradientBrush(creator, stops);

    private void OnCanvasUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (state != AudioWaveState.Playing || isDragging)
            return;

        int percent;
        bool completed;

        lock (sync)
        {
            elapsedMs += args.Timing.ElapsedTime.TotalMilliseconds;
            completed = elapsedMs >= duration;

            if (completed)
                elapsedMs = duration;

            percent = (int)(GetProgress() * 100);
        }

        if (Interlocked.Exchange(ref lastProgress, percent) != percent)
            DispatcherQueue.TryEnqueue(() => PublishProgressIfCurrent(percent));

        if (completed)
        {
            state = AudioWaveState.Paused;
            DispatcherQueue.TryEnqueue(UpdatePlaybackLoop);
        }
    }

    private void PublishProgressIfCurrent(int percent)
    {
        if (Volatile.Read(ref lastProgress) == percent)
            PublishProgress(percent);
    }

    private void PublishProgress(int percent)
    {
        SyncProgress(percent);
        ProgressChanged?.Invoke(this, percent);
    }

    private void SyncProgress(int percent)
    {
        isSyncingProgress = true;
        try
        {
            Progress = percent;
        }
        finally
        {
            isSyncingProgress = false;
        }
    }

    private void ApplyProgress(int percent)
    {
        if (isSyncingProgress)
            return;

        percent = Math.Clamp(percent, 0, 100);

        lock (sync)
            elapsedMs = percent / 100.0 * duration;

        if (Interlocked.Exchange(ref lastProgress, percent) != percent)
            ProgressChanged?.Invoke(this, percent);

        canvas?.Invalidate();
    }

    private void ApplyDuration(double milliseconds)
    {
        lock (sync)
        {
            duration = milliseconds;
            elapsedMs = 0;
        }

        Volatile.Write(ref lastProgress, -1);
        SyncProgress(0);
        canvas?.Invalidate();
    }

    private void ApplyBackgroundGradientStops(List<CanvasGradientStop> stops)
    {
        lock (sync)
        {
            backgroundStops = stops is { Count: > 0 } ? stops.ToArray() : null;
            backgroundBrush?.Dispose();
            backgroundBrush = null;
        }

        canvas?.Invalidate();
    }

    private void ApplyForegroundGradientStops(List<CanvasGradientStop> stops)
    {
        lock (sync)
        {
            foregroundStops = stops is { Count: > 0 } ? stops.ToArray() : null;
            foregroundBrush?.Dispose();
            foregroundBrush = null;
        }

        canvas?.Invalidate();
    }

    private void ReleaseBrushes()
    {
        lock (sync)
        {
            backgroundBrush?.Dispose();
            backgroundBrush = null;
            foregroundBrush?.Dispose();
            foregroundBrush = null;
        }
    }

    private void UpdatePlaybackLoop()
    {
        if (canvas == null)
            return;

        canvas.Paused = state != AudioWaveState.Playing;
        canvas.Invalidate();
    }

    public void Start()
    {
        if (state == AudioWaveState.Playing)
            return;

        lock (sync)
        {
            if (elapsedMs >= duration)
                elapsedMs = 0;
        }

        state = AudioWaveState.Playing;
        UpdatePlaybackLoop();
    }

    public void Pause()
    {
        if (state != AudioWaveState.Playing)
            return;

        state = AudioWaveState.Paused;
        UpdatePlaybackLoop();
    }

    public void Stop()
    {
        state = AudioWaveState.Stopped;

        lock (sync)
            elapsedMs = 0;

        Volatile.Write(ref lastProgress, -1);
        SyncProgress(0);
        UpdatePlaybackLoop();
    }

    public float[] GenerateSampleWaveData2(int count)
    {
        float[] sample = new float[count];
        for (int i = 0; i < sample.Length; i++)
            sample[i] = Random.Shared.Next(20, 80);

        return sample;
    }

    public float[] GenerateSampleWaveData(int count)
    {
        float[] sample = new float[count];
        float last = 50f;
        for (int i = 0; i < sample.Length; i++)
        {
            float target = Random.Shared.Next(20, 80);
            last += (target - last) * 0.3f;
            sample[i] = last;
        }

        return sample;
    }

    public void UpdateWaveData()
    {
        waveData = WaveDataProvider?.Invoke();
        canvas?.Invalidate();
    }
}
