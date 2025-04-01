namespace DevWinUI;

[Obsolete("This class is obsolete and will be removed in a future version. Please use NavigateToCommand instead.")]

public partial class SettingsNavigationAttach
{
    public static SlideNavigationTransitionInfo GetSlideNavigationTransitionInfo(DependencyObject obj)
    {
        return (SlideNavigationTransitionInfo)obj.GetValue(SlideNavigationTransitionInfoProperty);
    }

    public static void SetSlideNavigationTransitionInfo(DependencyObject obj, SlideNavigationTransitionInfo value)
    {
        obj.SetValue(SlideNavigationTransitionInfoProperty, value);
    }

    public static readonly DependencyProperty SlideNavigationTransitionInfoProperty =
        DependencyProperty.RegisterAttached("SlideNavigationTransitionInfo", typeof(SlideNavigationTransitionInfo), typeof(SettingsNavigationAttach), new PropertyMetadata(new SlideNavigationTransitionInfo { Effect = SlideNavigationTransitionEffect.FromRight }));

    public static IJsonNavigationService GetJsonNavigationService(DependencyObject obj)
    {
        return (IJsonNavigationService)obj.GetValue(JsonNavigationServiceProperty);
    }

    public static void SetJsonNavigationService(DependencyObject obj, IJsonNavigationService value)
    {
        obj.SetValue(JsonNavigationServiceProperty, value);
    }

    public static readonly DependencyProperty JsonNavigationServiceProperty =
        DependencyProperty.RegisterAttached("JsonNavigationService", typeof(IJsonNavigationService), typeof(SettingsNavigationAttach),
        new PropertyMetadata(null, OnJsonNavigationServiceChanged));

    private static void OnJsonNavigationServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is IJsonNavigationService jsonNavigationService)
        {
            if (d is Panel panel)
            {
                panel.Loaded += (sender, args) =>
                {
                    var items = panel.Children.Cast<SettingsCard>();
                    foreach (var item in items)
                    {
                        void OnItemClick(object sender, RoutedEventArgs e)
                        {
                            var pageType = item?.GetValue(NavigationHelperEx.NavigateToSettingProperty) as Type;

                            if (pageType != null)
                            {
                                var effect = GetSlideNavigationTransitionInfo(panel);
                                jsonNavigationService.NavigateTo(pageType, item.Header, false, effect);
                            }
                        }

                        item.Click -= OnItemClick;
                        item.Click += OnItemClick;
                    }
                };
            }
        }
    }
}
