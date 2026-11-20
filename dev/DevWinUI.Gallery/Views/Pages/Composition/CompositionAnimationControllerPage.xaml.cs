using System.Numerics;
using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;

namespace DevWinUIGallery.Views;

public sealed partial class CompositionAnimationControllerPage : Page
{
    private readonly CompositionAnimationController controller;
    public CompositionAnimationControllerPage()
    {
        InitializeComponent();
        controller = AnimationSetup();
    }
    private CompositionAnimationController AnimationSetup()
    {
        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        var visual = ElementCompositionPreview.GetElementVisual(Rectangle);

        int animationDuration = 4;
        float animationMax = 250f;
        var linear = compositor.CreateLinearEasingFunction();

        var animation = compositor.CreateVector3KeyFrameAnimation();
        animation.InsertKeyFrame(0.25f, new Vector3(animationMax, (float)Canvas.GetTop(Rectangle), 0f), linear);
        animation.InsertKeyFrame(0.5f, new Vector3(animationMax, animationMax, 0f), linear);
        animation.InsertKeyFrame(0.75f, new Vector3((float)Canvas.GetLeft(Rectangle), animationMax, 0f), linear);
        animation.InsertKeyFrame(1f, new Vector3((float)Canvas.GetLeft(Rectangle), (float)Canvas.GetTop(Rectangle), 0f), linear);
        animation.Duration = TimeSpan.FromSeconds(animationDuration);
        animation.IterationBehavior = AnimationIterationBehavior.Forever;

        // Plug it into the controller
        var animationController = new CompositionAnimationController(visual, nameof(Visual.Offset), animation);
        animationController.AttachSlider(slider);
        animationController.PlayIcon = PlayIcon;
        return animationController;
    }

    private void PlayPause_Animation(object sender, RoutedEventArgs e)
    {
        controller.PlayPause();
    }
    private void Stop_Animation(object sender, RoutedEventArgs e)
    {
        controller.Stop();

    }
    private void SpeedUp_Animation(object sender, RoutedEventArgs e)
    {
        controller.SpeedUp();

    }
    private void SlowDown_Animation(object sender, RoutedEventArgs e)
    {
        controller.SlowDown();

    }
    private void Reverse_Animation(object sender, RoutedEventArgs e)
    {
        controller.Reverse();
    }
}
