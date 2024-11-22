using Microsoft.UI.Xaml.Controls.AnimatedVisuals;

namespace DevWinUI;

public partial class JsonNavigationViewService : PageServiceEx, IJsonNavigationViewService
{
    private void InitializeBase(NavigationView navigationView, Frame frame, Dictionary<string, Type> pages)
    {
        Reset();

        _navigationView = navigationView ?? throw new ArgumentNullException(nameof(navigationView));
        this.Frame = frame ?? throw new ArgumentNullException(nameof(frame));
        this._pageKeyToTypeMap = pages ?? throw new ArgumentNullException(nameof(pages));

        _navigationView.BackRequested -= OnBackRequested;
        _navigationView.BackRequested += OnBackRequested;
        _navigationView.ItemInvoked -= OnItemInvoked;
        _navigationView.ItemInvoked += OnItemInvoked;

        var settingItem = (NavigationViewItem)SettingsItem;
        if (settingItem != null)
        {
            settingItem.Icon = GetAnimatedSettingsIcon();
        }

        FrameNavigated -= OnNavigated;
        FrameNavigated += OnNavigated;
    }
    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        _navigationView.IsBackEnabled = CanGoBack;

        if (_isBackNavigation)
        {
            if (e.SourcePageType == _settingsPage)
            {
                _navigationView.SelectedItem = SettingsItem;
            }
            else
            {
                if (e.Parameter is DataItem dataItem)
                {
                    if (!string.IsNullOrEmpty(dataItem.UniqueId))
                    {
                        if (dataItem.UniqueId.Equals(_defaultPage?.ToString()))
                        {
                            EnsureNavigationSelection(_defaultPage?.ToString());
                        }
                        else
                        {
                            EnsureNavigationSelection(dataItem.UniqueId);
                        }
                    }
                }
                else if (e.Parameter is DataGroup dataGroup)
                {
                    if (!string.IsNullOrEmpty(dataGroup.UniqueId))
                    {
                        EnsureNavigationSelection(dataGroup.UniqueId);
                    }
                }
            }
        }

        _isBackNavigation = false; // Reset after navigation is handled
    }
    public void Reset()
    {
        if (_navigationView != null)
        {
            _navigationView.MenuItems?.Clear();
            _navigationView.FooterMenuItems?.Clear();
            _navigationView.BackRequested -= OnBackRequested;
            _navigationView.ItemInvoked -= OnItemInvoked;
        }

        _pageKeyToTypeMap?.Clear();
        _pageKeyToTypeMap = null;
        FrameNavigated -= OnNavigated;
        _navigationView = null;
        Frame = null;
    }
    private IconElement GetAnimatedSettingsIcon()
    {
        var animatedIcon = new AnimatedIcon();
        animatedIcon.Source = new AnimatedSettingsVisualSource();
        animatedIcon.FallbackIconSource = new FontIconSource() { Glyph = "\uE713" };
        return animatedIcon;
    }
    public void UnregisterEvents()
    {
        if (_navigationView != null)
        {
            _navigationView.BackRequested -= OnBackRequested;
            _navigationView.ItemInvoked -= OnItemInvoked;
        }
    }

    private void OnBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => GoBack();
    private void OnItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked)
        {
            if (GetPageType(SettingsPageKey) != null)
            {
                string pageTitle = string.Empty;
                var item = SettingsItem as NavigationViewItem;
                if (item != null && item.Content != null)
                {
                    pageTitle = item.Content?.ToString();
                }
                NavigateTo(SettingsPageKey, pageTitle);
            }
        }
        else
        {
            var selectedItem = args.InvokedItemContainer as NavigationViewItem;

            if (_sectionPage != null && selectedItem.DataContext is DataGroup itemGroup && !string.IsNullOrEmpty(itemGroup.SectionId))
            {
                NavigateTo(SectionPageKey, itemGroup);
            }
            else if (_sectionPage != null && selectedItem.DataContext is DataItem item && !string.IsNullOrEmpty(item.SectionId))
            {
                NavigateTo(SectionPageKey, item);
            }
            else
            {
                if (selectedItem?.GetValue(NavigationHelperEx.NavigateToProperty) is string pageKey)
                {
                    var dataItem = selectedItem?.DataContext as DataItem;
                    NavigateTo(pageKey, dataItem);
                }
            }
        }
    }

    private void OnAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null)
        {
            if (args.ChosenSuggestion is DataItem)
            {
                var infoDataItem = args.ChosenSuggestion as DataItem;
                NavigateTo(infoDataItem.UniqueId, infoDataItem);
            }
        }
    }

    private void OnAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        var matches = SearchNavigationViewItems(DataSource.Instance.Groups.SelectMany(group => group.Items), sender.Text);

        if (matches.Any())
        {
            foreach (var item in matches)
            {
                if (string.IsNullOrEmpty(item.ImagePath))
                {
                    item.ImagePath = _autoSuggestBoxNotFoundImagePath;
                }
            }
            _autoSuggestBox.ItemsSource = matches.OrderByDescending(i => i.Title.StartsWith(sender.Text.ToLowerInvariant())).ThenBy(i => i.Title);
        }
        else
        {
            var noResultsItem = new DataItem();
            noResultsItem.Title = _autoSuggestBoxNotFoundString;
            noResultsItem.ImagePath = _autoSuggestBoxNotFoundImagePath;

            var noResultsList = new List<DataItem>();
            noResultsList.Add(noResultsItem);
            _autoSuggestBox.ItemsSource = noResultsList;
        }
    }

    public IEnumerable<DataItem> SearchNavigationViewItems(IEnumerable<DataItem> items, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            yield break;

        query = query.Trim();

        foreach (var item in items)
        {
            if (!string.IsNullOrEmpty(item.Title)
                && item.Title.Contains(query, StringComparison.OrdinalIgnoreCase)
                && item.UniqueId != null)
            {
                yield return item;
            }
        }
    }

    public async void GetMenuItemsAsync(string jsonFilePath, PathType pathType = PathType.Relative)
    {
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);
    }

    public void EnsureNavigationSelection(string id)
    {
        foreach (object rawGroup in this.AllMenuItems)
        {
            if (rawGroup is NavigationViewItem group)
            {
                if ((string)group.Tag == id)
                {
                    group.IsSelected = true;
                    _navigationView.SelectedItem = group;

                    // Only expand if it's back navigation
                    if (_isBackNavigation)
                    {
                        group.IsExpanded = true;
                    }
                    return;
                }

                if (group.MenuItems.Count > 0)
                {
                    foreach (object rawItem in group.MenuItems)
                    {
                        EnsureNavigationSelectionBase(rawItem, id, group);
                    }
                }
            }
        }
    }

    private void EnsureNavigationSelectionBase(object rawItem, string id, NavigationViewItem parentGroup)
    {
        if (rawItem is NavigationViewItem item)
        {
            if ((string)item.Tag == id)
            {
                _navigationView.SelectedItem = item;
                item.IsSelected = true;

                // Only expand the parent group if it's back navigation
                if (_isBackNavigation)
                {
                    parentGroup.IsExpanded = true;
                }
                return;
            }

            if (item.MenuItems.Count > 0)
            {
                foreach (var rawInnerItem in item.MenuItems)
                {
                    EnsureNavigationSelectionBase(rawInnerItem, id, parentGroup);
                }
            }
        }
    }

}
