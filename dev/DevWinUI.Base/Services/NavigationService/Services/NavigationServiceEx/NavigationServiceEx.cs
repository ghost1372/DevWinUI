namespace DevWinUI;

public partial class NavigationServiceEx : INavigationServiceEx
{
    public NavigationServiceEx()
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

    private void InitializeBase(NavigationView navigationView, Frame frame)
    {
        Reset();

        _navigationView = navigationView ?? throw new ArgumentNullException(nameof(navigationView));
        this.Frame = frame ?? throw new ArgumentNullException(nameof(frame));

        _navigationView.BackRequested -= OnNavigationViewBackRequested;
        _navigationView.BackRequested += OnNavigationViewBackRequested;
        _navigationView.SelectionChanged -= OnNavigationViewSelectionChanged;
        _navigationView.SelectionChanged += OnNavigationViewSelectionChanged;

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
            if (_navigationView.SelectedItem is NavigationViewItem currentItem)
            {
                if (e.NavigationMode == NavigationMode.Back || (currentItem.GetValue(NavigateToProperty) is Type currentType && !currentType.Equals(e.SourcePageType)))
                {
                    EnsureNavigationSelection(e.SourcePageType);
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

        FrameNavigated -= OnNavigated;
        _navigationView = null;
        Frame = null;
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
            NavigateTo(_settingsPage, pageTitle);
        }
        else
        {
            if (args.SelectedItemContainer is NavigationViewItem selectedItem && selectedItem.GetValue(NavigateToProperty) is Type currentType && currentType != null)
            {
                NavigateTo(currentType, selectedItem.GetValue(ParameterProperty));
            }
        }
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

    public void EnsureNavigationSelection(Type type)
    {
        foreach (object rawGroup in this.AllMenuItems)
        {
            if (rawGroup is NavigationViewItem group)
            {
                if (group.GetValue(NavigateToProperty).Equals(type))
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
                        EnsureNavigationSelectionBase(rawItem, type, group);
                    }
                }
            }
        }
    }

    private bool EnsureNavigationSelectionBase(object rawItem, Type type, NavigationViewItem parentGroup)
    {
        if (rawItem is NavigationViewItem item)
        {
            if (item.GetValue(NavigateToProperty).Equals(type))
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
                    if (EnsureNavigationSelectionBase(rawInnerItem, type, parentGroup))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
