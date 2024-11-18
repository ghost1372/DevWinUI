using Microsoft.UI.Xaml.Hosting;

namespace DevWinUI;
public static partial class VisualAttach
{
    public static readonly DependencyProperty IsBindCenterPointProperty =
    DependencyProperty.RegisterAttached("IsBindCenterPoint", typeof(bool), typeof(VisualAttach),
        new PropertyMetadata(false, (s, a) =>
        {
            if (a.NewValue != a.OldValue)
                if (s is UIElement ele)
                {
                    if (a.NewValue is true)
                        ElementCompositionPreview.GetElementVisual(ele).BindCenterPoint();
                    else
                        ElementCompositionPreview.GetElementVisual(ele).StopAnimation("CenterPoint");
                }
        }));

    public static bool GetIsBindCenterPoint(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsBindCenterPointProperty);
    }

    public static void SetIsBindCenterPoint(DependencyObject obj, bool value)
    {
        obj.SetValue(IsBindCenterPointProperty, value);
    }
}
