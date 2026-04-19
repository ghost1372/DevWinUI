namespace DevWinUI;

public static partial class FrameworkElementExtensions
{
    extension(FrameworkElement element)
    {
        public double GetTranslateX()
        {
            return element.GetCompositeTransform().TranslateX;
        }
        public double GetTranslateY()
        {
            return element.GetCompositeTransform().TranslateY;
        }

        public void TranslateX(double x)
        {
            element.GetCompositeTransform().TranslateX = x;
        }
        public void TranslateY(double y)
        {
            element.GetCompositeTransform().TranslateY = y;
        }

        public void TranslateDeltaX(double x)
        {
            element.GetCompositeTransform().TranslateX += x;
        }
        public void TranslateDeltaY(double y)
        {
            element.GetCompositeTransform().TranslateY += y;
        }

        public double GetScaleX()
        {
            return element.GetCompositeTransform().ScaleX;
        }
        public double GetScaleY()
        {
            return element.GetCompositeTransform().ScaleY;
        }

        public void ScaleX(double x)
        {
            element.GetCompositeTransform().ScaleX = x;
        }
        public void ScaleY(double y)
        {
            element.GetCompositeTransform().ScaleY = y;
        }

        public void ScaleDeltaX(double x)
        {
            element.GetCompositeTransform().ScaleX += x;
        }
        public void ScaleDeltaY(double y)
        {
            element.GetCompositeTransform().ScaleY += y;
        }

        public CompositeTransform GetCompositeTransform()
        {
            if (element == null)
            {
                throw new ArgumentNullException("elem");
            }

            var trans = element.RenderTransform as CompositeTransform;
            if (trans == null)
            {
                trans = new CompositeTransform();
                element.RenderTransform = trans;
            }
            return trans;
        }

        public bool IsFullyVisibile(FrameworkElement parent)
        {
            if (element == null || parent == null)
                return false;

            if (element.Visibility != Visibility.Visible)
                return false;

            var elementBounds = element.TransformToVisual(parent).TransformBounds(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
            var containerBounds = new Rect(0, 0, parent.ActualWidth, parent.ActualHeight);

            var originalElementWidth = elementBounds.Width;
            var originalElementHeight = elementBounds.Height;

            elementBounds.Intersect(containerBounds);

            var newElementWidth = elementBounds.Width;
            var newElementHeight = elementBounds.Height;

            return originalElementWidth.Equals(newElementWidth) && originalElementHeight.Equals(newElementHeight);
        }
    }
}
