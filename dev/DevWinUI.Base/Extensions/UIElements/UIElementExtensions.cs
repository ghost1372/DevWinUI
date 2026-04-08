namespace DevWinUI;

public static partial class UIElementExtensions
{
    extension(UIElement element)
    {
        public Storyboard FadeIn(double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (element.Opacity < 1.0)
            {
                return element.AnimateDoubleProperty("Opacity", element.Opacity, 1.0, duration, easingFunction);
            }
            return null;
        }
        public async Task FadeInAsync(double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element.Opacity < 1.0)
            {
                await element.AnimateDoublePropertyAsync("Opacity", element.Opacity, 1.0, duration, easingFunction);
            }
        }

        public Storyboard FadeOut(double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (element.Opacity > 0.0)
            {
                return element.AnimateDoubleProperty("Opacity", element.Opacity, 0.0, duration, easingFunction);
            }
            return null;
        }

        public async Task FadeOutAsync(double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element.Opacity > 0.0)
            {
                await element.AnimateDoublePropertyAsync("Opacity", element.Opacity, 0.0, duration, easingFunction);
            }
        }

        public Point RelativePosition(UIElement other) => element.TransformToVisual(other).TransformPoint(new Point(0, 0));
    }
}
