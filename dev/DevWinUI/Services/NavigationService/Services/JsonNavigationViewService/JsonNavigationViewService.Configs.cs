using Microsoft.Windows.ApplicationModel.Resources;
using System.Collections.ObjectModel;

namespace DevWinUI;

public partial class JsonNavigationViewService
{
    private async void ConfigJsonBase(string jsonFilePath, PathType pathType, OrderItemsType orderItems)
    {
        JsonFilePath = jsonFilePath;
        await DataSource.Instance.GetGroupsAsync();

        AddNavigationMenuItems(orderItems);

        EnsureNavigationSelection(_defaultPage?.ToString());
    }

    private void ConfigAutoSuggestBox(AutoSuggestBox autoSuggestBox, bool useItemTemplate = true, string autoSuggestBoxNotFoundString = null, string autoSuggestBoxNotFoundImagePath = null)
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

    private void ConfigDefaultPage(Type DefaultPage)
    {
        _defaultPage = DefaultPage;
    }

    private void ConfigSettingsPage(Type SettingsPage)
    {
        _settingsPage = SettingsPage;
        SetSettingsPage(_settingsPage);
    }

    private void ConfigSectionPage(Type sectionPage)
    {
        _sectionPage = sectionPage;
        SetSectionPage(_sectionPage);
    }

    private void ConfigFontFamilyForGlyph(string fontFamily)
    {
        _fontFamilyForGlyph = fontFamily;
    }

    private void ConfigLocalizerBase(ResourceManager resourceManager, ResourceContext resourceContext)
    {
        ResourceManager = resourceManager;
        ResourceContext = resourceContext;
    }

    private void ConfigBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
    {
        _mainBreadcrumb = breadcrumbBar;
        _useBreadcrumbBar = false;

        if (_mainBreadcrumb != null)
        {
            _mainBreadcrumb.NavigationView = _navigationView;
            _mainBreadcrumb.InternalFrame = Frame;
            _mainBreadcrumb.PageDictionary = pageDictionary;
            _mainBreadcrumb.Visibility = Visibility.Collapsed;
            _mainBreadcrumb.Initialize();

            _mainBreadcrumb.BreadCrumbs = new ObservableCollection<NavigationBreadcrumb>();
            _useBreadcrumbBar = true;
            _mainBreadcrumb.ItemClicked -= MainBreadcrumb_ItemClicked;
            _mainBreadcrumb.ItemClicked += MainBreadcrumb_ItemClicked;
            _mainBreadcrumb.ChangeBreadcrumbVisibility(false);
        }
    }

    private void ConfigBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions)
    {
        breadcrumbBar.HeaderVisibilityOptions = headerVisibilityOptions;
        ConfigBreadcrumbBar(breadcrumbBar, pageDictionary);
    }

    private void ConfigBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, bool allowDuplication)
    {
        _allowDuplication = allowDuplication;
        ConfigBreadcrumbBar(breadcrumbBar, pageDictionary);
    }

    private void ConfigBreadcrumbBar(BreadcrumbNavigator breadcrumbBar, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, bool allowDuplication)
    {
        breadcrumbBar.HeaderVisibilityOptions = headerVisibilityOptions;
        ConfigBreadcrumbBar(breadcrumbBar, pageDictionary, allowDuplication);
    }

    private void ConfigTitleBar(TitleBar titleBar)
    {
        _titleBar = titleBar;
        titleBar.BackRequested -= OnBackRequested;
        titleBar.BackRequested += OnBackRequested;
        titleBar.PaneToggleRequested -= OnPaneToggleRequested;
        titleBar.PaneToggleRequested += OnPaneToggleRequested;
        _titleBar.IsBackButtonVisible = _frame.CanGoBack;
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
}
