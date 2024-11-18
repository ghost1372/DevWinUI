namespace DevWinUI;
internal partial class NavigationViewItemAttach
{
    public static readonly DependencyProperty IsSubItemProperty =
        DependencyProperty.RegisterAttached("IsSubItem", typeof(bool), typeof(NavigationViewItemAttach), new PropertyMetadata(false, OnIsSubItemChanged));

    public static bool GetIsSubItem(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsSubItemProperty);
    }

    public static void SetIsSubItem(DependencyObject obj, bool value)
    {
        obj.SetValue(IsSubItemProperty, value);
    }

    private static void OnIsSubItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var navItem = d as NavigationViewItem;

        if (navItem != null)
        {
            var isSubmenu = (bool)e.NewValue;
            var resources = Application.Current.Resources;

            if (!resources.ContainsKey("NavigationViewItemExPadding"))
            {
                resources = navItem.Resources;
            }

            if (resources.ContainsKey("NavigationViewItemExPadding"))
            {
                if (isSubmenu)
                {
                    resources["NavigationViewItemExPadding"] = new Thickness(-31, 0, 0, 0);
                }
                else
                {
                    resources["NavigationViewItemExPadding"] = new Thickness(0);
                }
            }
        }
    }
}
