namespace DevWinUI;

internal static partial class ReasoningFx
{
    static readonly Vector2 EaseStart = new(0.2f, 0f);
    static readonly Vector2 EaseEnd = new(0f, 1f);
    static readonly TimeSpan ChevronDuration = TimeSpan.FromMilliseconds(200);
    static readonly TimeSpan PopDuration = TimeSpan.FromMilliseconds(260);

    internal static bool Motion => ReasoningOptions.IsAnimationEnabled;

    internal static void FadeUp(UIElement element, int delayMs, int durationMs = 220, float dy = 6f)
    {
        ElementCompositionPreview.SetIsTranslationEnabled(element, true);
        var visual = ElementCompositionPreview.GetElementVisual(element);
        visual.StopAnimation("Opacity");
        visual.StopAnimation("Translation");

        if (!Motion)
        {
            visual.Opacity = 1;
            visual.Properties.InsertVector3("Translation", Vector3.Zero);
            return;
        }

        var compositor = visual.Compositor;
        var ease = compositor.CreateCubicBezierEasingFunction(EaseStart, EaseEnd);
        var duration = TimeSpan.FromMilliseconds(durationMs);
        var delay = TimeSpan.FromMilliseconds(delayMs);

        var fade = compositor.CreateScalarKeyFrameAnimation();
        fade.InsertKeyFrame(0f, 0f);
        fade.InsertKeyFrame(1f, 1f, ease);
        fade.Duration = duration;
        fade.DelayTime = delay;
        fade.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
        visual.StartAnimation("Opacity", fade);

        var slide = compositor.CreateVector3KeyFrameAnimation();
        slide.InsertKeyFrame(0f, new Vector3(0, dy, 0));
        slide.InsertKeyFrame(1f, Vector3.Zero, ease);
        slide.Duration = duration;
        slide.DelayTime = delay;
        slide.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
        visual.StartAnimation("Translation", slide);
    }

    internal static void RotateChevron(UIElement box, bool open, bool animate)
    {
        var visual = ElementCompositionPreview.GetElementVisual(box);
        visual.CenterPoint = new Vector3(7, 7, 0);
        visual.StopAnimation("RotationAngleInDegrees");

        float to = open ? 90f : 0f;
        if (!animate)
        {
            visual.RotationAngleInDegrees = to;
            return;
        }

        var animation = visual.Compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(1f, to, visual.Compositor.CreateCubicBezierEasingFunction(EaseStart, EaseEnd));
        animation.Duration = ChevronDuration;
        visual.StartAnimation("RotationAngleInDegrees", animation);
    }

    internal static void Pop(UIElement element)
    {
        var visual = ElementCompositionPreview.GetElementVisual(element);
        visual.CenterPoint = new Vector3(8, 8, 0);

        var animation = visual.Compositor.CreateVector3KeyFrameAnimation();
        animation.InsertKeyFrame(0.0f, new Vector3(0.4f, 0.4f, 1f));
        animation.InsertKeyFrame(0.6f, new Vector3(1.15f, 1.15f, 1f));
        animation.InsertKeyFrame(1.0f, Vector3.One);
        animation.Duration = PopDuration;
        visual.StartAnimation("Scale", animation);
    }
}
