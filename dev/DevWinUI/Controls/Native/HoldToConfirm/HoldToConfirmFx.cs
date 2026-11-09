namespace DevWinUI;

internal static partial class HoldToConfirmFx
{
    private static CubicBezierEasingFunction Ease(Compositor c) =>
        c.CreateCubicBezierEasingFunction(new Vector2(0.2f, 0f), new Vector2(0f, 1f));

    internal static void Tween(UIElement el, float o0, float o1, float y0, float y1, int durMs, int delayMs = 0)
    {
        ElementCompositionPreview.SetIsTranslationEnabled(el, true);
        var v = ElementCompositionPreview.GetElementVisual(el);
        v.StopAnimation("Opacity");
        v.StopAnimation("Translation");
        v.Opacity = o1;
        v.Properties.InsertVector3("Translation", new Vector3(0, y1, 0));
        if (durMs <= 0) return;

        var comp = v.Compositor;
        var ease = Ease(comp);
        var dur = TimeSpan.FromMilliseconds(durMs);
        var delay = TimeSpan.FromMilliseconds(delayMs);

        var fade = comp.CreateScalarKeyFrameAnimation();
        fade.InsertKeyFrame(0f, o0);
        fade.InsertKeyFrame(1f, o1, ease);
        fade.Duration = dur;
        fade.DelayTime = delay;
        fade.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
        v.StartAnimation("Opacity", fade);

        var slide = comp.CreateVector3KeyFrameAnimation();
        slide.InsertKeyFrame(0f, new Vector3(0, y0, 0));
        slide.InsertKeyFrame(1f, new Vector3(0, y1, 0), ease);
        slide.Duration = dur;
        slide.DelayTime = delay;
        slide.DelayBehavior = AnimationDelayBehavior.SetInitialValueBeforeDelay;
        v.StartAnimation("Translation", slide);
    }

    internal static DoubleAnimation Dependent(DependencyObject target, string property, double from, double to, int durMs, int beginMs = 0)
    {
        var a = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = new Duration(TimeSpan.FromMilliseconds(durMs)),
            BeginTime = TimeSpan.FromMilliseconds(beginMs),
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
            FillBehavior = FillBehavior.Stop,
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(a, target);
        Storyboard.SetTargetProperty(a, property);
        return a;
    }

    internal static void ClipTo(FrameworkElement el, double padY)
    {
        el.SizeChanged += (_, e) =>
            el.Clip = new RectangleGeometry { Rect = new Rect(0, -padY, e.NewSize.Width, e.NewSize.Height + padY * 2) };
    }
}
