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
        DependencyProperty.Register(nameof(HeaderContent), typeof(object), typeof(MainLandingPage), new PropertyMetadata(null));

    public MainLandingPage()
    {
        this.InitializeComponent();
        Instance = this;
        DataSource.Instance.ParsingCompleted += OnDataSourceParsingCompleted;
        Loaded -= MainLandingPage_Loaded;
        Loaded += MainLandingPage_Loaded;
        Unloaded -= MainLandingPage_Unloaded;
        Unloaded += MainLandingPage_Unloaded;
    }

    private void MainLandingPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (IsEmpty)
        {
            DataSource.Instance.ParsingCompleted -= OnDataSourceParsingCompleted;
            DataSource.Instance.ParsingCompleted += OnDataSourceParsingCompleted;
        }

        RefreshIfEmpty();
    }

    private void MainLandingPage_Unloaded(object sender, RoutedEventArgs e)
    {
        DataSource.Instance.ParsingCompleted -= OnDataSourceParsingCompleted;
    }

    private void OnDataSourceParsingCompleted(object? sender, EventArgs e)
    {
        if (DispatcherQueue.HasThreadAccess)
        {
            RefreshIfEmpty();
        }
        else
        {
            DispatcherQueue.TryEnqueue(RefreshIfEmpty);
        }
    }

    private void RefreshIfEmpty()
    {
        if (!IsEmpty)
        {
            DataSource.Instance.ParsingCompleted -= OnDataSourceParsingCompleted;
            return;
        }

        if (CanExecuteInternalCommand)
        {
            GetData();

            if (!IsEmpty)
            {
                DataSource.Instance.ParsingCompleted -= OnDataSourceParsingCompleted;
            }
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
        GetData(useOrder, useOrderByDescending, orderby);
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
        if (Items == null)
        {
            return null;
        }

        var groupList = new ObservableCollection<GroupInfoList>(
            Items.GroupBy(item => item.BadgeString)
                .OrderBy(group => group.Key)
                .Select(group => new GroupInfoList(group) { Key = group.Key }));

        if (groupList.Count == 0)
        {
            return null;
        }

        for (var i = 0; i < groupList.Count; i++)
        {
            if (string.Equals(groupList[i].Key as string, "Preview", StringComparison.Ordinal))
            {
                var previewGroup = groupList[i];
                groupList.RemoveAt(i);
                groupList.Add(previewGroup);
                break;
            }
        }

        foreach (var item in groupList)
        {
            switch (item.Key as string)
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
}
