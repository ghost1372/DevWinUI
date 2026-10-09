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
    private const string DefaultJsonPath = @"Assets\NavViewMenu\AppData.json";

    private async void ConfigureJsonBase(string jsonFilePath, bool useTaskForDeserialization, PathType pathType, OrderItemsType orderItems)
    {
        JsonFilePath = jsonFilePath;
        _pathType = pathType;
        _orderItems = orderItems;
        DataSource.Instance.UseTaskForDeserialization = useTaskForDeserialization;
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);

        AddNavigationMenuItems(orderItems);
    }

    private JsonNavigationService ConfigureJsonCore(string jsonFilePath, bool useTaskForDeserialization, PathType pathType, OrderItemsType orderItems, ResourceManager resourceManager = null, ResourceContext resourceContext = null)
    {
        EnsureInitialized();
        if (resourceManager == null || resourceContext == null)
        {
            resourceManager = new ResourceManager();
            resourceContext = resourceManager.CreateResourceContext();
        }
        _resourceContext = resourceContext;
        _resourceManager = resourceManager;
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigureJsonBase(jsonFilePath, useTaskForDeserialization, pathType, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile()
        => ConfigureJsonCore(DefaultJsonPath, false, PathType.Relative, OrderItemsType.AscendingTopLevel);
    public JsonNavigationService ConfigureJsonFile(bool useTaskForDeserialization)
        => ConfigureJsonCore(DefaultJsonPath, useTaskForDeserialization, PathType.Relative, OrderItemsType.AscendingTopLevel);

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath)
        => ConfigureJsonCore(jsonFilePath, false, PathType.Relative, OrderItemsType.AscendingTopLevel);
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, bool useTaskForDeserialization)
        => ConfigureJsonCore(jsonFilePath, useTaskForDeserialization, PathType.Relative, OrderItemsType.AscendingTopLevel);

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, OrderItemsType orderItems)
        => ConfigureJsonCore(jsonFilePath, false, PathType.Relative, orderItems);
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, bool useTaskForDeserialization, OrderItemsType orderItems)
        => ConfigureJsonCore(jsonFilePath, useTaskForDeserialization, PathType.Relative, orderItems);

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType)
        => ConfigureJsonCore(jsonFilePath, false, pathType, OrderItemsType.AscendingTopLevel);
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, bool useTaskForDeserialization, PathType pathType)
        => ConfigureJsonCore(jsonFilePath, useTaskForDeserialization, pathType, OrderItemsType.AscendingTopLevel);

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, OrderItemsType orderItems)
        => ConfigureJsonCore(jsonFilePath, false, pathType, orderItems);
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, bool useTaskForDeserialization, PathType pathType, OrderItemsType orderItems)
        => ConfigureJsonCore(jsonFilePath, useTaskForDeserialization, pathType, orderItems);

    public JsonNavigationService ConfigureJsonFile(ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(DefaultJsonPath, false, PathType.Relative, OrderItemsType.AscendingTopLevel, resourceManager, resourceContext);
    public JsonNavigationService ConfigureJsonFile(bool useTaskForDeserialization, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(DefaultJsonPath, useTaskForDeserialization, PathType.Relative, OrderItemsType.AscendingTopLevel, resourceManager, resourceContext);

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(jsonFilePath, false, PathType.Relative, OrderItemsType.AscendingTopLevel, resourceManager, resourceContext);
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, bool useTaskForDeserialization, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(jsonFilePath, useTaskForDeserialization, PathType.Relative, OrderItemsType.AscendingTopLevel, resourceManager, resourceContext);

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, OrderItemsType orderItems, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(jsonFilePath, false, PathType.Relative, orderItems, resourceManager, resourceContext);
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, bool useTaskForDeserialization, OrderItemsType orderItems, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(jsonFilePath, useTaskForDeserialization, PathType.Relative, orderItems, resourceManager, resourceContext);

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(jsonFilePath, false, pathType, OrderItemsType.AscendingTopLevel, resourceManager, resourceContext);
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, bool useTaskForDeserialization, PathType pathType, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(jsonFilePath, useTaskForDeserialization, pathType, OrderItemsType.AscendingTopLevel, resourceManager, resourceContext);

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, OrderItemsType orderItems, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(jsonFilePath, false, pathType, orderItems, resourceManager, resourceContext);
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, bool useTaskForDeserialization, PathType pathType, OrderItemsType orderItems, ResourceManager resourceManager, ResourceContext resourceContext)
        => ConfigureJsonCore(jsonFilePath, useTaskForDeserialization, pathType, orderItems, resourceManager, resourceContext);
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
            _mainBreadcrumb.SettingsPageType = _settingsPage;
            _mainBreadcrumb.Initialize(Frame, _navigationView, pageDictionary);

            _useBreadcrumbBar = true;
            _mainBreadcrumb.ChangeBreadcrumbVisibility(false);
        }
    }
    private JsonNavigationService ConfigureBreadcrumbBarCore(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions? headerVisibilityOptions = null, NavigationTransitionInfo navigationTransitionInfo = null)
    {
        EnsureInitialized();
        if (headerVisibilityOptions.HasValue)
        {
            breadcrumbNavigator.HeaderVisibilityOptions = headerVisibilityOptions.Value;
        }
        if (navigationTransitionInfo != null)
        {
            breadcrumbNavigator.NavigationTransitionInfo = navigationTransitionInfo;
        }
        ConfigureBreadcrumbBarBase(breadcrumbNavigator, pageDictionary);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
        => ConfigureBreadcrumbBarCore(breadcrumbNavigator, pageDictionary);

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions)
        => ConfigureBreadcrumbBarCore(breadcrumbNavigator, pageDictionary, headerVisibilityOptions);

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, NavigationTransitionInfo navigationTransitionInfo)
        => ConfigureBreadcrumbBarCore(breadcrumbNavigator, pageDictionary, null, navigationTransitionInfo);

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, NavigationTransitionInfo navigationTransitionInfo)
        => ConfigureBreadcrumbBarCore(breadcrumbNavigator, pageDictionary, headerVisibilityOptions, navigationTransitionInfo);
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
            _titleBar.IsBackButtonVisible = Frame.CanGoBack;
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
