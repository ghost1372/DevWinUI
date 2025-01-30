namespace DevWinUI;
public static partial class BlurAnimationHelper
{
    private static Compositor _compositor;
    private static readonly Dictionary<FrameworkElement, (CompositionEffectBrush brush, SpriteVisual visual)> _elements = new();

    public static void ApplyBlurEffect(FrameworkElement target, float amount)
    {
        if (_elements.ContainsKey(target)) return; // Prevent reapplying

        _compositor = ElementCompositionPreview.GetElementVisual(target).Compositor;

        var blurEffect = new GaussianBlurEffect()
        {
            Name = "Blur",
            Source = new CompositionEffectSourceParameter("Backdrop"),
            BlurAmount = amount,
            BorderMode = EffectBorderMode.Hard
        };

        var blurEffectFactory = _compositor.CreateEffectFactory(blurEffect, new[] { "Blur.BlurAmount" });
        var brush = blurEffectFactory.CreateBrush();

        var backdropBrush = _compositor.CreateBackdropBrush();
        brush.SetSourceParameter("Backdrop", backdropBrush);

        var visual = _compositor.CreateSpriteVisual();
        visual.Brush = brush;

        ElementCompositionPreview.SetElementChildVisual(target, visual);

        void OnSizeChanged(object s, SizeChangedEventArgs e)
        {
            visual.Size = new System.Numerics.Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
        }
        target.SizeChanged -= OnSizeChanged;
        target.SizeChanged += OnSizeChanged;

        _elements[target] = (brush, visual);
    }

    public static void RemoveBlurEffect(FrameworkElement target)
    {
        if (_elements.TryGetValue(target, out var effectData))
        {
            ElementCompositionPreview.SetElementChildVisual(target, null);
            _elements.Remove(target);
        }
    }
    public static void StartBlurAnimation(FrameworkElement target, float from, float to, TimeSpan duration)
    {
        StartBlurAnimationBase(target, from, to, duration, AnimationIterationBehavior.Count);
    }
    public static void StartBlurAnimation(FrameworkElement target, float from, float to, TimeSpan duration, AnimationIterationBehavior animationIterationBehavior)
    {
        StartBlurAnimationBase(target, from, to, duration, animationIterationBehavior);
    }
    private static void StartBlurAnimationBase(FrameworkElement target, float from, float to, TimeSpan duration, AnimationIterationBehavior iterationBehavior)
    {
        if (!_elements.TryGetValue(target, out var effectData)) return;

        var blurAnimation = _compositor.CreateScalarKeyFrameAnimation();
        blurAnimation.InsertKeyFrame(0.0f, from);

        if (iterationBehavior == AnimationIterationBehavior.Forever)
        {
            blurAnimation.InsertKeyFrame(0.5f, to);
            blurAnimation.InsertKeyFrame(1.0f, from);
        }
        else
        {
            blurAnimation.InsertKeyFrame(1.0f, to);
            blurAnimation.IterationCount = 1;
        }

        blurAnimation.Duration = duration;
        blurAnimation.IterationBehavior = iterationBehavior;

        effectData.brush?.StartAnimation("Blur.BlurAmount", blurAnimation);
    }

    public static void StartReversBlurAnimation(FrameworkElement target, float to, float from, TimeSpan duration)
    {
        StartReversBlurAnimationBase(target, to, from, duration);
    }
    public static void StartReversBlurAnimation(FrameworkElement target, float to, TimeSpan duration)
    {
        StartReversBlurAnimationBase(target, to, GetCurrentBlurAmount(target), duration);
    }
    private static void StartReversBlurAnimationBase(FrameworkElement target, float to, float from, TimeSpan duration)
    {
        if (!_elements.TryGetValue(target, out var effectData)) return;

        var blurAnimation = _compositor.CreateScalarKeyFrameAnimation();
        blurAnimation.InsertKeyFrame(1.0f, from);
        blurAnimation.InsertKeyFrame(1.0f, to);

        blurAnimation.Duration = duration;
        blurAnimation.IterationBehavior = AnimationIterationBehavior.Count;
        blurAnimation.IterationCount = 1;

        effectData.brush?.StartAnimation("Blur.BlurAmount", blurAnimation);
    }
    public static void SetBlurAmount(FrameworkElement target, float amount)
    {
        if (_elements.TryGetValue(target, out var effectData))
        {
            effectData.brush?.Properties.InsertScalar("Blur.BlurAmount", amount);
        }
    }

    public static void StopAnimation(FrameworkElement target)
    {
        if (_elements.TryGetValue(target, out var effectData))
        {
            effectData.brush?.StopAnimation("Blur.BlurAmount");
        }
    }

    public static float GetCurrentBlurAmount(FrameworkElement target)
    {
        if (!_elements.TryGetValue(target, out var effectData)) return 0;

        if (effectData.brush == null)
        {
            return 0;
        }

        effectData.brush.Properties.TryGetScalar("Blur.BlurAmount", out float value);

        return value;
    }
}
