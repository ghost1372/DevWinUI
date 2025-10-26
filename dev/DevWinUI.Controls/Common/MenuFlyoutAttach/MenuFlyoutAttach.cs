using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;

public partial class MenuFlyoutAttach
{
    private static readonly DependencyProperty AutoCloseHandlerProperty =
        DependencyProperty.RegisterAttached("AutoCloseHandler", typeof(RoutedEventHandler), typeof(MenuFlyoutAttach), new PropertyMetadata(null));

    private static void SetAutoCloseHandler(ButtonBase button, RoutedEventHandler handler) =>
        button.SetValue(AutoCloseHandlerProperty, handler);

    private static RoutedEventHandler? GetAutoCloseHandler(ButtonBase button) =>
        (RoutedEventHandler?)button.GetValue(AutoCloseHandlerProperty);

    public static readonly DependencyProperty AutoCloseByClickOnSecondaryMenuItemsProperty =
        DependencyProperty.RegisterAttached("AutoCloseByClickOnSecondaryMenuItems", typeof(bool), typeof(MenuFlyoutAttach), new PropertyMetadata(false));

    public static void SetAutoCloseByClickOnSecondaryMenuItems(MenuFlyout flyout, bool value) =>
        flyout.SetValue(AutoCloseByClickOnSecondaryMenuItemsProperty, value);

    public static bool GetAutoCloseByClickOnSecondaryMenuItems(MenuFlyout flyout) =>
        (bool)flyout.GetValue(AutoCloseByClickOnSecondaryMenuItemsProperty);

    public static readonly DependencyProperty SecondaryMenuProperty =
        DependencyProperty.RegisterAttached("SecondaryMenu", typeof(MenuFlyoutSecondaryItems), typeof(MenuFlyoutAttach), new PropertyMetadata(null, OnSecondaryMenuChanged));

    public static void SetSecondaryMenu(MenuFlyout flyout, MenuFlyoutSecondaryItems value) =>
        flyout.SetValue(SecondaryMenuProperty, value);

    public static MenuFlyoutSecondaryItems GetSecondaryMenu(MenuFlyout flyout)
    {
        var list = (MenuFlyoutSecondaryItems)flyout.GetValue(SecondaryMenuProperty);
        if (list == null)
        {
            list = new MenuFlyoutSecondaryItems();
            flyout.SetValue(SecondaryMenuProperty, list);
        }
        return list;
    }


    public static readonly DependencyProperty SecondaryMenuPlacementProperty =
        DependencyProperty.RegisterAttached("SecondaryMenuPlacement", typeof(MenuFlyoutSecondaryMenuPlacement), typeof(MenuFlyoutAttach), new PropertyMetadata(MenuFlyoutSecondaryMenuPlacement.Top));

    public static void SetSecondaryMenuPlacement(MenuFlyout flyout, MenuFlyoutSecondaryMenuPlacement value) =>
        flyout.SetValue(SecondaryMenuPlacementProperty, value);

    public static MenuFlyoutSecondaryMenuPlacement GetSecondaryMenuPlacement(MenuFlyout flyout) =>
        (MenuFlyoutSecondaryMenuPlacement)flyout.GetValue(SecondaryMenuPlacementProperty);

    private static void OnSecondaryMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not MenuFlyout flyout)
            return;

        flyout.Opened -= OnFlyoutOpened;
        flyout.Opened += OnFlyoutOpened;

        var secondaryMenu = GetSecondaryMenu(flyout);
        if (secondaryMenu.Items.Count > 0)
        {
            // Apply custom style
            flyout.MenuFlyoutPresenterStyle = Application.Current.Resources["MenuFlyoutWithSecondaryMenuPresenterStyle"] as Style;
        }
        else
        {
            // Default style
            flyout.MenuFlyoutPresenterStyle = Application.Current.Resources["DefaultMenuFlyoutPresenterStyle"] as Style;
        }
    }

    private static void OnFlyoutOpened(object sender, object e)
    {
        if (sender is not MenuFlyout flyout)
            return;

        var popup = VisualTreeHelper.GetOpenPopupsForXamlRoot(flyout.Target?.XamlRoot)?.FirstOrDefault();
        if (popup?.Child is not MenuFlyoutPresenter presenter)
            return;


        if (DependencyObjectEx.FindDescendant(presenter, "RootStack") is not StackPanel rootStack || DependencyObjectEx.FindDescendant(presenter, "SecondaryItemsHost") is not ItemsRepeater secondaryRepeater || DependencyObjectEx.FindDescendant(presenter, "MainItemsPresenter") is not ItemsPresenter mainItems)
            return;

        // Bind ItemsRepeater to the collection
        secondaryRepeater.ItemsSource = GetSecondaryMenu(flyout).Items;

        // Reorder children
        var placement = GetSecondaryMenuPlacement(flyout);
        rootStack.Children.Clear();
        if (placement == MenuFlyoutSecondaryMenuPlacement.Top)
        {
            rootStack.Children.Add(secondaryRepeater);
            rootStack.Children.Add(new MenuFlyoutSeparator());
            rootStack.Children.Add(mainItems);
        }
        else
        {
            rootStack.Children.Add(mainItems);
            rootStack.Children.Add(new MenuFlyoutSeparator());
            rootStack.Children.Add(secondaryRepeater);
        }

        // Auto-close handling
        foreach (var item in GetSecondaryMenu(flyout).Items)
        {
            if (item is ButtonBase btn)
            {
                var oldHandler = GetAutoCloseHandler(btn);
                if (oldHandler != null)
                {
                    btn.Click -= oldHandler;
                    SetAutoCloseHandler(btn, null);
                }

                if (GetAutoCloseByClickOnSecondaryMenuItems(flyout))
                {
                    RoutedEventHandler handler = (s, args) => flyout.Hide();
                    btn.Click += handler;
                    SetAutoCloseHandler(btn, handler);
                }
            }
        }
    }
}
