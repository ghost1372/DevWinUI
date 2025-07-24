namespace DevWinUI;
public static partial class FrameworkElementExtensions
{
    public static double GetTranslateX(this FrameworkElement elem)
    {
        return elem.GetCompositeTransform().TranslateX;
    }
    public static double GetTranslateY(this FrameworkElement elem)
    {
        return elem.GetCompositeTransform().TranslateY;
    }

    public static void TranslateX(this FrameworkElement elem, double x)
    {
        elem.GetCompositeTransform().TranslateX = x;
    }
    public static void TranslateY(this FrameworkElement elem, double y)
    {
        elem.GetCompositeTransform().TranslateY = y;
    }

    public static void TranslateDeltaX(this FrameworkElement elem, double x)
    {
        elem.GetCompositeTransform().TranslateX += x;
    }
    public static void TranslateDeltaY(this FrameworkElement elem, double y)
    {
        elem.GetCompositeTransform().TranslateY += y;
    }

    public static double GetScaleX(this FrameworkElement elem)
    {
        return elem.GetCompositeTransform().ScaleX;
    }
    public static double GetScaleY(this FrameworkElement elem)
    {
        return elem.GetCompositeTransform().ScaleY;
    }

    public static void ScaleX(this FrameworkElement elem, double x)
    {
        elem.GetCompositeTransform().ScaleX = x;
    }
    public static void ScaleY(this FrameworkElement elem, double y)
    {
        elem.GetCompositeTransform().ScaleY = y;
    }

    public static void ScaleDeltaX(this FrameworkElement elem, double x)
    {
        elem.GetCompositeTransform().ScaleX += x;
    }
    public static void ScaleDeltaY(this FrameworkElement elem, double y)
    {
        elem.GetCompositeTransform().ScaleY += y;
    }

    public static CompositeTransform GetCompositeTransform(this FrameworkElement elem)
    {
        if (elem == null)
        {
            throw new ArgumentNullException("elem");
        }

        var trans = elem.RenderTransform as CompositeTransform;
        if (trans == null)
        {
            trans = new CompositeTransform();
            elem.RenderTransform = trans;
        }
        return trans;
    }
}
