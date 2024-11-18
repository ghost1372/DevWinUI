namespace DevWinUI;
public partial class BreadcrumbNavigator
{
    public static bool GetIsHeaderVisible(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsHeaderVisibleProperty);
    }

    public static void SetIsHeaderVisible(DependencyObject obj, bool value)
    {
        obj.SetValue(IsHeaderVisibleProperty, value);
    }

    public static readonly DependencyProperty IsHeaderVisibleProperty =
        DependencyProperty.RegisterAttached("IsHeaderVisible", typeof(bool), typeof(BreadcrumbNavigator), new PropertyMetadata(false));

    public static readonly DependencyProperty PageTitle =
            DependencyProperty.RegisterAttached("PageTitle", typeof(string), typeof(BreadcrumbNavigator), new PropertyMetadata(null));

    public static void SetPageTitle(DependencyObject obj, string value)
    {
        obj.SetValue(PageTitle, value);
    }

    public static string GetPageTitle(DependencyObject obj)
    {
        return (string)obj.GetValue(PageTitle);
    }

    public static bool GetClearNavigation(DependencyObject obj)
    {
        return (bool)obj.GetValue(ClearNavigationProperty);
    }

    public static void SetClearNavigation(DependencyObject obj, bool value)
    {
        obj.SetValue(ClearNavigationProperty, value);
    }

    public static readonly DependencyProperty ClearNavigationProperty =
        DependencyProperty.RegisterAttached("ClearNavigation", typeof(bool), typeof(BreadcrumbNavigator), new PropertyMetadata(false));
}
