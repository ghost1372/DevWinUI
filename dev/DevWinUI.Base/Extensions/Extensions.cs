using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;
public static partial class Extensions
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
    public static int Create(this Random random, int min, int max,
        Func<int, bool> regenerateIfMet = null, int regenrationMaxCount = 5)
    {
        var value = random.Next(min, max);

        if (regenerateIfMet != null)
        {
            var i = 0;
            while (i < regenrationMaxCount && regenerateIfMet(value))
            {
                value = random.Next(min, max);
                i++;
            }

            return value;
        }

        return value;
    }
    
    public static IEnumerable<T> GetValues<T>() where T : struct, Enum => Enum.GetValues<T>();

    public static CompositionFlickDirection FlickDirection(this ManipulationCompletedRoutedEventArgs e)
    {
        if (!e.IsInertial)
        {
            return CompositionFlickDirection.None;
        }

        var x = e.Cumulative.Translation.X;
        var y = e.Cumulative.Translation.Y;

        if (Math.Abs(x) > Math.Abs(y))
        {
            return x > 0 ? CompositionFlickDirection.Right : CompositionFlickDirection.Left;
        }

        return y > 0 ? CompositionFlickDirection.Down : CompositionFlickDirection.Up;
    }

    public static void FillAnimation(this ManipulationCompletedRoutedEventArgs e, double fullDimension,
        Action forward, Action backward,
        CompositionAnimationAxis orientation = CompositionAnimationAxis.Y, double ratio = 0.5)
    {
        var translation = e.Cumulative.Translation;
        var distance = orientation == CompositionAnimationAxis.X ? translation.X : translation.Y;

        if (distance >= fullDimension * ratio)
        {
            forward();
        }
        else
        {
            backward();
        }
    }
}
