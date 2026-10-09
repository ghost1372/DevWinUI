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
            var selectionId = (e.Parameter as BaseDataInfo)?.UniqueId;
            if (string.IsNullOrEmpty(selectionId))
            {
                selectionId = _pendingSelectionId;
            }

            if (!string.IsNullOrEmpty(selectionId) && _itemMap.ContainsKey(selectionId))
            {
                // Frame already navigated (e.g. back/forward), so selection must not navigate again
                _lastParameterUsed = e.Parameter;
                if (_navigationView.SelectedItem is not NavigationViewItem currentItem || !selectionId.Equals(currentItem.Tag as string))
                {
                    _suppressSelectionNavigation = true;
                    try
                    {
                        EnsureNavigationSelection(selectionId);
                    }
                    finally
                    {
                        _suppressSelectionNavigation = false;
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
        DataSource.Instance.Groups.Clear();
    }
    public void ReInitialize()
    {
        InitializeBase(_navigationView, Frame, _pageKeyToTypeMap);

        InternalLocalizationHelper.InitializeInternalLocalization(_resourceManager, _resourceContext);

        ConfigureJsonBase(JsonFilePath, _pathType, _orderItems);
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
            var suggestions = new List<DataItem>();

            var querySplit = sender.Text.Split(" ");
            foreach (var group in DataSource.Instance.Groups)
            {
                var matchingItems = group.Items.Where(
                    item =>
                    {
                        // Idea: check for every word entered (separated by space) if it is in the name, 
                        // e.g. for query "split button" the only result should "SplitButton" since its the only query to contain "split" and "button"
                        // If any of the sub tokens is not in the string, we ignore the item. So the search gets more precise with more words
                        bool flag = item.IncludedInBuild;
                        foreach (string queryToken in querySplit)
                        {
                            // Check if token is not in string
                            if (item.Title.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                // Token is not in string, so we ignore this item.
                                flag = false;
                            }
                        }
                        return flag;
                    });
                foreach (var item in matchingItems)
                {
                    if (string.IsNullOrEmpty(item.ImagePath))
                    {
                        item.ImagePath = _autoSuggestBoxNotFoundImagePath;
                    }
                    suggestions.Add(item);
                }
            }
            if (suggestions.Count > 0)
            {
                _autoSuggestBox.ItemsSource = suggestions.OrderByDescending(i => i.Title.StartsWith(sender.Text, StringComparison.CurrentCultureIgnoreCase)).ThenBy(i => i.Title).ToList();
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

    public async void GetMenuItemsAsync(string jsonFilePath, PathType pathType = PathType.Relative)
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
            _suppressSelectionNavigation = true;
            try
            {
                _navigationView.SelectedItem = parent;
            }
            finally
            {
                _suppressSelectionNavigation = false;
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
        bool changedSelection = false;
        foreach (object rawItem in this.AllMenuItems)
        {
            // Check if we encountered the separator
            if (!(rawItem is NavigationViewItem))
            {
                // Skipping this item
                continue;
            }

            var item = rawItem as NavigationViewItem;

            // Check if we are this category
            if ((string)item.Content == name)
            {
                _navigationView.SelectedItem = item;
                changedSelection = true;
            }
            // We are not :/
            else
            {
                // Maybe one of our items is?
                if (item.MenuItems.Count != 0)
                {
                    foreach (NavigationViewItem child in item.MenuItems)
                    {
                        if ((string)child.Content == name)
                        {
                            // We are the item corresponding to the selected one, update selection!

                            // Deal with differences in displaymodes
                            if (_navigationView.PaneDisplayMode == NavigationViewPaneDisplayMode.Top)
                            {
                                // In Topmode, the child is not visible, so set parent as selected
                                // Everything else does not work unfortunately
                                _navigationView.SelectedItem = item;
                                item.StartBringIntoView();
                            }
                            else
                            {
                                // Expand so we animate
                                if (item.MenuItems.Count > 0)
                                {
                                    item.IsExpanded = true;
                                }
                                // Set selected item
                                _navigationView.SelectedItem = child;
                                child.StartBringIntoView();
                            }
                            // Set to true to also skip out of outer for loop
                            changedSelection = true;
                            // Break out of child iteration for loop
                            break;
                        }
                    }
                }
            }
            // We updated selection, break here!
            if (changedSelection)
            {
                break;
            }
        }
        return changedSelection;
    }
}
