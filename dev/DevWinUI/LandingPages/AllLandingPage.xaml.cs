namespace DevWinUI;
public sealed partial class AllLandingPage : ItemsPageBase
{
    internal static AllLandingPage Instance { get; private set; }
    public AllLandingPage()
    {
        this.InitializeComponent();
        Instance = this;
        Loading -= AllLandingPage_Loading;
        Loading += AllLandingPage_Loading;
    }

    private void AllLandingPage_Loading(FrameworkElement sender, object args)
    {
        if (CanExecuteInternalCommand)
        {
            GetData(i => i.Title);
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
