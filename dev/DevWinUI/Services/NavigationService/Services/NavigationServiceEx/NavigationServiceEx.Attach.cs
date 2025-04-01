namespace DevWinUI;
public partial class NavigationServiceEx
{
    public static object GetParameter(DependencyObject obj)
    {
        return (object)obj.GetValue(ParameterProperty);
    }

    public static void SetParameter(DependencyObject obj, object value)
    {
        obj.SetValue(ParameterProperty, value);
    }

    public static readonly DependencyProperty ParameterProperty =
        DependencyProperty.RegisterAttached("Parameter", typeof(object), typeof(NavigationServiceEx), new PropertyMetadata(null));


    internal static Type GetParent(DependencyObject obj)
    {
        return (Type)obj.GetValue(ParentProperty);
    }

    internal static void SetParent(DependencyObject obj, Type value)
    {
        obj.SetValue(ParentProperty, value);
    }

    internal static readonly DependencyProperty ParentProperty =
        DependencyProperty.RegisterAttached("Parent", typeof(Type), typeof(NavigationServiceEx), new PropertyMetadata(null));

    public static Type GetNavigateTo(DependencyObject obj)
    {
        return (Type)obj.GetValue(NavigateToProperty);
    }

    public static void SetNavigateTo(DependencyObject obj, Type value)
    {
        obj.SetValue(NavigateToProperty, value);
    }

    public static readonly DependencyProperty NavigateToProperty =
        DependencyProperty.RegisterAttached("NavigateTo", typeof(Type), typeof(NavigationServiceEx), new PropertyMetadata(null));
}
