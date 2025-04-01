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

    public JsonNavigationService ConfigureJsonFile()
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(@"Assets\NavViewMenu\AppData.json", PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath)
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(jsonFilePath, PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, OrderItemsType orderItems)
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(jsonFilePath, PathType.Relative, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType)
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(jsonFilePath, pathType, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, OrderItemsType orderItems)
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        var resourceContext = resourceManager.CreateResourceContext();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(jsonFilePath, pathType, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(@"Assets\NavViewMenu\AppData.json", PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(jsonFilePath, PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, OrderItemsType orderItems, ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(jsonFilePath, PathType.Relative, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(jsonFilePath, pathType, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, OrderItemsType orderItems, ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        InternalLocalizationHelper.InitializeInternalLocalization(resourceManager, resourceContext);
        ConfigJsonBase(jsonFilePath, pathType, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureDefaultPage(Type defaultPage)
    {
        EnsureInitialized();
        ConfigDefaultPage(defaultPage);
        return this;
    }

    public JsonNavigationService ConfigureSettingsPage(Type settingsPage)
    {
        EnsureInitialized();
        ConfigSettingsPage(settingsPage);
        return this;
    }

    public JsonNavigationService ConfigureSectionPage(Type sectionPage)
    {
        EnsureInitialized();
        ConfigSectionPage(sectionPage);
        return this;
    }

    public JsonNavigationService ConfigureAutoSuggestBox(AutoSuggestBox autoSuggestBox, bool useItemTemplate, string notFoundString, string notFoundImagePath)
    {
        EnsureInitialized();
        ConfigAutoSuggestBox(autoSuggestBox, useItemTemplate, notFoundString, notFoundImagePath);
        return this;
    }
    public JsonNavigationService ConfigureAutoSuggestBox(AutoSuggestBox autoSuggestBox)
    {
        EnsureInitialized();
        ConfigAutoSuggestBox(autoSuggestBox, true, null, null);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
    {
        EnsureInitialized();
        ConfigBreadcrumbBar(breadcrumbNavigator, pageDictionary);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions)
    {
        EnsureInitialized();
        ConfigBreadcrumbBar(breadcrumbNavigator, pageDictionary, headerVisibilityOptions);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, NavigationTransitionInfo navigationTransitionInfo)
    {
        EnsureInitialized();
        ConfigBreadcrumbBar(breadcrumbNavigator, pageDictionary, navigationTransitionInfo);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, NavigationTransitionInfo navigationTransitionInfo)
    {
        EnsureInitialized();
        ConfigBreadcrumbBar(breadcrumbNavigator, pageDictionary, headerVisibilityOptions, navigationTransitionInfo);
        return this;
    }
    public JsonNavigationService ConfigureTitleBar(TitleBar titleBar)
    {
        EnsureInitialized();
        ConfigTitleBar(titleBar, true);
        return this;
    }
    public JsonNavigationService ConfigureTitleBar(TitleBar titleBar, bool autoManageBackButtonVisibility)
    {
        EnsureInitialized();
        ConfigTitleBar(titleBar, autoManageBackButtonVisibility);
        return this;
    }
    private void EnsureInitialized()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service must be initialized before configuration.");
    }
}
