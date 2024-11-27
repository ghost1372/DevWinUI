﻿using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.Windows.ApplicationModel.Resources;

namespace DevWinUI;

public interface IJsonNavigationService
{
    void UnregisterEvents();

    event NavigatedEventHandler FrameNavigated;
    IList<object>? MenuItems { get; }
    object? SettingsItem { get; }
    IEnumerable<DataItem> SearchNavigationViewItems(IEnumerable<DataItem> items, string query);

    bool CanGoBack { get; }
    Frame? Frame { get; set; }
    Window? Window { get; set; }
    bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null);
    bool NavigateTo(Type pageType, object? parameter = null, bool clearNavigation = false, NavigationTransitionInfo transitionInfo = null);
    bool GoBack();
    void GetMenuItemsAsync(string jsonFilePath, PathType pathType = PathType.Relative);
    void Reset();
    void EnsureNavigationSelection(string id);
    JsonNavigationService Initialize(NavigationView navigationView, Frame frame, Dictionary<string, Type> pages);
    JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType, OrderItemsType orderItems);
    JsonNavigationService ConfigureJsonFile(string jsonFilePath);
    JsonNavigationService ConfigureJsonFile(string jsonFilePath, OrderItemsType orderItems);
    JsonNavigationService ConfigureJsonFile(string jsonFilePath, PathType pathType);
    JsonNavigationService ConfigureDefaultPage(Type defaultPage);
    JsonNavigationService ConfigureSettingsPage(Type settingsPage);
    JsonNavigationService ConfigureSectionPage(Type sectionPage);
    JsonNavigationService ConfigureAutoSuggestBox(AutoSuggestBox autoSuggestBox);
    JsonNavigationService ConfigureAutoSuggestBox(AutoSuggestBox autoSuggestBox, bool useItemTemplate, string notFoundString, string notFoundImagePath);
    JsonNavigationService ConfigureLocalizer();
    JsonNavigationService ConfigureLocalizer(ResourceManager resourceManager);
    JsonNavigationService ConfigureLocalizer(ResourceManager resourceManager, ResourceContext resourceContext);
    JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary);
    JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions);
    JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, bool allowDuplication);
    JsonNavigationService ConfigureBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, bool allowDuplication);
    JsonNavigationService ConfigureTitleBar(TitleBar titleBar);
}
