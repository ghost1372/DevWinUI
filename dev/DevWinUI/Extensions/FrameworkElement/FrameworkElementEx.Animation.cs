namespace DevWinUI;
public partial class FrameworkElementEx
{
    public static double GetTranslateX(this FrameworkElement element)
    {
        return element.GetCompositeTransform().TranslateX;
    }
    public static double GetTranslateY(this FrameworkElement element)
    {
        return element.GetCompositeTransform().TranslateY;
    }

    public static void TranslateX(this FrameworkElement element, double x)
    {
        element.GetCompositeTransform().TranslateX = x;
    }
    public static void TranslateY(this FrameworkElement element, double y)
    {
        element.GetCompositeTransform().TranslateY = y;
    }

    public static void TranslateDeltaX(this FrameworkElement element, double x)
    {
        element.GetCompositeTransform().TranslateX += x;
    }
    public static void TranslateDeltaY(this FrameworkElement element, double y)
    {
        element.GetCompositeTransform().TranslateY += y;
    }

    public static double GetScaleX(this FrameworkElement element)
    {
        return element.GetCompositeTransform().ScaleX;
    }
    public static double GetScaleY(this FrameworkElement element)
    {
        return element.GetCompositeTransform().ScaleY;
    }

    public static void ScaleX(this FrameworkElement element, double x)
    {
        element.GetCompositeTransform().ScaleX = x;
    }
    public static void ScaleY(this FrameworkElement element, double y)
    {
        element.GetCompositeTransform().ScaleY = y;
    }

    public static void ScaleDeltaX(this FrameworkElement element, double x)
    {
        element.GetCompositeTransform().ScaleX += x;
    }
    public static void ScaleDeltaY(this FrameworkElement element, double y)
    {
        element.GetCompositeTransform().ScaleY += y;
    }

    public static CompositeTransform GetCompositeTransform(this FrameworkElement element)
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
}
