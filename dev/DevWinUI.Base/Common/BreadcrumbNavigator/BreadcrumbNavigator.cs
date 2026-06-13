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
    private bool _settingsDelayApplied = false;
    private bool _applySettingsDelayNextNavigation = false;

    public Type SettingsPageType
    {
        get => (Type)GetValue(SettingsPageTypeProperty);
        set => SetValue(SettingsPageTypeProperty, value);
    }
    public static readonly DependencyProperty SettingsPageTypeProperty =
        DependencyProperty.Register(nameof(SettingsPageType), typeof(Type), typeof(BreadcrumbNavigator), new PropertyMetadata(null));

    public int SettingsDelayMilliseconds
    {
        get => (int)GetValue(SettingsDelayMillisecondsProperty);
        set => SetValue(SettingsDelayMillisecondsProperty, value);
    }
    public static readonly DependencyProperty SettingsDelayMillisecondsProperty =
        DependencyProperty.Register(nameof(SettingsDelayMilliseconds), typeof(int), typeof(BreadcrumbNavigator), new PropertyMetadata(750));

    public int FadeDurationMilliseconds
    {
        get => (int)GetValue(FadeDurationMillisecondsProperty);
        set => SetValue(FadeDurationMillisecondsProperty, value);
    }
    public static readonly DependencyProperty FadeDurationMillisecondsProperty =
        DependencyProperty.Register(nameof(FadeDurationMilliseconds), typeof(int), typeof(BreadcrumbNavigator), new PropertyMetadata(750));

    public BreadcrumbEntranceAnimation EntranceAnimation
    {
        get => (BreadcrumbEntranceAnimation)GetValue(EntranceAnimationProperty);
        set => SetValue(EntranceAnimationProperty, value);
    }
    public static readonly DependencyProperty EntranceAnimationProperty =
        DependencyProperty.Register(nameof(EntranceAnimation), typeof(BreadcrumbEntranceAnimation), typeof(BreadcrumbNavigator), new PropertyMetadata(BreadcrumbEntranceAnimation.Fade));

    public void Initialize(Frame frame, Dictionary<Type, BreadcrumbPageConfig> pageDictionary)
    {
        Initialize(frame, null, pageDictionary);
    }
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

        ChangeBreadcrumbVisibility(isHeaderVisibile, targetPageType);

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

    public void HandleBackRequested(Type sourcePageType)
    {
        if (PageDictionary == null)
            return;

        var item = PageDictionary.FirstOrDefault(x => x.Key == sourcePageType);
        bool isHeaderVisible = false;
        if (item.Value != null)
        {
            isHeaderVisible = item.Value.IsHeaderVisible;
        }

        ChangeBreadcrumbVisibility(isHeaderVisible, sourcePageType);
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

        ChangeBreadcrumbVisibility(isHeaderVisibile, targetPageType);

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

    public async void ChangeBreadcrumbVisibility(bool IsBreadcrumbVisible, Type? pageType = null)
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
            bool applySettingsDelay = false;
            // Only apply the initial settings delay when navigation service indicated settings selection and not already applied
            if (SettingsPageType != null && pageType != null && SettingsPageType == pageType && !_settingsDelayApplied && _applySettingsDelayNextNavigation)
            {
                // Clear the prepare flag so only the immediate navigation gets the special entrance
                _applySettingsDelayNextNavigation = false;
                // if the frame is already showing the settings page, don't apply delay
                if (MainFrame?.CurrentSourcePageType != SettingsPageType)
                {
                    applySettingsDelay = true;
                }
            }

            if (applySettingsDelay)
            {
                // mark early to avoid re-entrancy/race applying delay multiple times
                _settingsDelayApplied = true;
                // Reserve layout space while waiting for transition: make invisible but occupy space
                Visibility = Visibility.Visible;
                Opacity = 0; // make invisible

                await Task.Delay(SettingsDelayMilliseconds);

                if (IsBreadcrumbVisible)
                {
                    await AnimateEntranceAsync(1, FadeDurationMilliseconds);
                }
                else
                {
                    // keep reserved space but remain invisible
                    await AnimateEntranceAsync(0, FadeDurationMilliseconds);
                }
            }
            else
            {
                if (IsBreadcrumbVisible)
                {
                    Visibility = Visibility.Visible;
                    await AnimateEntranceAsync(1, FadeDurationMilliseconds);
                }
                else
                {
                    // fade out then collapse to release space
                    await AnimateEntranceAsync(0, FadeDurationMilliseconds);
                    Visibility = Visibility.Collapsed;
                }
            }
        }
    }

    private Task AnimateEntranceAsync(double to, int durationMilliseconds)
    {
        var tcs = new TaskCompletionSource<bool>();
        var sb = new Storyboard();

        // Fade animation
        var fade = new DoubleAnimation()
        {
            To = to,
            Duration = new Duration(TimeSpan.FromMilliseconds(durationMilliseconds)),
            EnableDependentAnimation = true,
        };
        Storyboard.SetTarget(fade, this);
        Storyboard.SetTargetProperty(fade, "(UIElement.Opacity)");
        sb.Children.Add(fade);

        // Slide animations for entrance (only when fading in or out we can also slide)
        if (EntranceAnimation != BreadcrumbEntranceAnimation.Fade)
        {
            var trans = new CompositeTransform();
            this.RenderTransform = trans;

            double fromX = 0, fromY = 0;
            switch (EntranceAnimation)
            {
                case BreadcrumbEntranceAnimation.SlideFromLeft: fromX = -20; break;
                case BreadcrumbEntranceAnimation.SlideFromRight: fromX = 20; break;
                case BreadcrumbEntranceAnimation.SlideFromTop: fromY = -10; break;
                case BreadcrumbEntranceAnimation.SlideFromBottom: fromY = 10; break;
            }

            var taX = new DoubleAnimation()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(durationMilliseconds)),
                EnableDependentAnimation = true,
            };
            var taY = new DoubleAnimation()
            {
                To = 0,
                Duration = new Duration(TimeSpan.FromMilliseconds(durationMilliseconds)),
                EnableDependentAnimation = true,
            };

            // If hiding, animate back to offset, else animate from offset to 0
            if (to <= 0)
            {
                // hide: move to offset
                taX.To = fromX;
                taY.To = fromY;
            }
            else
            {
                // show: start from offset
                trans.TranslateX = fromX;
                trans.TranslateY = fromY;
                taX.To = 0;
                taY.To = 0;
            }

            Storyboard.SetTarget(taX, this);
            Storyboard.SetTargetProperty(taX, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
            Storyboard.SetTarget(taY, this);
            Storyboard.SetTargetProperty(taY, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

            sb.Children.Add(taX);
            sb.Children.Add(taY);
        }

        void OnCompleted(object? s, object? e)
        {
            sb.Completed -= OnCompleted;
            tcs.TrySetResult(true);
        }

        sb.Completed += OnCompleted;
        sb.Begin();

        return tcs.Task;
    }

    internal void PrepareForSettingsEntrance()
    {
        _applySettingsDelayNextNavigation = true;
    }
}
