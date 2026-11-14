using System.Collections.Specialized;

namespace DevWinUI;

public sealed partial class SpeedGraph2 : Control
{
    private const string successColor = "SystemFillColorSuccess";
    private const string criticalColor = "SystemFillColorCritical";
    private const string cautionColor = "SystemFillColorCaution";
    private string customColor = "SystemAccentColor";

    private GraphState graphState = GraphState.Normal;
    public ObservableCollection<Vector2> Points
    {
        get { return (ObservableCollection<Vector2>)GetValue(PointsProperty); }
        set { SetValue(PointsProperty, value); }
    }

    public static readonly DependencyProperty PointsProperty =
        DependencyProperty.Register(nameof(Points), typeof(ObservableCollection<Vector2>), typeof(SpeedGraph2), new PropertyMetadata(null, OnPointsPropertyChanged));

    public GraphState State
    {
        get => (GraphState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }

    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(nameof(State), typeof(GraphState), typeof(SpeedGraph2), new PropertyMetadata(GraphState.Normal, OnStatePropertyChanged));

    private static void OnStatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpeedGraph2)d;
        ctl.graphState = (GraphState)e.NewValue;
        ctl.ApplyGraphState();
    }

    public Color? NormalColor
    {
        get => (Color?)GetValue(NormalColorProperty);
        set => SetValue(NormalColorProperty, value);
    }

    public static readonly DependencyProperty NormalColorProperty =
        DependencyProperty.Register(nameof(NormalColor), typeof(Color?), typeof(SpeedGraph2), new PropertyMetadata(null, static (d, e) => ((SpeedGraph2)d).ApplyGraphState()));

    public Color? WarningColor
    {
        get => (Color?)GetValue(WarningColorProperty);
        set => SetValue(WarningColorProperty, value);
    }

    public static readonly DependencyProperty WarningColorProperty =
        DependencyProperty.Register(nameof(WarningColor), typeof(Color?), typeof(SpeedGraph2), new PropertyMetadata(null, static (d, e) => ((SpeedGraph2)d).ApplyGraphState()));

    public Color? SuccessColor
    {
        get => (Color?)GetValue(SuccessColorProperty);
        set => SetValue(SuccessColorProperty, value);
    }

    public static readonly DependencyProperty SuccessColorProperty =
        DependencyProperty.Register(nameof(SuccessColor), typeof(Color?), typeof(SpeedGraph2), new PropertyMetadata(null, static (d, e) => ((SpeedGraph2)d).ApplyGraphState()));

    public Color? ErrorColor
    {
        get => (Color?)GetValue(ErrorColorProperty);
        set => SetValue(ErrorColorProperty, value);
    }

    public static readonly DependencyProperty ErrorColorProperty =
        DependencyProperty.Register(nameof(ErrorColor), typeof(Color?), typeof(SpeedGraph2), new PropertyMetadata(null, static (d, e) => ((SpeedGraph2)d).ApplyGraphState()));

    public Color? CustomColor
    {
        get => (Color?)GetValue(CustomColorProperty);
        set => SetValue(CustomColorProperty, value);
    }

    public static readonly DependencyProperty CustomColorProperty =
        DependencyProperty.Register(nameof(CustomColor), typeof(Color?), typeof(SpeedGraph2), new PropertyMetadata(null, static (d, e) => ((SpeedGraph2)d).ApplyGraphState()));

    private static void OnPointsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpeedGraph2)d;
        ctl.highestValue = 0;

        if (e.OldValue is ObservableCollection<Vector2> oldPoints)
        {
            oldPoints.CollectionChanged -= ctl.OnPointsChanged;
        }

        if (ctl.isAttached && e.NewValue is ObservableCollection<Vector2> newPoints)
        {
            newPoints.CollectionChanged += ctl.OnPointsChanged;
        }

        ctl.UpdateGraph();
    }

    Compositor compositor;

    ContainerVisual rootVisual;
    RectangleClip rootClip;
    SpriteVisual container;
    ShapeVisual graphVisual;

    CompositionPathGeometry graphGeometry;
    InsetClip graphClip;

    SpriteVisual line;

    CompositionColorBrush backgroundBrush;
    CompositionColorGradientStop graphFillBottom;
    CompositionColorGradientStop graphFillTop;
    CompositionColorBrush graphStrokeBrush;

    LinearEasingFunction linearEasing;
    ScalarKeyFrameAnimation lineAnim;
    ScalarKeyFrameAnimation clipAnim;

    bool initialized;
    bool isAttached;

    float width;
    float height;

    float highestValue;

    public SpeedGraph2()
    {
        compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        Points = new ObservableCollection<Vector2>();
        this.SizeChanged += OnSizeChanged;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.NewSize.Width <= 0 || e.NewSize.Height <= 0)
            return;

        if (initialized)
        {
            ResizeGraph();
            return;
        }

        InitGraph();

        this.Loaded -= OnLoaded;
        this.Loaded += OnLoaded;
        AttachHandlers();

        UpdateGraph();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        AttachHandlers();

        ApplyGraphState();

        UpdateGraph();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        isAttached = false;

        this.Unloaded -= OnUnloaded;
        this.ActualThemeChanged -= OnThemeChanged;

        var points = Points;
        if (points != null)
        {
            points.CollectionChanged -= OnPointsChanged;
        }
    }

    private void AttachHandlers()
    {
        if (isAttached)
            return;

        isAttached = true;

        this.Unloaded += OnUnloaded;
        this.ActualThemeChanged += OnThemeChanged;

        var points = Points;
        if (points != null)
        {
            points.CollectionChanged += OnPointsChanged;
        }
    }

    private void OnPointsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateGraph();
    }

    private void OnAppThemeModeChanged(object? sender, EventArgs e)
    {
        ApplyGraphState();
    }

    private void OnThemeChanged(FrameworkElement sender, object args)
    {
        ApplyGraphState();
    }

    private void ApplyGraphState()
    {
        var resources = Application.Current.Resources;

        SetGraphColors(graphState switch
        {
            GraphState.Warning => WarningColor ?? resources[cautionColor] as Color?,
            GraphState.Success => SuccessColor ?? resources[successColor] as Color?,
            GraphState.Error => ErrorColor ?? resources[criticalColor] as Color?,
            GraphState.Custom => CustomColor ?? resources[customColor] as Color?,
            _ => NormalColor
        } ?? GetNormalAccentColor());
    }

    private void InitGraph()
    {
        rootVisual = compositor.CreateContainerVisual();
        rootVisual.Size = this.ActualSize;
        ElementCompositionPreview.SetElementChildVisual(this, rootVisual);

        var size = rootVisual.Size;
        width = size.X;
        height = size.Y;

        rootClip = compositor.CreateRectangleClip();
        rootClip.Top = 1.5f;
        rootClip.Left = 1.5f;
        rootClip.Bottom = height - 1.5f;
        rootClip.Right = width - 2f;
        rootClip.TopLeftRadius = new(4f);
        rootClip.TopRightRadius = new(4f);
        rootClip.BottomLeftRadius = new(4f);
        rootClip.BottomRightRadius = new(4f);
        rootVisual.Clip = rootClip;

        backgroundBrush = compositor.CreateColorBrush();

        var graphFillBrush = compositor.CreateLinearGradientBrush();
        graphFillBrush.StartPoint = new(0.5f, 0f);
        graphFillBrush.EndPoint = new(0.5f, 1f);
        graphFillTop = compositor.CreateColorGradientStop();
        graphFillTop.Offset = 0f;
        graphFillBottom = compositor.CreateColorGradientStop();
        graphFillBottom.Offset = 1f;
        graphFillBrush.ColorStops.Add(graphFillBottom);
        graphFillBrush.ColorStops.Add(graphFillTop);

        graphStrokeBrush = compositor.CreateColorBrush();

        ApplyGraphState();

        container = compositor.CreateSpriteVisual();
        container.Size = rootVisual.Size;
        container.Brush = backgroundBrush;

        graphVisual = compositor.CreateShapeVisual();
        graphVisual.Size = rootVisual.Size;
        var graphShape = compositor.CreateSpriteShape();
        graphShape.FillBrush = graphFillBrush;
        graphShape.StrokeBrush = graphStrokeBrush;
        graphShape.StrokeThickness = 1f;

        graphGeometry = compositor.CreatePathGeometry();
        graphShape.Geometry = graphGeometry;

        graphVisual.Shapes.Add(graphShape);

        container.Children.InsertAtBottom(graphVisual);

        graphClip = compositor.CreateInsetClip();
        graphClip.RightInset = width;
        container.Clip = graphClip;

        rootVisual.Children.InsertAtBottom(container);

        line = compositor.CreateSpriteVisual();
        line.Size = new(width, 1.5f);
        line.Brush = graphStrokeBrush;
        line.Offset = new(0f, height - 4f, 0);
        rootVisual.Children.InsertAtTop(line);

        highestValue = 0;

        linearEasing = compositor.CreateLinearEasingFunction();

        lineAnim = compositor.CreateScalarKeyFrameAnimation();
        lineAnim.Duration = TimeSpan.FromMilliseconds(72);

        clipAnim = compositor.CreateScalarKeyFrameAnimation();
        clipAnim.Duration = TimeSpan.FromMilliseconds(72);

        initialized = true;
    }

    private void ResizeGraph()
    {
        var size = this.ActualSize;
        width = size.X;
        height = size.Y;

        rootVisual.Size = size;
        rootClip.Bottom = height - 1.5f;
        rootClip.Right = width - 2f;
        container.Size = size;
        graphVisual.Size = size;
        line.Size = new(width, 1.5f);

        UpdateGraph();
    }

    float YValue(float y) => highestValue > 0f ? height - (y / highestValue) * Math.Max(height - 48f, 0f) - 4 : height - 4;

    void UpdateGraph()
    {
        if (!initialized)
            return;

        var points = Points;
        if (points == null || points.Count == 0)
        {
            highestValue = 0;
            graphVisual.IsVisible = false;

            line.StopAnimation("Offset.Y");
            line.Offset = new(0f, height - 4f, 0);

            graphClip.StopAnimation("RightInset");
            graphClip.RightInset = width;
            return;
        }

        graphVisual.IsVisible = true;

        var last = points[points.Count - 1];

        graphVisual.IsVisible = true;
        graphGeometry.Path = CreatePathFromPoints(points);

        lineAnim.InsertKeyFrame(1f, YValue(last.Y), linearEasing);
        line.StartAnimation("Offset.Y", lineAnim);

        clipAnim.InsertKeyFrame(1f, width - (width * last.X / 100f) - 1, linearEasing);
        graphClip.StartAnimation("RightInset", clipAnim);
    }

    CompositionPath CreatePathFromPoints(ObservableCollection<Vector2> points)
    {
        int count = points.Count;
        for (int i = 0; i < count; i++)
        {
            if (points[i].Y > highestValue)
                highestValue = points[i].Y;
        }

        using var pathBuilder = new CanvasPathBuilder(null);
        pathBuilder.BeginFigure(0f, height);
        for (int i = 0; i < count; i++)
        {
            var point = points[i];
            pathBuilder.AddLine(width * point.X / 100f, YValue(point.Y));
        }
        var last = points[count - 1];
        float lastX = width * last.X / 100f + 2;
        float lastY = YValue(last.Y);
        pathBuilder.AddLine(lastX, lastY);
        pathBuilder.AddLine(lastX, height);
        pathBuilder.EndFigure(CanvasFigureLoop.Closed);
        using var geometry = CanvasGeometry.CreatePath(pathBuilder);
        return new CompositionPath(geometry);
    }

    public void SetGraphNormal()
    {
        graphState = GraphState.Normal;
        ApplyGraphState();
    }
    public void SetGraphSuccess()
    {
        graphState = GraphState.Success;
        ApplyGraphState();
    }
    public void SetGraphError()
    {
        graphState = GraphState.Error;
        ApplyGraphState();
    }
    public void SetGraphWarning()
    {
        graphState = GraphState.Warning;
        ApplyGraphState();
    }
    public void SetGraphColor(string colorResourceKey)
    {
        customColor = colorResourceKey;
        graphState = GraphState.Custom;
        ApplyGraphState();
    }

    private Color GetNormalAccentColor()
    {
        return ActualTheme switch
        {
            ElementTheme.Light => (Color)Application.Current.Resources["SystemAccentColorDark1"],
            ElementTheme.Dark => (Color)Application.Current.Resources["SystemAccentColorLight2"],
            ElementTheme.Default or _ => (Application.Current.Resources["AccentFillColorDefaultBrush"] as SolidColorBrush)!.Color
        };
    }

    private void SetGraphColors(Color accentColor)
    {
        if (backgroundBrush == null)
            return;

        var veryLightColor = accentColor with { A = 0x0f };

        var slightlyDarkerColor = this.ActualTheme switch
        {
            ElementTheme.Light => accentColor with { A = 0x55 },
            _ => accentColor with { A = 0x7f }
        };

        backgroundBrush.Color = veryLightColor;

        graphFillTop.Color = slightlyDarkerColor;
        graphFillBottom.Color = veryLightColor;

        graphStrokeBrush.Color = accentColor;
    }
}
public enum GraphState
{
    Normal,
    Warning,
    Success,
    Error,
    Custom
}
