namespace DevWinUI;
public static partial class AnimationExtensions
{
    public static Storyboard AnimateX(this FrameworkElement element, double x, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element.GetTranslateX() != x)
        {
            return AnimateDoubleProperty(element.GetCompositeTransform(), "TranslateX", element.GetTranslateX(), x, duration, easingFunction);
        }
        return null;
    }
    public static async Task AnimateXAsync(this FrameworkElement element, double x, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element.GetTranslateX() != x)
        {
            await AnimateDoublePropertyAsync(element.GetCompositeTransform(), "TranslateX", element.GetTranslateX(), x, duration, easingFunction);
        }
    }

    public static Storyboard AnimateY(this FrameworkElement element, double y, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element.GetTranslateY() != y)
        {
            return AnimateDoubleProperty(element.GetCompositeTransform(), "TranslateY", element.GetTranslateY(), y, duration, easingFunction);
        }
        return null;
    }
    public static async Task AnimateYAsync(this FrameworkElement element, double y, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element.GetTranslateY() != y)
        {
            await AnimateDoublePropertyAsync(element.GetCompositeTransform(), "TranslateY", element.GetTranslateY(), y, duration, easingFunction);
        }
    }

    public static Storyboard AnimateScaleX(this FrameworkElement element, double x, double duration = 150, EasingFunctionBase easingFunction = null)
    {
        if (element.GetScaleX() != x)
        {
            return AnimateDoubleProperty(element.GetCompositeTransform(), "ScaleX", element.GetScaleX(), x, duration, easingFunction);
        }
        return null;
    }
    public static async Task AnimateScaleXAsync(this FrameworkElement element, double x, double duration = 150, EasingFunctionBase easingFunction = null)
    {
        if (element.GetScaleX() != x)
        {
            await AnimateDoublePropertyAsync(element.GetCompositeTransform(), "ScaleX", element.GetScaleX(), x, duration, easingFunction);
        }
    }

    public static Storyboard AnimateScaleY(this FrameworkElement element, double y, double duration = 150, EasingFunctionBase easingFunction = null)
    {
        if (element.GetScaleY() != y)
        {
            return AnimateDoubleProperty(element.GetCompositeTransform(), "ScaleY", element.GetScaleY(), y, duration, easingFunction);
        }
        return null;
    }
    public static async Task AnimateScaleYAsync(this FrameworkElement element, double y, double duration = 150, EasingFunctionBase easingFunction = null)
    {
        if (element.GetScaleY() != y)
        {
            await AnimateDoublePropertyAsync(element.GetCompositeTransform(), "ScaleY", element.GetScaleY(), y, duration, easingFunction);
        }
    }

    public static Storyboard AnimateWidth(this FrameworkElement element, double width, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element == null)
        {
            throw new ArgumentNullException("element");
        }

        if (element.ActualWidth != width)
        {
            return AnimateDoubleProperty(element, "Width", element.ActualWidth, width, duration, easingFunction);
        }
        return null;
    }
    public static async Task AnimateWidthAsync(this FrameworkElement element, double width, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element.ActualWidth != width)
        {
            await AnimateDoublePropertyAsync(element, "Width", element.ActualWidth, width, duration, easingFunction);
        }
    }

    public static Storyboard AnimateHeight(this FrameworkElement element, double height, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element == null)
        {
            throw new ArgumentNullException("element");
        }
        if (element.Height != height)
        {
            return AnimateDoubleProperty(element, "Height", element.ActualHeight, height, duration, easingFunction);
        }
        return null;
    }
    public static async Task AnimateHeightAsync(this FrameworkElement element, double height, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element.Height != height)
        {
            await AnimateDoublePropertyAsync(element, "Height", element.ActualHeight, height, duration, easingFunction);
        }
    }

    public static Storyboard FadeIn(this UIElement element, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element == null)
        {
            throw new ArgumentNullException("element");
        }
        if (element.Opacity < 1.0)
        {
            return AnimateDoubleProperty(element, "Opacity", element.Opacity, 1.0, duration, easingFunction);
        }
        return null;
    }
    public static async Task FadeInAsync(this UIElement element, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element.Opacity < 1.0)
        {
            await AnimateDoublePropertyAsync(element, "Opacity", element.Opacity, 1.0, duration, easingFunction);
        }
    }

    public static Storyboard FadeOut(this UIElement element, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element == null)
        {
            throw new ArgumentNullException("element");
        }
        if (element.Opacity > 0.0)
        {
            return AnimateDoubleProperty(element, "Opacity", element.Opacity, 0.0, duration, easingFunction);
        }
        return null;
    }
    public static async Task FadeOutAsync(this UIElement element, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        if (element.Opacity > 0.0)
        {
            await AnimateDoublePropertyAsync(element, "Opacity", element.Opacity, 0.0, duration, easingFunction);
        }
    }
    public static Task AnimateDoublePropertyAsync(this DependencyObject target, string property, double from, double to, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        Storyboard storyboard = AnimateDoubleProperty(target, property, from, to, duration, easingFunction);
        storyboard.Completed += (sender, e) =>
        {
            tcs.SetResult(true);
        };
        return tcs.Task;
    }
    public static Storyboard AnimateDoubleProperty(this DependencyObject target, string property, double from, double to, double duration = 250, EasingFunctionBase easingFunction = null)
    {
        var storyboard = new Storyboard();
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = TimeSpan.FromMilliseconds(duration),
            EasingFunction = easingFunction ?? new SineEase(),
            FillBehavior = FillBehavior.HoldEnd,
            EnableDependentAnimation = true
        };

        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, property);

        storyboard.Children.Add(animation);
        storyboard.FillBehavior = FillBehavior.HoldEnd;
        storyboard.Begin();

        return storyboard;
    }
}
