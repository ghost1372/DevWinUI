namespace DevWinUI;

internal partial class ColorBloomTransitionHelper : IDisposable
{
    private UIElement hostForVisual;
    private Compositor _compositor;
    private ContainerVisual _containerForVisuals;
    private ScalarKeyFrameAnimation? _bloomAnimation;
    private ManagedSurface _circleMaskSurface;

    public ColorBloomTransitionHelper(UIElement hostForVisual)
    {
        this.hostForVisual = hostForVisual;

        var visual = ElementCompositionPreview.GetElementVisual(hostForVisual);
        _compositor = visual.Compositor;

        _containerForVisuals = _compositor.CreateContainerVisual();
        ElementCompositionPreview.SetElementChildVisual(hostForVisual, _containerForVisuals);

        _circleMaskSurface = ImageLoader.Instance.LoadCircle(200, Microsoft.UI.Colors.White);
    }

    public delegate void ColorBloomTransitionCompletedEventHandler(object sender, EventArgs e);

    public event ColorBloomTransitionCompletedEventHandler? ColorBloomTransitionCompleted;

    public void Start(Windows.UI.Color color, Rect initialBounds, Rect finalBounds)
    {
        var colorVisual = CreateVisualWithColorAndPosition(color, initialBounds, finalBounds);

        _containerForVisuals.Children.InsertAtTop(colorVisual);

        TriggerBloomAnimation(colorVisual);
    }

    public void Dispose()
    {
        _circleMaskSurface.Dispose();
    }

    private SpriteVisual CreateVisualWithColorAndPosition(Windows.UI.Color color,
                                                          Windows.Foundation.Rect initialBounds,
                                                          Windows.Foundation.Rect finalBounds)
    {
        var width = (float)initialBounds.Width;
        var height = (float)initialBounds.Height;
        var positionX = initialBounds.X;
        var positionY = initialBounds.Y;

        var circleColorVisualDiameter = (float)Math.Min(width, height);

        if (_bloomAnimation == null)
            InitializeBloomAnimation(circleColorVisualDiameter / 2, finalBounds);

        var diagonal = Math.Sqrt(2 * (circleColorVisualDiameter * circleColorVisualDiameter));
        var deltaForOffset = (diagonal - circleColorVisualDiameter) / 2;

        var offset = new Vector3((float)positionX + (float)deltaForOffset + circleColorVisualDiameter / 2,
                                 (float)positionY + circleColorVisualDiameter / 2,
                                 0f);
        var size = new Vector2(circleColorVisualDiameter);

        SpriteVisual coloredCircleVisual = _compositor.CreateSpriteVisual();
        coloredCircleVisual.Brush = CreateCircleBrushWithColor(color);
        coloredCircleVisual.Offset = offset;
        coloredCircleVisual.Size = size;

        coloredCircleVisual.AnchorPoint = new Vector2(0.5f, 0.5f);

        return coloredCircleVisual;
    }

    private CompositionEffectBrush CreateCircleBrushWithColor(Windows.UI.Color color)
    {
        var colorBrush = _compositor.CreateColorBrush(color);

        var effect = new CompositeEffect
        {
            Mode = CanvasComposite.DestinationIn,
            Sources =
                {
                    new ColorSourceEffect()
                    {
                        Color = color
                    },
                    new CompositionEffectSourceParameter("mask")
                }
        };
        var factory = _compositor.CreateEffectFactory(effect);
        var brush = factory.CreateBrush();

        brush.SetSourceParameter("mask", _circleMaskSurface.Brush);

        return brush;
    }

    private void InitializeBloomAnimation(float initialRadius, Rect finalBounds)
    {
        var maxWidth = finalBounds.Width;
        var maxHeight = finalBounds.Height;

        var maxRadius = (float)Math.Sqrt((maxWidth * maxWidth) + (maxHeight * maxHeight));

        var scaleFactor = (float)Math.Round(maxRadius / initialRadius, MidpointRounding.AwayFromZero);

        var bloomEase = _compositor.CreateCubicBezierEasingFunction(
                new Vector2(0.1f, 0.4f),
                new Vector2(0.99f, 0.65f)
            );
        _bloomAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _bloomAnimation.InsertKeyFrame(1.0f, scaleFactor, bloomEase);
        _bloomAnimation.Duration = TimeSpan.FromMilliseconds(800);
    }

    private void TriggerBloomAnimation(SpriteVisual colorVisual)
    {
        if (_bloomAnimation == null)
            return;

        var batchTransaction = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        colorVisual.StartAnimation("Scale.X", _bloomAnimation);
        colorVisual.StartAnimation("Scale.Y", _bloomAnimation);

        batchTransaction.Completed += (sender, args) =>
        {
            _containerForVisuals.Children.Remove(colorVisual);

            ColorBloomTransitionCompleted?.Invoke(this, EventArgs.Empty);
        };

        batchTransaction.End();
    }
}
