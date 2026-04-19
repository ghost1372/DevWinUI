namespace DevWinUI;
public static partial class AnimationExtensions
{
    extension(FrameworkElement element)
    {
        public Storyboard AnimateX(double x, double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element.GetTranslateX() != x)
            {
                return element.GetCompositeTransform().AnimateDoubleProperty("TranslateX", element.GetTranslateX(), x, duration, easingFunction);
            }
            return null;
        }
        public async Task AnimateXAsync(double x, double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element.GetTranslateX() != x)
            {
                await element.GetCompositeTransform().AnimateDoublePropertyAsync("TranslateX", element.GetTranslateX(), x, duration, easingFunction);
            }
        }

        public Storyboard AnimateY(double y, double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element.GetTranslateY() != y)
            {
                return element.GetCompositeTransform().AnimateDoubleProperty("TranslateY", element.GetTranslateY(), y, duration, easingFunction);
            }
            return null;
        }
        public async Task AnimateYAsync(double y, double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element.GetTranslateY() != y)
            {
                await element.GetCompositeTransform().AnimateDoublePropertyAsync("TranslateY", element.GetTranslateY(), y, duration, easingFunction);
            }
        }

        public Storyboard AnimateScaleX(double x, double duration = 150, EasingFunctionBase easingFunction = null)
        {
            if (element.GetScaleX() != x)
            {
                return element.GetCompositeTransform().AnimateDoubleProperty("ScaleX", element.GetScaleX(), x, duration, easingFunction);
            }
            return null;
        }
        public async Task AnimateScaleXAsync(double x, double duration = 150, EasingFunctionBase easingFunction = null)
        {
            if (element.GetScaleX() != x)
            {
                await element.GetCompositeTransform().AnimateDoublePropertyAsync("ScaleX", element.GetScaleX(), x, duration, easingFunction);
            }
        }

        public Storyboard AnimateScaleY(double y, double duration = 150, EasingFunctionBase easingFunction = null)
        {
            if (element.GetScaleY() != y)
            {
                return element.GetCompositeTransform().AnimateDoubleProperty("ScaleY", element.GetScaleY(), y, duration, easingFunction);
            }
            return null;
        }
        public async Task AnimateScaleYAsync(double y, double duration = 150, EasingFunctionBase easingFunction = null)
        {
            if (element.GetScaleY() != y)
            {
                await element.GetCompositeTransform().AnimateDoublePropertyAsync("ScaleY", element.GetScaleY(), y, duration, easingFunction);
            }
        }

        public Storyboard AnimateWidth(double width, double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.ActualWidth != width)
            {
                return element.AnimateDoubleProperty("Width", element.ActualWidth, width, duration, easingFunction);
            }
            return null;
        }

        public async Task AnimateWidthAsync(double width, double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element.ActualWidth != width)
            {
                await element.AnimateDoublePropertyAsync("Width", element.ActualWidth, width, duration, easingFunction);
            }
        }

        public Storyboard AnimateHeight(double height, double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            if (element.Height != height)
            {
                return element.AnimateDoubleProperty("Height", element.ActualHeight, height, duration, easingFunction);
            }
            return null;
        }

        public async Task AnimateHeightAsync(double height, double duration = 250, EasingFunctionBase easingFunction = null)
        {
            if (element.Height != height)
            {
                await element.AnimateDoublePropertyAsync("Height", element.ActualHeight, height, duration, easingFunction);
            }
        }
    }
}
