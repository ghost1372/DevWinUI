﻿namespace DevWinUI;

[Obsolete("This class is obsolete and will be removed in a future version. Please use NavigateToCommand instead.")]
public partial class NavigationHelperEx
{
    public static Type GetNavigateToSetting(DependencyObject obj)
    {
        return (Type)obj.GetValue(NavigateToSettingProperty);
    }

    public static void SetNavigateToSetting(DependencyObject obj, Type value)
    {
        obj.SetValue(NavigateToSettingProperty, value);
    }

    public static readonly DependencyProperty NavigateToSettingProperty =
        DependencyProperty.RegisterAttached("NavigateToSetting", typeof(Type), typeof(NavigationHelperEx), new PropertyMetadata(null));

}
