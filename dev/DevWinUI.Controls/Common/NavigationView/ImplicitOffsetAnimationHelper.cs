namespace DevWinUI;
public static partial class ImplicitOffsetAnimationHelper
{
    public static readonly DependencyProperty EnableOffsetAnimationProperty =
        DependencyProperty.RegisterAttached(
            "EnableOffsetAnimation",
            typeof(bool),
            typeof(ImplicitOffsetAnimationHelper),
            new PropertyMetadata(false, OnEnableOffsetAnimationChanged));

    public static void SetEnableOffsetAnimation(UIElement element, bool value)
        => element.SetValue(EnableOffsetAnimationProperty, value);

    public static bool GetEnableOffsetAnimation(UIElement element)
        => (bool)element.GetValue(EnableOffsetAnimationProperty);

    private static void OnEnableOffsetAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element && (bool)e.NewValue)
        {
            element.Loaded += (s, args) =>
            {
                var visual = ElementCompositionPreview.GetElementVisual(element);
                var compositor = visual.Compositor;

                var animation = compositor.CreateVector3KeyFrameAnimation();
                animation.Target = "Offset";
                animation.Duration = TimeSpan.FromMilliseconds(400);
                animation.InsertExpressionKeyFrame(1f, "this.FinalValue");

                var implicitAnimations = compositor.CreateImplicitAnimationCollection();
                implicitAnimations["Offset"] = animation;

                visual.ImplicitAnimations = implicitAnimations;
            };
        }
    }
}
