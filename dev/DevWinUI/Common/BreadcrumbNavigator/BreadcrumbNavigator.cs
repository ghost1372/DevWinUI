namespace DevWinUI;
public sealed partial class BreadcrumbNavigator : BreadcrumbBar
{
    public bool AllowDuplication
    {
        get { return (bool)GetValue(AllowDuplicationProperty); }
        set { SetValue(AllowDuplicationProperty, value); }
    }

    public static readonly DependencyProperty AllowDuplicationProperty =
        DependencyProperty.Register(nameof(AllowDuplication), typeof(bool), typeof(BreadcrumbNavigator), new PropertyMetadata(false));

    public NavigationTransitionInfo NavigationTransitionInfo
    {
        get { return (NavigationTransitionInfo)GetValue(NavigationTransitionInfoProperty); }
        set { SetValue(NavigationTransitionInfoProperty, value); }
    }

    public static readonly DependencyProperty NavigationTransitionInfoProperty =
        DependencyProperty.Register(nameof(NavigationTransitionInfo), typeof(NavigationTransitionInfo), typeof(BreadcrumbNavigator), new PropertyMetadata(GetDefaultNavigationTransitionInfo()));

    private static NavigationTransitionInfo GetDefaultNavigationTransitionInfo()
    {
        return new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft };
    }
    public bool IsClickable
    {
        get { return (bool)GetValue(IsClickableProperty); }
        set { SetValue(IsClickableProperty, value); }
    }

    public static readonly DependencyProperty IsClickableProperty =
        DependencyProperty.Register(nameof(IsClickable), typeof(bool), typeof(BreadcrumbNavigator), new PropertyMetadata(true));

    public BreadcrumbNavigatorHeaderVisibilityOptions HeaderVisibilityOptions
    {
        get { return (BreadcrumbNavigatorHeaderVisibilityOptions)GetValue(HeaderVisibilityOptionsProperty); }
        set { SetValue(HeaderVisibilityOptionsProperty, value); }
    }

    public static readonly DependencyProperty HeaderVisibilityOptionsProperty =
        DependencyProperty.Register(nameof(HeaderVisibilityOptions), typeof(BreadcrumbNavigatorHeaderVisibilityOptions), typeof(BreadcrumbNavigator), new PropertyMetadata(BreadcrumbNavigatorHeaderVisibilityOptions.Both));

    public ObservableCollection<BreadcrumbStep> BreadCrumbs
    {
        get { return (ObservableCollection<BreadcrumbStep>)GetValue(BreadCrumbsProperty); }
        set { SetValue(BreadCrumbsProperty, value); }
    }

    public static readonly DependencyProperty BreadCrumbsProperty =
        DependencyProperty.Register(nameof(BreadCrumbs), typeof(ObservableCollection<BreadcrumbStep>), typeof(BreadcrumbNavigator), new PropertyMetadata(null, OnBreadCrumbsChanged));
    private static void OnBreadCrumbsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BreadcrumbNavigator)d;
        if (ctl != null)
        {
            ctl.ItemsSource = e.NewValue;
        }
    }

    private NavigationView MainNavigation { get; set; }
    private Frame MainFrame { get; set; }
    private Dictionary<Type, BreadcrumbPageConfig> PageDictionary;

    public void Initialize(Frame frame, NavigationView navigationView, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
    {
        this.MainNavigation = navigationView;
        this.MainFrame = frame;
        this.PageDictionary = pageDictionary;

        BreadCrumbs = new();

        ItemClicked -= BreadcrumbBar_ItemClicked;
        ItemClicked += BreadcrumbBar_ItemClicked;
        frame.Navigated -= Frame_Navigated;
        frame.Navigated += Frame_Navigated;
        frame.Navigating -= Frame_Navigating;
        frame.Navigating += Frame_Navigating;
    }

    private void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
    {
        // Fix BreadCrumbs when navigating back
        var currentItem = BreadCrumbs?.FirstOrDefault(x => x.Page == e.SourcePageType);
        if (currentItem != null)
        {
            int currentIndex = BreadCrumbs.IndexOf(currentItem);

            // Filter items from beginning to the current item
            var filteredItems = BreadCrumbs.Take(currentIndex + 1).ToList();

            // Update BreadCrumbs with the filtered items
            BreadCrumbs = new(filteredItems);
        }
        HandleBackRequested(e.SourcePageType);
    }

    private void Frame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
    }

    private void BreadcrumbBar_ItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (IsClickable)
        {
            NavigateFromBreadcrumb(args);
        }
    }
    public void AddNewItem(Type targetPageType)
    {
        AddNewItem(targetPageType, null);
    }

    public void AddNewItem(Type targetPageType, object parameter)
    {
        string pageTitle = string.Empty;
        string pageTitleAttached = string.Empty;
        bool isHeaderVisibile = false;
        bool clearNavigation = false;

        var item = PageDictionary.FirstOrDefault(x => x.Key == targetPageType);
        if (item.Value != null)
        {
            pageTitleAttached = item.Value.PageTitle;
            isHeaderVisibile = item.Value.IsHeaderVisible;
            clearNavigation = item.Value.ClearNavigation;
        }
        else
        {
            return;
        }

        if (!string.IsNullOrEmpty(pageTitleAttached))
        {
            pageTitle = pageTitleAttached;
        }
        else if (parameter != null)
        {
            if (parameter is string value)
            {
                pageTitle = value;
            }
            else if (parameter is BaseDataInfo dataInfo)
            {
                pageTitle = dataInfo.Title;
            }
        }

        if (clearNavigation)
        {
            BreadCrumbs?.Clear();
            this.MainFrame.BackStack.Clear();
        }

        ChangeBreadcrumbVisibility(isHeaderVisibile);

        if (isHeaderVisibile)
        {
            if (!string.IsNullOrEmpty(pageTitle))
            {
                if (BreadCrumbs != null)
                {
                    var currentItem = new BreadcrumbStep(pageTitle, targetPageType, parameter);

                    if (AllowDuplication)
                    {
                        BreadCrumbs?.Add(currentItem);
                    }
                    else
                    {
                        var itemExist = BreadCrumbs.Contains(currentItem, new GenericCompare<BreadcrumbStep>(x => x.Page));
                        if (!itemExist)
                        {
                            BreadCrumbs?.Add(currentItem);
                        }
                    }
                    
                }
            }
        }
    }

    private void HandleBackRequested(Type sourcePageType)
    {
        if (PageDictionary == null)
            return;

        var item = PageDictionary.FirstOrDefault(x => x.Key == sourcePageType);
        bool isHeaderVisible = false;
        if (item.Value != null)
        {
            isHeaderVisible = item.Value.IsHeaderVisible;
        }

        ChangeBreadcrumbVisibility(isHeaderVisible);
    }
    public void NavigateFromBreadcrumb(BreadcrumbBarItemClickedEventArgs args)
    {
        if (args == null)
        {
            throw new ArgumentNullException("args was null");
        }
        if (args.Index < BreadCrumbs.Count)
        {
        }

        string pageTitle = (args.Item as BreadcrumbStep).Label;
        bool isHeaderVisibile = false;
        object parameter = (args.Item as BreadcrumbStep).Parameter;
        Type targetPageType = (args.Item as BreadcrumbStep).Page;

        var item = PageDictionary.FirstOrDefault(x => x.Key == targetPageType);
        if (item.Value != null)
        {
            isHeaderVisibile = item.Value.IsHeaderVisible;
        }

        ChangeBreadcrumbVisibility(isHeaderVisibile);

        MainFrame.Navigate(targetPageType, parameter, NavigationTransitionInfo);

        int indexToRemoveAfter = args.Index;

        if (indexToRemoveAfter < BreadCrumbs.Count - 1)
        {
            int itemsToRemove = BreadCrumbs.Count - indexToRemoveAfter - 1;

            for (int i = 0; i < itemsToRemove; i++)
            {
                BreadCrumbs.RemoveAt(indexToRemoveAfter + 1);
            }
        }
        MainFrame.BackStack?.Remove(MainFrame.BackStack?.LastOrDefault());
        MainFrame.BackStack?.Remove(MainFrame.BackStack?.LastOrDefault());
    }

    public void ChangeBreadcrumbVisibility(bool IsBreadcrumbVisible)
    {
        if (HeaderVisibilityOptions == BreadcrumbNavigatorHeaderVisibilityOptions.None)
            return;

        if (HeaderVisibilityOptions == BreadcrumbNavigatorHeaderVisibilityOptions.Both ||
            HeaderVisibilityOptions == BreadcrumbNavigatorHeaderVisibilityOptions.NavigationViewOnly)
        {
            if (MainNavigation != null)
            {
                MainNavigation.AlwaysShowHeader = IsBreadcrumbVisible;
            }
        }

        if (HeaderVisibilityOptions != BreadcrumbNavigatorHeaderVisibilityOptions.NavigationViewOnly)
        {
            Visibility = IsBreadcrumbVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
