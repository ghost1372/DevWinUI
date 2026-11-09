using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;

namespace DevWinUI;

public partial class JsonNavigationService : PageServiceEx, IJsonNavigationService
{
    public JsonNavigationService()
    {
        NavigateToCommand = DelegateCommand.Create(OnNavigateToCommand);
    }

    private void OnNavigateToCommand(object? parameter)
    {
        if (parameter is NavigationParameterExtension navigationParameter)
        {
            NavigateTo(navigationParameter.PageType, navigationParameter.BreadCrumbHeader, false, navigationParameter.NavigationTransitionInfo);
        }
    }

    private void InitializeBase(NavigationView navigationView, Frame frame, Dictionary<string, Type> pages)
    {
        var preservedPages = new Dictionary<string, Type>(pages);

        Reset();

        _navigationView = navigationView ?? throw new ArgumentNullException(nameof(navigationView));
        this.Frame = frame ?? throw new ArgumentNullException(nameof(frame));
        this._pageKeyToTypeMap = preservedPages ?? throw new ArgumentNullException(nameof(preservedPages));

        _navigationView.BackRequested -= OnNavigationViewBackRequested;
        _navigationView.BackRequested += OnNavigationViewBackRequested;
        _navigationView.SelectionChanged -= OnNavigationViewSelectionChanged;
        _navigationView.SelectionChanged += OnNavigationViewSelectionChanged;

        if (SettingsItem is NavigationViewItem settingItem)
        {
            settingItem.Icon = GetAnimatedSettingsIcon();
        }

        FrameNavigated -= OnNavigated;
        FrameNavigated += OnNavigated;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (_navigationView == null || Frame == null)
        {
            return;
        }

        _navigationView.IsBackEnabled = CanGoBack;

        if (_isTitlebarConfigured && _autoManageBackButtonVisibility)
        {
            _titleBar.IsBackButtonVisible = Frame.CanGoBack;
        }

        if (e.SourcePageType == _settingsPage)
        {
            _navigationView.SelectedItem = SettingsItem;
        }
        else
        {
            var selectionId = e.SourcePageType == _defaultPage
               ? _defaultPage?.ToString()
               : (e.Parameter as BaseDataInfo)?.UniqueId;
            if (string.IsNullOrEmpty(selectionId) && _pendingSelectionPageType == e.SourcePageType)
            {
                selectionId = _pendingSelectionId;
            }

            if (!string.IsNullOrEmpty(selectionId) && _itemMap.ContainsKey(selectionId))
            {
                // Frame already navigated (e.g. back/forward), so selection must not navigate again
                _lastParameterUsed = e.Parameter;
                if (_navigationView.SelectedItem is not NavigationViewItem currentItem || !selectionId.Equals(currentItem.Tag as string))
                {
                    var wasSelectionSuppressed = _suppressSelectionNavigation;
                    _suppressSelectionNavigation = true;
                    try
                    {
                        EnsureNavigationSelection(selectionId);
                    }
                    finally
                    {
                        _suppressSelectionNavigation = wasSelectionSuppressed;
                    }
                }
            }
        }
    }
    public void Reset()
    {
        if (_navigationView != null)
        {
            _navigationView.MenuItems?.Clear();
            _navigationView.FooterMenuItems?.Clear();
            _navigationView.BackRequested -= OnNavigationViewBackRequested;
            _navigationView.SelectionChanged -= OnNavigationViewSelectionChanged;
        }

        _itemMap.Clear();
        _pageKeyToTypeMap?.Clear();
        _pageKeyToTypeMap = null;
        FrameNavigated -= OnNavigated;
        _navigationView = null;
        Frame = null;
        _jsonConfigurationVersion++;
        _suppressSelectionNavigation = false;
        _pendingSelectionId = null;
        _pendingSelectionPageType = null;
        _lastParameterUsed = null;
        _pendingClearNavigations.Clear();
        DataSource.Instance.Reset();
    }
    public void ReInitialize()
    {
        InitializeBase(_navigationView, Frame, _pageKeyToTypeMap);

        InternalLocalizationHelper.InitializeInternalLocalization(_resourceManager, _resourceContext);

        var configurationVersion = ++_jsonConfigurationVersion;
        _jsonLoadTask = ConfigureJsonBase(JsonFilePath, DataSource.Instance.UseTaskForDeserialization, _pathType, _orderItems, configurationVersion);
    }

    private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (_suppressSelectionNavigation)
        {
            return;
        }

        if (args.IsSettingsSelected)
        {
            string pageTitle = string.Empty;
            var item = SettingsItem as NavigationViewItem;
            if (item != null && item.Content != null)
            {
                pageTitle = item.Content.ToString();
            }
            DeferNavigation(sender, SettingsItem, () => NavigateTo(SettingsPageKey, pageTitle));
        }
        else
        {
            if (args.SelectedItemContainer is NavigationViewItem selectedItem && selectedItem.DataContext is BaseDataInfo dataInfo)
            {
                var targetKey = !string.IsNullOrEmpty(dataInfo.SectionId) && _sectionPage != null
                                ? SectionPageKey
                                : dataInfo.UniqueId;

                DeferNavigation(sender, selectedItem, () => NavigateTo(targetKey, dataInfo));
            }
        }
    }

    // Lets NavigationView draw the selection indicator before the page is created
    private void DeferNavigation(NavigationView sender, object? selected, Action navigate)
    {
        if (!_deferNavigation)
        {
            navigate();
            return;
        }

        var queue = sender.DispatcherQueue;
        if (queue == null || !queue.TryEnqueue(DispatcherQueuePriority.Low, () =>
        {
            // Skip if the selection changed (back, another click) while waiting
            if (ReferenceEquals(_navigationView, sender) && ReferenceEquals(sender.SelectedItem, selected))
            {
                navigate();
            }
        }))
        {
            navigate();
        }
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
            _navigationView.BackRequested -= OnNavigationViewBackRequested;
            _navigationView.SelectionChanged -= OnNavigationViewSelectionChanged;
        }
    }

    private void OnNavigationViewBackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args) => GoBack();

    private void OnAutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        if (args.ChosenSuggestion != null && args.ChosenSuggestion is DataItem infoDataItem)
        {
            var hasChangedSelection = SelectItemById(infoDataItem.UniqueId);

            // In case the menu selection has changed, it means that it has triggered
            // the selection changed event, that will navigate to the page already
            if (!hasChangedSelection)
            {
                NavigateTo(infoDataItem.UniqueId, infoDataItem);
            }
        }
    }

    private void OnAutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        {
            var query = sender.Text.Trim();
            if (query.Length == 0)
            {
                _autoSuggestBox.ItemsSource = null;
                return;
            }

            var suggestions = new List<DataItem>();
            var querySplit = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var group in DataSource.Instance.Groups)
            {
                foreach (var item in group.Items)
                {
                    // Every word entered (separated by space) must be in the title,
                    // e.g. "split button" only matches titles containing both "split" and "button"
                    if (!item.IncludedInBuild || item.IsSeparator || item.Title == null)
                    {
                        continue;
                    }

                    bool match = true;
                    foreach (string queryToken in querySplit)
                    {
                        if (item.Title.IndexOf(queryToken, StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            match = false;
                            break;
                        }
                    }

                    if (match)
                    {
                        if (string.IsNullOrEmpty(item.ImagePath))
                        {
                            item.ImagePath = _autoSuggestBoxNotFoundImagePath;
                        }
                        suggestions.Add(item);
                    }
                }
            }
            if (suggestions.Count > 0)
            {
                _autoSuggestBox.ItemsSource = suggestions.OrderByDescending(i => i.Title.StartsWith(query, StringComparison.OrdinalIgnoreCase)).ThenBy(i => i.Title, StringComparer.CurrentCulture).ToList();
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
    }

    public async Task GetMenuItemsAsync(string jsonFilePath, PathType pathType = PathType.Relative)
    {
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);
    }
    public void EnsureNavigationSelection(string id)
    {
        if (string.IsNullOrEmpty(id) || _navigationView == null || !_itemMap.TryGetValue(id, out var entry))
        {
            return;
        }

        var (item, parent) = entry;
        if (parent != null)
        {
            parent.IsExpanded = true;
        }
        else if (item.MenuItems.Count > 0)
        {
            item.IsExpanded = true;
        }

        if (!ReferenceEquals(_navigationView.SelectedItem, item))
        {
            _navigationView.SelectedItem = item;
        }
    }

    // Returns true when the selection changed (so SelectionChanged will navigate)
    private bool SelectItemById(string id)
    {
        if (string.IsNullOrEmpty(id) || _navigationView == null || !_itemMap.TryGetValue(id, out var entry))
        {
            return false;
        }

        var (item, parent) = entry;
        if (ReferenceEquals(_navigationView.SelectedItem, item))
        {
            return false;
        }

        if (parent != null && _navigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
        {
            // In Top mode the child is not visible, so select the parent without navigating to it
            var wasSelectionSuppressed = _suppressSelectionNavigation;
            _suppressSelectionNavigation = true;
            try
            {
                _navigationView.SelectedItem = parent;
            }
            finally
            {
                _suppressSelectionNavigation = wasSelectionSuppressed;
            }
            parent.StartBringIntoView();
            return false;
        }

        EnsureNavigationSelection(id);
        item.StartBringIntoView();
        return true;
    }

    public bool EnsureItemIsVisibleInNavigation(string name)
    {
        if (_navigationView == null || string.IsNullOrEmpty(name))
        {
            return false;
        }

        foreach (var rawItem in _navigationView.MenuItems)
        {
            if (rawItem is NavigationViewItem item && SelectNavigationItemByName(item, name))
            {
                return true;
            }
        }

        foreach (var rawItem in _navigationView.FooterMenuItems)
        {
            if (rawItem is NavigationViewItem item && SelectNavigationItemByName(item, name))
            {
                return true;
            }
        }

        return false;
    }

    private bool SelectNavigationItemByName(NavigationViewItem item, string name)
    {
        if (string.Equals(item.Content as string, name, StringComparison.Ordinal))
        {
            _navigationView.SelectedItem = item;
            return true;
        }

        foreach (var rawChild in item.MenuItems)
        {
            if (rawChild is not NavigationViewItem child ||
                !string.Equals(child.Content as string, name, StringComparison.Ordinal))
            {
                continue;
            }

            if (_navigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
            {
                _navigationView.SelectedItem = item;
                item.StartBringIntoView();
            }
            else
            {
                item.IsExpanded = true;
                _navigationView.SelectedItem = child;
                child.StartBringIntoView();
            }

            return true;
        }

        return false;
    }
}
