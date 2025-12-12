using Microsoft.UI.Xaml.Input;

namespace DevWinUI;

public static partial class CompositionExtensions
{
    public static CompositionPropertySet ScrollProperties(this ScrollViewer scrollViewer) =>
        ElementCompositionPreview.GetScrollViewerManipulationPropertySet(scrollViewer);

    public static Visual Visual(this UIElement element) =>
        ElementCompositionPreview.GetElementVisual(element);

    public static CompositionPropertySet GetPointerPositionProperties(this UIElement element) =>
        ElementCompositionPreview.GetPointerPositionPropertySet(element);

    public static void SetChildVisual(this UIElement element, Visual childVisual) =>
        ElementCompositionPreview.SetElementChildVisual(element, childVisual);

    public static ContainerVisual ContainerVisual(this UIElement element)
    {
        var hostVisual = ElementCompositionPreview.GetElementVisual(element);
        ContainerVisual root = hostVisual.Compositor.CreateContainerVisual();
        ElementCompositionPreview.SetElementChildVisual(element, root);
        return root;
    }

    public static void SetDropShadow(this FrameworkElement element, DropShadow shadow, FrameworkElement sizingElement = null)
    {
        var compositor = shadow.Compositor;

        var shadowVisual = compositor.CreateSpriteVisual();
        shadowVisual.Shadow = shadow;

        if (sizingElement == null)
        {
            sizingElement = element;
        }

        sizingElement.SizeChanged += (s, e) =>
        {
            if (e.PreviousSize.Equals(e.NewSize)) return;
            shadowVisual.Size = sizingElement.RenderSize.ToVector2();
        };
        shadowVisual.Size = sizingElement.RenderSize.ToVector2();

        element.SetChildVisual(shadowVisual);
    }

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
