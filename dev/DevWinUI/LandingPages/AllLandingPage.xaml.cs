namespace DevWinUI;
public sealed partial class AllLandingPage : ItemsPageBase
{
    internal static AllLandingPage Instance { get; private set; }

    public AllLandingPage()
    {
        this.InitializeComponent();
        Instance = this;
        DataSource.Instance.ParsingCompleted += OnDataSourceParsingCompleted;
        Loaded -= AllLandingPage_Loaded;
        Loaded += AllLandingPage_Loaded;
        Unloaded -= AllLandingPage_Unloaded;
        Unloaded += AllLandingPage_Unloaded;
    }

    private void AllLandingPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (IsEmpty)
        {
            DataSource.Instance.ParsingCompleted -= OnDataSourceParsingCompleted;
            DataSource.Instance.ParsingCompleted += OnDataSourceParsingCompleted;
        }

        RefreshIfEmpty();
    }

    private void AllLandingPage_Unloaded(object sender, RoutedEventArgs e)
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
            GetData(i => i.Title);

            if (!IsEmpty)
            {
                DataSource.Instance.ParsingCompleted -= OnDataSourceParsingCompleted;
            }
        }
    }

    public void GetData()
    {
        Items = GetAllItems().ToList();
    }

    /// <summary>
    /// Loads and orders the items in one step, so Items is only set once.
    /// </summary>
    public void GetData(Func<DataItem, object> orderBy, bool descending = false)
    {
        var items = GetAllItems();
        if (orderBy != null)
        {
            items = descending ? items.OrderByDescending(orderBy) : items.OrderBy(orderBy);
        }

        Items = items.ToList();
    }

    private static IEnumerable<DataItem> GetAllItems()
    {
        return DataSource.Instance.Groups
            .Where(group => !group.HideGroup && !group.IsSpecialSection)
            .SelectMany(group => group.Items)
            .Where(item => !item.HideItem);
    }

    public async Task GetDataAsync(string jsonFilePath, PathType pathType = PathType.Relative)
    {
        await DataSource.Instance.GetGroupsAsync(jsonFilePath, pathType);

        Items = GetAllItems().ToList();
    }

    public void OrderBy(Func<DataItem, object> orderby = null)
    {
        if (orderby != null)
        {
            Items = Items?.OrderBy(orderby)?.ToList();
        }
        else
        {
            Items = Items?.OrderBy(i => i.Title)?.ToList();
        }
    }

    public void OrderByDescending(Func<DataItem, object> orderByDescending = null)
    {
        if (orderByDescending != null)
        {
            Items = Items?.OrderByDescending(orderByDescending)?.ToList();
        }
        else
        {
            Items = Items?.OrderByDescending(i => i.Title)?.ToList();
        }
    }
}
