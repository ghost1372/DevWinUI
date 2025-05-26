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

    public static readonly DependencyProperty NormalizedCenterPointProperty =
        DependencyProperty.RegisterAttached("NormalizedCenterPoint", typeof(string), typeof(VisualAttach), new PropertyMetadata(false, OnNormalizedCenterPointChanged));
    /// <summary>
    /// Gets the <see cref="Visual.CenterPoint"/> of the <see cref="UIElement"/> normalized between 0.0 and 1.0
    /// is centered even when the visual is resized
    /// </summary>
    /// <param name="obj">The <see cref="UIElement"/></param>
    /// <returns>a string representing Vector2 as the normalized <see cref="Visual.CenterPoint"/></returns>
    public static string GetNormalizedCenterPoint(DependencyObject obj)
    {
        return (string)obj.GetValue(NormalizedCenterPointProperty);
    }

    /// <summary>
    /// Sets the normalized <see cref="Visual.CenterPoint"/> of the <see cref="UIElement"/>
    /// is centered even when the visual is resized
    /// </summary>
    /// <param name="obj">The <see cref="UIElement"/></param>
    /// <param name="value">A string representing a Vector2 normalized between 0.0 and 1.0</param>
    public static void SetNormalizedCenterPoint(DependencyObject obj, string value)
    {
        obj.SetValue(NormalizedCenterPointProperty, value);
    }
    private static void OnNormalizedCenterPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FrameworkElement element &&
            e.NewValue is string newValue)
        {
            Vector2 center = newValue.ToVector2();
            Visual visual = ElementCompositionPreview.GetElementVisual(element);
            const string expression = "Vector2(this.Target.Size.X * X, this.Target.Size.Y * Y)";
            ExpressionAnimation animation = visual.Compositor.CreateExpressionAnimation(expression);

            animation.SetScalarParameter("X", center.X);
            animation.SetScalarParameter("Y", center.Y);

            visual.StopAnimation("CenterPoint.XY");
            visual.StartAnimation("CenterPoint.XY", animation);
        }
    }
}
