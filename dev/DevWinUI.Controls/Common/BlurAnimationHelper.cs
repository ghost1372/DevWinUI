using CommunityToolkit.WinUI.Animations;
using CommunityToolkit.WinUI.Media;

namespace DevWinUI;
public partial class BlurAnimationHelper
{
    private class AnimationSetInfo
    {
        public AnimationSet AnimationSet { get; set; }
        public BlurEffect BlurEffect { get; set; }
    }

    private static Dictionary<UIElement, AnimationSetInfo> animationSets = new Dictionary<UIElement, AnimationSetInfo>();

    private static void InitializeAnimationSet(UIElement element)
    {
        if (!animationSets.ContainsKey(element))
        {
            var blurEffect = new BlurEffect { IsAnimatable = true };
            var pipelineVisualFactory = new PipelineVisualFactory();
            pipelineVisualFactory.Effects.Add(blurEffect);

            UIElementExtensions.SetVisualFactory(element, pipelineVisualFactory);

            var animationSet = new AnimationSet { IsSequential = true };
            var animationSetInfo = new AnimationSetInfo { AnimationSet = animationSet, BlurEffect = blurEffect };
            animationSets.Add(element, animationSetInfo);
        }
    }

    public static void StopBlurAnimation(UIElement element)
    {
        if (animationSets.TryGetValue(element, out var animationSetInfo))
        {
            animationSetInfo.AnimationSet?.Stop();
        }
    }

    public static void StartBlurAnimation(UIElement element, double from, double to)
    {
        StartBlurAnimation(element, from, to, TimeSpan.FromSeconds(1), EasingMode.EaseInOut, EasingType.Linear);
    }

    public static void StartBlurAnimation(UIElement element, double from, double to, TimeSpan duration)
    {
        StartBlurAnimation(element, from, to, duration, EasingMode.EaseInOut, EasingType.Linear);
    }

    public static void StartBlurAnimation(UIElement element, double from, double to, TimeSpan duration, EasingMode easingMode, EasingType easingType)
    {
        InitializeAnimationSet(element);

        var animationSetInfo = animationSets[element];
        var blurEffectAnim = new BlurEffectAnimation
        {
            EasingMode = easingMode,
            EasingType = easingType,
            From = from,
            To = to,
            Duration = duration,
            Target = animationSetInfo?.BlurEffect
        };

        animationSetInfo?.AnimationSet?.Clear();
        animationSetInfo?.AnimationSet?.Add(blurEffectAnim);
        Explicit.SetAnimations(element, new AnimationDictionary { animationSetInfo?.AnimationSet });
        animationSetInfo.AnimationSet?.Start();
    }
}
