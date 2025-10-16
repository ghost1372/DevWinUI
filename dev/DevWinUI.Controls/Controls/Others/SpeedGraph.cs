using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_TextTranslate),  Type = typeof(TranslateTransform))]
[TemplatePart(Name = nameof(PART_LineTranslate),  Type = typeof(TranslateTransform))]
[TemplatePart(Name = nameof(PART_GraphScale),  Type = typeof(ScaleTransform))]
[TemplatePart(Name = nameof(PART_PolygonShape),  Type = typeof(Polygon))]
[TemplatePart(Name = nameof(PART_CanvasControl),  Type = typeof(CanvasControl))]
public partial class SpeedGraph : Control
{
    private const string PART_CanvasControl = "PART_CanvasControl";
    private const string PART_PolygonShape = "PART_PolygonShape";
    private const string PART_GraphScale = "PART_GraphScale";
    private const string PART_LineTranslate = "PART_LineTranslate";
    private const string PART_TextTranslate = "PART_TextTranslate";

    private CanvasControl canvas;
    private Polygon polygonShape;
    private ScaleTransform graphScale;
    private TranslateTransform lineTranslate;
    private TranslateTransform textTranslate;

    private Size m_graphSize;
    private double m_ratio = 1.0;
    private DateTime m_start = DateTime.MinValue;

    public ulong MaxSpeed
    {
        get => (ulong)GetValue(MaxSpeedProperty);
        set => SetValue(MaxSpeedProperty, value);
    }

    public static readonly DependencyProperty MaxSpeedProperty =
        DependencyProperty.Register(nameof(MaxSpeed), typeof(ulong), typeof(SpeedGraph), new PropertyMetadata(1024UL));

    public ulong Total
    {
        get { return (ulong)GetValue(TotalProperty); }
        set { SetValue(TotalProperty, value); }
    }

    public static readonly DependencyProperty TotalProperty =
        DependencyProperty.Register(nameof(Total), typeof(ulong), typeof(SpeedGraph), new PropertyMetadata(0UL));

    public int BackgroundCircleDistance
    {
        get => (int)GetValue(BackgroundCircleDistanceProperty);
        set => SetValue(BackgroundCircleDistanceProperty, value);
    }
    public static readonly DependencyProperty BackgroundCircleDistanceProperty =
        DependencyProperty.Register(nameof(BackgroundCircleDistance), typeof(int), typeof(SpeedGraph), new PropertyMetadata(6));

    public Visibility SpeedLineVisibility
    {
        get { return (Visibility)GetValue(SpeedLineVisibilityProperty); }
        set { SetValue(SpeedLineVisibilityProperty, value); }
    }

    public static readonly DependencyProperty SpeedLineVisibilityProperty =
        DependencyProperty.Register(nameof(SpeedLineVisibility), typeof(Visibility), typeof(SpeedGraph), new PropertyMetadata(Visibility.Visible));

    public Visibility SpeedTextVisibility
    {
        get { return (Visibility)GetValue(SpeedTextVisibilityProperty); }
        set { SetValue(SpeedTextVisibilityProperty, value); }
    }

    public static readonly DependencyProperty SpeedTextVisibilityProperty =
        DependencyProperty.Register(nameof(SpeedTextVisibility), typeof(Visibility), typeof(SpeedGraph), new PropertyMetadata(Visibility.Visible));

    public string SpeedText
    {
        get { return (string)GetValue(SpeedTextProperty); }
        set { SetValue(SpeedTextProperty, value); }
    }

    public static readonly DependencyProperty SpeedTextProperty =
        DependencyProperty.Register(nameof(SpeedText), typeof(string), typeof(SpeedGraph), new PropertyMetadata("20.0 MB/s"));

    public bool AutoUpdateSpeedText
    {
        get { return (bool)GetValue(AutoUpdateSpeedTextProperty); }
        set { SetValue(AutoUpdateSpeedTextProperty, value); }
    }

    public static readonly DependencyProperty AutoUpdateSpeedTextProperty =
        DependencyProperty.Register(nameof(AutoUpdateSpeedText), typeof(bool), typeof(SpeedGraph), new PropertyMetadata(true));

    public SpeedGraph()
    {
        this.DefaultStyleKey = typeof(SpeedGraph);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        canvas = GetTemplateChild(PART_CanvasControl) as CanvasControl;
        polygonShape = GetTemplateChild(PART_PolygonShape) as Polygon;
        graphScale = GetTemplateChild(PART_GraphScale) as ScaleTransform;
        lineTranslate = GetTemplateChild(PART_LineTranslate) as TranslateTransform;
        textTranslate = GetTemplateChild(PART_TextTranslate) as TranslateTransform;

        canvas.Draw -= OnCanvasDraw;
        canvas.Draw += OnCanvasDraw;

        this.SizeChanged -= OnSizeChanged;
        this.SizeChanged += OnSizeChanged;
    }

    public void AddPoint(ulong progressBytes, ulong speed)
    {
        if (polygonShape == null || graphScale == null)
            return;

        var x = GetX(progressBytes);
        var points = polygonShape.Points;
        var count = points.Count;

        if (count == 0)
        {
            points.Add(new Point(0, ActualHeight));
            count++;
        }

        if (MaxSpeed == 0)
            MaxSpeed = speed;

        if (count == 1)
        {
            points.Add(new Point(x, GetY(speed)));
        }
        else
        {
            if (count == 2)
                points.Add(new Point(x, GetY(speed)));
            else
                points[count - 1] = new Point(x, GetY(speed));

            points.Add(new Point(x, m_graphSize.Height));
        }

        if (MaxSpeed < speed)
        {
            ResizeGraphPoint((float)MaxSpeed / speed);
            MaxSpeed = speed;
        }

        if (m_start == DateTime.MinValue)
            m_start = DateTime.Now;

        if (count > 2)
            MakeAnimation();
    }

    public void SetSpeed(ulong speed)
    {
        if (graphScale == null)
            return;

        // too large or invalid
        if (speed >= 1024UL * 1024 * 1024 * 1024)
            return;

        if (MaxSpeed < speed)
        {
            ResizeGraphPoint((float)MaxSpeed / speed);
            MaxSpeed = speed;
        }

        MakeAnimation((float)(m_graphSize.Height * (1.0f - (float)speed / MaxSpeed)));
    }

    public void SetSpeed(double percent, ulong speed)
    {
        if (polygonShape == null)
            return;

        float x = (float)(percent / 100.0 * m_graphSize.Width);
        var points = polygonShape.Points;
        var count = points.Count;

        if (count == 0)
        {
            points.Add(new Point(0, ActualHeight));
            count++;
        }

        if (MaxSpeed == 0)
            MaxSpeed = speed;

        if (count == 1)
        {
            points.Add(new Point(x, GetY(speed)));
        }
        else
        {
            if (count == 2)
                points.Add(new Point(x, GetY(speed)));
            else
                points[count - 1] = new Point(x, GetY(speed));

            points.Add(new Point(x, m_graphSize.Height));
        }

        if (MaxSpeed < speed)
        {
            ResizeGraphPoint((float)MaxSpeed / speed);
            MaxSpeed = speed;
        }

        if (m_start == DateTime.MinValue)
            m_start = DateTime.Now;

        if (count > 2)
        {
            if (AutoUpdateSpeedText)
            {
                SpeedText = GetFormattedSpeed(speed);
            }
            MakeAnimation();
        }
    }

    private float GetX(ulong progressBytes)
    {
        return (float)(m_graphSize.Width * ((float)progressBytes / Total));
    }

    private float GetY(ulong speed)
    {
        return (float)(m_graphSize.Height * (1.0f - ((float)speed / MaxSpeed / m_ratio)));
    }

    private void ResizeGraphPoint(float ratio)
    {
        m_ratio *= ratio;

        var scaleDown = new Storyboard();
        var scaleAnim = new DoubleAnimation
        {
            To = m_ratio,
            Duration = new Duration(TimeSpan.FromMilliseconds(300)),
            EasingFunction = new ExponentialEase
            {
                EasingMode = EasingMode.EaseInOut,
                Exponent = 6
            }
        };

        Storyboard.SetTarget(scaleAnim, graphScale);
        Storyboard.SetTargetProperty(scaleAnim, "ScaleY");
        scaleDown.Children.Add(scaleAnim);
        scaleDown.Begin();
    }

    private void MakeAnimation(float y)
    {
        var duration = TimeSpan.FromMilliseconds(300);

        var storyboard = new Storyboard();

        var lineTranslateAnim = new DoubleAnimation
        {
            To = y,
            Duration = new Duration(duration),
            EasingFunction = new ExponentialEase
            {
                Exponent = 7,
                EasingMode = EasingMode.EaseOut
            }
        };

        var textTranslateAnim = new DoubleAnimation
        {
            To = y,
            Duration = new Duration(duration),
            EasingFunction = new ExponentialEase
            {
                Exponent = 7,
                EasingMode = EasingMode.EaseOut
            }
        };

        Storyboard.SetTarget(lineTranslateAnim, lineTranslate);
        Storyboard.SetTargetProperty(lineTranslateAnim, "Y");

        Storyboard.SetTarget(textTranslateAnim, textTranslate);
        Storyboard.SetTargetProperty(textTranslateAnim, "Y");

        storyboard.Children.Add(lineTranslateAnim);
        storyboard.Children.Add(textTranslateAnim);

        storyboard.Begin();
    }

    private void MakeAnimation()
    {
        var points = polygonShape.Points;
        var p = points[points.Count - 2];
        var y = m_graphSize.Height - ((m_graphSize.Height - p.Y) * m_ratio);
        MakeAnimation((float)y);
    }

    private void OnCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var drawingSession = args.DrawingSession;
        double width = sender.ActualWidth;
        double height = sender.ActualHeight;
        var color = Color.FromArgb(32, 255, 255, 255);

        for (int i = 0; i < width; i += BackgroundCircleDistance)
        {
            for (int j = 0; j < height; j += BackgroundCircleDistance)
            {
                drawingSession.FillCircle(i, j, 1, color);
            }
        }
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        var newSize = e.NewSize;
        if (m_graphSize != newSize && newSize.Width != 0 && newSize.Height != 0)
        {
            m_graphSize = newSize;

            // You can rescale existing points here if needed
            var points = polygonShape.Points;
            var pointsCount = points.Count;
            for (int i = 0; i < pointsCount; ++i)
            {
                // adjust x if necessary
            }
        }
    }
    public void ResetGraph()
    {
        // Clear all points
        polygonShape?.Points.Clear();

        // Reset scale
        if (graphScale != null)
            graphScale.ScaleY = 1.0;

        // Reset line and text transforms
        if (lineTranslate != null)
            lineTranslate.Y = 0;
        if (textTranslate != null)
            textTranslate.Y = 0;

        // Reset internal state
        MaxSpeed = 1024;
        m_ratio = 1.0;
        m_start = DateTime.MinValue;
    }

    public void Pause()
    {
        VisualStateManager.GoToState(this, "PauseState", false);
    }

    public void Error()
    {
        VisualStateManager.GoToState(this, "ErrorState", false);
    }

    public void Normal()
    {
        VisualStateManager.GoToState(this, "Normal", false);
    }

    private string GetFormattedSpeed(double bytes)
    {
        if (bytes == 0)
            return "---";

        return $"{FileHelper.GetFileSize((long)bytes)}/s";
    }
}
