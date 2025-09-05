﻿using Microsoft.UI.Xaml.Controls.AnimatedVisuals;
using Microsoft.Windows.ApplicationModel.Resources;

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
        Reset();

        _navigationView = navigationView ?? throw new ArgumentNullException(nameof(navigationView));
        this.Frame = frame ?? throw new ArgumentNullException(nameof(frame));
        this._pageKeyToTypeMap = pages ?? throw new ArgumentNullException(nameof(pages));

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
            _titleBar.IsBackButtonVisible = _frame.CanGoBack;
        }

        if (e.SourcePageType == _settingsPage)
        {
            _navigationView.SelectedItem = SettingsItem;
        }
        else
        {
            if (_navigationView.SelectedItem is NavigationViewItem currentItem && currentItem.Tag is string currentTag && !string.IsNullOrEmpty(currentTag) && e.Parameter is BaseDataInfo dataInfo)
            {
                if (e.NavigationMode == NavigationMode.Back || !currentTag.Equals(dataInfo.UniqueId))
                {
                    EnsureNavigationSelection(dataInfo.UniqueId);
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

        _pageKeyToTypeMap?.Clear();
        _pageKeyToTypeMap = null;
        FrameNavigated -= OnNavigated;
        _navigationView = null;
        Frame = null;
        DataSource.Instance.Groups.Clear();
    }
    public void ReInitialize()
    {
        InitializeBase(_navigationView, _frame, _pageKeyToTypeMap);

        InternalLocalizationHelper.InitializeInternalLocalization(_resourceManager, _resourceContext);

        ConfigureJsonBase(JsonFilePath, _pathType, _orderItems);
    }
    private void OnNavigationViewSelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.IsSettingsSelected)
        {
            string pageTitle = string.Empty;
            var item = SettingsItem as NavigationViewItem;
            if (item != null && item.Content != null)
            {
                pageTitle = item.Content.ToString();
            }
            NavigateTo(SettingsPageKey, pageTitle);
        }
        else
        {
            if (args.SelectedItemContainer is NavigationViewItem selectedItem && selectedItem.DataContext is BaseDataInfo dataInfo)
            {
                var targetKey = !string.IsNullOrEmpty(dataInfo.SectionId) && _sectionPage != null
                                ? SectionPageKey
                                : dataInfo.UniqueId;

                NavigateTo(targetKey, dataInfo);
            }
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
            var hasChangedSelection = EnsureItemIsVisibleInNavigation(infoDataItem.Title);

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
        foreach (object rawGroup in this.AllMenuItems)
        {
            if (rawGroup is NavigationViewItem group)
            {
                if ((string)group.Tag == id)
                {
                    group.IsSelected = true;
                    _navigationView.SelectedItem = group;

                    if (group.MenuItems.Count > 0)
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

    private bool EnsureNavigationSelectionBase(object rawItem, string id, NavigationViewItem parentGroup)
    {
        if (rawItem is NavigationViewItem item)
        {
            if ((string)item.Tag == id)
            {
                _navigationView.SelectedItem = item;
                item.IsSelected = true;

                if (parentGroup.MenuItems.Count > 0)
                {
                    parentGroup.IsExpanded = true;
                }
                return true;
            }

            if (item.MenuItems.Count > 0)
            {
                foreach (var rawInnerItem in item.MenuItems)
                {
                    if (EnsureNavigationSelectionBase(rawInnerItem, id, parentGroup))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
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
                                // Ensure parent is expanded so we actually show the selection indicator
                                _navigationView.UpdateLayout();
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
