namespace DevWinUI;

internal partial class ColorSlideTransitionHelper
{
    private UIElement hostForVisual;
    private Compositor _compositor;
    private ContainerVisual _containerForVisuals;
    private ScalarKeyFrameAnimation _slideAnimation;
    private bool firstRun = true;

    public ColorSlideTransitionHelper(UIElement hostForVisual)
    {
        this.hostForVisual = hostForVisual;

        var visual = ElementCompositionPreview.GetElementVisual(hostForVisual);
        _compositor = visual.Compositor;

        _containerForVisuals = _compositor.CreateContainerVisual();

        _slideAnimation = CreateSlideAnimation();

        ElementCompositionPreview.SetElementChildVisual(hostForVisual, _containerForVisuals);
    }

    public delegate void ColorSlideTransitionCompletedEventHandler(object? sender, EventArgs e);

    public event ColorSlideTransitionCompletedEventHandler? ColorSlideTransitionCompleted;

    public void Start(Windows.UI.Color color, Rect finalBounds)
    {
        var colorVisual = CreateVisualWithColorAndPosition(color, finalBounds);

        _containerForVisuals.Children.InsertAtTop(colorVisual);

        TriggerSlideAnimation(colorVisual);
    }

    private SpriteVisual CreateVisualWithColorAndPosition(Windows.UI.Color color, Windows.Foundation.Rect finalBounds)
    {
        var offset = new Vector3((float)finalBounds.Right, (float)finalBounds.Top, 0f);
        var size = new Vector2((float)finalBounds.Width, (float)finalBounds.Height);

        SpriteVisual coloredRectangle = _compositor.CreateSpriteVisual();
        coloredRectangle.Brush = _compositor.CreateColorBrush(color);
        coloredRectangle.Offset = offset;
        coloredRectangle.Size = size;

        return coloredRectangle;
    }

    private ScalarKeyFrameAnimation CreateSlideAnimation()
    {
        var animation = _compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(1.0f, 0f);
        animation.Duration = TimeSpan.FromMilliseconds(800);
        return animation;
    }

    private void TriggerSlideAnimation(SpriteVisual colorVisual)
    {
        var batchTransaction = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        colorVisual.StartAnimation("Offset.X", _slideAnimation);

        batchTransaction.Completed += SlideAnimationCompleted;

        batchTransaction.End();
    }

    private void SlideAnimationCompleted(object? sender, CompositionBatchCompletedEventArgs args)
    {
        if (!firstRun)
        {
            foreach (var childVisual in _containerForVisuals.Children)
            {
                _containerForVisuals.Children.Remove(childVisual);
                break;
            }
        }
        else
        {
            firstRun = false;
        }

        ColorSlideTransitionCompleted?.Invoke(this, EventArgs.Empty);
    }
}
