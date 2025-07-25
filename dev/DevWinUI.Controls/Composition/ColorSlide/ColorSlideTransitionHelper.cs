﻿namespace DevWinUI;

/// <summary>
/// A helper class encapsulating the function and visuals for a Color Slide transition animation.
/// </summary>
internal partial class ColorSlideTransitionHelper
{
    private UIElement hostForVisual;
    private Compositor _compositor;
    private ContainerVisual _containerForVisuals;
    private ScalarKeyFrameAnimation _slideAnimation;
    private bool firstRun = true;

    /// <summary>
    /// Creates an instance of the ColorSlideTransitionHelper. 
    /// Any visuals to be later created and animated will be hosted within the specified UIElement.
    /// </summary>
    public ColorSlideTransitionHelper(UIElement hostForVisual)
    {
        this.hostForVisual = hostForVisual;

        // we have an element in the XAML tree that will host our Visuals
        var visual = ElementCompositionPreview.GetElementVisual(hostForVisual);
        _compositor = visual.Compositor;

        // create a container
        // adding children to this container adds them to the live visual tree
        _containerForVisuals = _compositor.CreateContainerVisual();

        InitializeSlideAnimation();


        ElementCompositionPreview.SetElementChildVisual(hostForVisual, _containerForVisuals);
    }

    public delegate void ColorSlideTransitionCompletedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Indicates that the color slide transition has completed.
    /// </summary>
    public event ColorSlideTransitionCompletedEventHandler ColorSlideTransitionCompleted;


    /// <summary>
    /// Starts the color slide transition using the specified color and boundary sizes.
    /// 
    /// The slide is achieved by creating a rectangular solid colored visual whose scale is progressively 
    /// animated to fully flood a given area.
    /// 
    /// <param name="color">Using the specified color</param>
    /// <param name="initialBounds">The transition begins with a visual with these bounds and position</param>  
    /// <param name="finalBounds">The transition ends when the visual has slid to an area of this bounding size</param>
    /// </summary>
    public void Start(Windows.UI.Color color, Rect finalBounds)
    {
        var colorVisual = CreateVisualWithColorAndPosition(color, finalBounds);

        // add our solid colored rectangular visual to the live visual tree via the container
        _containerForVisuals.Children.InsertAtTop(colorVisual);

        // now that we have a visual, let's run the animation 
        TriggerSlideAnimation(colorVisual);
    }

    /// <summary>
    /// Creates a Visual using the specific color and constraints
    /// </summary>
    private SpriteVisual CreateVisualWithColorAndPosition(Windows.UI.Color color,
                                                          Windows.Foundation.Rect finalBounds)
    {

        var offset = new Vector3((float)finalBounds.Right, (float)finalBounds.Top, 0f);
        var size = new Vector2((float)finalBounds.Width, (float)finalBounds.Height);

        SpriteVisual coloredRectangle = _compositor.CreateSpriteVisual();
        coloredRectangle.Brush = _compositor.CreateColorBrush(color);
        coloredRectangle.Offset = offset;
        coloredRectangle.Size = size;

        return coloredRectangle;

    }

    /// <summary>
    /// Creates an animation template for a "color slide" type effect on a rectangular colored visual.
    /// This is a sub-second animation on the Scale property of the visual.
    /// </summary>
    private void InitializeSlideAnimation()
    {
        _slideAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _slideAnimation.InsertKeyFrame(1.0f, 0f);
        _slideAnimation.Duration = TimeSpan.FromMilliseconds(800); // keeping this under a sec to not be obtrusive

    }

    /// <summary>
    /// Runs the animation
    /// </summary>
    private void TriggerSlideAnimation(SpriteVisual colorVisual)
    {

        // animate the Scale of the visual within a scoped batch
        // this gives us transactionality and allows us to do work once the transaction completes
        var batchTransaction = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        // as with all animations on Visuals, these too will run independent of the UI thread
        // so if the UI thread is busy with app code or doing layout on state/page transition,
        // this animation still run uninterruped and glitch free.
        colorVisual.StartAnimation("Offset.X", _slideAnimation);

        batchTransaction.Completed += SlideAnimationCompleted;

        batchTransaction.End();

    }

    /// <summary>
    /// Cleans up after the slide animation has ended
    /// </summary>
    private void SlideAnimationCompleted(object sender, CompositionBatchCompletedEventArgs args)
    {
        if (!firstRun)
        {
            foreach (var childVisual in _containerForVisuals.Children)
            {
                _containerForVisuals.Children.Remove(childVisual);
                break; // we only need to remove the first child
            }
        }
        else
        {
            firstRun = false;
        }

        // notify interested parties
        ColorSlideTransitionCompleted?.Invoke(this, EventArgs.Empty);
    }
}
