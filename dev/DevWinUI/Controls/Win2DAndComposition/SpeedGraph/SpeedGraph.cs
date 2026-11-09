using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(CanvasControl))]
[TemplatePart(Name = nameof(PART_SpeedGraphNoDataAvailableText), Type = typeof(TextBlock))]
[TemplatePart(Name = nameof(PART_GraphGrid), Type = typeof(Grid))]
[TemplatePart(Name = nameof(PART_HostGrid), Type = typeof(Grid))]
[TemplatePart(Name = nameof(PART_GraphScale), Type = typeof(ScaleTransform))]
[TemplatePart(Name = nameof(PART_Shape), Type = typeof(Polygon))]
public partial class SpeedGraph : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private const string PART_SpeedGraphNoDataAvailableText = "PART_SpeedGraphNoDataAvailableText";
    private const string PART_GraphGrid = "PART_GraphGrid";
    private const string PART_HostGrid = "PART_HostGrid";
    private const string PART_GraphScale = "PART_GraphScale";
    private const string PART_Shape = "PART_Shape";

    private static readonly string[] speedUnits = { "B/s", "KB/s", "MB/s", "GB/s", "TB/s" };

    private CanvasControl canvas;
    private TextBlock noDataAvailableTextBlock;
    private Grid graphGrid;
    private Grid hostGrid;
    private ScaleTransform graphScale;
    private Polygon polygon;

    private Storyboard scaleStoryboard;
    private Microsoft.UI.Composition.Visual hostVisual;
    private Microsoft.UI.Composition.ScalarKeyFrameAnimation offsetAnimation;
    private Microsoft.UI.Composition.CubicBezierEasingFunction offsetEasing;

    private bool m_hasData;
    private TimeSpan speedLineAndTextAnimationDuration = TimeSpan.FromMilliseconds(300);

    private SpeedGraphData m_graphData;

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        canvas = GetTemplateChild(PART_Canvas) as CanvasControl;
        noDataAvailableTextBlock = GetTemplateChild(PART_SpeedGraphNoDataAvailableText) as TextBlock;
        graphGrid = GetTemplateChild(PART_GraphGrid) as Grid;
        hostGrid = GetTemplateChild(PART_HostGrid) as Grid;
        graphScale = GetTemplateChild(PART_GraphScale) as ScaleTransform;
        polygon = GetTemplateChild(PART_Shape) as Polygon;

        scaleStoryboard = null;
        offsetAnimation = null;
        offsetEasing = null;
        hostVisual = hostGrid != null ? ElementCompositionPreview.GetElementVisual(hostGrid) : null;

        m_hasData = false;
        m_graphData = new SpeedGraphData(polygon);

        if (canvas != null)
        {
            canvas.Draw -= OnCanvasDraw;
            canvas.Draw += OnCanvasDraw;

            canvas.ActualThemeChanged -= OnCanvasActualThemeChanged;
            canvas.ActualThemeChanged += OnCanvasActualThemeChanged;
        }

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        UpdateBackgroundShape();
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (m_graphData == null)
            return;

        var newSize = e.NewSize;
        if (newSize.Width <= 0 || newSize.Height <= 0)
            return;

        float oldWidth = m_graphData.Width;
        float oldHeight = m_graphData.Height;
        m_graphData.NewSize(newSize);

        if (oldWidth <= 0 || oldHeight <= 0)
            return;

        float scaleX = (float)(newSize.Width / oldWidth);
        float scaleY = (float)(newSize.Height / oldHeight);
        if (scaleX == 1.0f && scaleY == 1.0f)
            return;

        var points = GetPoints();
        if (points == null)
            return;

        for (int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            points[i] = new Point(p.X * scaleX, p.Y * scaleY);
        }
    }

    private void OnCanvasActualThemeChanged(FrameworkElement sender, object args)
    {
        ((CanvasControl)sender).Invalidate();
    }

    private void OnCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var ds = args.DrawingSession;

        float width = (float)sender.ActualWidth;
        float height = (float)sender.ActualHeight;
        int step = Math.Max(1, BackgroundShapeDistance);

        Color color = sender.ActualTheme == ElementTheme.Light
            ? Microsoft.UI.ColorHelper.FromArgb(32, 0, 0, 0)
            : Microsoft.UI.ColorHelper.FromArgb(32, 255, 255, 255);

        switch (BackgroundMode)
        {
            case SpeedGraphBackgroundMode.Dot:
                {
                    for (int x = 0; x < width; x += step)
                    {
                        for (int y = 0; y < height; y += step)
                        {
                            ds.FillCircle(x, y, 1, color);
                        }
                    }
                    break;
                }

            case SpeedGraphBackgroundMode.Cross:
                {
                    // vertical lines
                    for (int x = 0; x < width; x += step)
                    {
                        ds.DrawLine(x, 0, x, height, color, 1);
                    }

                    // horizontal lines
                    for (int y = 0; y < height; y += step)
                    {
                        ds.DrawLine(0, y, width, y, color, 1);
                    }

                    break;
                }
        }
    }

    public void SetSpeed(double percent, ulong speed)
    {
        if (m_graphData == null)
            return;

        if (!m_hasData)
        {
            m_graphData.NewSize(new Size(ActualSize.X, ActualSize.Y));
        }

        var result = m_graphData.SetSpeed(percent, speed);
        float newScaleRatio = result.NewScaleRatio;
        bool needAnimation = result.NeedAnimation;

        if (newScaleRatio != 1.0f)
            ResizeGraphPoint(newScaleRatio);

        if (AutoUpdateSpeedText)
        {
            SpeedText = GetSpeedReadable(speed);
        }

        if (needAnimation)
        {
            MakeAnimation();
        }

        if (!m_hasData)
        {
            graphGrid.Visibility = Visibility.Visible;
            noDataAvailableTextBlock.Visibility = Visibility.Collapsed;

            m_hasData = true;
        }
    }

    private Storyboard CreateScaleAnimation()
    {
        var storyboard = new Storyboard();
        var easing = new ExponentialEase();
        easing.EasingMode = EasingMode.EaseInOut;
        easing.Exponent = 6;

        var scaleYAnimation = new DoubleAnimation
        {
            To = m_graphData.GetRatio(),
            Duration = new Duration(speedLineAndTextAnimationDuration),
            EasingFunction = easing
        };

        Storyboard.SetTarget(scaleYAnimation, graphScale);
        Storyboard.SetTargetProperty(scaleYAnimation, "ScaleY");

        storyboard.Children.Add(scaleYAnimation);
        return storyboard;
    }
    private void ResizeGraphPoint(float ratio)
    {
        m_graphData.SetRatio(ratio);
        scaleStoryboard = CreateScaleAnimation();
        scaleStoryboard.Begin();
    }
    private void MakeAnimation(double y)
    {
        if (hostVisual == null)
            return;

        if (offsetAnimation == null)
        {
            var compositor = hostVisual.Compositor;
            offsetEasing = compositor.CreateCubicBezierEasingFunction(new System.Numerics.Vector2(0.0f, 0.0f), new System.Numerics.Vector2(0.2f, 1.0f));
            offsetAnimation = compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.Duration = speedLineAndTextAnimationDuration;
        }

        offsetAnimation.InsertKeyFrame(1.0f, (float)y, offsetEasing);
        hostVisual.StartAnimation("Offset.Y", offsetAnimation);
    }

    private void MakeAnimation()
    {
        var p = m_graphData.GetLastPoint();

        double y = m_graphData.Height -
                   ((m_graphData.Height - p.Y) * m_graphData.GetRatio());

        MakeAnimation(y);
    }

    private void UpdateBackgroundShape()
    {
        canvas?.Invalidate();
    }

    public void NormalGraph()
    {
        VisualStateManager.GoToState(this, "Normal", false);
    }

    public void PauseGraph()
    {
        VisualStateManager.GoToState(this, "Pause", false);
    }

    public void ErrorGraph()
    {
        VisualStateManager.GoToState(this, "Error", false);
    }

    public PointCollection GetPoints()
    {
        return polygon?.Points;
    }

    public void ResetGraph()
    {
        if (m_graphData == null)
            return;

        scaleStoryboard?.Stop();
        graphScale.ScaleY = 1.0;

        if (hostVisual != null)
        {
            hostVisual.StopAnimation("Offset.Y");
            var offset = hostVisual.Offset;
            offset.Y = 0f;
            hostVisual.Offset = offset;
        }

        m_graphData.Reset();

        if (AutoUpdateSpeedText)
        {
            ClearValue(SpeedTextProperty);
        }

        graphGrid.Visibility = Visibility.Collapsed;
        noDataAvailableTextBlock.Visibility = Visibility.Visible;
        m_hasData = false;
    }
    private static string GetSpeedReadable(ulong bytesPerSecond)
    {
        double size = bytesPerSecond;
        int unitIndex = 0;

        while (size >= 1024 && unitIndex < speedUnits.Length - 1)
        {
            size /= 1024;
            unitIndex++;
        }

        return $"{size:0.##} {speedUnits[unitIndex]}";
    }
}
