namespace DevWinUI;

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

    public static IJsonNavigationViewService GetJsonNavigationViewService(DependencyObject obj)
    {
        return (IJsonNavigationViewService)obj.GetValue(JsonNavigationViewServiceProperty);
    }

    public static void SetJsonNavigationViewService(DependencyObject obj, IJsonNavigationViewService value)
    {
        obj.SetValue(JsonNavigationViewServiceProperty, value);
    }

    public static readonly DependencyProperty JsonNavigationViewServiceProperty =
        DependencyProperty.RegisterAttached("JsonNavigationViewService", typeof(IJsonNavigationViewService), typeof(SettingsNavigationAttach),
        new PropertyMetadata(null, OnJsonNavigationViewServiceChanged));

    private static void OnJsonNavigationViewServiceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue is IJsonNavigationViewService jsonNavigationService)
        {
            if (d is Panel panel)
            {
                // Optional: Handle the Loaded event to set the default again when the ComboBox is loaded
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
