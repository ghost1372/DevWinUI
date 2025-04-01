namespace DevWinUI;

public partial class JsonNavigationService
{
    private async void ConfigJsonBase(string jsonFilePath, PathType pathType, OrderItemsType orderItems)
    {
        JsonFilePath = jsonFilePath;
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);

        AddNavigationMenuItems(orderItems);
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

    private void ConfigBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
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

    private void ConfigBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions)
    {
        breadcrumbNavigator.HeaderVisibilityOptions = headerVisibilityOptions;
        ConfigBreadcrumbBar(breadcrumbNavigator, pageDictionary);
    }

    private void ConfigBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, NavigationTransitionInfo navigationTransitionInfo)
    {
        breadcrumbNavigator.NavigationTransitionInfo = navigationTransitionInfo;
        ConfigBreadcrumbBar(breadcrumbNavigator, pageDictionary);
    }

    private void ConfigBreadcrumbBar(BreadcrumbNavigator breadcrumbNavigator, Dictionary<Type, BreadcrumbPageConfig> pageDictionary, BreadcrumbNavigatorHeaderVisibilityOptions headerVisibilityOptions, NavigationTransitionInfo navigationTransitionInfo)
    {
        breadcrumbNavigator.HeaderVisibilityOptions = headerVisibilityOptions;
        breadcrumbNavigator.NavigationTransitionInfo = navigationTransitionInfo;
        ConfigBreadcrumbBar(breadcrumbNavigator, pageDictionary);
    }

    private void ConfigTitleBar(TitleBar titleBar, bool autoManageBackButtonVisibility)
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
}
