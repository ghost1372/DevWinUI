namespace DevWinUI;

public partial class NavigationServiceEx
{
    private bool _isInitialized;
    public NavigationServiceEx Initialize(NavigationView navigationView, Frame frame)
    {
        InitializeBase(navigationView, frame);

        _isInitialized = true;

        return this; // Enable chaining
    }

    public NavigationServiceEx ConfigureDefaultPage(Type defaultPage)
    {
        EnsureInitialized();
        _defaultPage = defaultPage;
        EnsureNavigationSelection(_defaultPage);
        return this;
    }

    public NavigationServiceEx ConfigureSettingsPage(Type settingsPage)
    {
        EnsureInitialized();
        _settingsPage = settingsPage;
        return this;
    }
    private void ConfigureBreadcrumbBarBase(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
    {
        _mainBreadcrumb = breadcrumbNavigator;
        _useBreadcrumbBar = false;

        if (_mainBreadcrumb != null)
        {
            _mainBreadcrumb.Visibility = Visibility.Collapsed;
            _mainBreadcrumb.Initialize(Frame, _navigationView, pageDictionary);

            _useBreadcrumbBar = true;
            _mainBreadcrumb.ChangeBreadcrumbVisibility(false);
        }
    }
    public NavigationServiceEx ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
    {
        EnsureInitialized();
        ConfigureBreadcrumbBarBase(breadcrumbNavigator, pageDictionary);
        return this;
    }

    public NavigationServiceEx ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions)
    {
        EnsureInitialized();
        breadcrumbNavigator.HeaderVisibilityOptions = headerVisibilityOptions;
        ConfigureBreadcrumbBarBase(breadcrumbNavigator, pageDictionary);
        return this;
    }

    public NavigationServiceEx ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, NavigationTransitionInfo navigationTransitionInfo)
    {
        EnsureInitialized();
        breadcrumbNavigator.NavigationTransitionInfo = navigationTransitionInfo;
        ConfigureBreadcrumbBarBase(breadcrumbNavigator, pageDictionary);
        return this;
    }

    public NavigationServiceEx ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, NavigationTransitionInfo navigationTransitionInfo)
    {
        EnsureInitialized();
        breadcrumbNavigator.HeaderVisibilityOptions = headerVisibilityOptions;
        breadcrumbNavigator.NavigationTransitionInfo = navigationTransitionInfo;
        ConfigureBreadcrumbBarBase(breadcrumbNavigator, pageDictionary);
        return this;
    }

    private void ConfigureTitleBarBase(TitleBar titleBar, bool autoManageBackButtonVisibility)
    {
        _titleBar = titleBar;
        titleBar.BackRequested -= OnBackRequested;
        titleBar.BackRequested += OnBackRequested;
        titleBar.PaneToggleRequested -= OnPaneToggleRequested;
        titleBar.PaneToggleRequested += OnPaneToggleRequested;
        _autoManageBackButtonVisibility = autoManageBackButtonVisibility;
        if (_autoManageBackButtonVisibility)
        {
            _titleBar.IsBackButtonVisible = _frame.CanGoBack;
        }

        _isTitlebarConfigured = true;
    }
    private void OnBackRequested(TitleBar sender, object args)
    {
        GoBack();
    }
    private void OnPaneToggleRequested(TitleBar sender, object args)
    {
        _navigationView.IsPaneOpen = !_navigationView.IsPaneOpen;
    }
    public NavigationServiceEx ConfigureTitleBar(TitleBar titleBar)
    {
        EnsureInitialized();
        ConfigureTitleBarBase(titleBar, true);
        return this;
    }
    public NavigationServiceEx ConfigureTitleBar(TitleBar titleBar, bool autoManageBackButtonVisibility)
    {
        EnsureInitialized();
        ConfigureTitleBarBase(titleBar, autoManageBackButtonVisibility);
        return this;
    }
    private void EnsureInitialized()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service must be initialized before configuration.");
    }
}
