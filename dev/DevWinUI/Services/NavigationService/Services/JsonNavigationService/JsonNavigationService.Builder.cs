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
    
    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, OrderItemsType orderItems)
    {
        EnsureInitialized();
        ConfigJsonBase(jsonFilePath, pathType, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath)
    {
        EnsureInitialized();
        ConfigJsonBase(jsonFilePath, PathType.Relative, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, OrderItemsType orderItems)
    {
        EnsureInitialized();
        ConfigJsonBase(jsonFilePath, PathType.Relative, orderItems);
        return this;
    }

    public JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType)
    {
        EnsureInitialized();
        ConfigJsonBase(jsonFilePath, pathType, OrderItemsType.AscendingTopLevel);
        return this;
    }

    public JsonNavigationService ConfigureDefaultPage(Type defaultPage)
    {
        EnsureInitialized();
        ConfigDefaultPage(defaultPage);
        NavigateTo(_defaultPage, new DataItem { UniqueId = _defaultPage?.ToString() });
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
    
    public JsonNavigationService ConfigureLocalizer()
    {
        EnsureInitialized();
        var resourceManager = new ResourceManager();
        ConfigLocalizerBase(resourceManager, resourceManager.CreateResourceContext());
        return this;
    }
    public JsonNavigationService ConfigureLocalizer(ResourceManager resourceManager)
    {
        EnsureInitialized();
        ConfigLocalizerBase(resourceManager, resourceManager.CreateResourceContext());
        return this;
    }

    public JsonNavigationService ConfigureLocalizer(ResourceManager resourceManager, ResourceContext resourceContext)
    {
        EnsureInitialized();
        ConfigLocalizerBase(resourceManager, resourceContext);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
    {
        EnsureInitialized();
        ConfigBreadcrumbBar(breadcrumbBar, pageDictionary);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions)
    {
        EnsureInitialized();
        ConfigBreadcrumbBar(breadcrumbBar, pageDictionary, headerVisibilityOptions);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, bool allowDuplication)
    {
        EnsureInitialized();
        ConfigBreadcrumbBar(breadcrumbBar, pageDictionary, allowDuplication);
        return this;
    }

    public JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, bool allowDuplication)
    {
        EnsureInitialized();
        ConfigBreadcrumbBar(breadcrumbBar, pageDictionary, headerVisibilityOptions, allowDuplication);
        return this;
    }
    public JsonNavigationService ConfigureTitleBar(TitleBar titleBar)
    {
        EnsureInitialized();
        ConfigTitleBar(titleBar);
        return this;
    }

    private void EnsureInitialized()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service must be initialized before configuration.");
    }
}
