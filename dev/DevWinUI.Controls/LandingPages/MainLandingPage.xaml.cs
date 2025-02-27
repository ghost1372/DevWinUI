namespace DevWinUI;
public sealed partial class MainLandingPage : ItemsPageBase
{
    internal static MainLandingPage Instance { get; private set; }

    public string PreviewGroupText
    {
        get => (string)GetValue(PreviewGroupTextProperty);
        set => SetValue(PreviewGroupTextProperty, value);
    }

    public static readonly DependencyProperty PreviewGroupTextProperty =
        DependencyProperty.Register(nameof(PreviewGroupText), typeof(string), typeof(MainLandingPage), new PropertyMetadata("Preview"));

    public string UpdatedGroupText
    {
        get => (string)GetValue(UpdatedGroupTextProperty);
        set => SetValue(UpdatedGroupTextProperty, value);
    }

    public static readonly DependencyProperty UpdatedGroupTextProperty =
        DependencyProperty.Register(nameof(UpdatedGroupText), typeof(string), typeof(MainLandingPage), new PropertyMetadata("Recently updated"));

    public string NewGroupText
    {
        get => (string)GetValue(NewGroupTextProperty);
        set => SetValue(NewGroupTextProperty, value);
    }

    public static readonly DependencyProperty NewGroupTextProperty =
        DependencyProperty.Register(nameof(NewGroupText), typeof(string), typeof(MainLandingPage), new PropertyMetadata("Recently added"));

    public object HeaderContent
    {
        get => (object)GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    public static readonly DependencyProperty HeaderContentProperty =
        DependencyProperty.Register(nameof(HeaderContent), typeof(object), typeof(MainLandingPage), new PropertyMetadata(null, OnHeaderContentChanged));
    private static void OnHeaderContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MainLandingPage)d;
        if (ctl != null)
        {
            ctl.UpdateHeaderImageHeight();
        }
    }

    public Thickness HeaderMargin
    {
        get => (Thickness)GetValue(HeaderMarginProperty);
        set => SetValue(HeaderMarginProperty, value);
    }

    public static readonly DependencyProperty HeaderMarginProperty =
        DependencyProperty.Register(nameof(HeaderMargin), typeof(Thickness), typeof(MainLandingPage), new PropertyMetadata(new Thickness(-24, 0, -24, 0)));

    public bool UseFullScreenHeaderImage
    {
        get { return (bool)GetValue(UseFullScreenHeaderImageProperty); }
        set { SetValue(UseFullScreenHeaderImageProperty, value); }
    }

    public static readonly DependencyProperty UseFullScreenHeaderImageProperty =
        DependencyProperty.Register(nameof(UseFullScreenHeaderImage), typeof(bool), typeof(MainLandingPage), new PropertyMetadata(false, OnFullScreenHeaderImageChanged));
    private static void OnFullScreenHeaderImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MainLandingPage)d;
        if (ctl != null)
        {
            ctl.ToggleFullScreen((bool)e.NewValue);
        }
    }

    public MainLandingPage()
    {
        this.InitializeComponent();
        Instance = this;
        Loading -= MainLandingPage_Loading;
        Loading += MainLandingPage_Loading;
    }
    private void UpdateHeaderImageHeight()
    {
        if (HeaderContent == null)
        {
            MainHomePageHeaderImage.MinHeight = 0;
            MainBorder.MinHeight = 0;
        }
        else
        {
            MainHomePageHeaderImage.MinHeight = 396;
            MainBorder.MinHeight = 396;
        }
    }

    private void ToggleFullScreen(bool value)
    {
        if (value)
        {
            FullScreenHomePageHeaderImage.Visibility = Visibility.Visible;
            MainBorder.Visibility = Visibility.Visible;
            MainHomePageHeaderImage.Visibility = Visibility.Collapsed;
        }
        else
        {
            MainHomePageHeaderImage.Visibility = Visibility.Visible;
            MainBorder.Visibility = Visibility.Collapsed;
            FullScreenHomePageHeaderImage.Visibility = Visibility.Collapsed;
        }
    }
    private void MainLandingPage_Loading(FrameworkElement sender, object args)
    {
        if (CanExecuteInternalCommand)
        {
            GetData();
        }
    }

    public void GetData()
    {
        GetData(true, false, null);
    }
    public void GetData(bool useOrder)
    {
        GetData(useOrder, false, null);
    }
    public void GetData(bool useOrder, bool useOrderByDescending)
    {
        GetData(useOrder, useOrderByDescending, null);
    }
    private void GetData(bool useOrder, bool useOrderByDescending, Func<DataItem, object> orderby)
    {
        var allItems = DataSource.Instance.Groups
            .Where(group => !group.HideGroup)
            .SelectMany(group => group.Items)
            .Where(item => item.BadgeString != null && !item.HideItem);

        if (useOrder)
        {
            if (orderby == null)
            {
                if (useOrderByDescending)
                {
                    Items = allItems.OrderByDescending(i => i.Title).ToList();
                }
                else
                {
                    Items = allItems.OrderBy(i => i.Title).ToList();
                }
            }
            else
            {
                if (useOrderByDescending)
                {
                    Items = allItems.OrderByDescending(orderby).ToList();
                }
                else
                {
                    Items = allItems.OrderBy(orderby).ToList();
                }
            }
        }
        else
        {
            Items = allItems.ToList();
        }

        GetCollectionViewSource().Source = FormatData();
    }
    public async Task GetDataAsync(string jsonFilePath, PathType pathType, bool useOrder, bool useOrderByDescending, Func<DataItem, object> orderby)
    {
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);

        var allItems = DataSource.Instance.Groups
            .Where(group => !group.HideGroup)
            .SelectMany(group => group.Items)
            .Where(item => item.BadgeString != null && !item.HideItem);

        if (useOrder)
        {
            if (orderby == null)
            {
                if (useOrderByDescending)
                {
                    Items = allItems.OrderByDescending(i => i.Title).ToList();
                }
                else
                {
                    Items = allItems.OrderBy(i => i.Title).ToList();
                }
            }
            else
            {
                if (useOrderByDescending)
                {
                    Items = allItems.OrderByDescending(orderby).ToList();
                }
                else
                {
                    Items = allItems.OrderBy(orderby).ToList();
                }
            }
        }
        else
        {
            Items = allItems.ToList();
        }

        GetCollectionViewSource().Source = FormatData();
    }
    public async Task GetDataAsync(string jsonFilePath, PathType pathType = PathType.Relative)
    {
        await GetDataAsync(jsonFilePath, pathType, true, false, null);
    }
    public async Task GetDataAsync(string jsonFilePath, PathType pathType, bool useOrder)
    {
        await GetDataAsync(jsonFilePath, pathType, useOrder, false, null);
    }
    public async Task GetDataAsync(string jsonFilePath, PathType pathType, bool useOrder, bool useOrderByDescending)
    {
        await GetDataAsync(jsonFilePath, pathType, useOrder, useOrderByDescending, null);
    }

    public CollectionViewSource GetCollectionViewSource()
    {
        return itemsCVS;
    }

    public ObservableCollection<GroupInfoList> FormatData()
    {
        // Flatten the items list to include nested items
        var allItems = new List<DataItem>();
        foreach (var item in this.Items)
        {
            allItems.Add(item);
        }

        // Group the flattened items by BadgeString
        var query = from item in allItems
                    group item by item.BadgeString into g
                    orderby g.Key
                    select new GroupInfoList(g) { Key = g.Key };

        ObservableCollection<GroupInfoList> groupList = new(query);

        if (groupList.Any())
        {
            // Move "Preview" to the back of the list
            foreach (var item in groupList?.ToList())
            {
                if (item?.Key.ToString() == "Preview")
                {
                    groupList?.Remove(item);
                    groupList?.Insert(groupList.Count, item);
                }
            }

            // Update group titles based on the key
            foreach (var item in groupList)
            {
                switch (item.Key.ToString())
                {
                    case "New":
                        item.Title = NewGroupText;
                        break;
                    case "Updated":
                        item.Title = UpdatedGroupText;
                        break;
                    case "Preview":
                        item.Title = PreviewGroupText;
                        break;
                }
            }

            return groupList;
        }
        return null;
    }

    protected override bool GetIsNarrowLayoutState()
    {
        return LayoutVisualStates.CurrentState == NarrowLayout;
    }
}
