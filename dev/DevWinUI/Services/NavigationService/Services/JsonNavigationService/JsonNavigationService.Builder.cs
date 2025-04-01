using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;

public partial class JsonNavigationService
{
    private bool _isInitialized;
    public JsonNavigationService Initialize(NavigationView navigationView, Frame frame, Dictionary<string, Type> pages)
    {
        InitializeBase(navigationView, frame, pages);

        _isInitialized = true;

        return this; // Enable chaining
    }
    private async void ConfigureJsonBase(string jsonFilePath, PathType pathType, OrderItemsType orderItems)
    {
        JsonFilePath = jsonFilePath;
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);

        AddNavigationMenuItems(orderItems);
    }
    public JsonNavigationService ConfigureJsonFile()
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(@"Assets\NavViewMenu\AppData.json", PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath)
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, OrderItemsType orderItems)
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, PathType.Relative, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType)
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, pathType, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, OrderItemsType orderItems)
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, pathType, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(@"Assets\NavViewMenu\AppData.json", PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, OrderItemsType orderItems, ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, PathType.Relative, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, pathType, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, OrderItemsType orderItems, ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, pathType, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureDefaultPage(Type defaultPage)
    {
        EnsureInitialized();

        _defaultPage = defaultPage;

        return this;
    }

    public JsonNavigationService ConfigureSettingsPage(Type settingsPage)
    {
        EnsureInitialized();

        _settingsPage = settingsPage;
        SetSettingsPage(_settingsPage);

        return this;
    }

    public JsonNavigationService ConfigureSectionPage(Type sectionPage)
    {
        EnsureInitialized();

        _sectionPage = sectionPage;
        SetSectionPage(_sectionPage);

        return this;
    }
    private void ConfigureAutoSuggestBoxBase(AutoSuggestBox autoSuggestBox, bool useItemTemplate = true, string autoSuggestBoxNotFoundString = null, string autoSuggestBoxNotFoundImagePath = null)
    {
        _autoSuggestBox = autoSuggestBox;

        if (_autoSuggestBox != null)
        {
            _autoSuggestBoxNotFoundString = autoSuggestBoxNotFoundString;
            _autoSuggestBoxNotFoundImagePath = autoSuggestBoxNotFoundImagePath;

            if (string.IsNullOrEmpty(_autoSuggestBoxNotFoundString))
            {
                _autoSuggestBoxNotFoundString = "No result found";
            }

            if (string.IsNullOrEmpty(_autoSuggestBoxNotFoundImagePath))
            {
                _autoSuggestBoxNotFoundImagePath = "ms-appx:///Assets/icon.png";
            }

            _autoSuggestBox.TextChanged -= OnAutoSuggestBox_TextChanged;
            _autoSuggestBox.TextChanged += OnAutoSuggestBox_TextChanged;
            _autoSuggestBox.QuerySubmitted -= OnAutoSuggestBox_QuerySubmitted;
            _autoSuggestBox.QuerySubmitted += OnAutoSuggestBox_QuerySubmitted;

            if (useItemTemplate)
            {
                _autoSuggestBox.Resources.MergedDictionaries.AddIfNotExists(new InternalAutoSuggestBoxItemTemplate());
                _autoSuggestBox.ItemTemplate = _autoSuggestBox.Resources["InternalAutoSuggestBoxItemTemplate"] as DataTemplate;
            }
        }
    }

    public JsonNavigationService ConfigureAutoSuggestBox(AutoSuggestBox autoSuggestBox, bool useItemTemplate, string notFoundString, string notFoundImagePath)
    {
        EnsureInitialized();
        ConfigureAutoSuggestBoxBase(autoSuggestBox, useItemTemplate, notFoundString, notFoundImagePath);
        return this;
    }
    public JsonNavigationService ConfigureAutoSuggestBox(AutoSuggestBox autoSuggestBox)
    {
        EnsureInitialized();
        ConfigureAutoSuggestBoxBase(autoSuggestBox, true, null, null);
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
    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
    {
        EnsureInitialized();
        ConfigureBreadcrumbBarBase(breadcrumbNavigator, pageDictionary);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions)
    {
        EnsureInitialized();
        breadcrumbNavigator.HeaderVisibilityOptions = headerVisibilityOptions;
        ConfigureBreadcrumbBarBase(breadcrumbNavigator, pageDictionary);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, NavigationTransitionInfo navigationTransitionInfo)
    {
        EnsureInitialized();
        breadcrumbNavigator.NavigationTransitionInfo = navigationTransitionInfo;
        ConfigureBreadcrumbBarBase(breadcrumbNavigator, pageDictionary);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, NavigationTransitionInfo navigationTransitionInfo)
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
    private void OnPaneToggleRequested(TitleBar sender, object args)
    {
        _navigationView.IsPaneOpen = !_navigationView.IsPaneOpen;
    }

    private void OnBackRequested(TitleBar sender, object args)
    {
        GoBack();
    }
    public JsonNavigationService ConfigureTitleBar(TitleBar titleBar)
    {
        EnsureInitialized();
        ConfigureTitleBarBase(titleBar, true);
        return this;
    }
    public JsonNavigationService ConfigureTitleBar(TitleBar titleBar, bool autoManageBackButtonVisibility)
    {
        EnsureInitialized();
        ConfigureTitleBarBase(titleBar, autoManageBackButtonVisibility);
        return this;
    }
    public JsonNavigationService ConfigFontFamilyForGlyph(string fontFamily)
    {
        EnsureInitialized();
        _fontFamilyForGlyph = fontFamily;
        return this;
    }
    private void EnsureInitialized()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service must be initialized before configuration.");
    }
}
