namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Panel), Type = typeof(PerspectiveHost))]
public partial class PerspectiveZoom : ContentControl
{
    private const string PART_Panel = "PART_Panel";

    private PerspectiveHost _panel;
    private FrameworkElement hostedControl;
    private Compositor _compositor;
    private bool _zoomed;

    private Visual _visual;

    public TimeSpan ZoomOutRotationDuration
    {
        get { return (TimeSpan)GetValue(ZoomOutRotationDurationProperty); }
        set { SetValue(ZoomOutRotationDurationProperty, value); }
    }

    public static readonly DependencyProperty ZoomOutRotationDurationProperty =
        DependencyProperty.Register(nameof(ZoomOutRotationDuration), typeof(TimeSpan), typeof(PerspectiveZoom), new PropertyMetadata(TimeSpan.FromMilliseconds(1000)));

    public TimeSpan ZoomOutOffsetDuration
    {
        get { return (TimeSpan)GetValue(ZoomOutOffsetDurationProperty); }
        set { SetValue(ZoomOutOffsetDurationProperty, value); }
    }

    public static readonly DependencyProperty ZoomOutOffsetDurationProperty =
        DependencyProperty.Register(nameof(ZoomOutOffsetDuration), typeof(TimeSpan), typeof(PerspectiveZoom), new PropertyMetadata(TimeSpan.FromMilliseconds(1000)));

    public TimeSpan ZoomInRotationDuration
    {
        get { return (TimeSpan)GetValue(ZoomInRotationDurationProperty); }
        set { SetValue(ZoomInRotationDurationProperty, value); }
    }

    public static readonly DependencyProperty ZoomInRotationDurationProperty =
        DependencyProperty.Register(nameof(ZoomInRotationDuration), typeof(TimeSpan), typeof(PerspectiveZoom), new PropertyMetadata(TimeSpan.FromMilliseconds(1000)));

    public TimeSpan ZoomInOffsetDuration
    {
        get { return (TimeSpan)GetValue(ZoomInOffsetDurationProperty); }
        set { SetValue(ZoomInOffsetDurationProperty, value); }
    }

    public static readonly DependencyProperty ZoomInOffsetDurationProperty =
        DependencyProperty.Register(nameof(ZoomInOffsetDuration), typeof(TimeSpan), typeof(PerspectiveZoom), new PropertyMetadata(TimeSpan.FromMilliseconds(1000)));

    public double ZoomFactor
    {
        get { return (double)GetValue(ZoomFactorProperty); }
        set { SetValue(ZoomFactorProperty, value); }
    }

    public static readonly DependencyProperty ZoomFactorProperty =
        DependencyProperty.Register(nameof(ZoomFactor), typeof(double), typeof(PerspectiveZoom), new PropertyMetadata(0.8));

    public double RotationAngle
    {
        get { return (double)GetValue(RotationAngleProperty); }
        set { SetValue(RotationAngleProperty, value); }
    }

    public static readonly DependencyProperty RotationAngleProperty =
        DependencyProperty.Register(nameof(RotationAngle), typeof(double), typeof(PerspectiveZoom), new PropertyMetadata(45.0));

    protected override void OnContentChanged(object oldContent, object newContent)
    {
        base.OnContentChanged(oldContent, newContent);
        if (newContent != null && newContent is FrameworkElement element)
        {
            hostedControl = element;
            _visual = ElementCompositionPreview.GetElementVisual(element);
            _compositor = ElementCompositionPreview.GetElementVisual(element).Compositor;
            hostedControl.SizeChanged -= PerspectiveZoom_SizeChanged;
            hostedControl.SizeChanged += PerspectiveZoom_SizeChanged;
        }
    }

    public PerspectiveZoom()
    {
        DefaultStyleKey = typeof(PerspectiveZoom);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (Content != null && Content is FrameworkElement element)
        {
            hostedControl = element;
            _visual = ElementCompositionPreview.GetElementVisual(element);
            _compositor = ElementCompositionPreview.GetElementVisual(element).Compositor;

            hostedControl.SizeChanged -= PerspectiveZoom_SizeChanged;
            hostedControl.SizeChanged += PerspectiveZoom_SizeChanged;
        }

        _panel = GetTemplateChild(PART_Panel) as PerspectiveHost;
    }

    private void PerspectiveZoom_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        _panel.PerspectiveDepth = e.NewSize.Width;
    }
    public void ToggleZoom(FrameworkElement targetItem)
    {
        if (_zoomed)
            ZoomOut();
        else
            ZoomIn(targetItem);
    }
    public void ZoomOut()
    {
        if (_compositor == null || _visual == null || !_zoomed)
            return;

        ScalarKeyFrameAnimation rotationAnimation = _compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.InsertKeyFrame(1, 0f);
        rotationAnimation.Duration = ZoomOutRotationDuration;
        _visual.StartAnimation("RotationAngleInDegrees", rotationAnimation);

        Vector3KeyFrameAnimation offsetAnimaton = _compositor.CreateVector3KeyFrameAnimation();
        offsetAnimaton.InsertKeyFrame(1, new Vector3(0, 0, 0));
        offsetAnimaton.Duration = ZoomOutOffsetDuration;
        _visual.StartAnimation("Offset", offsetAnimaton);

        _zoomed = false;
    }

    public void ZoomIn(FrameworkElement targetItem)
    {
        if (_compositor == null || _visual == null || targetItem == null || _zoomed)
            return;

        GeneralTransform coordinate = targetItem.TransformToVisual(hostedControl);
        Vector2 clickedItemCenterPosition = coordinate.TransformPoint(new Point(0, 0)).ToVector2() +
                                            new Vector2((float)targetItem.ActualWidth / 2, (float)targetItem.ActualHeight / 2);

        Vector2 targetOffset = new Vector2((float)hostedControl.ActualWidth / 2, (float)hostedControl.ActualHeight / 2) - clickedItemCenterPosition;

        _visual.Size = new Vector2((float)hostedControl.ActualWidth, (float)hostedControl.ActualHeight);
        _visual.CenterPoint = new Vector3(_visual.Size.X / 2, _visual.Size.Y / 2, 0);
        _visual.RotationAxis = new Vector3(0, 1, 0);

        // Kick off the rotation animation
        ScalarKeyFrameAnimation rotationAnimation = _compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.InsertKeyFrame(0, 0);
        rotationAnimation.InsertKeyFrame(1, targetOffset.X > 0 ? -(float)RotationAngle : (float)RotationAngle);
        rotationAnimation.Duration = ZoomInRotationDuration;
        _visual.StartAnimation("RotationAngleInDegrees", rotationAnimation);

        // Calcuate the offset for the point we are zooming towards
        Vector3 zoomedOffset = new Vector3(targetOffset.X, targetOffset.Y, (float)_panel.ActualWidth * (float)ZoomFactor) * (float)ZoomFactor;

        Vector3KeyFrameAnimation offsetAnimaton = _compositor.CreateVector3KeyFrameAnimation();
        offsetAnimaton.InsertKeyFrame(0, new Vector3(0, 0, 0));
        offsetAnimaton.InsertKeyFrame(1, zoomedOffset);
        offsetAnimaton.Duration = ZoomInOffsetDuration;
        _visual.StartAnimation("Offset", offsetAnimaton);

        _zoomed = true;
    }
}

