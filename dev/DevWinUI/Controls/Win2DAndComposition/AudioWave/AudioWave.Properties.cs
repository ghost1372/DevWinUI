namespace DevWinUI;

public partial class AudioWave
{
    public TimeSpan Duration
    {
        get { return (TimeSpan)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(AudioWave), new PropertyMetadata(TimeSpan.FromSeconds(4), OnDurationChanged));

    private static void OnDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is TimeSpan time)
            {
                ctl.duration = time.TotalMilliseconds;
                ctl.elapsedMs = 0;
                ctl.songPos_ = 0;
                ctl.canvas?.Invalidate();
            }
        }
    }

    public double BarWidth
    {
        get { return (double)GetValue(BarWidthProperty); }
        set { SetValue(BarWidthProperty, value); }
    }

    public static readonly DependencyProperty BarWidthProperty =
        DependencyProperty.Register(nameof(BarWidth), typeof(double), typeof(AudioWave), new PropertyMetadata(6.0d, OnBarWidthChanged));
    private static void OnBarWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is double width)
            {
                ctl.barWidth = (float)width;
                ctl.canvas?.Invalidate();
            }
        }
    }
    public double BarSpacing
    {
        get { return (double)GetValue(BarSpacingProperty); }
        set { SetValue(BarSpacingProperty, value); }
    }

    public static readonly DependencyProperty BarSpacingProperty =
        DependencyProperty.Register(nameof(BarSpacing), typeof(double), typeof(AudioWave), new PropertyMetadata(3.0d, OnBarSpacingChanged));
    private static void OnBarSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is double space)
            {
                ctl.barSpacing = (float)space;
                ctl.canvas?.Invalidate();
            }
        }
    }

    public List<CanvasGradientStop> BarBackgroundGradientStops
    {
        get { return (List<CanvasGradientStop>)GetValue(BarBackgroundGradientStopsProperty); }
        set { SetValue(BarBackgroundGradientStopsProperty, value); }
    }

    public static readonly DependencyProperty BarBackgroundGradientStopsProperty =
        DependencyProperty.Register(nameof(BarBackgroundGradientStops), typeof(List<CanvasGradientStop>), typeof(AudioWave), new PropertyMetadata(null, OnBarBackgroundGradientStopsChanged));
    private static void OnBarBackgroundGradientStopsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is List<CanvasGradientStop> stops)
            {
                ctl.barBackgroundGradientStops = stops;
                ctl.canvas?.Invalidate();
            }
        }
    }
    public Color BarBackground
    {
        get { return (Color)GetValue(BarBackgroundProperty); }
        set { SetValue(BarBackgroundProperty, value); }
    }

    public static readonly DependencyProperty BarBackgroundProperty =
        DependencyProperty.Register(nameof(BarBackground), typeof(Color), typeof(AudioWave), new PropertyMetadata(Colors.Gray, OnBarBackgroundChanged));
    private static void OnBarBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is Color color)
            {
                ctl.barBackground = color;
                ctl.canvas?.Invalidate();
            }
        }
    }
    public List<CanvasGradientStop> BarForegroundGradientStops
    {
        get { return (List<CanvasGradientStop>)GetValue(BarForegroundGradientStopsProperty); }
        set { SetValue(BarForegroundGradientStopsProperty, value); }
    }

    public static readonly DependencyProperty BarForegroundGradientStopsProperty =
        DependencyProperty.Register(nameof(BarForegroundGradientStops), typeof(List<CanvasGradientStop>), typeof(AudioWave), new PropertyMetadata(null, OnBarForegroundGradientStopsChanged));
    private static void OnBarForegroundGradientStopsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is List<CanvasGradientStop> stops)
            {
                ctl.barForegroundGradientStops = stops;
                ctl.canvas?.Invalidate();
            }
        }
    }
    public Color BarForeground
    {
        get { return (Color)GetValue(BarForegroundProperty); }
        set { SetValue(BarForegroundProperty, value); }
    }

    public static readonly DependencyProperty BarForegroundProperty =
        DependencyProperty.Register(nameof(BarForeground), typeof(Color), typeof(AudioWave), new PropertyMetadata(Colors.DeepPink, OnBarForegroundChanged));

    private static void OnBarForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is Color color)
            {
                ctl.barForeground = color;
                ctl.canvas?.Invalidate();
            }
        }
    }

    public double BarRadiusX
    {
        get { return (double)GetValue(BarRadiusXProperty); }
        set { SetValue(BarRadiusXProperty, value); }
    }

    public static readonly DependencyProperty BarRadiusXProperty =
        DependencyProperty.Register(nameof(BarRadiusX), typeof(double), typeof(AudioWave), new PropertyMetadata(3.0d, OnBarRadiusXChanged));

    private static void OnBarRadiusXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is double radius)
            {
                ctl.barRadiusX = (float)radius;
                ctl.canvas?.Invalidate();
            }
        }
    }

    public double BarRadiusY
    {
        get { return (double)GetValue(BarRadiusYProperty); }
        set { SetValue(BarRadiusYProperty, value); }
    }

    public static readonly DependencyProperty BarRadiusYProperty =
        DependencyProperty.Register(nameof(BarRadiusY), typeof(double), typeof(AudioWave), new PropertyMetadata(3.0d, OnBarRadiusYChanged));

    private static void OnBarRadiusYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            if (e.NewValue is double radius)
            {
                ctl.barRadiusY = (float)radius;
                ctl.canvas?.Invalidate();
            }
        }
    }

    public int Progress
    {
        get { return (int)GetValue(ProgressProperty); }
        set { SetValue(ProgressProperty, value); }
    }

    public static readonly DependencyProperty ProgressProperty =
        DependencyProperty.Register(nameof(Progress), typeof(int), typeof(AudioWave), new PropertyMetadata(0, OnProgressChanged));

    private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AudioWave)d;
        if (ctl != null)
        {
            ctl.OnProgressChanged();
        }
    }

    public bool CanSelectedByMouse
    {
        get { return (bool)GetValue(CanSelectedByMouseProperty); }
        set { SetValue(CanSelectedByMouseProperty, value); }
    }

    public static readonly DependencyProperty CanSelectedByMouseProperty =
        DependencyProperty.Register(nameof(CanSelectedByMouse), typeof(bool), typeof(AudioWave), new PropertyMetadata(false));
}
