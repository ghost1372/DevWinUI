using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Shapes;
using System.Diagnostics;

namespace DevWinUI;

[TemplatePart(Name = "PART_Label", Type = typeof(TextBlock))]
[TemplatePart(Name = "PART_Timer", Type = typeof(TextBlock))]
public sealed partial class LoadingState : Control
{
    private const float Dim = 0.18f;
    private const int CellCount = 9;
    private static readonly int[] Ring = { 0, 1, 2, 5, 8, 7, 6, 3 };

    private readonly Rectangle?[] _cells = new Rectangle?[CellCount];

    private DispatcherQueueTimer? _timer;
    private long _startTimestamp = Stopwatch.GetTimestamp();
    private TextBlock? _label;
    private TextBlock? _timerText;
    private Storyboard? _shimmer;
    private bool _loaded;

    public LoadingState()
    {
        DefaultStyleKey = typeof(LoadingState);

        AutomationProperties.SetName(this, Label);

        ActualThemeChanged += OnActualThemeChanged;
        RegisterPropertyChangedCallback(ForegroundProperty, OnForegroundChanged);
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public TimeSpan Elapsed => Stopwatch.GetElapsedTime(_startTimestamp);

    public void Restart()
    {
        _startTimestamp = Stopwatch.GetTimestamp();
        UpdateTimerText();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        StopShimmer();

        _label = GetTemplateChild("PART_Label") as TextBlock;
        _timerText = GetTemplateChild("PART_Timer") as TextBlock;
        for (int i = 0; i < _cells.Length; i++)
        {
            _cells[i] = GetTemplateChild("PART_Cell" + i) as Rectangle;
        }

        if (_label is not null) _label.Text = Label;
        if (_timerText is not null) _timerText.Visibility = ShowElapsed ? Visibility.Visible : Visibility.Collapsed;
        UpdateTimerText();

        BuildGrid();
        if (_loaded) ApplyMotion();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
        Restart();
        UpdateTimer();
        ApplyMotion();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _loaded = false;
        _timer?.Stop();
        StopGrid();
        StopShimmer();
    }

    private void OnActualThemeChanged(FrameworkElement sender, object args)
    {
        if (_loaded) ApplyShimmer();
    }

    private void OnForegroundChanged(DependencyObject sender, DependencyProperty dp)
    {
        if (_loaded) ApplyShimmer();
    }

    private void UpdateTimer()
    {
        if (!_loaded || !ShowElapsed)
        {
            _timer?.Stop();
            return;
        }

        if (_timer is null)
        {
            _timer = DispatcherQueue.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += OnTick;
        }

        _timer.Start();
    }

    private void OnTick(DispatcherQueueTimer sender, object args) => UpdateTimerText();

    private void UpdateTimerText()
    {
        if (_timerText is null) return;

        string text = Format(Elapsed);
        if (_timerText.Text != text) _timerText.Text = text;
    }

    private static string Format(TimeSpan t)
    {
        return t.TotalHours >= 1
            ? $"{(int)t.TotalHours}:{t.Minutes:00}:{t.Seconds:00}"
            : $"{(int)t.TotalMinutes}:{t.Seconds:00}";
    }

    private void ApplyMotion()
    {
        StartGrid();
        ApplyShimmer();
    }

    private void BuildGrid()
    {
        StopGrid();
        double radius = Mode == LoadingStateMode.Dots ? 3 : 1;
        foreach (var cell in _cells)
        {
            if (cell is null) continue;
            cell.RadiusX = radius;
            cell.RadiusY = radius;
        }
    }

    private void StopGrid()
    {
        foreach (var cell in _cells)
        {
            if (cell is null) continue;
            var visual = ElementCompositionPreview.GetElementVisual(cell);
            visual.StopAnimation("Opacity");
            visual.Opacity = Dim;
        }
    }

    private void StartGrid()
    {
        StopGrid();
        if (!_loaded) return;

        bool orbit = Mode == LoadingStateMode.Orbit;
        int cycleMs = orbit ? 1200 : 1400;

        for (int i = 0; i < _cells.Length; i++)
        {
            if (_cells[i] is not { } cell) continue;

            int row = i / 3, col = i % 3;
            int delayMs;
            if (orbit)
            {
                int k = Array.IndexOf(Ring, i);
                if (k < 0) continue;
                delayMs = k * 150;
            }
            else
            {
                delayMs = 100 * (col + Math.Abs(row - 1));
            }

            var visual = ElementCompositionPreview.GetElementVisual(cell);
            var compositor = visual.Compositor;

            var animation = compositor.CreateScalarKeyFrameAnimation();
            animation.InsertKeyFrame(0.00f, Dim);
            animation.InsertKeyFrame(0.12f, 1f, compositor.CreateCubicBezierEasingFunction(new Vector2(0.2f, 0f), new Vector2(0f, 1f)));
            animation.InsertKeyFrame(0.55f, Dim, compositor.CreateCubicBezierEasingFunction(new Vector2(0.4f, 0f), new Vector2(1f, 1f)));
            animation.InsertKeyFrame(1.00f, Dim);
            animation.Duration = TimeSpan.FromMilliseconds(cycleMs);
            animation.DelayTime = TimeSpan.FromMilliseconds(delayMs);
            animation.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
            animation.IterationBehavior = AnimationIterationBehavior.Forever;
            visual.StartAnimation("Opacity", animation);
        }
    }

    private void StopShimmer()
    {
        _shimmer?.Stop();
        _shimmer = null;
        _label?.ClearValue(TextBlock.ForegroundProperty);
    }

    private void ApplyShimmer()
    {
        StopShimmer();
        if (_label is null || !_loaded) return;

        var high = Foreground is SolidColorBrush solid ? solid.Color : Colors.White;
        var low = Windows.UI.Color.FromArgb(0x8C, high.R, high.G, high.B);

        var move = new TranslateTransform();
        var brush = new LinearGradientBrush
        {
            StartPoint = new Point(0, 0),
            EndPoint = new Point(1, 0),
            RelativeTransform = move,
        };
        brush.GradientStops.Add(new GradientStop { Color = low, Offset = 0.00 });
        brush.GradientStops.Add(new GradientStop { Color = low, Offset = 0.35 });
        brush.GradientStops.Add(new GradientStop { Color = high, Offset = 0.50 });
        brush.GradientStops.Add(new GradientStop { Color = low, Offset = 0.65 });
        brush.GradientStops.Add(new GradientStop { Color = low, Offset = 1.00 });
        _label.Foreground = brush;

        var animation = new DoubleAnimation
        {
            From = -1,
            To = 1,
            Duration = new Duration(TimeSpan.FromMilliseconds(1800)),
            RepeatBehavior = RepeatBehavior.Forever,
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(animation, move);
        Storyboard.SetTargetProperty(animation, "X");

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        storyboard.Begin();
        _shimmer = storyboard;
    }
}
