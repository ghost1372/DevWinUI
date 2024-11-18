using System.Collections.ObjectModel;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.Foundation;

namespace DevWinUI;
public sealed partial class BreadcrumbNavigator : BreadcrumbBar
{
    private bool userHasItemClickEvent = false;

    public new event TypedEventHandler<BreadcrumbBar, BreadcrumbBarItemClickedEventArgs> ItemClicked
    {
        add
        {
            // If a handler other than your own is added, set the flag
            if (value != (TypedEventHandler<BreadcrumbBar, BreadcrumbBarItemClickedEventArgs>)OnItemClicked)
            {
                userHasItemClickEvent = true;
            }
            base.ItemClicked += value;
        }
        remove
        {
            // If a handler other than your own is removed, check the flag
            if (value != (TypedEventHandler<BreadcrumbBar, BreadcrumbBarItemClickedEventArgs>)OnItemClicked)
            {
                userHasItemClickEvent = false;
            }
            base.ItemClicked -= value;
        }
    }
    public ObservableCollection<NavigationBreadcrumb> BreadCrumbs
    {
        get { return (ObservableCollection<NavigationBreadcrumb>)GetValue(BreadCrumbsProperty); }
        set { SetValue(BreadCrumbsProperty, value); }
    }

    public static readonly DependencyProperty BreadCrumbsProperty =
        DependencyProperty.Register(nameof(BreadCrumbs), typeof(ObservableCollection<NavigationBreadcrumb>), typeof(BreadcrumbNavigator), new PropertyMetadata(null, OnBreadCrumbsChanged));

    private static void OnBreadCrumbsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (BreadcrumbNavigator)d;
        if (ctl != null)
        {
            ctl.ItemsSource = e.NewValue;
        }
    }

    public bool UseBuiltInEventForFrame
    {
        get { return (bool)GetValue(UseBuiltInEventForFrameProperty); }
        set { SetValue(UseBuiltInEventForFrameProperty, value); }
    }

    public static readonly DependencyProperty UseBuiltInEventForFrameProperty =
        DependencyProperty.Register(nameof(UseBuiltInEventForFrame), typeof(bool), typeof(BreadcrumbNavigator), new PropertyMetadata(false));

    public Frame Frame
    {
        get { return (Frame)GetValue(FrameProperty); }
        set { SetValue(FrameProperty, value); }
    }

    public static readonly DependencyProperty FrameProperty =
        DependencyProperty.Register(nameof(Frame), typeof(Frame), typeof(BreadcrumbNavigator), new PropertyMetadata(null));

    public Dictionary<Type, BreadcrumbPageConfig> PageDictionary
    {
        get { return (Dictionary<Type, BreadcrumbPageConfig>)GetValue(PageDictionaryProperty); }
        set { SetValue(PageDictionaryProperty, value); }
    }

    public static readonly DependencyProperty PageDictionaryProperty =
        DependencyProperty.Register(nameof(PageDictionary), typeof(Dictionary<Type, BreadcrumbPageConfig>), typeof(BreadcrumbNavigator), new PropertyMetadata(null));

    public NavigationView NavigationView
    {
        get { return (NavigationView)GetValue(NavigationViewProperty); }
        set { SetValue(NavigationViewProperty, value); }
    }

    public static readonly DependencyProperty NavigationViewProperty =
        DependencyProperty.Register(nameof(NavigationView), typeof(NavigationView), typeof(BreadcrumbNavigator), new PropertyMetadata(null));

    internal Frame InternalFrame
    {
        get { return (Frame)GetValue(InternalFrameProperty); }
        set { SetValue(InternalFrameProperty, value); }
    }

    internal static readonly DependencyProperty InternalFrameProperty =
        DependencyProperty.Register(nameof(InternalFrame), typeof(Frame), typeof(BreadcrumbNavigator), new PropertyMetadata(null));

    public BreadcrumbNavigatorHeaderVisibilityOptions HeaderVisibilityOptions
    {
        get { return (BreadcrumbNavigatorHeaderVisibilityOptions)GetValue(HeaderVisibilityOptionsProperty); }
        set { SetValue(HeaderVisibilityOptionsProperty, value); }
    }

    public static readonly DependencyProperty HeaderVisibilityOptionsProperty =
        DependencyProperty.Register(nameof(HeaderVisibilityOptions), typeof(BreadcrumbNavigatorHeaderVisibilityOptions), typeof(BreadcrumbNavigator), new PropertyMetadata(BreadcrumbNavigatorHeaderVisibilityOptions.Both));

    private bool removeLastBackStackItem = false;
    public BreadcrumbNavigator()
    {
        ItemClicked -= OnItemClicked;
        ItemClicked += OnItemClicked;
        if (Frame != null && UseBuiltInEventForFrame)
        {
            Frame.Navigating -= OnFrameNavigating;
            Frame.Navigating += OnFrameNavigating;
        }
    }

    internal void Initialize()
    {
        if (InternalFrame != null)
        {
            InternalFrame.Navigated -= OnInternalFrameNavigated;
            InternalFrame.Navigated += OnInternalFrameNavigated;
            InternalFrame.Navigating -= OnInternalFrameNavigating;
            InternalFrame.Navigating += OnInternalFrameNavigating;
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

    private void OnInternalFrameNavigating(object sender, NavigatingCancelEventArgs e)
    {
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
    private void OnInternalFrameNavigated(object sender, NavigationEventArgs e)
    {
        if (removeLastBackStackItem && InternalFrame != null)
        {
            InternalFrame.BackStack?.Remove(InternalFrame.BackStack?.LastOrDefault());
            InternalFrame.BackStack?.Remove(InternalFrame.BackStack?.LastOrDefault());
            removeLastBackStackItem = false;
        }
    }

    private void OnFrameNavigating(object sender, NavigatingCancelEventArgs e)
    {
        AddNewItem(e.SourcePageType, null);
    }

    private void OnItemClicked(BreadcrumbBar sender, BreadcrumbBarItemClickedEventArgs args)
    {
        if (!userHasItemClickEvent)
        {
            OnItemClicked(args);
        }

        removeLastBackStackItem = true;
    }

    public void OnItemClicked(BreadcrumbBarItemClickedEventArgs args)
    {
        if (args.Index < BreadCrumbs.Count - 1)
        {
            var crumb = (NavigationBreadcrumb)args.Item;
            SlideNavigationTransitionInfo info = new SlideNavigationTransitionInfo();
            info.Effect = SlideNavigationTransitionEffect.FromLeft;
            AddNewItem(crumb.Page, crumb.Parameter, info, null);
            FixIndex(args.Index);
        }
    }

    public void FixIndex(int BreadcrumbBarIndex)
    {
        int indexToRemoveAfter = BreadcrumbBarIndex;

        if (indexToRemoveAfter < BreadCrumbs.Count - 1)
        {
            int itemsToRemove = BreadCrumbs.Count - indexToRemoveAfter - 1;

            for (int i = 0; i < itemsToRemove; i++)
            {
                BreadCrumbs.RemoveAt(indexToRemoveAfter + 1);
            }
        }
    }

    internal void AddNewItem(Type targetPageType, NavigationTransitionInfo navigationTransitionInfo, object parameter, object currentPageParameter, bool allowDuplication, Action updateBreadcrumb)
    {
        string pageTitle = string.Empty;
        string pageTitleAttached = string.Empty;
        bool isHeaderVisibile = false;
        bool clearNavigation = false;

        if (PageDictionary == null)
        {
#pragma warning disable IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.
            DependencyObject obj = Activator.CreateInstance(targetPageType) as DependencyObject;
#pragma warning restore IL2067 // Target parameter argument does not satisfy 'DynamicallyAccessedMembersAttribute' in call to target method. The parameter of method does not have matching annotations.

            pageTitleAttached = GetPageTitle(obj);
            isHeaderVisibile = GetIsHeaderVisible(obj);
            clearNavigation = GetClearNavigation(obj);
        }
        else
        {
            var item = PageDictionary.FirstOrDefault(x => x.Key == targetPageType);
            if (item.Value != null)
            {
                pageTitleAttached = item.Value.PageTitle;
                isHeaderVisibile = item.Value.IsHeaderVisible;
                clearNavigation = item.Value.ClearNavigation;
            }
        }
        
        if (!string.IsNullOrEmpty(pageTitleAttached))
        {
            pageTitle = pageTitleAttached;
        }
        else
        {
            if (currentPageParameter != null && currentPageParameter is string value)
            {
                pageTitle = value;
            }
            else if (currentPageParameter != null && currentPageParameter is DataItem dataItem)
            {
                pageTitle = dataItem.Title;
            }
            else if (currentPageParameter != null && currentPageParameter is DataGroup dataGroup)
            {
                pageTitle = dataGroup.Title;
            }
        }

        if (Frame != null && clearNavigation)
        {
            BreadCrumbs?.Clear();
            this.Frame.BackStack.Clear();
        }

        if (isHeaderVisibile)
        {
            if (!string.IsNullOrEmpty(pageTitle))
            {
                if (BreadCrumbs != null)
                {
                    var currentItem = new NavigationBreadcrumb(pageTitle, targetPageType, parameter);

                    if (allowDuplication)
                    {
                        BreadCrumbs?.Add(currentItem);
                        updateBreadcrumb?.Invoke();
                    }
                    else
                    {
                        var itemExist = BreadCrumbs.Contains(currentItem, new GenericCompare<NavigationBreadcrumb>(x => x.Page));
                        if (!itemExist)
                        {
                            BreadCrumbs?.Add(currentItem);
                            updateBreadcrumb?.Invoke();
                        }
                    }
                }
            }
        }

        if (BreadCrumbs == null || BreadCrumbs?.Count == 0)
        {
            ChangeBreadcrumbVisibility(false);
        }
        else
        {
            ChangeBreadcrumbVisibility(isHeaderVisibile);
        }

        if (this.Frame != null)
        {
            this.Frame.Navigate(targetPageType, parameter, navigationTransitionInfo);
        }
    }

    public void AddNewItem(Type targetPageType, object parameter, NavigationTransitionInfo navigationTransitionInfo, Action updateBreadcrumb)
    {
        AddNewItem(targetPageType, navigationTransitionInfo, parameter, null, false, updateBreadcrumb);
    }
    public void AddNewItem(Type targetPageType, Action updateBreadcrumb)
    {
        AddNewItem(targetPageType, null, null, null, false, updateBreadcrumb);
    }
    public void ChangeBreadcrumbVisibility(bool IsBreadcrumbVisible)
    {
        if (HeaderVisibilityOptions == BreadcrumbNavigatorHeaderVisibilityOptions.None)
            return;

        if (HeaderVisibilityOptions == BreadcrumbNavigatorHeaderVisibilityOptions.Both ||
            HeaderVisibilityOptions == BreadcrumbNavigatorHeaderVisibilityOptions.NavigationViewOnly)
        {
            if (NavigationView != null)
            {
                NavigationView.AlwaysShowHeader = IsBreadcrumbVisible;
            }
        }

        if (HeaderVisibilityOptions != BreadcrumbNavigatorHeaderVisibilityOptions.NavigationViewOnly)
        {
            Visibility = IsBreadcrumbVisible ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
