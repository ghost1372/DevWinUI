using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

public static partial class CompositionExtensions
{
    public static void StartShadowBlurRadiusAnimation(this DropShadow shadow, Color? shadowColor = null,
        Vector3? shadowOffset = null, float? fromShadowOpacity = null, float toShadowOpacity = 1.0f,
        float? fromBlurRadius = null, float toBlurRadius = 16.0f, int duration = 800, int delay = 0,
        UIElement maskingElement = null, Action completed = null)
    {
        CompositionScopedBatch batch = null;

        var compositor = shadow.Compositor;

        if (completed != null)
        {
            batch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            batch.Completed += (s, e) => completed.Invoke();
        }

        if (maskingElement != null)
        {
            switch (maskingElement)
            {
                case TextBlock textBlock:
                    shadow.Mask = textBlock.GetAlphaMask();
                    break;
                case Shape shape:
                    shadow.Mask = shape.GetAlphaMask();
                    break;
                case Image image:
                    shadow.Mask = image.GetAlphaMask();
                    break;
            }
        }

        if (!shadowColor.HasValue)
        {
            shadowColor = Colors.Black;
        }

        shadow.Color = shadowColor.Value;

        if (!shadowOffset.HasValue)
        {
            shadowOffset = Vector3.Zero;
        }

        var shadowOpacityAnimation = compositor.CreateScalarKeyFrameAnimation();
        shadowOpacityAnimation.Duration = TimeSpan.FromMilliseconds(duration);
        if (delay > 0) shadowOpacityAnimation.DelayTime = TimeSpan.FromMilliseconds(delay);
        if (fromShadowOpacity.HasValue) shadowOpacityAnimation.InsertKeyFrame(0.0f, fromShadowOpacity.Value);
        shadowOpacityAnimation.InsertKeyFrame(1.0f, toShadowOpacity);
        shadow.StartAnimation(nameof(DropShadow.Opacity), shadowOpacityAnimation);

        var shadowBlurAnimation = compositor.CreateScalarKeyFrameAnimation();
        shadowBlurAnimation.Duration = TimeSpan.FromMilliseconds(duration);
        if (delay > 0) shadowBlurAnimation.DelayTime = TimeSpan.FromMilliseconds(delay);
        if (fromBlurRadius.HasValue) shadowBlurAnimation.InsertKeyFrame(0.0f, fromBlurRadius.Value);
        shadowBlurAnimation.InsertKeyFrame(1.0f, toBlurRadius);
        shadow.StartAnimation(nameof(DropShadow.BlurRadius), shadowBlurAnimation);

        var shadowOffsetAnimation = compositor.CreateVector3KeyFrameAnimation();
        shadowOffsetAnimation.Duration = TimeSpan.FromMilliseconds(duration);
        if (delay > 0) shadowOffsetAnimation.DelayTime = TimeSpan.FromMilliseconds(delay);
        shadowOffsetAnimation.InsertKeyFrame(1.0f, shadowOffset.Value);
        shadow.StartAnimation(nameof(DropShadow.Offset), shadowOffsetAnimation);

        batch?.End();
    }
}
