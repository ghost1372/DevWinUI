namespace DevWinUI;

public interface INavigationServiceEx
{
    void UnregisterEvents();

    event NavigatedEventHandler FrameNavigated;
    IList<object>? MenuItems { get; }
    object? SettingsItem { get; }
    bool CanGoBack { get; }
    Frame? Frame { get; set; }
    bool NavigateTo(Type pageType, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null);
    bool GoBack();
    void Reset();
    void EnsureNavigationSelection(Type type);
    NavigationServiceEx Initialize(NavigationView navigationView, Frame frame);
    NavigationServiceEx ConfigureDefaultPage(Type defaultPage);
    NavigationServiceEx ConfigureSettingsPage(Type settingsPage);
    NavigationServiceEx ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary);
    NavigationServiceEx ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions);
    NavigationServiceEx ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, NavigationTransitionInfo navigationTransitionInfo);
    NavigationServiceEx ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, NavigationTransitionInfo navigationTransitionInfo);
    NavigationServiceEx ConfigureTitleBar(TitleBar titleBar);
    IDelegateCommand NavigateToCommand { get; }
}
