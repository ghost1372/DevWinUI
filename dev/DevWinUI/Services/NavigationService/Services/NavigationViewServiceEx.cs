using System.Diagnostics.CodeAnalysis;

namespace DevWinUI;

public partial class NavigationViewServiceEx : INavigationViewServiceEx
{
    private readonly INavigationServiceEx _navigationService;

    private readonly IPageServiceEx _pageService;

    private NavigationView? _navigationView;
    private AutoSuggestBox? _autoSuggestBox;

    public IList<object>? MenuItems => _navigationView?.MenuItems;

    public object? SettingsItem => _navigationView?.SettingsItem;

    private string _notFoundString;

    public NavigationViewServiceEx(INavigationServiceEx navigationService, IPageServiceEx pageService)
    {
        _navigationService = navigationService;
        _pageService = pageService;
    }

    [MemberNotNull(nameof(_navigationView))]
    public void Initialize(NavigationView navigationView)
    {
        _navigationView = navigationView;
        _navigationView.BackRequested += OnBackRequested;
        _navigationView.ItemInvoked += OnItemInvoked;

        _navigationService.Navigated += (s, e) =>
        {
            navigationView.IsBackEnabled = _navigationService.CanGoBack;
            var selectedItem = GetSelectedItem(e.SourcePageType);
            if (selectedItem != null)
            {
                navigationView.SelectedItem = selectedItem;
            }
        };

        if (_pageService.GetPageType(_pageService.DefaultPageKey) != null)
        {
            _navigationService.NavigateTo(_pageService.DefaultPageKey);
        }
    }

    public void ConfigAutoSuggestBox(AutoSuggestBox autoSuggestBox, string notFoundString)
    {
        _autoSuggestBox = autoSuggestBox;

        if (_autoSuggestBox != null)
        {
            _notFoundString = notFoundString;
            if (string.IsNullOrEmpty(_notFoundString))
            {
                _notFoundString = "No result found";
            }

            _autoSuggestBox.TextChanged += _autoSuggestBox_TextChanged;
            _autoSuggestBox.QuerySubmitted += _autoSuggestBox_QuerySubmitted;
        }
    }

    private void _autoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null)
        {
            var item = args.ChosenSuggestion as string;
            var matches = GetNavigationViewItems(_navigationView.MenuItems, item);
            var pageType = matches.GetValue(NavigationHelperEx.NavigateToProperty);
            if (pageType != null)
            {
                _navigationService.NavigateTo(pageType.ToString());
            }
        }
    }

    private void _autoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var suggestions = new List<string>();
            var matches = SearchNavigationViewItems(_navigationView.MenuItems, sender.Text);

            var querySplit = _autoSuggestBox.Text.Split(' ');
            var matchingItems = matches.Where(
                item =>
                {
                    var flag = true;
                    foreach (var queryToken in querySplit)
                    {
                        if (item.Content.ToString().IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                        {
                            flag = false;
                        }

                    }
                    return flag;
                });

            foreach (var item in matchingItems)
            {
                suggestions.Add(item.Content.ToString());
            }
            _autoSuggestBox.ItemsSource = suggestions.Count > 0 ? suggestions : (new string[] { _notFoundString });
        }
    }

    private IEnumerable<NavigationViewItem> SearchNavigationViewItems(IList<object> items, string query)
    {
        query = query.Trim()?.ToLowerInvariant();

        foreach (NavigationViewItem item in items)
        {
            if (item.Content.ToString().ToLowerInvariant().Contains(query) && item.GetValue(NavigationHelperEx.NavigateToProperty) != null)
            {
                yield return item;
            }

            if (item.MenuItems != null && item.MenuItems.Any())
            {
                var nestedMatches = SearchNavigationViewItems(item.MenuItems, query);
                foreach (var nestedMatch in nestedMatches)
                {
                    yield return nestedMatch;
                }
            }
        }
    }

    private NavigationViewItem GetNavigationViewItems(IEnumerable<object> items, string query)
    {
        query = query.Trim()?.ToLowerInvariant();

        foreach (var item in items)
        {
            if (item is NavigationViewItem navigationItem)
            {
                if (navigationItem.Content.ToString().ToLowerInvariant().Equals(query, StringComparison.OrdinalIgnoreCase))
                {
                    return navigationItem;
                }

                if (navigationItem.MenuItems != null && navigationItem.MenuItems.Any())
                {
                    var nestedMatch = GetNavigationViewItems(navigationItem.MenuItems, query);
                    if (nestedMatch != null)
                    {
                        return nestedMatch;
                    }
                }
            }
        }

        return null;
    }

    public void UnregisterEvents()
    {
        if (_navigationView != null)
        {
            _navigationView.BackRequested -= OnBackRequested;
            _navigationView.ItemInvoked -= OnItemInvoked;
        }
    }

    public NavigationViewItem? GetSelectedItem(Type pageType)
    {
        if (_navigationView != null)
        {
            return GetSelectedItem(_navigationView.MenuItems, pageType) ?? GetSelectedItem(_navigationView.FooterMenuItems, pageType);
        }

        return null;
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => _navigationService.GoBack();

    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            if (_pageService.GetPageType(_pageService.SettingsPageKey) != null)
            {
                _navigationService.NavigateTo(_pageService.SettingsPageKey);
            }
        }
        else
        {
            var selectedItem = args.InvokedItemContainer as NavigationViewItem;

            if (selectedItem?.GetValue(NavigationHelperEx.NavigateToProperty) is string pageKey)
            {
                _navigationService.NavigateTo(pageKey);
            }
        }
    }

    private NavigationViewItem? GetSelectedItem(IEnumerable<object> menuItems, Type pageType)
    {
        foreach (var item in menuItems.OfType<NavigationViewItem>())
        {
            if (IsMenuItemForPageType(item, pageType))
            {
                return item;
            }

            var selectedChild = GetSelectedItem(item.MenuItems, pageType);
            if (selectedChild != null)
            {
                return selectedChild;
            }
        }

        return null;
    }

    private bool IsMenuItemForPageType(NavigationViewItem menuItem, Type sourcePageType)
    {
        if (menuItem.GetValue(NavigationHelperEx.NavigateToProperty) is string pageKey)
        {
            return _pageService.GetPageType(pageKey) == sourcePageType;
        }

        return false;
    }
}
